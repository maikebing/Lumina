using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a reusable composite control hosted inside a form or another container.
/// </summary>
public class UserControl : ContainerControlBase
{
    /// <summary>
    /// Gets or sets how the control participates in automatic scaling.
    /// </summary>
    public AutoScaleMode AutoScaleMode { get; set; }

    /// <summary>
    /// Gets or sets the design-time scaling dimensions used as the baseline for automatic scaling.
    /// </summary>
    public SizeF AutoScaleDimensions { get; set; }

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_CLIPCHILDREN | Win32.WS_CLIPSIBLINGS;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyNativeThemeState();
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyNativeThemeState();
    }

    private void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
    }
}
