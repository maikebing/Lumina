using System.Runtime.InteropServices;
using Lumina.Ext.DWM;
using Lumina.Ext.Hooks;

namespace Lumina.Ext.Backdrops;

/// <summary>
/// Aero reflection + parallax — hooks CGlassColorizationParameters::AdjustWindowColorization
/// to inject reflection alpha and a parallax brightness offset into the colorization struct.
/// </summary>
internal static unsafe class AeroEffect
{
    private static InlineHook? _hook;

    internal static float ReflectionAlpha  = 0.15f;
    internal static float ParallaxStrength = 0.05f;

    internal static void Install()
    {
        nint target = UdwmOffsets.CGlassColorizationParameters_AdjustWindowColorization;
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
    private static void Detour(nint pParams)
    {
        // Call original first so the struct is fully populated
        var trampoline = (delegate* unmanaged<nint, void>)_hook!.Trampoline;
        trampoline(pParams);

        if (pParams == 0) return;

        // Undocumented CGlassColorizationParameters layout inferred from runtime behavior:
        //   +0x00  float  baseAlpha
        //   +0x04  float  reflectionAlpha
        //   +0x08  float  blurBalance
        float* fields = (float*)pParams;
        // Inject reflection alpha
        if (ReflectionAlpha > 0f)
            fields[1] = ReflectionAlpha;
        // Parallax: slightly brighten the blur balance
        fields[2] = Math.Clamp(fields[2] + ParallaxStrength, 0f, 1f);
    }
}
