namespace Lumina.Forms;

/// <summary>
/// Represents a month calendar backed by the native common control.
/// </summary>
public class MonthCalendar : Control
{
    /// <inheritdoc />
    protected override string ClassName => "SysMonthCal32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP;

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(160, requestedHeight);

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyExplorerTheme();
    }
}
