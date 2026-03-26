namespace Lumina.NativeForms;

/// <summary>
/// Defines the application-wide visual style defaults used by NativeForms windows.
/// </summary>
public sealed class ApplicationVisualStyleSettings
{
    /// <summary>
    /// Gets or sets how the application chooses light or dark mode.
    /// </summary>
    public ThemeMode ThemeMode { get; set; } = ThemeMode.System;

    /// <summary>
    /// Gets or sets a value indicating whether NativeForms should apply a Lumina backdrop by default.
    /// </summary>
    public bool ApplyBackdropEffects { get; set; } = true;

    /// <summary>
    /// Gets or sets the preferred system-aligned visual style. Set to <see cref="VisualStyleKind.System"/>
    /// to let NativeForms detect the best default from the current operating system.
    /// </summary>
    public VisualStyleKind PreferredVisualStyle { get; set; } = VisualStyleKind.System;

    /// <summary>
    /// Gets or sets the preferred window effect. Set to <c>null</c> to let NativeForms pick the OS-appropriate default.
    /// </summary>
    public EffectKind? PreferredEffect { get; set; }

    /// <summary>
    /// Gets or sets the options used for the preferred effect. When omitted, NativeForms chooses sensible defaults.
    /// </summary>
    public EffectOptions? PreferredEffectOptions { get; set; }

    /// <summary>
    /// Gets or sets an application-wide semantic palette override. This can be used without supplying a full theme file.
    /// </summary>
    public ThemePalette? Palette { get; set; }

    /// <summary>
    /// Gets or sets the active file-driven theme. When set, it can supply a preferred theme mode, effect, and palette.
    /// </summary>
    public NativeTheme? Theme { get; set; }
}
