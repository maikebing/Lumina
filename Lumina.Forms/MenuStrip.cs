using System.Drawing;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents a menu bar hosted on a form.
/// </summary>
public class MenuStrip : ToolStrip
{
    private const int MenuItemHorizontalPadding = 12;

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
            x += hostSize.Width;
        }
    }

    private protected override Size ResolveHostSize(ToolStripItem item, int availableHeight)
    {
        Size textSize = MeasureItemText(item);
        int width = Math.Max(MenuItemHorizontalPadding, textSize.Width + MenuItemHorizontalPadding);
        int resolvedWidth = item.Size.Width > 0 ? item.Size.Width : width;
        int resolvedHeight = item.Size.Height > 0 ? item.Size.Height : Math.Max(1, availableHeight);
        return new Size(resolvedWidth, resolvedHeight);
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
        private const int TextInset = 4;

        public event EventHandler? Click;

        private bool _hovered;

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
                    if (!_hovered)
                    {
                        _hovered = true;
                        BackColor = Color.FromArgb(unchecked((int)CurrentVisualStyle.Palette.ControlHoverBackground));
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
                        BackColor = Color.Empty;
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

                uint backgroundArgb = _hovered && BackColor != Color.Empty
                    ? unchecked((uint)BackColor.ToArgb())
                    : CurrentVisualStyle.Palette.SurfaceBackground;
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

                _ = Win32.SetBkMode(hdc, Win32.TRANSPARENT);
                _ = Win32.SetTextColor(hdc, Win32.ToColorRef(CurrentVisualStyle.Palette.SurfaceForeground));

                var textBounds = new Win32.RECT
                {
                    Left = TextInset,
                    Top = clientRect.Top,
                    Right = Math.Max(TextInset, clientRect.Right - TextInset),
                    Bottom = clientRect.Bottom,
                };

                string text = Text ?? string.Empty;
                _ = Win32.DrawTextW(
                    hdc,
                    text,
                    text.Length,
                    ref textBounds,
                    Win32.DT_LEFT | Win32.DT_VCENTER | Win32.DT_SINGLELINE | Win32.DT_HIDEPREFIX);
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
    }
}
