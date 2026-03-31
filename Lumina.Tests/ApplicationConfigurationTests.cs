using Lumina.Forms;
using Xunit;

namespace Lumina.Tests;

public class ApplicationConfigurationTests
{
    [Fact]
    public void Initialize_AppliesNativeFormsStartupDefaults()
    {
        try
        {
            Application.ConfigureVisualStyles(settings =>
            {
                settings.ThemeMode = ThemeMode.Dark;
                settings.PreferredVisualStyle = VisualStyleKind.Mica;
                settings.PreferredEffect = EffectKind.Mica;
                settings.Palette = null;
                settings.Theme = null;
            });

            ApplicationConfiguration.Initialize();

            ResolvedVisualStyle resolvedStyle = Application.CurrentVisualStyle;

            Assert.Equal(ThemeMode.Dark, resolvedStyle.ThemeMode);
            Assert.Equal(VisualStyleKind.Mica, resolvedStyle.VisualStyleKind);
            Assert.Equal(EffectKind.Mica, resolvedStyle.EffectKind);
        }
        finally
        {
            Application.ConfigureVisualStyles(settings =>
            {
                settings.ThemeMode = ThemeMode.System;
                settings.ApplyBackdropEffects = true;
                settings.PreferredVisualStyle = VisualStyleKind.System;
                settings.PreferredEffect = null;
                settings.PreferredEffectOptions = null;
                settings.Palette = null;
                settings.Theme = null;
            });
        }
    }

    [Fact]
    public void FormCurrentVisualStyle_UsesWindowThemeAndPaletteOverrides()
    {
        try
        {
            Application.ConfigureVisualStyles(settings =>
            {
                settings.ThemeMode = ThemeMode.System;
                settings.ApplyBackdropEffects = true;
                settings.PreferredVisualStyle = VisualStyleKind.System;
                settings.PreferredEffect = null;
                settings.PreferredEffectOptions = null;
                settings.Palette = null;
                settings.Theme = null;
            });

            using var form = new Form();
            var theme = new NativeTheme
            {
                Name = "Window Override",
                ThemeMode = ThemeMode.Dark,
                PreferredVisualStyle = VisualStyleKind.Fluent,
                PreferredEffect = EffectKind.Acrylic,
                PreferredEffectOptions = new EffectOptions
                {
                    BlendColor = 0xCC_12_34_56,
                    BlurRadius = 16,
                    Opacity = 0.8f,
                },
                Palette = ThemePalette.CreateDark(VisualStyleKind.Fluent),
            };

            var paletteOverride = ThemePalette.CreateDark(VisualStyleKind.Fluent);
            paletteOverride.Accent = 0xFF_FF_7A_00;

            form.UseTheme(theme);
            form.SetPalette(paletteOverride);

            ResolvedVisualStyle resolvedStyle = form.CurrentVisualStyle;

            Assert.Equal(ThemeMode.Dark, resolvedStyle.ThemeMode);
            Assert.Equal(VisualStyleKind.Fluent, resolvedStyle.VisualStyleKind);
            Assert.Equal(EffectKind.Acrylic, resolvedStyle.EffectKind);
            Assert.Equal(0xCC_12_34_56u, resolvedStyle.EffectOptions!.BlendColor);
            Assert.Equal(0xFF_FF_7A_00u, resolvedStyle.Palette.Accent);
        }
        finally
        {
            Application.ConfigureVisualStyles(settings =>
            {
                settings.ThemeMode = ThemeMode.System;
                settings.ApplyBackdropEffects = true;
                settings.PreferredVisualStyle = VisualStyleKind.System;
                settings.PreferredEffect = null;
                settings.PreferredEffectOptions = null;
                settings.Palette = null;
                settings.Theme = null;
            });
        }
    }

    [Fact]
    public void CurrentVisualStyle_UsesMicaAltForLightWindows11Style()
    {
        try
        {
            Application.ConfigureVisualStyles(settings =>
            {
                settings.ThemeMode = ThemeMode.Light;
                settings.ApplyBackdropEffects = true;
                settings.PreferredVisualStyle = VisualStyleKind.Mica;
                settings.PreferredEffect = null;
                settings.PreferredEffectOptions = null;
                settings.Palette = null;
                settings.Theme = null;
            });

            ResolvedVisualStyle resolvedStyle = Application.CurrentVisualStyle;

            Assert.Equal(ThemeMode.Light, resolvedStyle.ThemeMode);
            Assert.Equal(VisualStyleKind.Mica, resolvedStyle.VisualStyleKind);
            Assert.Equal(EffectKind.MicaAlt, resolvedStyle.EffectKind);
            Assert.Equal(0xFF_F3_F3_F3u, resolvedStyle.Palette.WindowBackground);
        }
        finally
        {
            Application.ConfigureVisualStyles(settings =>
            {
                settings.ThemeMode = ThemeMode.System;
                settings.ApplyBackdropEffects = true;
                settings.PreferredVisualStyle = VisualStyleKind.System;
                settings.PreferredEffect = null;
                settings.PreferredEffectOptions = null;
                settings.Palette = null;
                settings.Theme = null;
            });
        }
    }
}
