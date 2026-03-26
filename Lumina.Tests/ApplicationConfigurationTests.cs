using Lumina.NativeForms;
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
                settings.PreferredEffect = EffectKind.Mica;
                settings.Theme = null;
            });

            ApplicationConfiguration.Initialize();

            ResolvedVisualStyle resolvedStyle = Application.CurrentVisualStyle;

            Assert.Equal(ThemeMode.Dark, resolvedStyle.ThemeMode);
            Assert.Equal(EffectKind.Mica, resolvedStyle.EffectKind);
        }
        finally
        {
            Application.ConfigureVisualStyles(settings =>
            {
                settings.ThemeMode = ThemeMode.System;
                settings.ApplyBackdropEffects = true;
                settings.PreferredEffect = null;
                settings.PreferredEffectOptions = null;
                settings.Theme = null;
            });
        }
    }
}
