using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

internal static class Win32DarkModeApi
{
    private static readonly Lazy<State> s_state = new(Initialize);

    internal static bool IsSupported => s_state.Value.IsSupported;

    internal static string StatusDescription => s_state.Value.StatusDescription;

    internal static bool TryEnableImmersivePopupMenus()
    {
        State state = s_state.Value;
        if (!state.IsSupported)
        {
            Debug.WriteLine($"[Lumina.Forms] Win32DarkModeApi unavailable: {state.StatusDescription}");
            return false;
        }

        try
        {
            if (state.SetPreferredAppMode is not null)
            {
                _ = state.SetPreferredAppMode(PreferredAppMode.AllowDark);
            }
            else if (state.AllowDarkModeForApp is not null)
            {
                _ = state.AllowDarkModeForApp(true);
            }

            state.FlushMenuThemes?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Lumina.Forms] Win32DarkModeApi invocation failed: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    internal static bool TryPrepareImmersiveMenuPresentation()
    {
        return TryEnableImmersivePopupMenus();
    }

    private static State Initialize()
    {
        DarkModeCapabilities.Snapshot capabilities = DarkModeCapabilities.Current;
        if (!capabilities.SupportsWin32DarkModeApis)
        {
            return State.Unsupported("OS does not expose Win32 dark mode APIs for popup menus.");
        }

        nint uxTheme = Win32.GetModuleHandleW("uxtheme.dll");
        if (uxTheme == 0)
        {
            uxTheme = LoadLibraryExW("uxtheme.dll", 0, LOAD_LIBRARY_SEARCH_SYSTEM32);
        }

        if (uxTheme == 0)
        {
            return State.Unsupported("Failed to load uxtheme.dll from the system directory.");
        }

        var setPreferredAppMode = GetDelegate<SetPreferredAppModeDelegate>(uxTheme, (nint)135);
        var allowDarkModeForApp = capabilities.Build < 18362
            ? GetDelegate<AllowDarkModeForAppDelegate>(uxTheme, (nint)135)
            : null;
        var flushMenuThemes = GetDelegate<FlushMenuThemesDelegate>(uxTheme, (nint)136);

        bool isSupported = setPreferredAppMode is not null || allowDarkModeForApp is not null;
        return isSupported
            ? new State(true, setPreferredAppMode, allowDarkModeForApp, flushMenuThemes, "SetPreferredAppMode/AllowDarkModeForApp available.")
            : State.Unsupported(capabilities.Build < 18362
                ? "Missing uxtheme ordinal #135 for AllowDarkModeForApp."
                : "Missing uxtheme ordinal #135 for SetPreferredAppMode.");
    }

    private static TDelegate? GetDelegate<TDelegate>(nint moduleHandle, nint ordinal) where TDelegate : Delegate
    {
        nint proc = GetProcAddress(moduleHandle, ordinal);
        if (proc == 0)
        {
            return null;
        }

        try
        {
            return Marshal.GetDelegateForFunctionPointer<TDelegate>(proc);
        }
        catch
        {
            return null;
        }
    }

    private static nint GetProcAddress(nint moduleHandle, nint ordinal)
    {
        string ordinalName = "#" + ordinal.ToInt64().ToString(System.Globalization.CultureInfo.InvariantCulture);
        return Win32.GetProcAddress(moduleHandle, ordinalName);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint LoadLibraryExW(string fileName, nint fileHandle, uint flags);

    private const uint LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

    private enum PreferredAppMode
    {
        Default,
        AllowDark,
        ForceDark,
        ForceLight,
        Max,
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate PreferredAppMode SetPreferredAppModeDelegate(PreferredAppMode appMode);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate bool AllowDarkModeForAppDelegate(bool allow);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void FlushMenuThemesDelegate();

    private readonly record struct State(
        bool IsSupported,
        SetPreferredAppModeDelegate? SetPreferredAppMode,
        AllowDarkModeForAppDelegate? AllowDarkModeForApp,
        FlushMenuThemesDelegate? FlushMenuThemes,
        string StatusDescription)
    {
        internal static State Unsupported(string statusDescription) => new(false, null, null, null, statusDescription);
    }
}
