using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace Lumina.Forms;

/// <summary>
/// Applies the current Lumina theme (dark/light) to a Win32 common dialog window
/// and all of its child controls.
/// </summary>
internal static class CommonDialogThemeHelper
{
    private const nuint DialogSubclassId = 0x4C554D41;
    private static readonly Win32.SubclassProc s_dialogSubclassProc = DialogSubclassProc;
    private static readonly ConcurrentDictionary<nint, DialogThemeState> s_states = new();

    [ThreadStatic]
    private static bool s_darkMode;

    /// <summary>
    /// Applies the dark/light theme to <paramref name="hWnd"/> and all child controls.
    /// Call this from a WM_INITDIALOG hook proc.
    /// </summary>
    internal static void Apply(nint hWnd, bool useDarkMode)
        => Apply(hWnd, useDarkMode, null);

    internal static void Apply(nint hWnd, bool useDarkMode, ThemePalette? palette)
        => Apply(hWnd, useDarkMode, palette, false);

    internal static void Apply(nint hWnd, bool useDarkMode, ThemePalette? palette, bool uniformBackground)
    {
        if (hWnd == 0)
        {
            return;
        }

        ThemePalette resolvedPalette = CoercePaletteForTheme((palette ?? Application.CurrentVisualStyle.Palette).Clone(), useDarkMode);
        DialogThemeState newState = DialogThemeState.Create(resolvedPalette, uniformBackground);
        if (s_states.TryRemove(hWnd, out DialogThemeState? existing))
        {
            existing.Dispose();
        }

        s_states[hWnd] = newState;
        if (!Win32.SetWindowSubclass(hWnd, s_dialogSubclassProc, DialogSubclassId, 0))
        {
            if (s_states.TryRemove(hWnd, out DialogThemeState? failed))
            {
                failed.Dispose();
            }
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

        _ = Win32.InvalidateRect(hWnd, 0, true);
        _ = Win32.UpdateWindow(hWnd);
    }

    private static ThemePalette CoercePaletteForTheme(ThemePalette palette, bool useDarkMode)
    {
        bool windowIsDark = IsDarkColor(palette.WindowBackground);
        bool surfaceIsDark = IsDarkColor(palette.SurfaceBackground);

        if (useDarkMode && (!windowIsDark || !surfaceIsDark))
        {
            ThemePalette fallback = ThemePalette.CreateDark();
            palette.WindowBackground = fallback.WindowBackground;
            palette.WindowForeground = fallback.WindowForeground;
            palette.SurfaceBackground = fallback.SurfaceBackground;
            palette.SurfaceForeground = fallback.SurfaceForeground;
            palette.ControlBackground = fallback.ControlBackground;
            palette.ControlForeground = fallback.ControlForeground;
        }
        else if (!useDarkMode && (windowIsDark || surfaceIsDark))
        {
            ThemePalette fallback = ThemePalette.CreateLight();
            palette.WindowBackground = fallback.WindowBackground;
            palette.WindowForeground = fallback.WindowForeground;
            palette.SurfaceBackground = fallback.SurfaceBackground;
            palette.SurfaceForeground = fallback.SurfaceForeground;
            palette.ControlBackground = fallback.ControlBackground;
            palette.ControlForeground = fallback.ControlForeground;
        }

        return palette;
    }

    private static bool IsDarkColor(uint argb)
    {
        int r = (int)((argb >> 16) & 0xFF);
        int g = (int)((argb >> 8) & 0xFF);
        int b = (int)(argb & 0xFF);

        // Perceived luminance in [0,255].
        int luminance = (r * 299 + g * 587 + b * 114) / 1000;
        return luminance < 128;
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

    private static nint DialogSubclassProc(nint hWnd, uint uMsg, nint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
    {
        if (!s_states.TryGetValue(hWnd, out DialogThemeState? state))
        {
            return Win32.DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        switch (uMsg)
        {
            case Win32.WM_ERASEBKGND:
                if (Win32.GetClientRect(hWnd, out Win32.RECT clientRect))
                {
                    _ = Win32.FillRect(wParam, ref clientRect, state.BackgroundBrush);
                    return 1;
                }

                break;

            case Win32.WM_CTLCOLORMSGBOX:
            case Win32.WM_CTLCOLORDLG:
            case Win32.WM_CTLCOLORSTATIC:
                _ = Win32.SetBkMode(wParam, Win32.TRANSPARENT);
                _ = Win32.SetTextColor(wParam, state.TextColorRef);
                _ = Win32.SetBkColor(wParam, state.BackgroundColorRef);
                return state.BackgroundBrush;

            case Win32.WM_CTLCOLORBTN:
                _ = Win32.SetBkMode(wParam, Win32.TRANSPARENT);
                _ = Win32.SetTextColor(wParam, state.ControlTextColorRef);
                _ = Win32.SetBkColor(wParam, state.ControlBackgroundColorRef);
                return state.ControlBackgroundBrush;

            case Win32.WM_CTLCOLOREDIT:
            case Win32.WM_CTLCOLORLISTBOX:
                _ = Win32.SetBkMode(wParam, Win32.TRANSPARENT);
                _ = Win32.SetTextColor(wParam, state.ControlTextColorRef);
                _ = Win32.SetBkColor(wParam, state.ControlBackgroundColorRef);
                return state.ControlBackgroundBrush;

            case Win32.WM_NCDESTROY:
                if (s_states.TryRemove(hWnd, out DialogThemeState? removed))
                {
                    removed.Dispose();
                }

                break;
        }

        return Win32.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    private sealed class DialogThemeState : IDisposable
    {
        public nint BackgroundBrush { get; }
        public nint ControlBackgroundBrush { get; }
        public uint BackgroundColorRef { get; }
        public uint ControlBackgroundColorRef { get; }
        public uint TextColorRef { get; }
        public uint ControlTextColorRef { get; }

        private DialogThemeState(
            nint backgroundBrush,
            nint controlBackgroundBrush,
            uint backgroundColorRef,
            uint controlBackgroundColorRef,
            uint textColorRef,
            uint controlTextColorRef)
        {
            BackgroundBrush = backgroundBrush;
            ControlBackgroundBrush = controlBackgroundBrush;
            BackgroundColorRef = backgroundColorRef;
            ControlBackgroundColorRef = controlBackgroundColorRef;
            TextColorRef = textColorRef;
            ControlTextColorRef = controlTextColorRef;
        }

        public static DialogThemeState Create(ThemePalette palette, bool uniformBackground)
        {
            uint backgroundColorRef = Win32.ToColorRef(palette.WindowBackground);
            uint controlBackgroundColorRef = uniformBackground
                ? backgroundColorRef
                : Win32.ToColorRef(palette.SurfaceBackground);
            uint textColorRef = Win32.ToColorRef(palette.WindowForeground);
            uint controlTextColorRef = uniformBackground
                ? textColorRef
                : Win32.ToColorRef(palette.SurfaceForeground);

            nint backgroundBrush = Win32.CreateSolidBrush(backgroundColorRef);
            nint controlBackgroundBrush = Win32.CreateSolidBrush(controlBackgroundColorRef);

            return new DialogThemeState(
                backgroundBrush,
                controlBackgroundBrush,
                backgroundColorRef,
                controlBackgroundColorRef,
                textColorRef,
                controlTextColorRef);
        }

        public void Dispose()
        {
            if (BackgroundBrush != 0)
            {
                _ = Win32.DeleteObject(BackgroundBrush);
            }

            if (ControlBackgroundBrush != 0)
            {
                _ = Win32.DeleteObject(ControlBackgroundBrush);
            }
        }
    }
}
