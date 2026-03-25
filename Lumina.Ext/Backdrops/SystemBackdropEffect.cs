using System.Runtime.InteropServices;
using Lumina.Ext.DWM;

namespace Lumina.Ext.Backdrops;

internal enum SystemBackdropType
{
    None    = 0,
    Auto    = 1,
    Mica    = 2,
    Acrylic = 3,
    MicaAlt = 4,
}

internal static unsafe class SystemBackdropEffect
{
    // Captured for the EnumWindows callback
    private static SystemBackdropType _pendingType;

    internal static void Apply(nint hwnd, SystemBackdropType type)
    {
        if (!OsVersion.SupportsSystemBackdrop) return;
        int value = (int)type;
        NativeMethods.DwmSetWindowAttribute(
            hwnd, NativeMethods.DWMWA_SYSTEMBACKDROP_TYPE,
            in value, sizeof(int));
    }

    internal static void Remove(nint hwnd)
    {
        int value = (int)SystemBackdropType.None;
        NativeMethods.DwmSetWindowAttribute(
            hwnd, NativeMethods.DWMWA_SYSTEMBACKDROP_TYPE,
            in value, sizeof(int));
    }

    internal static void ApplyToAll(SystemBackdropType type)
    {
        if (!OsVersion.SupportsSystemBackdrop) return;
        _pendingType = type;
        NativeMethods.EnumWindows(EnumProc, 0);
    }

    private static bool EnumProc(nint hwnd, nint _)
    {
        if (NativeMethods.IsWindowVisible(hwnd))
            Apply(hwnd, _pendingType);
        return true;
    }
}
