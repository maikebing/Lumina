using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Threading;

namespace Lumina.Forms;

internal sealed class NativeMenu : IDisposable
{
    private const int TopLevelMenuHorizontalPadding = 12;
    private const int TopLevelMenuVerticalPadding = 7;
    private const nuint OwnerDrawItemDataStart = 1;

    private const int CommandIdStart = 0x8000;
    private const int CommandIdEnd = 0xEFFF;

    private static int s_nextCommandId = CommandIdStart;
    private static nuint s_nextOwnerDrawItemData = OwnerDrawItemDataStart;

    private readonly Dictionary<uint, ToolStripItem> _commands = [];
    private readonly Dictionary<nuint, ToolStripItem> _ownerDrawItems = [];
    private readonly List<nint> _imageHandles = [];
    private nint _backgroundBrush;
    private bool _ownsBackgroundBrush;
    private readonly bool _isMenuBar;
    private readonly ResolvedVisualStyle _visualStyle;

    private NativeMenu(nint handle, bool isMenuBar, ResolvedVisualStyle visualStyle)
    {
        Handle = handle;
        _isMenuBar = isMenuBar;
        _visualStyle = visualStyle;
    }

    internal nint Handle { get; private set; }

    internal static NativeMenu CreateMenuBar(IEnumerable<ToolStripItem> items, ResolvedVisualStyle visualStyle)
        => Create(items, isMenuBar: true, visualStyle);

    internal static NativeMenu CreatePopup(IEnumerable<ToolStripItem> items, ResolvedVisualStyle visualStyle)
        => Create(items, isMenuBar: false, visualStyle);

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

    internal bool TryMeasureOwnerDrawItem(nuint itemData, nint fontHandle, out int width, out int height)
    {
        width = 0;
        height = 0;

        if (!_ownerDrawItems.TryGetValue(itemData, out ToolStripItem? item))
        {
            return false;
        }

        string displayText = GetTopLevelDisplayText(item);
        if (string.IsNullOrEmpty(displayText))
        {
            width = TopLevelMenuHorizontalPadding * 2;
            height = 24;
            return true;
        }

        nint screenDc = Win32.GetDC(0);
        if (screenDc == 0)
        {
            return false;
        }

        nint previousObject = 0;
        try
        {
            if (fontHandle != 0)
            {
                previousObject = Win32.SelectObject(screenDc, fontHandle);
            }

            if (!Win32.GetTextExtentPoint32W(screenDc, displayText, displayText.Length, out Win32.SIZE size))
            {
                return false;
            }

            int textHeight = size.cy;
            if (textHeight <= 0 && Win32.GetTextMetricsW(screenDc, out Win32.TEXTMETRICW metrics))
            {
                textHeight = metrics.tmHeight;
            }

            width = size.cx + (TopLevelMenuHorizontalPadding * 2);
            height = Math.Max(24, textHeight + (TopLevelMenuVerticalPadding * 2));
            return true;
        }
        finally
        {
            if (previousObject != 0)
            {
                _ = Win32.SelectObject(screenDc, previousObject);
            }

            _ = Win32.ReleaseDC(0, screenDc);
        }
    }

    internal bool TryDrawOwnerDrawItem(nuint itemData, in Win32.DRAWITEMSTRUCT drawItem)
    {
        if (!_ownerDrawItems.TryGetValue(itemData, out ToolStripItem? item))
        {
            return false;
        }

        ThemePalette palette = _visualStyle.Palette;
        bool isDarkMode = _visualStyle.IsDarkMode;
        bool disabled = (drawItem.itemState & (Win32.ODS_DISABLED | Win32.ODS_GRAYED)) != 0 || !item.Enabled;

        uint backgroundArgb = isDarkMode
            ? palette.SurfaceBackground
            : palette.ControlBackground;
        uint foregroundArgb = disabled
            ? palette.MutedForeground
            : (isDarkMode ? palette.SurfaceForeground : palette.ControlForeground);

        nint backgroundBrush = Win32.CreateSolidBrush(Win32.ToColorRef(backgroundArgb));
        if (backgroundBrush == 0)
        {
            return false;
        }

        try
        {
            Win32.RECT bounds = drawItem.rcItem;
            _ = Win32.FillRect(drawItem.hDC, ref bounds, backgroundBrush);
            _ = Win32.SetBkMode(drawItem.hDC, Win32.TRANSPARENT);
            _ = Win32.SetTextColor(drawItem.hDC, Win32.ToColorRef(foregroundArgb));

            Win32.RECT textBounds = bounds;
            textBounds.Left += TopLevelMenuHorizontalPadding;
            textBounds.Right -= TopLevelMenuHorizontalPadding;

            string displayText = GetTopLevelDisplayText(item);
            uint format = Win32.DT_SINGLELINE | Win32.DT_VCENTER | Win32.DT_CENTER;
            _ = Win32.DrawTextW(drawItem.hDC, displayText, displayText.Length, ref textBounds, format);
            return true;
        }
        finally
        {
            _ = Win32.DeleteObject(backgroundBrush);
        }
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

        if (_backgroundBrush != 0 && _ownsBackgroundBrush)
        {
            _ = Win32.DeleteObject(_backgroundBrush);
        }

        _backgroundBrush = 0;
        _ownsBackgroundBrush = false;

        if (Handle != 0)
        {
            _ = Win32.DestroyMenu(Handle);
            Handle = 0;
        }
    }

    private static NativeMenu Create(IEnumerable<ToolStripItem> items, bool isMenuBar, ResolvedVisualStyle visualStyle)
    {
        nint menuHandle = isMenuBar
            ? Win32.CreateMenu()
            : Win32.CreatePopupMenu();

        var nativeMenu = new NativeMenu(menuHandle, isMenuBar, visualStyle);
        if (menuHandle != 0)
        {
            nativeMenu.ApplyDarkMenuStyling(menuHandle, visualStyle);
            nativeMenu.PopulateMenu(menuHandle, items);
        }

        return nativeMenu;
    }

    private void ApplyDarkMenuStyling(nint menuHandle, ResolvedVisualStyle visualStyle)
    {
        if (menuHandle == 0 || !DarkModeNative.IsSupported)
        {
            return;
        }

        DarkModeNative.RefreshImmersiveState();

        uint menuArgb = visualStyle.IsDarkMode
            ? visualStyle.Palette.SurfaceBackground
            : visualStyle.Palette.ControlBackground;

        _backgroundBrush = Win32.CreateSolidBrush(Win32.ToColorRef(menuArgb));
        _ownsBackgroundBrush = _backgroundBrush != 0;
        if (_backgroundBrush == 0)
        {
            return;
        }

        var menuInfo = new Win32.MENUINFO
        {
            cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<Win32.MENUINFO>(),
            fMask = Win32.MIM_BACKGROUND | Win32.MIM_APPLYTOSUBMENUS,
            hbrBack = _backgroundBrush,
        };

        _ = Win32.SetMenuInfo(menuHandle, ref menuInfo);
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

                if (_isMenuBar)
                {
                    itemInfo.fMask |= Win32.MIIM_DATA;
                    itemInfo.fMask &= ~Win32.MIIM_STRING;
                    itemInfo.fType = Win32.MFT_OWNERDRAW;
                    itemInfo.dwTypeData = null;
                    itemInfo.cch = 0;
                    itemInfo.dwItemData = NextOwnerDrawItemData();
                    _ownerDrawItems[itemInfo.dwItemData] = item;
                }

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

            if (_isMenuBar)
            {
                commandItemInfo.fMask |= Win32.MIIM_DATA;
                commandItemInfo.fMask &= ~Win32.MIIM_STRING;
                commandItemInfo.fType = Win32.MFT_OWNERDRAW;
                commandItemInfo.dwTypeData = null;
                commandItemInfo.cch = 0;
                commandItemInfo.dwItemData = NextOwnerDrawItemData();
                _ownerDrawItems[commandItemInfo.dwItemData] = item;
            }

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

    private static nuint NextOwnerDrawItemData()
    {
        while (true)
        {
            nuint current = Volatile.Read(ref s_nextOwnerDrawItemData);
            nuint next = current == nuint.MaxValue ? OwnerDrawItemDataStart : current + 1;

            if (Interlocked.CompareExchange(ref s_nextOwnerDrawItemData, next, current) == current)
            {
                return current;
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

    private static string GetTopLevelDisplayText(ToolStripItem item)
    {
        string sourceText = string.IsNullOrWhiteSpace(item.Text)
            ? GetDisplayText(item).Split('\t')[0]
            : item.Text;

        return NormalizeTopLevelMenuText(sourceText);
    }

    private static string NormalizeTopLevelMenuText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var builder = new System.Text.StringBuilder(text.Length);
        for (int index = 0; index < text.Length; index++)
        {
            char current = text[index];
            if (current != '&')
            {
                builder.Append(current);
                continue;
            }

            if (index + 1 < text.Length && text[index + 1] == '&')
            {
                builder.Append('&');
                index++;
            }
        }

        return builder.ToString();
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
