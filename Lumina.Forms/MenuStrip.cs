using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents a menu bar hosted on a form.
/// </summary>
public class MenuStrip : ToolStrip
{
    private const int TopLevelContentHorizontalPadding = 6;
    private const int TopLevelContentVerticalPadding = 1;
    private const int MenuItemImageSize = 16;
    private const int MenuItemImageSpacing = 6;
    private const int TopLevelItemSpacing = 6;

    private NativeMenu? _nativeMenu;

    internal bool UsesNativeMenuBar => false;

    /// <summary>
    /// Initializes a menu strip with WinForms-compatible top-level defaults.
    /// </summary>
    public MenuStrip()
    {
        Size = new Size(200, 24);
        CanOverflow = false;
        GripStyle = ToolStripGripStyle.Hidden;
        Stretch = true;
        ShowItemToolTips = false;
        Padding = new Padding(6, 2, 0, 2);
    }

    /// <inheritdoc />
    protected override bool ShouldCreateNativeHandle => !UsesNativeMenuBar;

    /// <inheritdoc />
    public override void PerformLayout()
    {
        if (!UsesNativeMenuBar)
        {
            DockToTop();
        }

        base.PerformLayout();
    }

    private protected override void LayoutItemHosts()
    {
        int availableHeight = Math.Max(1, Height - Padding.Vertical);
        int x = Padding.Left;
        int y = Padding.Top;

        foreach (ToolStripItem item in Items)
        {
            if (!TryGetItemHost(item, out Control? host) || host is null)
            {
                continue;
            }

            ApplyItemState(host, item);
            host.Visible = item.Visible;
            if (!item.Visible)
            {
                continue;
            }

            Size hostSize = ResolveHostSize(item, availableHeight);
            int itemHeight = Math.Min(availableHeight, Math.Max(1, hostSize.Height));
            host.SetBounds(x, y, hostSize.Width, itemHeight);
            x += hostSize.Width + TopLevelItemSpacing;
        }
    }

    private protected override Size ResolveHostSize(ToolStripItem item, int availableHeight)
    {
        string text = ShouldShowTopLevelText(item) ? ResolveItemText(item) : string.Empty;
        Size textSize = string.IsNullOrEmpty(text)
            ? Size.Empty
            : MeasureTextCore(text, useMnemonicPrefix: true);

        int width = TopLevelContentHorizontalPadding;
        if (ShouldShowTopLevelImage(item))
        {
            width += MenuItemImageSize;
            if (textSize.Width > 0)
            {
                width += MenuItemImageSpacing;
            }
        }

        width += textSize.Width + TopLevelContentHorizontalPadding;
        int resolvedWidth = item.Size.Width > 0 ? Math.Max(item.Size.Width, width) : width;
        int resolvedHeight = item.Size.Height > 0 ? item.Size.Height : Math.Max(1, availableHeight);
        return new Size(resolvedWidth, resolvedHeight);
    }

    private protected override void ApplyItemState(Control host, ToolStripItem item)
    {
        if (host is TopLevelMenuItemHost topLevelMenuItemHost)
        {
            topLevelMenuItemHost.SetContent(
                ShouldShowTopLevelText(item) ? ResolveItemText(item) : string.Empty,
                ShouldShowTopLevelImage(item) ? item.Image : null);
            topLevelMenuItemHost.Enabled = item.Enabled;
            return;
        }

        base.ApplyItemState(host, item);
    }

    private protected override void OnDropDownStateChanged(ToolStripDropDownItem item, Control host, bool isOpen)
    {
        if (host is TopLevelMenuItemHost topLevelMenuItemHost)
        {
            topLevelMenuItemHost.SetPopupOpen(isOpen);
        }
    }

    /// <inheritdoc />
    private protected override bool ShouldCreateHostControl(ToolStripItem item)
        => !UsesNativeMenuBar && base.ShouldCreateHostControl(item);

    /// <summary>
    /// Creates a menu-style top-level host for a direct command item.
    /// </summary>
    /// <param name="item">The menu item that needs a host.</param>
    /// <returns>A non-button host that behaves like a menu caption.</returns>
    protected override Control CreateButtonHost(ToolStripItem item)
    {
        var host = new TopLevelMenuItemHost();
        host.Click += (_, _) => item.PerformClick();
        return host;
    }

    /// <summary>
    /// Creates a menu-style top-level host for a drop-down item.
    /// </summary>
    /// <param name="item">The drop-down item that needs a host.</param>
    /// <returns>A non-button host that opens the drop-down menu.</returns>
    protected override Control CreateDropDownHost(ToolStripItem item)
    {
        var host = new TopLevelMenuItemHost();
        host.Click += (_, _) => ShowDropDownWithSiblingNavigation((ToolStripDropDownItem)item, host);
        return host;
    }

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

    internal void SynchronizeNativeMenu()
    {
        if (!OperatingSystem.IsWindows() || !UsesNativeMenuBar)
        {
            ReleaseNativeMenu();
            return;
        }

        _nativeMenu?.Dispose();
        DarkModeNative.RefreshImmersiveState();
        _nativeMenu = NativeMenu.CreateMenuBar(Items, Owner?.CurrentVisualStyle ?? Application.CurrentVisualStyle);
    }

    internal nint GetNativeMenuHandle()
        => OperatingSystem.IsWindows()
            ? _nativeMenu?.Handle ?? 0
            : 0;

    internal bool TryHandleNativeCommand(int commandId)
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        if (_nativeMenu is null || !_nativeMenu.TryGetCommand(unchecked((uint)commandId), out ToolStripItem item))
        {
            return false;
        }

        item.PerformClick();
        return true;
    }

    internal void ReleaseNativeMenu()
    {
        if (OperatingSystem.IsWindows())
        {
            _nativeMenu?.Dispose();
        }

        _nativeMenu = null;
    }

    private static bool ShouldShowTopLevelImage(ToolStripItem item)
        => item.Image is not null
            && item.DisplayStyle is ToolStripItemDisplayStyle.Image or ToolStripItemDisplayStyle.ImageAndText;

    private static bool ShouldShowTopLevelText(ToolStripItem item)
        => item.DisplayStyle is ToolStripItemDisplayStyle.Text or ToolStripItemDisplayStyle.ImageAndText;

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

    private void DockToTop()
    {
        if (Owner is null)
        {
            return;
        }

        int clientWidth = Owner.ClientSize.Width;
        if (Owner.Handle != 0 && Win32.GetClientRect(Owner.Handle, out var clientRect))
        {
            clientWidth = clientRect.Width;
        }

        int height = Math.Max(1, Height);
        int width = Math.Max(1, clientWidth);

        if (Left == 0 && Top == 0 && Width == width && Height == height)
        {
            return;
        }

        SetBounds(0, 0, width, height);
    }

    /// <inheritdoc />
    protected override void OnDisposing()
    {
        ReleaseNativeMenu();
        base.OnDisposing();
    }

    private sealed class TopLevelMenuItemHost : Label
    {
        private const int HoverInsetHorizontal = 2;
        private const int HoverInsetVertical = 1;
        private const int HoverCornerRadius = 6;
        private const int HighDpiOpticalLift = 1;

        public event EventHandler? Click;

        private bool _hovered;
        private bool _popupOpen;
        private Image? _image;

        private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

        private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

        protected override bool UseParentBackgroundForTheming => false;

        protected override uint Style => base.Style | Win32.SS_NOTIFY;

        protected override int GetNativeHeight(int requestedHeight)
            => requestedHeight;

        protected override void OnHandleCreated()
        {
            base.OnHandleCreated();
            Refresh();
        }

        public void SetContent(string text, Image? image)
        {
            Text = text;
            _image = image;
            Refresh();
        }

        public void SetPopupOpen(bool isOpen)
        {
            if (_popupOpen == isOpen)
            {
                return;
            }

            _popupOpen = isOpen;
            Refresh();
        }

        protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
        {
            switch (message)
            {
                case Win32.WM_ERASEBKGND:
                    result = 1;
                    return true;

                case Win32.WM_PAINT:
                    PaintHost();
                    result = 0;
                    return true;

                case Win32.WM_MOUSEMOVE:
                    if (Enabled && !_hovered)
                    {
                        _hovered = true;
                        Refresh();

                        if (Handle != 0)
                        {
                            var track = new Win32.TRACKMOUSEEVENT
                            {
                                cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<Win32.TRACKMOUSEEVENT>(),
                                dwFlags = Win32.TME_LEAVE,
                                hwndTrack = Handle,
                            };

                            _ = Win32.TrackMouseEvent(ref track);
                        }
                    }

                    result = 0;
                    return false;

                case Win32.WM_MOUSELEAVE:
                    if (_hovered)
                    {
                        _hovered = false;
                        Refresh();
                    }

                    result = 0;
                    return false;

                case Win32.WM_LBUTTONUP:
                    Click?.Invoke(this, EventArgs.Empty);
                    result = 0;
                    return true;
            }

            result = 0;
            return false;
        }

        private void PaintHost()
        {
            if (Handle == 0)
            {
                return;
            }

            nint hdc = Win32.BeginPaint(Handle, out Win32.PAINTSTRUCT paintStruct);
            if (hdc == 0)
            {
                return;
            }

            nint backgroundBrush = 0;
            bool ownsBrush = false;
            nint previousFont = 0;

            try
            {
                if (!Win32.GetClientRect(Handle, out Win32.RECT clientRect))
                {
                    return;
                }

                uint backgroundArgb = CurrentVisualStyle.Palette.SurfaceBackground;
                backgroundBrush = Win32.CreateSolidBrush(Win32.ToColorRef(backgroundArgb));
                ownsBrush = backgroundBrush != 0;
                if (backgroundBrush != 0)
                {
                    _ = Win32.FillRect(hdc, ref clientRect, backgroundBrush);
                }

                nint fontHandle = Owner?.UiFontHandle ?? 0;
                if (fontHandle == 0)
                {
                    fontHandle = Win32.GetStockObject(Win32.DEFAULT_GUI_FONT);
                }

                if (fontHandle != 0)
                {
                    previousFont = Win32.SelectObject(hdc, fontHandle);
                }

                using var graphics = Graphics.FromHdc(hdc);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle clientBounds = Rectangle.FromLTRB(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom);
                bool drawHighlight = Enabled && (_hovered || _popupOpen);
                if (drawHighlight)
                {
                    DrawHighlightBackground(graphics, clientBounds, isActive: _popupOpen);
                }

                Rectangle contentBounds = Rectangle.FromLTRB(
                    clientBounds.Left + TopLevelContentHorizontalPadding,
                    clientBounds.Top + TopLevelContentVerticalPadding,
                    Math.Max(clientBounds.Left + TopLevelContentHorizontalPadding + 1, clientBounds.Right - TopLevelContentHorizontalPadding),
                    Math.Max(clientBounds.Top + TopLevelContentVerticalPadding + 1, clientBounds.Bottom - TopLevelContentVerticalPadding));

                string text = Text ?? string.Empty;
                Win32.TEXTMETRICW fontMetrics = default;
                bool hasFontMetrics = !string.IsNullOrEmpty(text) && Win32.GetTextMetricsW(hdc, out fontMetrics);
                int textLeft = contentBounds.Left;
                if (_image is not null)
                {
                    textLeft += MenuItemImageSize + (string.IsNullOrEmpty(text) ? 0 : MenuItemImageSpacing);
                }

                int maxTextWidth = Math.Max(1, contentBounds.Right - textLeft);
                int textHeight = ResolveTextLayoutHeight(hdc, text, maxTextWidth, hasFontMetrics ? fontMetrics : null);
                int contentHeight = Math.Max(_image is null ? 0 : MenuItemImageSize, textHeight);
                int opticalLift = ResolveOpticalLift(text, hasFontMetrics ? fontMetrics : null);
                int contentTop = Math.Max(contentBounds.Top, contentBounds.Top + Math.Max(0, (contentBounds.Height - contentHeight) / 2) - opticalLift);

                if (_image is not null)
                {
                    Rectangle imageBounds = new(
                        contentBounds.Left,
                        contentTop + Math.Max(0, (contentHeight - MenuItemImageSize) / 2),
                        MenuItemImageSize,
                        MenuItemImageSize);
                    graphics.DrawImage(_image, imageBounds);
                }

                _ = Win32.SetBkMode(hdc, Win32.TRANSPARENT);
                uint textArgb = !Enabled
                    ? CurrentVisualStyle.Palette.DisabledForeground
                    : _popupOpen && CurrentVisualStyle.Palette.ControlPressedForeground != 0
                        ? CurrentVisualStyle.Palette.ControlPressedForeground
                        : CurrentVisualStyle.Palette.SurfaceForeground;
                _ = Win32.SetTextColor(hdc, Win32.ToColorRef(textArgb));

                int textTop = contentTop + Math.Max(0, (contentHeight - textHeight) / 2);
                var textBounds = new Win32.RECT
                {
                    Left = textLeft,
                    Top = textTop,
                    Right = Math.Max(textLeft + 1, contentBounds.Right),
                    Bottom = textTop + Math.Max(1, textHeight),
                };

                if (!string.IsNullOrEmpty(text))
                {
                    _ = Win32.DrawTextW(
                        hdc,
                        text,
                        text.Length,
                        ref textBounds,
                        Win32.DT_LEFT | Win32.DT_SINGLELINE | Win32.DT_HIDEPREFIX);
                }
            }
            finally
            {
                if (previousFont != 0)
                {
                    _ = Win32.SelectObject(hdc, previousFont);
                }

                if (ownsBrush)
                {
                    _ = Win32.DeleteObject(backgroundBrush);
                }

                _ = Win32.EndPaint(Handle, ref paintStruct);
            }
        }

        private int ResolveTextLayoutHeight(nint hdc, string text, int maxTextWidth, Win32.TEXTMETRICW? metrics)
        {
            if (string.IsNullOrEmpty(text))
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

            return MeasureTextHeight(hdc, text, maxTextWidth);
        }

        private static int ResolveOpticalLift(string text, Win32.TEXTMETRICW? metrics)
        {
            if (string.IsNullOrEmpty(text) || !ContainsCjkCharacter(text))
            {
                return 0;
            }

            float dpi = Win32.GetSystemDpiScaleDimensions().Height;
            if (dpi < 120f)
            {
                return 0;
            }

            if (metrics is Win32.TEXTMETRICW textMetrics)
            {
                int visualHeight = textMetrics.tmAscent + textMetrics.tmDescent;
                if (textMetrics.tmHeight > visualHeight)
                {
                    return HighDpiOpticalLift;
                }
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

        private void DrawHighlightBackground(Graphics graphics, Rectangle clientBounds, bool isActive)
        {
            Rectangle hoverBounds = Rectangle.FromLTRB(
                clientBounds.Left + HoverInsetHorizontal,
                clientBounds.Top + HoverInsetVertical,
                Math.Max(clientBounds.Left + HoverInsetHorizontal + 1, clientBounds.Right - HoverInsetHorizontal),
                Math.Max(clientBounds.Top + HoverInsetVertical + 1, clientBounds.Bottom - HoverInsetVertical));

            if (hoverBounds.Width <= 0 || hoverBounds.Height <= 0)
            {
                return;
            }

            using GraphicsPath path = CreateRoundedRectanglePath(hoverBounds, HoverCornerRadius);
            ThemePalette palette = CurrentVisualStyle.Palette;
            uint hoverBase = palette.ControlHoverBackground != 0 ? palette.ControlHoverBackground : palette.Accent;
            uint activeBase = palette.ControlPressedBackground != 0 ? palette.ControlPressedBackground : hoverBase;
            uint backgroundArgb = isActive
                ? BlendOpaque(palette.SurfaceBackground, activeBase, CurrentVisualStyle.IsDarkMode ? 0.72f : 0.88f)
                : BlendOpaque(palette.SurfaceBackground, hoverBase, CurrentVisualStyle.IsDarkMode ? 0.28f : 0.45f);
            uint borderArgb = isActive
                ? BlendOpaque(palette.SurfaceBorder, palette.Accent != 0 ? palette.Accent : palette.ControlBorder, CurrentVisualStyle.IsDarkMode ? 0.56f : 0.46f)
                : BlendOpaque(palette.SurfaceBorder, palette.ControlBorder != 0 ? palette.ControlBorder : hoverBase, CurrentVisualStyle.IsDarkMode ? 0.18f : 0.24f);
            using var backgroundBrush = new SolidBrush(Color.FromArgb(unchecked((int)backgroundArgb)));
            graphics.FillPath(backgroundBrush, path);

            if (isActive)
            {
                using var borderPen = new Pen(Color.FromArgb(unchecked((int)borderArgb)));
                graphics.DrawPath(borderPen, path);
            }
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = Math.Max(1, radius * 2);
            var path = new GraphicsPath();

            if (bounds.Width <= diameter || bounds.Height <= diameter)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static int MeasureTextHeight(nint hdc, string text, int maxWidth)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var measurementBounds = new Win32.RECT
            {
                Left = 0,
                Top = 0,
                Right = Math.Max(1, maxWidth),
                Bottom = 0,
            };

            _ = Win32.DrawTextW(
                hdc,
                text,
                text.Length,
                ref measurementBounds,
                Win32.DT_LEFT | Win32.DT_SINGLELINE | Win32.DT_HIDEPREFIX | Win32.DT_CALCRECT);

            return Math.Max(1, measurementBounds.Bottom - measurementBounds.Top);
        }
    }
}

