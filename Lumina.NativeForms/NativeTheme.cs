using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lumina.NativeForms;

/// <summary>
/// Represents a file-driven NativeForms theme that can be shared, edited, and loaded at runtime.
/// </summary>
public sealed class NativeTheme
{
    /// <summary>
    /// Gets or sets the display name of the theme.
    /// </summary>
    public string Name { get; set; } = "NativeForms Theme";

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
    /// Creates a default light theme.
    /// </summary>
    /// <returns>A new built-in light theme.</returns>
    public static NativeTheme CreateLightTheme() => new()
    {
        Name = "Lumina Native Light",
        Description = "Built-in light palette for NativeForms.",
        ThemeMode = ThemeMode.Light,
        PreferredVisualStyle = VisualStyleKind.Fluent,
        PreferredEffect = EffectKind.Blur,
        PreferredEffectOptions = new EffectOptions { BlurRadius = 18 },
        Palette = ThemePalette.CreateLight(VisualStyleKind.Fluent),
    };

    /// <summary>
    /// Creates a default dark theme.
    /// </summary>
    /// <returns>A new built-in dark theme.</returns>
    public static NativeTheme CreateDarkTheme() => new()
    {
        Name = "Lumina Native Dark",
        Description = "Built-in dark palette for NativeForms.",
        ThemeMode = ThemeMode.Dark,
        PreferredVisualStyle = VisualStyleKind.Mica,
        PreferredEffect = EffectKind.Mica,
        Palette = ThemePalette.CreateDark(VisualStyleKind.Mica),
    };
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(NativeTheme))]
[JsonSerializable(typeof(ThemePalette))]
[JsonSerializable(typeof(EffectOptions))]
internal partial class NativeThemeJsonContext : JsonSerializerContext
{
}
