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
        ApplyNativeThemeState();
        ApplyExplorerTheme();
        ApplyNativeColors();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyNativeThemeState();
        ApplyNativeColors();
    }

    private void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
    }

    /// <inheritdoc />
    protected override string GetPreferredThemeClass(ResolvedVisualStyle visualStyle)
        => visualStyle.IsDarkMode && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763)
            ? "Explorer"
            : "CFD";

    /// <inheritdoc />
    protected override string GetFallbackThemeClass(ResolvedVisualStyle visualStyle)
        => base.GetPreferredThemeClass(visualStyle);

    private void ApplyNativeColors()
    {
        if (Handle == 0)
        {
            return;
        }

        ThemePalette palette = CurrentVisualStyle.Palette;
        uint controlBackground = Win32.ToColorRef(palette.ControlBackground);
        uint controlForeground = Win32.ToColorRef(palette.ControlForeground);
        uint surfaceBackground = Win32.ToColorRef(palette.SurfaceBackground);
        uint mutedForeground = Win32.ToColorRef(palette.MutedForeground);

        _ = SetCalendarColor(Win32.MCSC_BACKGROUND, surfaceBackground);
        _ = SetCalendarColor(Win32.MCSC_MONTHBK, controlBackground);
        _ = SetCalendarColor(Win32.MCSC_TEXT, controlForeground);
        _ = SetCalendarColor(Win32.MCSC_TITLEBK, surfaceBackground);
        _ = SetCalendarColor(Win32.MCSC_TITLETEXT, controlForeground);
        _ = SetCalendarColor(Win32.MCSC_TRAILINGTEXT, mutedForeground);
    }

    private nint SetCalendarColor(int colorIndex, uint colorRef)
    {
        return Win32.SendMessageW(Handle, Win32.MCM_SETCOLOR, (nint)colorIndex, unchecked((nint)colorRef));
    }
}
