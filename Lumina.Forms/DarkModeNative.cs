using System.Runtime.InteropServices;

namespace Lumina.Forms;

internal static class DarkModeNative
{
    private const int WCA_USEDARKMODECOLORS = 26;
    private const int OrdinalOpenNcThemeData = 49;
    private const int OrdinalRefreshImmersiveColorPolicyState = 104;
    private const int OrdinalGetIsImmersiveColorUsingHighContrast = 106;
    private const int OrdinalShouldAppsUseDarkMode = 132;
    private const int OrdinalAllowDarkModeForWindow = 133;
    private const int OrdinalAppModeOrAllowDarkModeForApp = 135;
    private const int OrdinalFlushMenuThemes = 136;
    private const int OrdinalIsDarkModeAllowedForWindow = 137;

    private static readonly object s_sync = new();

    private static bool s_initialized;
    private static bool s_supported;
    private static int s_buildNumber;

    private static ShouldAppsUseDarkModeDelegate? s_shouldAppsUseDarkMode;
    private static AllowDarkModeForWindowDelegate? s_allowDarkModeForWindow;
    private static AllowDarkModeForAppDelegate? s_allowDarkModeForApp;
    private static SetPreferredAppModeDelegate? s_setPreferredAppMode;
    private static FlushMenuThemesDelegate? s_flushMenuThemes;
    private static RefreshImmersiveColorPolicyStateDelegate? s_refreshImmersiveColorPolicyState;
    private static IsDarkModeAllowedForWindowDelegate? s_isDarkModeAllowedForWindow;
    private static GetIsImmersiveColorUsingHighContrastDelegate? s_getIsImmersiveColorUsingHighContrast;
    private static SetWindowCompositionAttributeDelegate? s_setWindowCompositionAttribute;

    internal static bool IsSupported
    {
        get
        {
            EnsureInitialized();
            return s_supported;
        }
    }

    internal static bool IsDarkModeEnabled
    {
        get
        {
            EnsureInitialized();
            return s_supported
                && s_shouldAppsUseDarkMode is not null
                && s_shouldAppsUseDarkMode()
                && !IsHighContrast();
        }
    }

    internal static bool AllowWindowDarkMode(nint hwnd, bool allow)
    {
        EnsureInitialized();
        if (!s_supported || hwnd == 0 || s_allowDarkModeForWindow is null)
        {
            return false;
        }

        return s_allowDarkModeForWindow(hwnd, allow);
    }

    internal static void RefreshTitleBarTheme(nint hwnd)
    {
        EnsureInitialized();
        if (!s_supported || hwnd == 0)
        {
            return;
        }

        bool useDark = s_isDarkModeAllowedForWindow is not null
            && s_shouldAppsUseDarkMode is not null
            && s_isDarkModeAllowedForWindow(hwnd)
            && s_shouldAppsUseDarkMode()
            && !IsHighContrast();

        if (s_buildNumber < 18362)
        {
            _ = Win32.SetPropW(hwnd, "UseImmersiveDarkModeColors", useDark ? (nint)1 : 0);
            return;
        }

        if (s_setWindowCompositionAttribute is null)
        {
            return;
        }

        int enabled = useDark ? 1 : 0;
        var data = new Win32.WINDOWCOMPOSITIONATTRIBDATA
        {
            Attrib = WCA_USEDARKMODECOLORS,
            pvData = Marshal.AllocHGlobal(sizeof(int)),
            cbData = (nuint)sizeof(int),
        };

        try
        {
            Marshal.WriteInt32(data.pvData, enabled);
            _ = s_setWindowCompositionAttribute(hwnd, ref data);
        }
        finally
        {
            Marshal.FreeHGlobal(data.pvData);
        }
    }

    internal static bool IsColorSchemeChangeMessage(nint lParam)
    {
        EnsureInitialized();
        if (!s_supported)
        {
            return false;
        }

        bool isImmersiveColorSet = false;
        if (lParam != 0)
        {
            string? settingName = Marshal.PtrToStringUni(lParam);
            isImmersiveColorSet = string.Equals(settingName, "ImmersiveColorSet", StringComparison.OrdinalIgnoreCase);
        }

        if (isImmersiveColorSet)
        {
            s_refreshImmersiveColorPolicyState?.Invoke();
        }

        _ = s_getIsImmersiveColorUsingHighContrast?.Invoke(ImmersiveHighContrastCacheMode.Refresh);
        return isImmersiveColorSet;
    }

    internal static void RefreshImmersiveState()
    {
        EnsureInitialized();
        if (!s_supported)
        {
            return;
        }

        s_refreshImmersiveColorPolicyState?.Invoke();
        _ = s_getIsImmersiveColorUsingHighContrast?.Invoke(ImmersiveHighContrastCacheMode.Refresh);
        s_flushMenuThemes?.Invoke();
    }

    internal static void ApplyThemeToWindow(nint hwnd, bool useDarkMode)
    {
        EnsureInitialized();
        if (!s_supported || hwnd == 0)
        {
            return;
        }

        _ = AllowWindowDarkMode(hwnd, useDarkMode);
        _ = Win32.SendMessageW(hwnd, Win32.WM_THEMECHANGED, 0, 0);
    }

    private static void EnsureInitialized()
    {
        if (s_initialized)
        {
            return;
        }

        lock (s_sync)
        {
            if (s_initialized)
            {
                return;
            }

            InitializeCore();
            s_initialized = true;
        }
    }

    private static void InitializeCore()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        DarkModeCapabilities.Snapshot snapshot = DarkModeCapabilities.Current;
        if (!snapshot.IsWindows10OrGreater || !IsSupportedBuild(snapshot.Build))
        {
            return;
        }

        nint uxTheme = Win32.LoadLibraryExW("uxtheme.dll", 0, Win32.LOAD_LIBRARY_SEARCH_SYSTEM32);
        if (uxTheme == 0)
        {
            return;
        }

        s_buildNumber = snapshot.Build;
        s_shouldAppsUseDarkMode = GetOrdinalDelegate<ShouldAppsUseDarkModeDelegate>(uxTheme, OrdinalShouldAppsUseDarkMode);
        s_allowDarkModeForWindow = GetOrdinalDelegate<AllowDarkModeForWindowDelegate>(uxTheme, OrdinalAllowDarkModeForWindow);
        s_refreshImmersiveColorPolicyState = GetOrdinalDelegate<RefreshImmersiveColorPolicyStateDelegate>(uxTheme, OrdinalRefreshImmersiveColorPolicyState);
        s_getIsImmersiveColorUsingHighContrast = GetOrdinalDelegate<GetIsImmersiveColorUsingHighContrastDelegate>(uxTheme, OrdinalGetIsImmersiveColorUsingHighContrast);
        s_flushMenuThemes = GetOrdinalDelegate<FlushMenuThemesDelegate>(uxTheme, OrdinalFlushMenuThemes);
        s_isDarkModeAllowedForWindow = GetOrdinalDelegate<IsDarkModeAllowedForWindowDelegate>(uxTheme, OrdinalIsDarkModeAllowedForWindow);

        nint ord135 = Win32.GetProcAddress(uxTheme, (nint)OrdinalAppModeOrAllowDarkModeForApp);
        if (ord135 != 0)
        {
            if (snapshot.Build < 18362)
            {
                s_allowDarkModeForApp = Marshal.GetDelegateForFunctionPointer<AllowDarkModeForAppDelegate>(ord135);
            }
            else
            {
                s_setPreferredAppMode = Marshal.GetDelegateForFunctionPointer<SetPreferredAppModeDelegate>(ord135);
            }
        }

        nint user32 = Win32.GetModuleHandleW("user32.dll");
        if (user32 != 0)
        {
            nint setWindowCompositionAttribute = Win32.GetProcAddress(user32, "SetWindowCompositionAttribute");
            if (setWindowCompositionAttribute != 0)
            {
                s_setWindowCompositionAttribute = Marshal.GetDelegateForFunctionPointer<SetWindowCompositionAttributeDelegate>(setWindowCompositionAttribute);
            }
        }

        s_supported = s_shouldAppsUseDarkMode is not null
            && s_allowDarkModeForWindow is not null
            && s_refreshImmersiveColorPolicyState is not null
            && s_getIsImmersiveColorUsingHighContrast is not null
            && s_isDarkModeAllowedForWindow is not null
            && (s_allowDarkModeForApp is not null || s_setPreferredAppMode is not null);

        if (!s_supported)
        {
            return;
        }

        if (s_allowDarkModeForApp is not null)
        {
            _ = s_allowDarkModeForApp(true);
        }
        else if (s_setPreferredAppMode is not null)
        {
            _ = s_setPreferredAppMode(PreferredAppMode.AllowDark);
        }

        RefreshImmersiveState();
    }

    private static bool IsHighContrast()
    {
        var highContrast = new Win32.HIGHCONTRASTW
        {
            cbSize = (uint)Marshal.SizeOf<Win32.HIGHCONTRASTW>(),
        };

        return Win32.SystemParametersInfoW(Win32.SPI_GETHIGHCONTRAST, highContrast.cbSize, ref highContrast, 0)
            && (highContrast.dwFlags & Win32.HCF_HIGHCONTRASTON) != 0;
    }

    private static bool IsSupportedBuild(int build)
        => build is 17763 or 18362 or 18363 or 19041 or 19042 or 19043 or 19044 or 19045 or >= 22000;

    private static TDelegate? GetOrdinalDelegate<TDelegate>(nint moduleHandle, int ordinal)
        where TDelegate : Delegate
    {
        nint procAddress = Win32.GetProcAddress(moduleHandle, (nint)ordinal);
        return procAddress == 0
            ? null
            : Marshal.GetDelegateForFunctionPointer<TDelegate>(procAddress);
    }

    private enum PreferredAppMode
    {
        Default,
        AllowDark,
        ForceDark,
        ForceLight,
        Max,
    }

    private enum ImmersiveHighContrastCacheMode
    {
        UseCachedValue,
        Refresh,
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate bool ShouldAppsUseDarkModeDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate bool AllowDarkModeForWindowDelegate(nint hwnd, bool allow);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate bool AllowDarkModeForAppDelegate(bool allow);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate PreferredAppMode SetPreferredAppModeDelegate(PreferredAppMode appMode);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void FlushMenuThemesDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void RefreshImmersiveColorPolicyStateDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate bool IsDarkModeAllowedForWindowDelegate(nint hwnd);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate bool GetIsImmersiveColorUsingHighContrastDelegate(ImmersiveHighContrastCacheMode mode);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate bool SetWindowCompositionAttributeDelegate(nint hwnd, ref Win32.WINDOWCOMPOSITIONATTRIBDATA data);
}