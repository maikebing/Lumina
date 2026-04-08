using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Applies the current Lumina theme (dark/light) to a Win32 common dialog window
/// and all of its child controls.
/// </summary>
internal static class CommonDialogThemeHelper
{
    [ThreadStatic]
    private static bool s_darkMode;

    /// <summary>
    /// Applies the dark/light theme to <paramref name="hWnd"/> and all child controls.
    /// Call this from a WM_INITDIALOG hook proc.
    /// </summary>
    internal static void Apply(nint hWnd, bool useDarkMode)
    {
        if (hWnd == 0)
        {
            return;
        }

        // Title bar — requires DWM attribute separate from uxtheme
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
        {
            int darkInt = useDarkMode ? 1 : 0;
            _ = Win32.DwmSetWindowAttribute(hWnd, Win32.DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkInt, sizeof(int));
        }

        // Dialog window itself
        DarkModeNative.ApplyThemeToWindow(hWnd, useDarkMode);
        _ = Win32.SetWindowTheme(hWnd, useDarkMode ? "DarkMode_Explorer" : "Explorer", null);

        // Every child control
        unsafe
        {
            s_darkMode = useDarkMode;
            nint enumProc = (nint)(delegate* unmanaged[Stdcall]<nint, nint, bool>)&EnumChildProc;
            _ = Win32.EnumChildWindows(hWnd, enumProc, 0);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static bool EnumChildProc(nint hwnd, nint _)
    {
        try
        {
            DarkModeNative.ApplyThemeToWindow(hwnd, s_darkMode);
            _ = Win32.SetWindowTheme(hwnd, s_darkMode ? "DarkMode_Explorer" : "Explorer", null);
        }
        catch
        {
            // Never propagate exceptions across unmanaged boundary.
        }

        return true;
    }
}
