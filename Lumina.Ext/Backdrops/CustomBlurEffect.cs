using System.Runtime.InteropServices;
using Lumina.Ext.DWM;
using Lumina.Ext.Hooks;

namespace Lumina.Ext.Backdrops;

[StructLayout(LayoutKind.Sequential)]
internal struct DWM_BLURBEHIND
{
    internal uint dwFlags;                     // DWM_BB_ENABLE=1, DWM_BB_BLURREGION=2
    internal bool fEnable;
    internal nint hRgnBlur;
    internal bool fTransitionOnMaximized;
}

internal static unsafe class CustomBlurEffect
{
    private static InlineHook? _hook;

    internal static void Install()
    {
        nint target = UdwmOffsets.CTopLevelWindow_UpdateNCAreaBackground;
        if (target == 0) return;

        _hook = new InlineHook(target);
        _hook.Install((nint)(delegate* unmanaged<nint, void>)&Detour);
    }

    internal static void Uninstall()
    {
        _hook?.Dispose();
        _hook = null;
    }

    [UnmanagedCallersOnly]
    private static void Detour(nint pThis)
    {
        // Call original first
        var trampoline = (delegate* unmanaged<nint, void>)_hook!.Trampoline;
        trampoline(pThis);

        // Apply legacy DwmEnableBlurBehindWindow for builds that don't support SystemBackdrop
        if (!OsVersion.SupportsSystemBackdrop)
        {
            // pThis is CTopLevelWindow* — hwnd is at offset 0x10 in known builds
            nint hwnd = *(nint*)(pThis + 0x10);
            if (hwnd != 0 && NativeMethods.IsWindowVisible(hwnd))
            {
                var bb = new DWM_BLURBEHIND
                {
                    dwFlags  = 1, // DWM_BB_ENABLE
                    fEnable  = true,
                    hRgnBlur = 0,
                    fTransitionOnMaximized = false,
                };
                NativeMethods.DwmEnableBlurBehindWindow(hwnd, ref bb);
            }
        }
    }
}
