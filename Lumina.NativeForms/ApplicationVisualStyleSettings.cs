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
    /// Gets or sets the preferred window effect. Set to <c>null</c> to let NativeForms pick the OS-appropriate default.
    /// </summary>
    public EffectKind? PreferredEffect { get; set; }

    /// <summary>
    /// Gets or sets the options used for the preferred effect. When omitted, NativeForms chooses sensible defaults.
    /// </summary>
    public EffectOptions? PreferredEffectOptions { get; set; }
}
