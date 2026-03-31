using System.Text.Json;
using Lumina.Forms;
using Xunit;

namespace Lumina.Tests;

public class NativeThemeTests
{
    [Fact]
    public void CreateLightTheme_HasExpectedDefaults()
    {
        var theme = NativeTheme.CreateLightTheme();

        Assert.Equal("Lumina Native Light", theme.Name);
        Assert.Equal(ThemeMode.Light, theme.ThemeMode);
        Assert.Equal(VisualStyleKind.Fluent, theme.PreferredVisualStyle);
        Assert.Equal(EffectKind.Blur, theme.PreferredEffect);
        Assert.NotNull(theme.Palette);
    }

    [Fact]
    public void CreateDarkTheme_HasExpectedDefaults()
    {
        var theme = NativeTheme.CreateDarkTheme();

        Assert.Equal("Lumina Native Dark", theme.Name);
        Assert.Equal(ThemeMode.Dark, theme.ThemeMode);
        Assert.Equal(VisualStyleKind.Mica, theme.PreferredVisualStyle);
        Assert.Equal(EffectKind.Mica, theme.PreferredEffect);
        Assert.Equal(0xFF_20_20_24u, theme.Palette.WindowBackground);
    }

    [Fact]
    public void ToJson_FromJson_RoundTrip()
    {
        var original = new NativeTheme
        {
            Name = "Custom Theme",
            Description = "Round-trip test theme.",
            Author = "Lumina.Tests",
            ThemeMode = ThemeMode.Dark,
            PreferredVisualStyle = VisualStyleKind.Fluent,
            PreferredEffect = EffectKind.Acrylic,
            PreferredEffectOptions = new EffectOptions
            {
                BlendColor = 0xCC_12_34_56,
                BlurRadius = 24,
                Opacity = 0.85f,
            },
            Palette = new ThemePalette
            {
                WindowBackground = 0xFF_10_10_12,
                WindowForeground = 0xFF_F5_F5_F5,
                Accent = 0xFF_FF_6B_35,
            },
        };

        string json = original.ToJson();
        var restored = NativeTheme.FromJson(json);

        Assert.Equal(original.Name, restored.Name);
        Assert.Equal(original.Description, restored.Description);
        Assert.Equal(original.Author, restored.Author);
        Assert.Equal(original.ThemeMode, restored.ThemeMode);
        Assert.Equal(original.PreferredVisualStyle, restored.PreferredVisualStyle);
        Assert.Equal(original.PreferredEffect, restored.PreferredEffect);
        Assert.NotNull(restored.PreferredEffectOptions);
        Assert.Equal(original.PreferredEffectOptions!.BlendColor, restored.PreferredEffectOptions!.BlendColor);
        Assert.Equal(original.PreferredEffectOptions.BlurRadius, restored.PreferredEffectOptions.BlurRadius);
        Assert.Equal(original.Palette.WindowBackground, restored.Palette.WindowBackground);
        Assert.Equal(original.Palette.Accent, restored.Palette.Accent);
    }

    [Fact]
    public void FromJson_InvalidJson_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => NativeTheme.FromJson("{not valid}"));
    }

    [Fact]
    public void SaveJson_LoadJson_RoundTrip()
    {
        var original = NativeTheme.CreateDarkTheme();
        string path = Path.Combine(Path.GetTempPath(), $"lumina_theme_{Guid.NewGuid():N}.json");

        try
        {
            original.SaveJson(path);

            Assert.True(File.Exists(path));

            var loaded = NativeTheme.LoadJson(path);
            Assert.Equal(original.Name, loaded.Name);
            Assert.Equal(original.ThemeMode, loaded.ThemeMode);
            Assert.Equal(original.Palette.ControlBackground, loaded.Palette.ControlBackground);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
