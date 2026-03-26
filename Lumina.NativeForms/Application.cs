using Microsoft.Win32;

namespace Lumina.NativeForms;

public static class Application
{
    private const string PersonalizeRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private static bool s_visualStylesEnabled;

    /// <summary>
    /// Gets the mutable application-wide visual style settings used for new NativeForms windows.
    /// </summary>
    public static ApplicationVisualStyleSettings VisualStyleSettings { get; } = new();

    /// <summary>
    /// Gets the current resolved visual style after OS detection and application overrides are applied.
    /// </summary>
    public static ResolvedVisualStyle CurrentVisualStyle => GetResolvedVisualStyle();

    /// <summary>
    /// Enables NativeForms visual styling and resolves the default backdrop/theme from the current operating system.
    /// </summary>
    public static void EnableVisualStyles()
    {
        s_visualStylesEnabled = true;
    }

    /// <summary>
    /// Matches the WinForms compatibility API. NativeForms renders using native Win32 text, so this is currently a no-op.
    /// </summary>
    public static void SetCompatibleTextRenderingDefault(bool defaultValue)
    {
    }

    /// <summary>
    /// Starts the application message loop with the provided main form.
    /// </summary>
    public static int Run(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        EnsureVisualStylesInitialized();
        form.Show();
        return RunMessageLoop();
    }

    /// <summary>
    /// Runs the shared Win32 message loop for the current thread.
    /// </summary>
    public static int RunMessageLoop()
    {
        EnsureVisualStylesInitialized();

        while (Win32.GetMessage(out var message, 0, 0, 0) > 0)
        {
            _ = Win32.TranslateMessage(in message);
            _ = Win32.DispatchMessage(in message);
        }

        return 0;
    }

    /// <summary>
    /// Applies a batch of visual style changes to the application defaults.
    /// </summary>
    public static void ConfigureVisualStyles(Action<ApplicationVisualStyleSettings> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(VisualStyleSettings);
    }

    internal static void EnsureVisualStylesInitialized()
    {
        if (!s_visualStylesEnabled)
        {
            EnableVisualStyles();
        }
    }

    internal static ResolvedVisualStyle GetResolvedVisualStyle()
    {
        var themeMode = ResolveThemeMode(VisualStyleSettings.ThemeMode);
        var effectKind = ResolveEffectKind(VisualStyleSettings);
        var effectOptions = ResolveEffectOptions(VisualStyleSettings, effectKind, themeMode);
        return new ResolvedVisualStyle(themeMode, effectKind, effectOptions);
    }

    internal static ThemeMode ResolveThemeMode(ThemeMode requestedThemeMode)
    {
        return requestedThemeMode switch
        {
            ThemeMode.Light => ThemeMode.Light,
            ThemeMode.Dark => ThemeMode.Dark,
            _ => DetectSystemThemeMode(),
        };
    }

    private static ThemeMode DetectSystemThemeMode()
    {
        if (!OperatingSystem.IsWindows())
        {
            return ThemeMode.Light;
        }

        try
        {
            object? value = Registry.GetValue($@"HKEY_CURRENT_USER\{PersonalizeRegistryKey}", "AppsUseLightTheme", 1);
            if (value is int intValue)
            {
                return intValue == 0 ? ThemeMode.Dark : ThemeMode.Light;
            }
        }
        catch
        {
        }

        return ThemeMode.Light;
    }

    private static EffectKind ResolveEffectKind(ApplicationVisualStyleSettings settings)
    {
        if (!settings.ApplyBackdropEffects)
        {
            return EffectKind.None;
        }

        if (settings.PreferredEffect is { } preferredEffect)
        {
            return preferredEffect;
        }

        if (!OperatingSystem.IsWindows())
        {
            return EffectKind.None;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            return EffectKind.Mica;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0))
        {
            return EffectKind.Blur;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(6, 2))
        {
            return EffectKind.None;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            return EffectKind.Aero;
        }

        return EffectKind.None;
    }

    private static EffectOptions? ResolveEffectOptions(
        ApplicationVisualStyleSettings settings,
        EffectKind effectKind,
        ThemeMode themeMode)
    {
        if (settings.PreferredEffectOptions is not null)
        {
            return settings.PreferredEffectOptions;
        }

        return effectKind switch
        {
            EffectKind.Blur => new EffectOptions { BlurRadius = 20 },
            EffectKind.Acrylic => new EffectOptions
            {
                BlendColor = themeMode == ThemeMode.Dark ? 0xCC_20_20_20 : 0xCC_F2_F2_F2,
                BlurRadius = 18,
                Opacity = 0.9f,
            },
            _ => null,
        };
    }
}
