using System.Runtime.InteropServices;

namespace Lumina.Ext;

/// <summary>
/// DLL 入口点 — 注入到 dwm.exe 后由 DllMain 启动
/// </summary>
internal static class ExtMain
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
    }

    private static void Shutdown()
    {
        // Phase 2: 卸载所有 Hook
    }
}
