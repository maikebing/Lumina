using System.Drawing;
using System.Reflection;
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

    [Fact]
    public void FormThemeColors_RespectExplicitBackColorAndForeColorOverrides()
    {
        using var form = new Form
        {
            BackColor = Color.FromArgb(unchecked((int)0xFF_12_34_56u)),
            ForeColor = Color.FromArgb(unchecked((int)0xFF_EE_DD_CCu)),
        };

        uint backgroundColorRef = InvokeThemeColorRef(form, "GetThemeBackgroundColorRef", slotValue: 0);
        uint foregroundColorRef = InvokeThemeColorRef(form, "GetThemeForegroundColorRef", slotValue: 0);

        Assert.Equal(ToColorRef(0xFF_12_34_56u), backgroundColorRef);
        Assert.Equal(ToColorRef(0xFF_EE_DD_CCu), foregroundColorRef);
    }

    [Fact]
    public void LabelThemeColors_InheritParentBackgroundAndHonorExplicitForeground()
    {
        using var form = new Form();
        var panel = new Panel
        {
            BackColor = Color.FromArgb(unchecked((int)0xFF_20_30_40u)),
        };
        var label = new Label
        {
            ForeColor = Color.FromArgb(unchecked((int)0xFF_90_A0_B0u)),
        };

        form.Controls.Add(panel);
        panel.Controls.Add(label);

        object?[] parameters = [(nint)0, 0u, 0u, false];
        MethodInfo? method = typeof(Control).GetMethod("TryGetThemeColors", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        bool handled = Assert.IsType<bool>(method!.Invoke(label, parameters));

        Assert.True(handled);
        Assert.NotEqual(0, Assert.IsType<nint>(parameters[0]));
        Assert.Equal(ToColorRef(0xFF_20_30_40u), Assert.IsType<uint>(parameters[1]));
        Assert.Equal(ToColorRef(0xFF_90_A0_B0u), Assert.IsType<uint>(parameters[2]));
        Assert.True(Assert.IsType<bool>(parameters[3]));
    }

    private static uint InvokeThemeColorRef(Form form, string methodName, int slotValue)
    {
        Type slotType = typeof(Form).Assembly.GetType("Lumina.Forms.ThemeColorSlot", throwOnError: true)!;
        object slot = Enum.ToObject(slotType, slotValue);

        MethodInfo? method = typeof(Form).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        return Assert.IsType<uint>(method!.Invoke(form, [slot]));
    }

    private static uint ToColorRef(uint argb)
    {
        uint red = (argb >> 16) & 0xFF;
        uint green = (argb >> 8) & 0xFF;
        uint blue = argb & 0xFF;
        return red | (green << 8) | (blue << 16);
    }
}
