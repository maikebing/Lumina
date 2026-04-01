namespace Lumina.Forms;

/// <summary>
/// Represents a WinForms-compatible list view backed by the native common control.
/// </summary>
public class ListView : Control
{
    private const uint ModernExtendedStyles =
        Win32.LVS_EX_DOUBLEBUFFER |
        Win32.LVS_EX_LABELTIP |
        Win32.LVS_EX_FULLROWSELECT;

    /// <summary>
    /// Initializes a list view with more spacious default layout metrics.
    /// </summary>
    public ListView()
    {
        Margin = new Padding(6);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control should preserve legacy image behavior.
    /// </summary>
    public bool UseCompatibleStateImageBehavior { get; set; } = true;

    /// <inheritdoc />
    protected override string ClassName => "SysListView32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_VSCROLL;

    /// <inheritdoc />
    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyExplorerTheme();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyExtendedStyles();
    }

    /// <inheritdoc />
    protected override string GetPreferredThemeClass(ResolvedVisualStyle visualStyle)
        => visualStyle.IsDarkMode && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763)
            ? "DarkMode_Explorer"
            : "ItemsView";

    /// <inheritdoc />
    protected override string GetFallbackThemeClass(ResolvedVisualStyle visualStyle)
        => "Explorer";

    private void ApplyExtendedStyles()
    {
        if (Handle == 0)
        {
            return;
        }

        _ = Win32.SendMessageW(
            Handle,
            Win32.LVM_SETEXTENDEDLISTVIEWSTYLE,
            (nint)ModernExtendedStyles,
            (nint)ModernExtendedStyles);
    }
}
