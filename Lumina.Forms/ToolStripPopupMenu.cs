using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

internal static class ToolStripPopupMenu
{
    // Thread-local state used by the WH_MSGFILTER hook for top-level sibling navigation.

    [ThreadStatic]
    private static int s_requestedTopLevelIndex;  // -1 = no navigation requested

    [ThreadStatic]
    private static int s_menuDepth;               // 0 = no popup showing; 1 = root popup; 2+ = nested

    [ThreadStatic]
    private static bool s_currentItemHasSubMenu;  // most recently selected item has sub-menu

    [ThreadStatic]
    private static nint s_msgHook;                // current thread hook handle

    [ThreadStatic]
    private static int s_currentTopLevelIndex;

    [ThreadStatic]
    private static Rectangle[]? s_topLevelItemBounds;

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
    /// Shows the popup and returns the next top-level item index to activate,
    /// or -1 when the popup closed normally.
    /// Install a WH_MSGFILTER hook so Left/Right and top-level hover changes
    /// at the root-popup level are captured and converted into a sibling switch.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal static int ShowForMenuBar(
        ToolStripItemCollection items,
        nint ownerHandle,
        Point screenLocation,
        ResolvedVisualStyle visualStyle,
        int currentTopLevelIndex,
        Rectangle[] topLevelItemBounds)
        => ShowForMenuBarCore(items, ownerHandle, screenLocation, visualStyle, currentTopLevelIndex, topLevelItemBounds);

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
                        int previousIndex = ResolveSiblingIndex(-1);
                        if (previousIndex >= 0)
                        {
                            s_requestedTopLevelIndex = previousIndex;
                            msg->message = (uint)Win32.WM_KEYDOWN;
                            msg->wParam = (nuint)Win32.VK_ESCAPE;
                            msg->lParam = 0;
                        }
                    }
                    else if (vk == (nuint)Win32.VK_RIGHT && !s_currentItemHasSubMenu)
                    {
                        int nextIndex = ResolveSiblingIndex(1);
                        if (nextIndex >= 0)
                        {
                            s_requestedTopLevelIndex = nextIndex;
                            msg->message = (uint)Win32.WM_KEYDOWN;
                            msg->wParam = (nuint)Win32.VK_ESCAPE;
                            msg->lParam = 0;
                        }
                    }
                }
                else if (msg->message == (uint)Win32.WM_MOUSEMOVE && s_menuDepth <= 1)
                {
                    int hoveredIndex = HitTestTopLevelItem(msg->pt);
                    if (hoveredIndex >= 0 && hoveredIndex != s_currentTopLevelIndex)
                    {
                        s_requestedTopLevelIndex = hoveredIndex;
                        msg->message = (uint)Win32.WM_KEYDOWN;
                        msg->wParam = (nuint)Win32.VK_ESCAPE;
                        msg->lParam = 0;
                    }
                }
            }
        }

        return Win32.CallNextHookEx(s_msgHook, code, wParam, lParam);
    }

    internal static void Show(ToolStripItemCollection items, nint ownerHandle, Point screenLocation, ResolvedVisualStyle visualStyle)
        => ShowCore(items, ownerHandle, screenLocation, visualStyle);

    private static int ShowForMenuBarCore(
        ToolStripItemCollection items,
        nint ownerHandle,
        Point screenLocation,
        ResolvedVisualStyle visualStyle,
        int currentTopLevelIndex,
        Rectangle[] topLevelItemBounds)
    {
        if (!OperatingSystem.IsWindows() || ownerHandle == 0)
        {
            return -1;
        }

        using NativeMenu nativeMenu = NativeMenu.CreatePopup(items, visualStyle);
        if (nativeMenu.Handle == 0)
        {
            return -1;
        }

        s_requestedTopLevelIndex = -1;
        s_currentTopLevelIndex = currentTopLevelIndex;
        s_topLevelItemBounds = topLevelItemBounds;
        DarkModeNative.RefreshImmersiveState();

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
                return -1;
            }

            _ = Win32.PostMessageW(ownerHandle, Win32.WM_NULL, 0, 0);
            return s_requestedTopLevelIndex;
        }
        finally
        {
            if (s_msgHook != 0)
            {
                Win32.UnhookWindowsHookEx(s_msgHook);
                s_msgHook = 0;
            }

            s_requestedTopLevelIndex = -1;
            s_currentTopLevelIndex = -1;
            s_topLevelItemBounds = null;
        }
    }

    private static int ResolveSiblingIndex(int offset)
    {
        Rectangle[]? topLevelItemBounds = s_topLevelItemBounds;
        if (topLevelItemBounds is null || topLevelItemBounds.Length == 0 || s_currentTopLevelIndex < 0)
        {
            return -1;
        }

        int count = topLevelItemBounds.Length;
        return ((s_currentTopLevelIndex + offset) % count + count) % count;
    }

    private static int HitTestTopLevelItem(Win32.POINT cursor)
    {
        Rectangle[]? topLevelItemBounds = s_topLevelItemBounds;
        if (topLevelItemBounds is null)
        {
            return -1;
        }

        Point screenPoint = new(cursor.x, cursor.y);
        for (int i = 0; i < topLevelItemBounds.Length; i++)
        {
            Rectangle bounds = topLevelItemBounds[i];
            if (bounds.Width > 0 && bounds.Height > 0 && bounds.Contains(screenPoint))
            {
                return i;
            }
        }

        return -1;
    }

    private static void ShowCore(ToolStripItemCollection items, nint ownerHandle, Point screenLocation, ResolvedVisualStyle visualStyle)
    {
        if (!OperatingSystem.IsWindows() || ownerHandle == 0)
        {
            return;
        }

        using NativeMenu nativeMenu = NativeMenu.CreatePopup(items, visualStyle);
        if (nativeMenu.Handle == 0)
        {
            return;
        }

        DarkModeNative.RefreshImmersiveState();
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

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static nint CreateMenuBitmap(ToolStripItem item)
        => NativeMenu.CreateMenuBitmap(item);
}
