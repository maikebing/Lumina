using Microsoft.Win32;
using Lumina;
using System.Runtime.InteropServices;
using System.Text;

namespace Lumina.Forms;

/// <summary>
/// Provides the LuminaForms application bootstrap surface, including message loop startup and application-wide visual style defaults.
/// </summary>
public static class Application
{
    private const string PersonalizeRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string VisualStylesManifestFileName = "Lumina.Forms.CommonControls.manifest";
    private static readonly string s_visualStylesManifestPath = Path.Combine(Path.GetTempPath(), "Lumina.Forms", VisualStylesManifestFileName);
    private static readonly string s_visualStylesManifestContents =
        """
        <?xml version="1.0" encoding="utf-8" standalone="yes"?>
        <assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
          <assemblyIdentity version="1.0.0.0" name="Lumina.Forms.VisualStyles" processorArchitecture="*" type="win32" />
          <description>Lumina.Forms common controls v6 activation context</description>
          <dependency>
            <dependentAssembly>
              <assemblyIdentity
                type="win32"
                name="Microsoft.Windows.Common-Controls"
                version="6.0.0.0"
                processorArchitecture="*"
                publicKeyToken="6595b64144ccf1df"
                language="*" />
            </dependentAssembly>
          </dependency>
        </assembly>
        """;
    private static readonly Lock s_syncRoot = new();
    private static readonly HashSet<Form> s_openForms = [];
    private static bool s_commonControlsInitialized;
    private static nint s_visualStylesActivationContext;
    private static nuint s_visualStylesActivationCookie;
    private static bool s_visualStylesEnabled;
    private static bool s_compatibleTextRenderingDefault;

    /// <summary>
    /// Gets the mutable application-wide visual style settings used for new LuminaForms windows.
    /// </summary>
    public static ApplicationVisualStyleSettings VisualStyleSettings { get; } = new();

    /// <summary>
    /// Gets the current resolved visual style after OS detection and application overrides are applied.
    /// </summary>
    public static ResolvedVisualStyle CurrentVisualStyle => GetResolvedVisualStyle();

    /// <summary>
    /// Enables LuminaForms visual styling, activates themed common controls, and resolves the default backdrop/theme from the current operating system.
    /// </summary>
    public static void EnableVisualStyles()
    {
        EnsureCommonControlsInitialized();
        EnsureThemedCommonControlsActivated();
        s_visualStylesEnabled = true;
        NotifyVisualStylesChanged();
    }

    /// <summary>
    /// Matches the WinForms compatibility API. LuminaForms renders using native Win32 text, so this is currently a no-op.
    /// </summary>
    /// <param name="defaultValue">Ignored for compatibility with the WinForms API.</param>
    public static void SetCompatibleTextRenderingDefault(bool defaultValue)
    {
        s_compatibleTextRenderingDefault = defaultValue;
    }

    /// <summary>
    /// Requests that the current UI thread exit its LuminaForms message loop.
    /// </summary>
    public static void ExitThread()
    {
        DisposeThemedCommonControlsActivation();
        Win32.PostQuitMessage(0);
    }

    /// <summary>
    /// Requests that all currently open LuminaForms windows close and the application exit.
    /// </summary>
    public static void Exit()
    {
        Form[] openForms = GetOpenFormsSnapshot();
        if (openForms.Length == 0)
        {
            ExitThread();
            return;
        }

        foreach (Form form in openForms)
        {
            form.Close();
        }
    }

    /// <summary>
    /// Starts the application message loop with the provided main form.
    /// </summary>
    /// <param name="form">The main form to show and use as the message loop owner.</param>
    /// <returns>The exit code returned by the thread message loop.</returns>
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
    /// <returns>The exit code returned by the Win32 message loop.</returns>
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
    /// <param name="configure">A delegate that updates <see cref="VisualStyleSettings"/>.</param>
    public static void ConfigureVisualStyles(Action<ApplicationVisualStyleSettings> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(VisualStyleSettings);
        NotifyVisualStylesChanged();
    }

    /// <summary>
    /// Applies a file-driven theme to future LuminaForms windows.
    /// </summary>
    /// <param name="theme">The theme to activate.</param>
    public static void UseTheme(NativeTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        VisualStyleSettings.Theme = theme;
        NotifyVisualStylesChanged();
    }

    /// <summary>
    /// Loads a JSON theme file and applies it to future LuminaForms windows.
    /// </summary>
    /// <param name="path">The JSON theme file path.</param>
    public static void LoadTheme(string path)
    {
        UseTheme(NativeTheme.LoadJson(path));
    }

    /// <summary>
    /// Clears the currently active file-driven theme.
    /// </summary>
    public static void ResetTheme()
    {
        VisualStyleSettings.Theme = null;
        NotifyVisualStylesChanged();
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
        return ResolveVisualStyle(
            VisualStyleSettings.ThemeMode,
            VisualStyleSettings.PreferredVisualStyle,
            VisualStyleSettings.ApplyBackdropEffects,
            VisualStyleSettings.PreferredEffect,
            VisualStyleSettings.PreferredEffectOptions,
            VisualStyleSettings.Theme,
            VisualStyleSettings.Palette);
    }

    internal static ThemeMode ResolveThemeMode(ThemeMode requestedThemeMode, NativeTheme? theme = null)
    {
        return requestedThemeMode switch
        {
            ThemeMode.Light => ThemeMode.Light,
            ThemeMode.Dark => ThemeMode.Dark,
            _ => theme?.ThemeMode switch
            {
                ThemeMode.Light => ThemeMode.Light,
                ThemeMode.Dark => ThemeMode.Dark,
                _ => DetectSystemThemeMode(),
            },
        };
    }

    internal static VisualStyleKind ResolveVisualStyleKind(VisualStyleKind requestedVisualStyleKind, NativeTheme? theme = null)
    {
        if (requestedVisualStyleKind != VisualStyleKind.System)
        {
            return requestedVisualStyleKind;
        }

        if (theme?.PreferredVisualStyle is { } themeVisualStyle && themeVisualStyle != VisualStyleKind.System)
        {
            return themeVisualStyle;
        }

        return DetectSystemVisualStyleKind();
    }

    internal static ResolvedVisualStyle ResolveVisualStyle(
        ThemeMode requestedThemeMode,
        VisualStyleKind preferredVisualStyle,
        bool applyBackdropEffects,
        EffectKind? preferredEffect,
        EffectOptions? preferredEffectOptions,
        NativeTheme? theme,
        ThemePalette? palette)
    {
        ThemeMode themeMode = ResolveThemeMode(requestedThemeMode, theme);
        VisualStyleKind visualStyleKind = ResolveVisualStyleKind(preferredVisualStyle, theme);
        EffectKind effectKind = ResolveEffectKind(applyBackdropEffects, preferredEffect, theme, themeMode, visualStyleKind);
        EffectOptions? effectOptions = ResolveEffectOptions(preferredEffectOptions, theme, effectKind, themeMode);
        ThemePalette resolvedPalette = ResolvePalette(palette, theme, themeMode, visualStyleKind);
        return new ResolvedVisualStyle(themeMode, visualStyleKind, effectKind, effectOptions, theme, resolvedPalette);
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

    private static VisualStyleKind DetectSystemVisualStyleKind()
    {
        if (!OperatingSystem.IsWindows())
        {
            return VisualStyleKind.Classic;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            return VisualStyleKind.Mica;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0))
        {
            return VisualStyleKind.Fluent;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(6, 2))
        {
            return VisualStyleKind.Modern;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            return VisualStyleKind.AeroGlass;
        }

        return VisualStyleKind.Classic;
    }

    private static void EnsureCommonControlsInitialized()
    {
        lock (s_syncRoot)
        {
            if (s_commonControlsInitialized)
            {
                return;
            }

            var initCommonControls = new Win32.INITCOMMONCONTROLSEX
            {
                dwSize = (uint)Marshal.SizeOf<Win32.INITCOMMONCONTROLSEX>(),
                dwICC = Win32.ICC_WIN95_CLASSES | Win32.ICC_DATE_CLASSES | Win32.ICC_STANDARD_CLASSES | Win32.ICC_USEREX_CLASSES,
            };

            _ = Win32.InitCommonControlsEx(ref initCommonControls);
            s_commonControlsInitialized = true;
        }
    }

    private static void EnsureThemedCommonControlsActivated()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        lock (s_syncRoot)
        {
            if (s_visualStylesActivationCookie != 0)
            {
                return;
            }

            try
            {
                EnsureVisualStylesManifestFile();

                var activationContext = new Win32.ACTCTXW
                {
                    cbSize = (uint)Marshal.SizeOf<Win32.ACTCTXW>(),
                    lpSource = s_visualStylesManifestPath,
                };

                nint handle = Win32.CreateActCtxW(ref activationContext);
                if (handle == new nint(-1) || handle == 0)
                {
                    return;
                }

                if (!Win32.ActivateActCtx(handle, out nuint activationCookie))
                {
                    Win32.ReleaseActCtx(handle);
                    return;
                }

                s_visualStylesActivationContext = handle;
                s_visualStylesActivationCookie = activationCookie;
            }
            catch
            {
            }
        }
    }

    private static void EnsureVisualStylesManifestFile()
    {
        string? directory = Path.GetDirectoryName(s_visualStylesManifestPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        byte[] manifestBytes = Encoding.UTF8.GetBytes(s_visualStylesManifestContents);
        File.WriteAllBytes(s_visualStylesManifestPath, manifestBytes);
    }

    private static void DisposeThemedCommonControlsActivation()
    {
        lock (s_syncRoot)
        {
            if (s_visualStylesActivationCookie != 0)
            {
                _ = Win32.DeactivateActCtx(0, s_visualStylesActivationCookie);
                s_visualStylesActivationCookie = 0;
            }

            if (s_visualStylesActivationContext != 0 && s_visualStylesActivationContext != new nint(-1))
            {
                Win32.ReleaseActCtx(s_visualStylesActivationContext);
                s_visualStylesActivationContext = 0;
            }
        }
    }

    private static EffectKind ResolveEffectKind(
        bool applyBackdropEffects,
        EffectKind? preferredEffect,
        NativeTheme? theme,
        ThemeMode themeMode,
        VisualStyleKind visualStyleKind)
    {
        if (!applyBackdropEffects)
        {
            return EffectKind.None;
        }

        if (preferredEffect is { } preferred)
        {
            return preferred;
        }

        if (theme?.PreferredEffect is { } themedEffect)
        {
            return themedEffect;
        }

        return visualStyleKind switch
        {
            VisualStyleKind.Mica => themeMode == ThemeMode.Dark
                ? EffectKind.Mica
                : EffectKind.MicaAlt,
            VisualStyleKind.Fluent => EffectKind.Blur,
            VisualStyleKind.AeroGlass => EffectKind.Aero,
            _ => EffectKind.None,
        };
    }

    private static EffectOptions? ResolveEffectOptions(
        EffectOptions? preferredEffectOptions,
        NativeTheme? theme,
        EffectKind effectKind,
        ThemeMode themeMode)
    {
        if (preferredEffectOptions is not null)
        {
            return preferredEffectOptions;
        }

        if (theme?.PreferredEffectOptions is not null)
        {
            return theme.PreferredEffectOptions;
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

    private static ThemePalette ResolvePalette(
        ThemePalette? palette,
        NativeTheme? theme,
        ThemeMode themeMode,
        VisualStyleKind visualStyleKind)
    {
        if (palette is not null)
        {
            return palette;
        }

        if (theme?.Palette is not null)
        {
            return theme.Palette;
        }

        return ThemePalette.CreateSystem(themeMode, visualStyleKind);
    }

    internal static void RegisterOpenForm(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        lock (s_syncRoot)
        {
            _ = s_openForms.Add(form);
        }
    }

    internal static bool UnregisterOpenForm(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        lock (s_syncRoot)
        {
            _ = s_openForms.Remove(form);
            return s_openForms.Count == 0;
        }
    }

    internal static nint GetAnyOpenFormHandle()
    {
        lock (s_syncRoot)
        {
            foreach (Form form in s_openForms)
            {
                if (form.Handle != 0)
                {
                    return form.Handle;
                }
            }
        }

        return 0;
    }

    private static Form[] GetOpenFormsSnapshot()
    {
        lock (s_syncRoot)
        {
            return [.. s_openForms];
        }
    }

    private static void NotifyVisualStylesChanged()
    {
        foreach (Form form in GetOpenFormsSnapshot())
        {
            form.RefreshVisualStyles();
        }
    }
}
