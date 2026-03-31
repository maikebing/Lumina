using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace Lumina.Forms;

internal static class ToolStripPopupMenu
{
    private static int s_nextCommandId = 30000;

    // ── Thread-local state used by the WH_MSGFILTER hook (Left/Right sibling navigation) ──

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
        if (ownerHandle == 0)
        {
            return 0;
        }

        nint menuHandle = Win32.CreatePopupMenu();
        if (menuHandle == 0)
        {
            return 0;
        }

        var commands = new Dictionary<uint, ToolStripItem>();
        List<nint> imageHandles = [];
        s_menuCloseDirection = 0;

        unsafe
        {
            nint hookPtr = (nint)(delegate* unmanaged[Stdcall]<int, nint, nint, nint>)&MsgFilterHookProc;
            s_msgHook = Win32.SetWindowsHookExW(Win32.WH_MSGFILTER, hookPtr, 0, Win32.GetCurrentThreadId());
        }

        try
        {
            PopulateMenu(menuHandle, items, commands, imageHandles);
            _ = Win32.SetForegroundWindow(ownerHandle);

            uint command = Win32.TrackPopupMenu(
                menuHandle,
                Win32.TPM_RIGHTBUTTON | Win32.TPM_RETURNCMD,
                screenLocation.X,
                screenLocation.Y,
                0,
                ownerHandle,
                0);

            if (command != 0 && commands.TryGetValue(command, out ToolStripItem? item))
            {
                item.PerformClick();
                return 0;
            }

            return s_menuCloseDirection;
        }
        finally
        {
            if (s_msgHook != 0)
            {
                Win32.UnhookWindowsHookEx(s_msgHook);
                s_msgHook = 0;
            }

            foreach (nint imageHandle in imageHandles)
            {
                if (imageHandle != 0)
                {
                    _ = Win32.DeleteObject(imageHandle);
                }
            }

            _ = Win32.DestroyMenu(menuHandle);
        }
    }

    // The hook proc is an unmanaged static function pointer — AOT-safe.
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
        if (ownerHandle == 0)
        {
            return;
        }

        nint menuHandle = Win32.CreatePopupMenu();
        if (menuHandle == 0)
        {
            return;
        }

        var commands = new Dictionary<uint, ToolStripItem>();
        List<nint> imageHandles = [];
        try
        {
            PopulateMenu(menuHandle, items, commands, imageHandles);
            _ = Win32.SetForegroundWindow(ownerHandle);

            uint command = Win32.TrackPopupMenu(
                menuHandle,
                Win32.TPM_RIGHTBUTTON | Win32.TPM_RETURNCMD,
                screenLocation.X,
                screenLocation.Y,
                0,
                ownerHandle,
                0);

            if (command != 0 && commands.TryGetValue(command, out ToolStripItem? item))
            {
                item.PerformClick();
            }
        }
        finally
        {
            foreach (nint imageHandle in imageHandles)
            {
                if (imageHandle != 0)
                {
                    _ = Win32.DeleteObject(imageHandle);
                }
            }

            _ = Win32.DestroyMenu(menuHandle);
        }
    }

    private static void PopulateMenu(nint menuHandle, IEnumerable<ToolStripItem> items, Dictionary<uint, ToolStripItem> commands, List<nint> imageHandles)
    {
        uint position = 0;
        foreach (ToolStripItem item in items)
        {
            if (!item.Visible)
            {
                continue;
            }

            if (item is ToolStripSeparator)
            {
                var separatorInfo = new Win32.MENUITEMINFOW
                {
                    cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<Win32.MENUITEMINFOW>(),
                    fMask = Win32.MIIM_FTYPE,
                    fType = Win32.MFT_SEPARATOR,
                };

                _ = Win32.InsertMenuItemW(menuHandle, position++, true, ref separatorInfo);
                continue;
            }

            nint menuBitmap = OperatingSystem.IsWindows()
                ? CreateMenuBitmap(item)
                : 0;
            if (menuBitmap != 0)
            {
                imageHandles.Add(menuBitmap);
            }

            if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0)
            {
                nint subMenuHandle = Win32.CreatePopupMenu();
                if (subMenuHandle == 0)
                {
                    continue;
                }

                PopulateMenu(subMenuHandle, dropDownItem.DropDownItems, commands, imageHandles);
                var itemInfo = CreateMenuItemInfo(item, menuBitmap);
                itemInfo.fMask |= Win32.MIIM_SUBMENU;
                itemInfo.hSubMenu = subMenuHandle;
                itemInfo.fType = Win32.MFT_STRING;
                if (!item.Enabled)
                {
                    itemInfo.fState |= Win32.MFS_DISABLED;
                }

                if (item is ToolStripMenuItem menuItemDropDown && menuItemDropDown.Checked)
                {
                    itemInfo.fState |= Win32.MFS_CHECKED;
                    if (menuItemDropDown.RadioCheck)
                    {
                        itemInfo.fType |= Win32.MFT_RADIOCHECK;
                    }
                }

                _ = Win32.InsertMenuItemW(menuHandle, position++, true, ref itemInfo);
                continue;
            }

            uint commandId = unchecked((uint)Interlocked.Increment(ref s_nextCommandId));
            commands[commandId] = item;
            var commandItemInfo = CreateMenuItemInfo(item, menuBitmap);
            commandItemInfo.fMask |= Win32.MIIM_ID;
            commandItemInfo.wID = commandId;
            if (!item.Enabled || item is ToolStripComboBox || item is ToolStripTextBox || item is ToolStripProgressBar)
            {
                commandItemInfo.fState |= Win32.MFS_DISABLED;
            }

            if (item is ToolStripMenuItem menuItemCmd && menuItemCmd.Checked)
            {
                commandItemInfo.fState |= Win32.MFS_CHECKED;
                if (menuItemCmd.RadioCheck)
                {
                    commandItemInfo.fType |= Win32.MFT_RADIOCHECK;
                }
            }

            _ = Win32.InsertMenuItemW(menuHandle, position++, true, ref commandItemInfo);
        }
    }

    private static Win32.MENUITEMINFOW CreateMenuItemInfo(ToolStripItem item, nint menuBitmap)
    {
        string displayText = GetDisplayText(item);
        var itemInfo = new Win32.MENUITEMINFOW
        {
            cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<Win32.MENUITEMINFOW>(),
            fMask = Win32.MIIM_FTYPE | Win32.MIIM_STATE | Win32.MIIM_STRING,
            fType = Win32.MFT_STRING,
            fState = 0,
            dwTypeData = displayText,
            cch = (uint)displayText.Length,
        };

        if (menuBitmap != 0)
        {
            itemInfo.fMask |= Win32.MIIM_BITMAP;
            itemInfo.hbmpItem = menuBitmap;
        }

        return itemInfo;
    }

    [SupportedOSPlatform("windows")]
    private static nint CreateMenuBitmap(ToolStripItem item)
    {
        if (!item.SupportsMenuImage)
        {
            return 0;
        }

        using var surface = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(surface);
        graphics.Clear(Color.Transparent);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

        using var preparedImage = PrepareMenuImage(item.Image!, item.ImageTransparentColor);
        Rectangle targetBounds = CalculateImageBounds(preparedImage.Size, new Size(16, 16));
        graphics.DrawImage(preparedImage, targetBounds);

        return surface.GetHbitmap(Color.FromArgb(0));
    }

    [SupportedOSPlatform("windows")]
    private static Image PrepareMenuImage(Image sourceImage, Color transparentColor)
    {
        var bitmap = new Bitmap(sourceImage);
        if (transparentColor != Color.Empty)
        {
            bitmap.MakeTransparent(transparentColor);
        }

        return bitmap;
    }

    private static Rectangle CalculateImageBounds(Size imageSize, Size canvasSize)
    {
        if (imageSize.Width <= 0 || imageSize.Height <= 0)
        {
            return Rectangle.Empty;
        }

        double scale = Math.Min((double)canvasSize.Width / imageSize.Width, (double)canvasSize.Height / imageSize.Height);
        int width = Math.Max(1, (int)Math.Round(imageSize.Width * scale, MidpointRounding.AwayFromZero));
        int height = Math.Max(1, (int)Math.Round(imageSize.Height * scale, MidpointRounding.AwayFromZero));
        int x = Math.Max(0, (canvasSize.Width - width) / 2);
        int y = Math.Max(0, (canvasSize.Height - height) / 2);
        return new Rectangle(x, y, width, height);
    }

    private static string GetDisplayText(ToolStripItem item)
    {
        string baseText;
        if (!string.IsNullOrWhiteSpace(item.Text))
        {
            baseText = item.Text;
        }
        else if (item is ToolStripComboBox comboBox && comboBox.SelectedIndex >= 0 && comboBox.SelectedIndex < comboBox.Items.Count)
        {
            baseText = comboBox.Items[comboBox.SelectedIndex]?.ToString() ?? comboBox.Name;
        }
        else if (item is ToolStripProgressBar progressBar)
        {
            baseText = $"{progressBar.Name} {progressBar.Value}".Trim();
        }
        else
        {
            baseText = string.IsNullOrWhiteSpace(item.Name)
                ? item.GetType().Name
                : item.Name;
        }

        if (item is ToolStripMenuItem menuItem)
        {
            string shortcutText = menuItem.GetShortcutDisplayText();
            if (!string.IsNullOrWhiteSpace(shortcutText))
            {
                return $"{baseText}\t{shortcutText}";
            }
        }

        return baseText;
    }
}