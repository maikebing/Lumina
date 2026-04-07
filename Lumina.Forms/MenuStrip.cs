using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a menu bar hosted on a form.
/// </summary>
public class MenuStrip : ToolStrip
{
    private NativeMenu? _nativeMenu;

    internal bool UsesNativeMenuBar => OperatingSystem.IsWindows();

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
        if (!UsesNativeMenuBar)
        {
            ReleaseNativeMenu();
            return;
        }

        _nativeMenu?.Dispose();
        DarkModeNative.RefreshImmersiveState();
        _nativeMenu = NativeMenu.CreateMenuBar(Items, Owner?.CurrentVisualStyle ?? Application.CurrentVisualStyle);
    }

    internal nint GetNativeMenuHandle()
        => _nativeMenu?.Handle ?? 0;

    internal bool TryHandleNativeCommand(int commandId)
    {
        if (_nativeMenu is null || !_nativeMenu.TryGetCommand(unchecked((uint)commandId), out ToolStripItem item))
        {
            return false;
        }

        item.PerformClick();
        return true;
    }

    internal void ReleaseNativeMenu()
    {
        _nativeMenu?.Dispose();
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

        int height = Math.Max(28, Height);
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
        public event EventHandler? Click;

        private bool _hovered;

        private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

        private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

        protected override bool UseParentBackgroundForTheming => false;

        protected override uint Style => base.Style | Win32.SS_NOTIFY;

        protected override int GetNativeHeight(int requestedHeight)
            => Math.Max(26, requestedHeight);

        protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
        {
            switch (message)
            {
                case Win32.WM_MOUSEMOVE:
                    if (!_hovered)
                    {
                        _hovered = true;
                        BackColor = Color.FromArgb(unchecked((int)CurrentVisualStyle.Palette.ControlHoverBackground));
                    }

                    result = 0;
                    return false;

                case Win32.WM_MOUSELEAVE:
                    if (_hovered)
                    {
                        _hovered = false;
                        BackColor = Color.Empty;
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
    }
}
