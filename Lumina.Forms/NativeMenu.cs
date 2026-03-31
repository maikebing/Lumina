using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Threading;

namespace Lumina.Forms;

internal sealed class NativeMenu : IDisposable
{
    private const int CommandIdStart = 0x8000;
    private const int CommandIdEnd = 0xEFFF;

    private static int s_nextCommandId = CommandIdStart;

    private readonly Dictionary<uint, ToolStripItem> _commands = [];
    private readonly List<nint> _imageHandles = [];

    private NativeMenu(nint handle)
    {
        Handle = handle;
    }

    internal nint Handle { get; private set; }

    internal static NativeMenu CreateMenuBar(IEnumerable<ToolStripItem> items)
        => Create(items, isMenuBar: true);

    internal static NativeMenu CreatePopup(IEnumerable<ToolStripItem> items)
        => Create(items, isMenuBar: false);

    internal bool TryGetCommand(uint commandId, out ToolStripItem item)
    {
        if (_commands.TryGetValue(commandId, out ToolStripItem? resolvedItem) && resolvedItem is not null)
        {
            item = resolvedItem;
            return true;
        }

        item = null!;
        return false;
    }

    public void Dispose()
    {
        foreach (nint imageHandle in _imageHandles)
        {
            if (imageHandle != 0)
            {
                _ = Win32.DeleteObject(imageHandle);
            }
        }

        _imageHandles.Clear();

        if (Handle != 0)
        {
            _ = Win32.DestroyMenu(Handle);
            Handle = 0;
        }
    }

    private static NativeMenu Create(IEnumerable<ToolStripItem> items, bool isMenuBar)
    {
        nint menuHandle = isMenuBar
            ? Win32.CreateMenu()
            : Win32.CreatePopupMenu();

        var nativeMenu = new NativeMenu(menuHandle);
        if (menuHandle != 0)
        {
            nativeMenu.PopulateMenu(menuHandle, items);
        }

        return nativeMenu;
    }

    private void PopulateMenu(nint menuHandle, IEnumerable<ToolStripItem> items)
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

            nint menuBitmap = CreateBitmapHandle(item);
            if (menuBitmap != 0)
            {
                _imageHandles.Add(menuBitmap);
            }

            if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0)
            {
                nint subMenuHandle = Win32.CreatePopupMenu();
                if (subMenuHandle == 0)
                {
                    continue;
                }

                PopulateMenu(subMenuHandle, dropDownItem.DropDownItems);
                var itemInfo = CreateMenuItemInfo(item, menuBitmap);
                itemInfo.fMask |= Win32.MIIM_SUBMENU;
                itemInfo.hSubMenu = subMenuHandle;

                if (!item.Enabled || IsUnsupportedMenuCommand(item))
                {
                    itemInfo.fState |= Win32.MFS_DISABLED;
                }

                ApplyMenuItemCheckState(item, ref itemInfo);
                _ = Win32.InsertMenuItemW(menuHandle, position++, true, ref itemInfo);
                continue;
            }

            uint commandId = NextCommandId();
            _commands[commandId] = item;

            var commandItemInfo = CreateMenuItemInfo(item, menuBitmap);
            commandItemInfo.fMask |= Win32.MIIM_ID;
            commandItemInfo.wID = commandId;

            if (!item.Enabled || IsUnsupportedMenuCommand(item))
            {
                commandItemInfo.fState |= Win32.MFS_DISABLED;
            }

            ApplyMenuItemCheckState(item, ref commandItemInfo);
            _ = Win32.InsertMenuItemW(menuHandle, position++, true, ref commandItemInfo);
        }
    }

    private static uint NextCommandId()
    {
        while (true)
        {
            int current = Volatile.Read(ref s_nextCommandId);
            int next = current >= CommandIdEnd
                ? CommandIdStart
                : current + 1;

            if (Interlocked.CompareExchange(ref s_nextCommandId, next, current) == current)
            {
                return (uint)next;
            }
        }
    }

    private static bool IsUnsupportedMenuCommand(ToolStripItem item)
        => item is ToolStripComboBox or ToolStripTextBox or ToolStripProgressBar;

    private static void ApplyMenuItemCheckState(ToolStripItem item, ref Win32.MENUITEMINFOW itemInfo)
    {
        if (item is not ToolStripMenuItem menuItem || !menuItem.Checked)
        {
            return;
        }

        itemInfo.fState |= Win32.MFS_CHECKED;
        if (menuItem.RadioCheck)
        {
            itemInfo.fType |= Win32.MFT_RADIOCHECK;
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
    internal static nint CreateMenuBitmap(ToolStripItem item)
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

    internal static string GetDisplayText(ToolStripItem item)
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

    private static nint CreateBitmapHandle(ToolStripItem item)
        => item.SupportsMenuImage && OperatingSystem.IsWindows()
            ? CreateMenuBitmap(item)
            : 0;

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
}
