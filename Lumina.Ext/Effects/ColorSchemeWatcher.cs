using System.Runtime.InteropServices;
using Lumina.Ext.DWM;

namespace Lumina.Ext.Effects;

/// <summary>
/// Polls the AppsUseLightTheme registry value on a native thread and fires
/// a callback when the light/dark preference changes.
/// AOT-safe: uses CreateThread + native registry APIs, no managed Thread.
/// </summary>
internal static unsafe class ColorSchemeWatcher
{
    internal static bool IsDarkMode { get; private set; }

    // Set before calling Start(). Signature: void OnChanged(bool isDark)
    internal static delegate* unmanaged<bool, void> OnChanged;

    private static volatile int _stop;   // 0 = run, 1 = stop
    private static nint _thread;

    internal static void Start()
    {
        _stop = 0;
        // Read initial value
        IsDarkMode = ReadIsDark();
        _thread = NativeMethods.CreateThread(0, 0, (nint)(delegate* unmanaged<nint, uint>)&ThreadProc, 0, 0, out _);
    }

    internal static void Stop()
    {
        System.Threading.Interlocked.Exchange(ref _stop, 1);
        if (_thread != 0)
        {
            NativeMethods.WaitForSingleObject(_thread, 3000);
            NativeMethods.CloseHandle(_thread);
            _thread = 0;
        }
    }

    [UnmanagedCallersOnly]
    private static uint ThreadProc(nint _)
    {
        while (_stop == 0)
        {
            NativeMethods.Sleep(2000);
            if (_stop != 0) break;

            bool dark = ReadIsDark();
            if (dark != IsDarkMode)
            {
                IsDarkMode = dark;
                if (OnChanged != null)
                    OnChanged(dark);
            }
        }
        return 0;
    }

    private static bool ReadIsDark()
    {
        const string key   = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        const string value = "AppsUseLightTheme";

        if (NativeMethods.RegOpenKeyExW(NativeMethods.HKEY_CURRENT_USER, key, 0,
                NativeMethods.KEY_READ, out nint hKey) != 0)
            return false;

        try
        {
            uint type, size = 4;
            uint data = 0;
            if (NativeMethods.RegQueryValueExW(hKey, value, 0, out type, ref data, ref size) != 0)
                return false;
            return data == 0; // 0 = dark, 1 = light
        }
        finally
        {
            NativeMethods.RegCloseKey(hKey);
        }
    }
}
