namespace Lumina.Forms;

/// <summary>
/// Represents a menu bar hosted on a form.
/// </summary>
public class MenuStrip : ToolStrip
{
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

    private sealed class TopLevelMenuItemHost : Label
    {
        public event EventHandler? Click;

        protected override uint Style => base.Style | Win32.SS_NOTIFY;

        protected override int GetNativeHeight(int requestedHeight)
            => Math.Max(20, requestedHeight);

        protected override bool OnCommand(int notificationCode)
        {
            if (notificationCode != Win32.STN_CLICKED)
            {
                return false;
            }

            Click?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
