using Microsoft.Win32;

namespace Lumina.App.Config;

/// <summary>
/// 管理 Lumina.App 的开机自启注册表项。
/// </summary>
internal static class AutoStart
{
    private const string RunKey  = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "Lumina";

    /// <summary>当前是否已启用开机自启。</summary>
    internal static bool IsEnabled
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey);
            return key?.GetValue(AppName) is string path
                   && path.Equals(ExePath, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>启用开机自启。</summary>
    internal static void Enable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        key?.SetValue(AppName, ExePath);
    }

    /// <summary>禁用开机自启。</summary>
    internal static void Disable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        key?.DeleteValue(AppName, throwOnMissingValue: false);
    }

    /// <summary>切换开机自启状态。</summary>
    internal static void Toggle()
    {
        if (IsEnabled) Disable();
        else           Enable();
    }

    private static string ExePath =>
        $"\"{Environment.ProcessPath}\"";
}
