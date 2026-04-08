using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Lumina.Forms;

[SupportedOSPlatform("windows")]
internal static class NativeMenuRenderer
{
    private const int HorizontalPadding = 12;
    private const int VerticalPadding = 6;
    private const int SelectionInsetHorizontal = 4;
    private const int SelectionInsetVertical = 2;
    private const int SelectionCornerRadius = 6;
    private const int GlyphColumnWidth = 24;
    private const int GlyphSize = 16;
    private const int TextShortcutSpacing = 24;
    private const int ArrowColumnWidth = 16;
    private const int SubMenuArrowWidth = 5;
    private const int SubMenuArrowHeight = 9;
    private const int MinItemWidth = 120;
    private const int MinItemHeight = 28;
    private const int SeparatorHeight = 9;
    private const int IndicatorCornerRadius = 4;
    private const int HighDpiOpticalLift = 1;

    private static readonly object s_sync = new();
    private static readonly Dictionary<nuint, OwnerDrawMenuItemData> s_items = [];

    internal static nuint Register(ToolStripItem item, ResolvedVisualStyle visualStyle)
    {
        var data = new OwnerDrawMenuItemData(item, visualStyle);
        GCHandle handle = GCHandle.Alloc(data);
        nuint key = (nuint)GCHandle.ToIntPtr(handle);

        lock (s_sync)
        {
            s_items[key] = data;
        }

        return key;
    }

    internal static void Unregister(nuint key)
    {
        OwnerDrawMenuItemData? data;
        lock (s_sync)
        {
            if (!s_items.Remove(key, out data))
            {
                return;
            }
        }

        if (data is null)
        {
            return;
        }

        data.Dispose();

        GCHandle handle = GCHandle.FromIntPtr((nint)key);
        if (handle.IsAllocated)
        {
            handle.Free();
        }
    }

    internal static bool TryHandleMeasureItem(nint lParam)
    {
        if (lParam == 0)
        {
            return false;
        }

        Win32.MEASUREITEMSTRUCT measureItem = Marshal.PtrToStructure<Win32.MEASUREITEMSTRUCT>(lParam);
        if (measureItem.CtlType != Win32.ODT_MENU
            || !TryGetItemData(measureItem.itemData, out OwnerDrawMenuItemData? data)
            || data is null)
        {
            return false;
        }

        Size size = data.Measure();
        measureItem.itemWidth = (uint)Math.Max(1, size.Width);
        measureItem.itemHeight = (uint)Math.Max(1, size.Height);
        Marshal.StructureToPtr(measureItem, lParam, false);
        return true;
    }

    internal static bool TryHandleDrawItem(nint lParam)
    {
        if (lParam == 0)
        {
            return false;
        }

        Win32.DRAWITEMSTRUCT drawItem = Marshal.PtrToStructure<Win32.DRAWITEMSTRUCT>(lParam);
        if (drawItem.CtlType != Win32.ODT_MENU
            || !TryGetItemData(drawItem.itemData, out OwnerDrawMenuItemData? data)
            || data is null)
        {
            return false;
        }

        data.Draw(drawItem);
        return true;
    }

    private static bool TryGetItemData(nuint key, out OwnerDrawMenuItemData? data)
    {
        lock (s_sync)
        {
            return s_items.TryGetValue(key, out data);
        }
    }

    private static Size MeasureText(string text, bool useMnemonicPrefix)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Size.Empty;
        }

        nint screenDc = Win32.GetDC(0);
        if (screenDc == 0)
        {
            return new Size(text.Length * 8, 16);
        }

        nint fontHandle = Win32.GetStockObject(Win32.DEFAULT_GUI_FONT);
        nint previousFont = 0;

        try
        {
            if (fontHandle != 0)
            {
                previousFont = Win32.SelectObject(screenDc, fontHandle);
            }

            var textBounds = new Win32.RECT();
            uint flags = Win32.DT_SINGLELINE | Win32.DT_CALCRECT | (useMnemonicPrefix ? Win32.DT_HIDEPREFIX : Win32.DT_NOPREFIX);
            _ = Win32.DrawTextW(screenDc, text, text.Length, ref textBounds, flags);

            int width = Math.Max(0, textBounds.Width);
            int height = Math.Max(0, textBounds.Height);
            if ((width == 0 || height == 0) && Win32.GetTextExtentPoint32W(screenDc, text, text.Length, out Win32.SIZE textSize))
            {
                width = Math.Max(width, textSize.cx);
                height = Math.Max(height, textSize.cy);
            }

            return new Size(width, height);
        }
        finally
        {
            if (previousFont != 0)
            {
                _ = Win32.SelectObject(screenDc, previousFont);
            }

            _ = Win32.ReleaseDC(0, screenDc);
        }
    }

    private static Rectangle ToRectangle(Win32.RECT rect)
        => Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);

    private static uint BlendOpaque(uint background, uint foreground, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);

        float backgroundRed = (background >> 16) & 0xFF;
        float backgroundGreen = (background >> 8) & 0xFF;
        float backgroundBlue = background & 0xFF;

        float foregroundRed = (foreground >> 16) & 0xFF;
        float foregroundGreen = (foreground >> 8) & 0xFF;
        float foregroundBlue = foreground & 0xFF;

        byte red = (byte)Math.Clamp((backgroundRed * (1f - amount)) + (foregroundRed * amount), 0f, 255f);
        byte green = (byte)Math.Clamp((backgroundGreen * (1f - amount)) + (foregroundGreen * amount), 0f, 255f);
        byte blue = (byte)Math.Clamp((backgroundBlue * (1f - amount)) + (foregroundBlue * amount), 0f, 255f);

        return 0xFF_00_00_00
            | ((uint)red << 16)
            | ((uint)green << 8)
            | blue;
    }

    private static bool IsDarkColor(uint argb)
    {
        float red = ((argb >> 16) & 0xFF) / 255f;
        float green = ((argb >> 8) & 0xFF) / 255f;
        float blue = (argb & 0xFF) / 255f;
        float luminance = (0.2126f * red) + (0.7152f * green) + (0.0722f * blue);
        return luminance < 0.5f;
    }

    private static void DrawText(nint hdc, string text, Rectangle bounds, uint colorArgb, uint flags)
    {
        if (string.IsNullOrEmpty(text) || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        nint fontHandle = Win32.GetStockObject(Win32.DEFAULT_GUI_FONT);
        nint previousFont = 0;

        try
        {
            if (fontHandle != 0)
            {
                previousFont = Win32.SelectObject(hdc, fontHandle);
            }

            _ = Win32.SetBkMode(hdc, Win32.TRANSPARENT);
            _ = Win32.SetTextColor(hdc, Win32.ToColorRef(colorArgb));

            Win32.RECT textRect = new()
            {
                Left = bounds.Left,
                Top = bounds.Top,
                Right = bounds.Right,
                Bottom = bounds.Bottom,
            };

            _ = Win32.DrawTextW(hdc, text, text.Length, ref textRect, flags);
        }
        finally
        {
            if (previousFont != 0)
            {
                _ = Win32.SelectObject(hdc, previousFont);
            }
        }
    }

    private static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
    {
        int diameter = Math.Min(Math.Max(1, radius * 2), Math.Min(bounds.Width, bounds.Height));
        var path = new GraphicsPath();
        if (diameter <= 2)
        {
            path.AddRectangle(bounds);
            return path;
        }

        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static Rectangle CalculateImageBounds(Size imageSize, Rectangle canvas)
    {
        if (imageSize.Width <= 0 || imageSize.Height <= 0 || canvas.Width <= 0 || canvas.Height <= 0)
        {
            return Rectangle.Empty;
        }

        double scale = Math.Min((double)canvas.Width / imageSize.Width, (double)canvas.Height / imageSize.Height);
        int width = Math.Max(1, (int)Math.Round(imageSize.Width * scale, MidpointRounding.AwayFromZero));
        int height = Math.Max(1, (int)Math.Round(imageSize.Height * scale, MidpointRounding.AwayFromZero));
        int x = canvas.Left + Math.Max(0, (canvas.Width - width) / 2);
        int y = canvas.Top + Math.Max(0, (canvas.Height - height) / 2);
        return new Rectangle(x, y, width, height);
    }

    private sealed class OwnerDrawMenuItemData : IDisposable
    {
        private readonly ThemePalette _palette;
        private readonly string _text;
        private readonly string _shortcutText;
        private readonly Size _textSize;
        private readonly Size _shortcutSize;
        private readonly bool _isSeparator;
        private readonly bool _hasSubMenu;
        private readonly bool _isChecked;
        private readonly bool _radioCheck;
        private readonly Image? _preparedImage;

        internal OwnerDrawMenuItemData(ToolStripItem item, ResolvedVisualStyle visualStyle)
        {
            _palette = visualStyle.Palette;
            _isSeparator = item is ToolStripSeparator;
            _hasSubMenu = item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0;
            _isChecked = item is ToolStripMenuItem menuItem && menuItem.Checked;
            _radioCheck = item is ToolStripMenuItem radioMenuItem && radioMenuItem.RadioCheck;
            _text = ResolveDisplayText(item);
            _shortcutText = item is ToolStripMenuItem shortcutMenuItem
                ? shortcutMenuItem.GetShortcutDisplayText()
                : string.Empty;
            _textSize = MeasureText(_text, useMnemonicPrefix: true);
            _shortcutSize = MeasureText(_shortcutText, useMnemonicPrefix: false);

            if (item.SupportsMenuImage)
            {
                _preparedImage = NativeMenu.PrepareMenuImage(item.Image!, item.ImageTransparentColor);
            }
        }

        internal Size Measure()
        {
            if (_isSeparator)
            {
                return new Size(MinItemWidth, SeparatorHeight);
            }

            int width = (HorizontalPadding * 2) + GlyphColumnWidth + _textSize.Width;
            if (_shortcutSize.Width > 0)
            {
                width += TextShortcutSpacing + _shortcutSize.Width;
            }

            if (_hasSubMenu)
            {
                width += ArrowColumnWidth;
            }

            int contentHeight = Math.Max(_textSize.Height, _preparedImage is null ? 0 : GlyphSize);
            int height = Math.Max(MinItemHeight, contentHeight + (VerticalPadding * 2));
            return new Size(Math.Max(MinItemWidth, width), height);
        }

        internal void Draw(Win32.DRAWITEMSTRUCT drawItem)
        {
            Rectangle bounds = ToRectangle(drawItem.rcItem);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            bool disabled = (drawItem.itemState & (Win32.ODS_DISABLED | Win32.ODS_GRAYED)) != 0;
            bool selected = (drawItem.itemState & Win32.ODS_SELECTED) != 0 && !disabled;

            uint backgroundArgb = _palette.SurfaceBackground;
            uint foregroundArgb = disabled
                ? _palette.DisabledForeground
                : selected
                    ? (_palette.ControlHoverForeground != 0 ? _palette.ControlHoverForeground : _palette.SurfaceForeground)
                    : _palette.SurfaceForeground;

            using var graphics = Graphics.FromHdc(drawItem.hDC);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var backgroundBrush = new SolidBrush(Color.FromArgb(unchecked((int)backgroundArgb))))
            {
                graphics.FillRectangle(backgroundBrush, bounds);
            }

            if (_isSeparator)
            {
                int y = bounds.Top + Math.Max(1, bounds.Height / 2);
                using var pen = new Pen(Color.FromArgb(unchecked((int)_palette.SurfaceBorder)));
                graphics.DrawLine(
                    pen,
                    bounds.Left + HorizontalPadding + GlyphColumnWidth,
                    y,
                    bounds.Right - HorizontalPadding,
                    y);
                return;
            }

            if (selected)
            {
                DrawSelectionBackground(graphics, bounds);
            }

            Win32.TEXTMETRICW fontMetrics = default;
            bool hasFontMetrics = Win32.GetTextMetricsW(drawItem.hDC, out fontMetrics);
            int textHeight = ResolveTextLayoutHeight(hasFontMetrics ? fontMetrics : null);
            int opticalLift = ResolveOpticalLift(hasFontMetrics ? fontMetrics : null);
            int contentHeight = Math.Max(textHeight, _preparedImage is null ? 0 : GlyphSize);
            int contentTop = Math.Max(bounds.Top + 1, bounds.Top + Math.Max(0, (bounds.Height - contentHeight) / 2) - opticalLift);

            Rectangle glyphBounds = new(
                bounds.Left + HorizontalPadding,
                contentTop + Math.Max(0, (contentHeight - GlyphSize) / 2),
                GlyphSize,
                GlyphSize);

            DrawGlyph(graphics, glyphBounds, foregroundArgb, selected, disabled);

            int contentRight = bounds.Right - HorizontalPadding - (_hasSubMenu ? ArrowColumnWidth : 0);
            int textLeft = bounds.Left + HorizontalPadding + GlyphColumnWidth;
            int textRight = contentRight;
            Rectangle shortcutBounds = Rectangle.Empty;

            if (_shortcutSize.Width > 0)
            {
                int shortcutLeft = Math.Max(textLeft, contentRight - _shortcutSize.Width);
                shortcutBounds = new Rectangle(shortcutLeft, contentTop, Math.Max(0, contentRight - shortcutLeft), contentHeight);
                textRight = Math.Max(textLeft, shortcutLeft - TextShortcutSpacing);
            }

            int textTop = contentTop + Math.Max(0, (contentHeight - textHeight) / 2);
            Rectangle textBounds = new Rectangle(textLeft, textTop, Math.Max(0, textRight - textLeft), Math.Max(1, textHeight));
            DrawText(drawItem.hDC, _text, textBounds, foregroundArgb, Win32.DT_LEFT | Win32.DT_SINGLELINE | Win32.DT_HIDEPREFIX);

            if (_shortcutSize.Width > 0)
            {
                uint shortcutColorArgb = disabled
                    ? _palette.DisabledForeground
                    : (_palette.MutedForeground != 0 ? _palette.MutedForeground : foregroundArgb);
                Rectangle alignedShortcutBounds = new(shortcutBounds.Left, textTop, shortcutBounds.Width, Math.Max(1, textHeight));
                DrawText(drawItem.hDC, _shortcutText, alignedShortcutBounds, shortcutColorArgb, Win32.DT_RIGHT | Win32.DT_SINGLELINE | Win32.DT_NOPREFIX);
            }

            if (_hasSubMenu)
            {
                Rectangle arrowBounds = new(
                    bounds.Right - HorizontalPadding - ArrowColumnWidth + Math.Max(0, (ArrowColumnWidth - SubMenuArrowWidth) / 2),
                    contentTop + Math.Max(0, (contentHeight - SubMenuArrowHeight) / 2),
                    SubMenuArrowWidth,
                    SubMenuArrowHeight);
                DrawSubMenuArrow(graphics, arrowBounds, foregroundArgb);
            }

        }

        public void Dispose()
        {
            _preparedImage?.Dispose();
        }

        private void DrawGlyph(Graphics graphics, Rectangle glyphBounds, uint foregroundArgb, bool selected, bool disabled)
        {
            if (_preparedImage is not null)
            {
                Rectangle imageBounds = CalculateImageBounds(_preparedImage.Size, glyphBounds);
                graphics.DrawImage(_preparedImage, imageBounds);
                return;
            }

            if (!_isChecked)
            {
                return;
            }

            Rectangle indicatorBounds = Rectangle.Inflate(glyphBounds, -1, -1);
            if (indicatorBounds.Width <= 0 || indicatorBounds.Height <= 0)
            {
                return;
            }

            uint indicatorBackgroundArgb = BlendOpaque(_palette.SurfaceBackground, _palette.Accent, selected ? 0.32f : 0.20f);
            uint indicatorForegroundArgb = disabled
                ? _palette.DisabledForeground
                : (_palette.SelectionForeground != 0 ? _palette.SelectionForeground : foregroundArgb);

            using (GraphicsPath path = CreateRoundedRectanglePath(indicatorBounds, IndicatorCornerRadius))
            using (var fillBrush = new SolidBrush(Color.FromArgb(unchecked((int)indicatorBackgroundArgb))))
            {
                graphics.FillPath(fillBrush, path);
            }

            using var pen = new Pen(Color.FromArgb(unchecked((int)indicatorForegroundArgb)), 2f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
            };

            if (_radioCheck)
            {
                Rectangle dotBounds = Rectangle.Inflate(indicatorBounds, -4, -4);
                using var dotBrush = new SolidBrush(Color.FromArgb(unchecked((int)indicatorForegroundArgb)));
                graphics.FillEllipse(dotBrush, dotBounds);
                return;
            }

            PointF start = new(indicatorBounds.Left + 3f, indicatorBounds.Top + (indicatorBounds.Height * 0.55f));
            PointF middle = new(indicatorBounds.Left + (indicatorBounds.Width * 0.42f), indicatorBounds.Bottom - 4f);
            PointF end = new(indicatorBounds.Right - 3f, indicatorBounds.Top + 4f);
            graphics.DrawLines(pen, [start, middle, end]);
        }

        private void DrawSelectionBackground(Graphics graphics, Rectangle bounds)
        {
            Rectangle selectionBounds = Rectangle.FromLTRB(
                bounds.Left + SelectionInsetHorizontal,
                bounds.Top + SelectionInsetVertical,
                Math.Max(bounds.Left + SelectionInsetHorizontal + 1, bounds.Right - SelectionInsetHorizontal),
                Math.Max(bounds.Top + SelectionInsetVertical + 1, bounds.Bottom - SelectionInsetVertical));

            if (selectionBounds.Width <= 0 || selectionBounds.Height <= 0)
            {
                return;
            }

            bool darkSurface = IsDarkColor(_palette.SurfaceBackground);
            uint hoverBase = _palette.ControlHoverBackground != 0 ? _palette.ControlHoverBackground : _palette.Accent;
            uint borderBase = _palette.Accent != 0 ? _palette.Accent : (_palette.ControlBorder != 0 ? _palette.ControlBorder : hoverBase);
            uint selectionBackgroundArgb = BlendOpaque(_palette.SurfaceBackground, hoverBase, darkSurface ? 0.34f : 0.52f);
            uint selectionBorderArgb = BlendOpaque(_palette.SurfaceBorder, borderBase, darkSurface ? 0.18f : 0.24f);

            using GraphicsPath path = CreateRoundedRectanglePath(selectionBounds, SelectionCornerRadius);
            using var backgroundBrush = new SolidBrush(Color.FromArgb(unchecked((int)selectionBackgroundArgb)));
            using var borderPen = new Pen(Color.FromArgb(unchecked((int)selectionBorderArgb)));
            graphics.FillPath(backgroundBrush, path);
            graphics.DrawPath(borderPen, path);
        }

        private int ResolveTextLayoutHeight(Win32.TEXTMETRICW? metrics)
        {
            if (string.IsNullOrEmpty(_text))
            {
                return 0;
            }

            if (metrics is Win32.TEXTMETRICW textMetrics)
            {
                int visualHeight = Math.Max(1, textMetrics.tmAscent + textMetrics.tmDescent);
                if (visualHeight > 0)
                {
                    return visualHeight;
                }
            }

            return Math.Max(1, _textSize.Height);
        }

        private int ResolveOpticalLift(Win32.TEXTMETRICW? metrics)
        {
            if (string.IsNullOrEmpty(_text) || !ContainsCjkCharacter(_text))
            {
                return 0;
            }

            float dpi = Win32.GetSystemDpiScaleDimensions().Height;
            if (dpi < 120f)
            {
                return 0;
            }

            if (metrics is Win32.TEXTMETRICW textMetrics && textMetrics.tmHeight > textMetrics.tmAscent + textMetrics.tmDescent)
            {
                return HighDpiOpticalLift;
            }

            return HighDpiOpticalLift;
        }

        private static bool ContainsCjkCharacter(string text)
        {
            foreach (char ch in text)
            {
                if ((ch >= '\u2E80' && ch <= '\u9FFF') || (ch >= '\uF900' && ch <= '\uFAFF'))
                {
                    return true;
                }
            }

            return false;
        }

        private static void DrawSubMenuArrow(Graphics graphics, Rectangle bounds, uint colorArgb)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            using var pen = new Pen(Color.FromArgb(unchecked((int)colorArgb)), 1.8f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round,
            };

            float midY = bounds.Top + (bounds.Height / 2f);
            PointF start = new(bounds.Left + 1f, bounds.Top + 1f);
            PointF middle = new(bounds.Right - 1.5f, midY);
            PointF end = new(bounds.Left + 1f, bounds.Bottom - 1f);
            graphics.DrawLines(pen, [start, middle, end]);
        }

        private static string ResolveDisplayText(ToolStripItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                return item.Text;
            }

            if (item is ToolStripComboBox comboBox && comboBox.SelectedIndex >= 0 && comboBox.SelectedIndex < comboBox.Items.Count)
            {
                return comboBox.Items[comboBox.SelectedIndex]?.ToString() ?? comboBox.Name;
            }

            if (item is ToolStripProgressBar progressBar)
            {
                return $"{progressBar.Name} {progressBar.Value}".Trim();
            }

            return string.IsNullOrWhiteSpace(item.Name)
                ? item.GetType().Name
                : item.Name;
        }
    }
}
