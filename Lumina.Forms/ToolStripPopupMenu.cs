using System.Drawing;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

internal static class ToolStripPopupMenu
{
    // Thread-local state used by the WH_MSGFILTER hook for Left/Right sibling navigation.

    [ThreadStatic]
    private static int s_menuCloseDirection;      // -1=left, 0=none, 1=right

    [ThreadStatic]
    private static int s_menuDepth;               // 0 = no popup showing; 1 = root popup; 2+ = nested

    [ThreadStatic]
    private static bool s_currentItemHasSubMenu;  // most recently selected item has sub-menu

    [ThreadStatic]
    private static nint s_msgHook;                // current thread hook handle

    /// <summary>
    /// Called from Form.WindowProc for WM_INITMENUPOPUP / WM_UNINITMENUPOPUP so that
    /// the hook proc knows the current nesting depth.
    /// </summary>
    internal static void NotifyMenuDepthChange(int delta) => s_menuDepth += delta;

    /// <summary>
    /// Called from Form.WindowProc for WM_MENUSELECT so the hook proc can decide whether
    /// VK_RIGHT should be intercepted (no sub-menu) or passed through (opens sub-menu).
    /// </summary>
    internal static void NotifyMenuSelectionChanged(bool itemHasSubMenu) =>
        s_currentItemHasSubMenu = itemHasSubMenu;

    /// <summary>
    /// Shows the popup and returns a navigation direction: -1 = navigate left,
    /// 0 = closed normally (item clicked or Escape), 1 = navigate right.
    /// Install a WH_MSGFILTER hook so Left/Right at the root-popup level
    /// are captured and converted to Escape, then reported as a direction.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal static int ShowForMenuBar(ToolStripItemCollection items, nint ownerHandle, Point screenLocation)
    {
        MenuRenderingMode requestedMode = MenuRenderingModeResolver.ResolveForPopupMenus();
        return requestedMode switch
        {
            MenuRenderingMode.ImmersivePopup => ShowForMenuBarImmersive(items, ownerHandle, screenLocation, requestedMode),
            _ => ShowForMenuBarClassic(items, ownerHandle, screenLocation, requestedMode),
        };
    }

    // The hook proc is an unmanaged static function pointer and AOT-safe.
    // lParam points to a MSG struct when nCode == MSGF_MENU.
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint MsgFilterHookProc(int code, nint wParam, nint lParam)
    {
        if (code == Win32.MSGF_MENU)
        {
            unsafe
            {
                Win32.MSG* msg = (Win32.MSG*)lParam;
                if (msg->message == (uint)Win32.WM_KEYDOWN)
                {
                    nuint vk = msg->wParam;
                    if (vk == (nuint)Win32.VK_LEFT && s_menuDepth <= 1)
                    {
                        // At root level: turn Left into Escape and record direction.
                        s_menuCloseDirection = -1;
                        msg->wParam = (nuint)Win32.VK_ESCAPE;
                    }
                    else if (vk == (nuint)Win32.VK_RIGHT && !s_currentItemHasSubMenu)
                    {
                        // Right on a leaf item: turn it into Escape and record direction.
                        s_menuCloseDirection = 1;
                        msg->wParam = (nuint)Win32.VK_ESCAPE;
                    }
                }
            }
        }

        return Win32.CallNextHookEx(s_msgHook, code, wParam, lParam);
    }

    internal static void Show(ToolStripItemCollection items, nint ownerHandle, Point screenLocation)
    {
        MenuRenderingMode requestedMode = MenuRenderingModeResolver.ResolveForPopupMenus();
        switch (requestedMode)
        {
            case MenuRenderingMode.ImmersivePopup:
                ShowImmersive(items, ownerHandle, screenLocation, requestedMode);
                break;

            default:
                ShowClassic(items, ownerHandle, screenLocation, requestedMode);
                break;
        }
    }

    private static int ShowForMenuBarClassic(ToolStripItemCollection items, nint ownerHandle, Point screenLocation, MenuRenderingMode requestedMode)
    {
        LogMenuDecision(requestedMode, MenuRenderingMode.Classic, immersiveAttempted: false, immersiveEnabled: false);

        if (ownerHandle == 0)
        {
            return 0;
        }

        using NativeMenu nativeMenu = NativeMenu.CreatePopup(items);
        if (nativeMenu.Handle == 0)
        {
            return 0;
        }

        s_menuCloseDirection = 0;

        unsafe
        {
            nint hookPtr = (nint)(delegate* unmanaged[Stdcall]<int, nint, nint, nint>)&MsgFilterHookProc;
            s_msgHook = Win32.SetWindowsHookExW(Win32.WH_MSGFILTER, hookPtr, 0, Win32.GetCurrentThreadId());
        }

        try
        {
            _ = Win32.SetForegroundWindow(ownerHandle);

            uint command = Win32.TrackPopupMenu(
                nativeMenu.Handle,
                Win32.TPM_RIGHTBUTTON | Win32.TPM_RETURNCMD,
                screenLocation.X,
                screenLocation.Y,
                0,
                ownerHandle,
                0);

            if (command != 0 && nativeMenu.TryGetCommand(command, out ToolStripItem item))
            {
                item.PerformClick();
                _ = Win32.PostMessageW(ownerHandle, Win32.WM_NULL, 0, 0);
                return 0;
            }

            _ = Win32.PostMessageW(ownerHandle, Win32.WM_NULL, 0, 0);
            return s_menuCloseDirection;
        }
        finally
        {
            if (s_msgHook != 0)
            {
                Win32.UnhookWindowsHookEx(s_msgHook);
                s_msgHook = 0;
            }
        }
    }

    private static int ShowForMenuBarImmersive(ToolStripItemCollection items, nint ownerHandle, Point screenLocation, MenuRenderingMode requestedMode)
    {
        bool enabled = Win32DarkModeApi.TryEnableImmersivePopupMenus();
        LogMenuDecision(requestedMode, enabled ? MenuRenderingMode.ImmersivePopup : MenuRenderingMode.Classic, immersiveAttempted: true, immersiveEnabled: enabled);

        if (!enabled)
        {
            return ShowForMenuBarClassic(items, ownerHandle, screenLocation, requestedMode);
        }

        return ShowForMenuBarImmersiveCore(items, ownerHandle, screenLocation);
    }

    private static void ShowClassic(ToolStripItemCollection items, nint ownerHandle, Point screenLocation, MenuRenderingMode requestedMode)
    {
        LogMenuDecision(requestedMode, MenuRenderingMode.Classic, immersiveAttempted: false, immersiveEnabled: false);

        if (ownerHandle == 0)
        {
            return;
        }

        using NativeMenu nativeMenu = NativeMenu.CreatePopup(items);
        if (nativeMenu.Handle == 0)
        {
            return;
        }

        _ = Win32.SetForegroundWindow(ownerHandle);

        uint command = Win32.TrackPopupMenu(
            nativeMenu.Handle,
            Win32.TPM_RIGHTBUTTON | Win32.TPM_RETURNCMD,
            screenLocation.X,
            screenLocation.Y,
            0,
            ownerHandle,
            0);

        if (command != 0 && nativeMenu.TryGetCommand(command, out ToolStripItem item))
        {
            item.PerformClick();
        }

        _ = Win32.PostMessageW(ownerHandle, Win32.WM_NULL, 0, 0);
    }

    private static void ShowImmersive(ToolStripItemCollection items, nint ownerHandle, Point screenLocation, MenuRenderingMode requestedMode)
    {
        bool enabled = Win32DarkModeApi.TryEnableImmersivePopupMenus();
        LogMenuDecision(requestedMode, enabled ? MenuRenderingMode.ImmersivePopup : MenuRenderingMode.Classic, immersiveAttempted: true, immersiveEnabled: enabled);

        if (!enabled)
        {
            ShowClassic(items, ownerHandle, screenLocation, requestedMode);
            return;
        }

        ShowImmersiveCore(items, ownerHandle, screenLocation);
    }

    private static int ShowForMenuBarImmersiveCore(ToolStripItemCollection items, nint ownerHandle, Point screenLocation)
    {
        return ShowForMenuBarClassic(items, ownerHandle, screenLocation, MenuRenderingMode.ImmersivePopup);
    }

    private static void ShowImmersiveCore(ToolStripItemCollection items, nint ownerHandle, Point screenLocation)
    {
        ShowClassic(items, ownerHandle, screenLocation, MenuRenderingMode.ImmersivePopup);
    }

    private static void LogMenuDecision(MenuRenderingMode requestedMode, MenuRenderingMode effectiveMode, bool immersiveAttempted, bool immersiveEnabled)
    {
        DarkModeCapabilities.Snapshot capabilities = DarkModeCapabilities.Current;
        Debug.WriteLine($"[Lumina.Forms] PopupMenu requested={requestedMode}, effective={effectiveMode}, immersiveAttempted={immersiveAttempted}, immersiveEnabled={immersiveEnabled}, os={capabilities.Major}.{capabilities.Minor}.{capabilities.Build}");
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static nint CreateMenuBitmap(ToolStripItem item)
        => NativeMenu.CreateMenuBitmap(item);
}
