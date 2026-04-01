namespace Lumina.Forms;

/// <summary>
/// Represents a WinForms-compatible date picker backed by the native common control.
/// </summary>
public class DateTimePicker : Control
{
    private DateTime _value = DateTime.Now;

    /// <summary>
    /// Gets or sets the current date value.
    /// </summary>
    public DateTime Value
    {
        get => _value;
        set => _value = value;
    }

    /// <inheritdoc />
    protected override string ClassName => "SysDateTimePick32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP;

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(30, requestedHeight);

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyExplorerTheme();
    }

    /// <inheritdoc />
    protected override string GetPreferredThemeClass(ResolvedVisualStyle visualStyle)
        => visualStyle.IsDarkMode && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763)
            ? "DarkMode_CFD"
            : "CFD";

    /// <inheritdoc />
    protected override string GetFallbackThemeClass(ResolvedVisualStyle visualStyle)
        => base.GetPreferredThemeClass(visualStyle);
}
