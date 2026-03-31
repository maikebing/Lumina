using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lumina.Forms;

/// <summary>
/// Represents a file-driven LuminaForms theme that can be shared, edited, and loaded at runtime.
/// </summary>
public sealed class NativeTheme
{
    /// <summary>
    /// Gets or sets the display name of the theme.
    /// </summary>
    public string Name { get; set; } = "LuminaForms Theme";

    /// <summary>
    /// Gets or sets an optional short description for the theme.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author or generator of the theme.
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the preferred base theme mode for the theme.
    /// </summary>
    public ThemeMode ThemeMode { get; set; } = ThemeMode.System;

    /// <summary>
    /// Gets or sets an optional preferred Lumina effect for the theme.
    /// </summary>
    public EffectKind? PreferredEffect { get; set; }

    /// <summary>
    /// Gets or sets an optional preferred system-aligned visual style family for the theme.
    /// </summary>
    public VisualStyleKind PreferredVisualStyle { get; set; } = VisualStyleKind.System;

    /// <summary>
    /// Gets or sets optional effect options associated with <see cref="PreferredEffect"/>.
    /// </summary>
    public EffectOptions? PreferredEffectOptions { get; set; }

    /// <summary>
    /// Gets or sets the semantic palette used by the theme.
    /// </summary>
    public ThemePalette Palette { get; set; } = ThemePalette.CreateLight();

    /// <summary>
    /// Serializes the theme to JSON.
    /// </summary>
    /// <returns>A JSON representation of the theme.</returns>
    public string ToJson() =>
        JsonSerializer.Serialize(this, NativeThemeJsonContext.Default.NativeTheme);

    /// <summary>
    /// Deserializes a theme from JSON.
    /// </summary>
    /// <param name="json">The JSON payload to parse.</param>
    /// <returns>The deserialized theme.</returns>
    public static NativeTheme FromJson(string json) =>
        JsonSerializer.Deserialize(json, NativeThemeJsonContext.Default.NativeTheme)
        ?? throw new JsonException("The theme JSON payload produced a null result.");

    /// <summary>
    /// Saves the theme as a JSON file.
    /// </summary>
    /// <param name="path">The output file path.</param>
    public void SaveJson(string path) => File.WriteAllText(path, ToJson());

    /// <summary>
    /// Loads a theme from a JSON file.
    /// </summary>
    /// <param name="path">The input file path.</param>
    /// <returns>The loaded theme.</returns>
    public static NativeTheme LoadJson(string path) => FromJson(File.ReadAllText(path));

    /// <summary>
    /// Creates a default system-following theme.
    /// </summary>
    /// <returns>A new built-in theme aligned to the current Windows theme mode, accent color, and visual style.</returns>
    public static NativeTheme CreateSystemTheme() => new()
    {
        Name = "Lumina System",
        Description = "Built-in theme that follows the current operating system theme mode, backdrop, and accent color.",
        ThemeMode = ThemeMode.System,
        PreferredVisualStyle = VisualStyleKind.System,
        Palette = ThemePalette.CreateSystem(),
    };

    /// <summary>
    /// Creates a default light theme.
    /// </summary>
    /// <returns>A new built-in light theme.</returns>
    public static NativeTheme CreateLightTheme() => new()
    {
        Name = "Lumina Windows 11 Light",
        Description = "Built-in Windows 11 inspired light palette for LuminaForms.",
        ThemeMode = ThemeMode.Light,
        PreferredVisualStyle = VisualStyleKind.Mica,
        PreferredEffect = EffectKind.MicaAlt,
        Palette = ThemePalette.CreateSystem(ThemeMode.Light, VisualStyleKind.Mica),
    };

    /// <summary>
    /// Creates a default dark theme.
    /// </summary>
    /// <returns>A new built-in dark theme.</returns>
    public static NativeTheme CreateDarkTheme() => new()
    {
        Name = "Lumina Windows 11 Dark",
        Description = "Built-in Windows 11 inspired dark palette for LuminaForms.",
        ThemeMode = ThemeMode.Dark,
        PreferredVisualStyle = VisualStyleKind.Mica,
        PreferredEffect = EffectKind.Mica,
        Palette = ThemePalette.CreateSystem(ThemeMode.Dark, VisualStyleKind.Mica),
    };

    /// <summary>
    /// Creates a built-in customizable showcase theme with a richer accent color.
    /// </summary>
    /// <param name="name">The display name for the theme.</param>
    /// <param name="themeMode">The preferred light or dark mode.</param>
    /// <param name="accent">The custom accent color in ARGB format.</param>
    /// <param name="preferredVisualStyle">The preferred system-aligned visual style family.</param>
    /// <returns>A new built-in custom theme.</returns>
    public static NativeTheme CreateCustomTheme(
        string name = "Lumina Showcase",
        ThemeMode themeMode = ThemeMode.Dark,
        uint accent = 0xFF_8B_5C_F6,
        VisualStyleKind preferredVisualStyle = VisualStyleKind.System) => new()
    {
        Name = name,
        Description = "Built-in custom theme preset with a configurable accent color for demo and testing scenarios.",
        ThemeMode = themeMode,
        PreferredVisualStyle = preferredVisualStyle,
        Palette = ThemePalette.CreateCustom(themeMode, accent, preferredVisualStyle),
    };
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(NativeTheme))]
[JsonSerializable(typeof(ThemePalette))]
[JsonSerializable(typeof(EffectOptions))]
internal partial class NativeThemeJsonContext : JsonSerializerContext
{
}
