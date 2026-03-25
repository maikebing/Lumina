using System.Runtime.InteropServices;
using Lumina.Ext.Backdrops;
using Lumina.Ext.Effects;

namespace Lumina.Ext;

/// <summary>
/// DLL 入口点 — 注入到 dwm.exe 后由 DllMain 启动
/// </summary>
internal static unsafe class ExtMain
{
    private const uint DLL_PROCESS_ATTACH = 1;
    private const uint DLL_PROCESS_DETACH = 0;

    [UnmanagedCallersOnly(EntryPoint = "DllMain")]
    public static bool DllMain(nint hModule, uint reason, nint reserved)
    {
        switch (reason)
        {
            case DLL_PROCESS_ATTACH:
                Startup();
                break;
            case DLL_PROCESS_DETACH:
                Shutdown();
                break;
        }
        return true;
    }

    private static void Startup()
    {
        DWM.UdwmOffsets.Load();

        // 注册配色方案监听（在安装 hook 前，以便首次应用正确参数）
        ColorSchemeWatcher.OnChanged = &OnColorSchemeChangedNative;
        ColorSchemeWatcher.Start();

        if (DWM.UdwmOffsets.IsLoaded)
        {
            AccentBlurEffect.Install();
            CustomBlurEffect.Install();
            AeroEffect.Install();
        }

        // SystemBackdrop 效果通过 DwmSetWindowAttribute 应用，无需 hook
        SystemBackdropEffect.ApplyToAll(SystemBackdropType.Mica);
    }

    private static void Shutdown()
    {
        ColorSchemeWatcher.Stop();
        AccentBlurEffect.Uninstall();
        CustomBlurEffect.Uninstall();
        AeroEffect.Uninstall();
    }

    [UnmanagedCallersOnly]
    private static void OnColorSchemeChangedNative(bool isDark)
    {
        AccentBlurEffect.Parameters = isDark
            ? BlurParameters.Dark
            : BlurParameters.Default;
    }
}
