using System.Runtime.InteropServices;
using Lumina.Ext.DWM;
using Lumina.Ext.Effects;
using Lumina.Ext.Hooks;

namespace Lumina.Ext.Backdrops;

[StructLayout(LayoutKind.Sequential)]
internal struct ACCENT_POLICY
{
    internal uint AccentState;   // 3 = ACCENT_ENABLE_BLURBEHIND, 4 = ACCENT_ENABLE_ACRYLICBLURBEHIND
    internal uint AccentFlags;
    internal uint GradientColor; // AABBGGRR
    internal int  AnimationId;
}

internal static unsafe class AccentBlurEffect
{
    private static InlineHook? _hook;
    internal static BlurParameters Parameters = BlurParameters.Default;

    internal static void Install()
    {
        nint target = UdwmOffsets.CAccent_UpdateAccentPolicy;
        if (target == 0) return;

        _hook = new InlineHook(target);
        _hook.Install((nint)(delegate* unmanaged<nint, nint, void>)&Detour);
    }

    internal static void Uninstall()
    {
        _hook?.Dispose();
        _hook = null;
    }

    [UnmanagedCallersOnly]
    private static void Detour(nint pThis, nint pPolicy)
    {
        if (pPolicy != 0)
        {
            var policy = (ACCENT_POLICY*)pPolicy;
            // Upgrade plain blur to acrylic with our blend color
            if (policy->AccentState == 3)
            {
                policy->AccentState  = 4; // ACCENT_ENABLE_ACRYLICBLURBEHIND
                policy->GradientColor = Parameters.BlendColor;
            }
        }

        // Call original via trampoline
        var trampoline = (delegate* unmanaged<nint, nint, void>)_hook!.Trampoline;
        trampoline(pThis, pPolicy);
    }
}
