using System.Runtime.InteropServices;

namespace Lumina.App;

/// <summary>
/// 监视电源状态，电池供电时自动降级效果以节省性能。
/// </summary>
internal static partial class PowerMonitor
{
    private const uint PBT_APMPOWERSTATUSCHANGE = 0x000A;
    private const uint WM_POWERBROADCAST        = 0x0218;

    /// <summary>当前是否处于节能模式（电池供电且未充电）。</summary>
    internal static bool IsPowerSaving { get; private set; }

    /// <summary>节能状态变化时触发。</summary>
    internal static event Action<bool>? PowerSavingChanged;

    /// <summary>处理来自托盘窗口的 WM_POWERBROADCAST 消息。</summary>
    internal static void HandleMessage(uint msg, nint wParam)
    {
        if (msg != WM_POWERBROADCAST) return;
        if ((uint)wParam != PBT_APMPOWERSTATUSCHANGE) return;
        Refresh();
    }

    /// <summary>主动查询一次电源状态。</summary>
    internal static void Refresh()
    {
        var status = new SYSTEM_POWER_STATUS();
        if (!GetSystemPowerStatus(ref status)) return;

        // ACLineStatus: 0 = 电池, 1 = 交流电, 255 = 未知
        bool onBattery = status.ACLineStatus == 0;
        if (onBattery == IsPowerSaving) return;

        IsPowerSaving = onBattery;
        PowerSavingChanged?.Invoke(IsPowerSaving);
    }

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetSystemPowerStatus(ref SYSTEM_POWER_STATUS lpSystemPowerStatus);

    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEM_POWER_STATUS
    {
        public byte  ACLineStatus;
        public byte  BatteryFlag;
        public byte  BatteryLifePercent;
        public byte  SystemStatusFlag;
        public uint  BatteryLifeTime;
        public uint  BatteryFullLifeTime;
    }
}
