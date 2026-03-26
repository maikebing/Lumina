namespace Lumina.NativeForms;

/// <summary>
/// Represents the effective visual style resolved from application settings and the current operating system.
/// </summary>
public sealed class ResolvedVisualStyle
{
    internal ResolvedVisualStyle(ThemeMode themeMode, EffectKind effectKind, EffectOptions? effectOptions)
    {
        ThemeMode = themeMode;
        EffectKind = effectKind;
        EffectOptions = effectOptions;
    }

    /// <summary>
    /// Gets the resolved light or dark mode that should be applied to windows.
    /// </summary>
    public ThemeMode ThemeMode { get; }

    /// <summary>
    /// Gets a value indicating whether the resolved appearance is dark.
    /// </summary>
    public bool IsDarkMode => ThemeMode == ThemeMode.Dark;

    /// <summary>
    /// Gets the default Lumina effect that should be applied to new windows.
    /// </summary>
    public EffectKind EffectKind { get; }

    /// <summary>
    /// Gets the options associated with the resolved effect, if any.
    /// </summary>
    public EffectOptions? EffectOptions { get; }
}
