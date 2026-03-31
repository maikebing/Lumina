using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Threading;

namespace Lumina.Forms;

internal static class ToolStripPopupMenu
{
    private static int s_nextCommandId = 30000;

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

                if (item is ToolStripMenuItem { Checked: true })
                {
                    itemInfo.fState |= Win32.MFS_CHECKED;
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

            if (item is ToolStripMenuItem { Checked: true })
            {
                commandItemInfo.fState |= Win32.MFS_CHECKED;
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