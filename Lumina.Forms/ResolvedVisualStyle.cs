namespace Lumina.Forms;

/// <summary>
/// Represents the effective visual style resolved from application settings and the current operating system.
/// </summary>
public sealed class ResolvedVisualStyle
{
    internal ResolvedVisualStyle(
        ThemeMode themeMode,
        VisualStyleKind visualStyleKind,
        EffectKind effectKind,
        EffectOptions? effectOptions,
        NativeTheme? theme,
        ThemePalette palette)
    {
        ThemeMode = themeMode;
        VisualStyleKind = visualStyleKind;
        EffectKind = effectKind;
        EffectOptions = effectOptions;
        Theme = theme;
        Palette = palette;
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
    /// Gets the system-aligned visual style family chosen for the current window.
    /// </summary>
    public VisualStyleKind VisualStyleKind { get; }

    /// <summary>
    /// Gets the default Lumina effect that should be applied to new windows.
    /// </summary>
    public EffectKind EffectKind { get; }

    /// <summary>
    /// Gets the options associated with the resolved effect, if any.
    /// </summary>
    public EffectOptions? EffectOptions { get; }

    /// <summary>
    /// Gets the active file-driven theme, if one was supplied by the application.
    /// </summary>
    public NativeTheme? Theme { get; }

    /// <summary>
    /// Gets the resolved semantic palette for the current visual style.
    /// </summary>
    public ThemePalette Palette { get; }
}
