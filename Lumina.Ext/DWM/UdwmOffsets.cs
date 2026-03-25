namespace Lumina.Ext.DWM;

/// <summary>
/// udwm.dll 各 Windows 版本的函数 RVA 偏移表。
/// Phase 1 中所有偏移均为占位值 (0)；Phase 2 补全真实值后
/// 直接修改此表即可，调用侧 API 不变。
/// </summary>
internal static class UdwmOffsets
{
    // ── 解析后的函数指针（由 Load() 填充）────────────────────
    internal static nint CAccent_UpdateAccentPolicy                        { get; private set; }
    internal static nint CTopLevelWindow_UpdateNCAreaBackground            { get; private set; }
    internal static nint CGlassColorizationParameters_AdjustWindowColorization { get; private set; }

    // ── 是否成功解析 ─────────────────────────────────────────
    internal static bool IsLoaded { get; private set; }

    /// <summary>在 DLL_PROCESS_ATTACH 时调用，查询 udwm.dll 基址并叠加 RVA。</summary>
    internal static void Load()
    {
        uint build = OsVersion.BuildNumber;
        if (!TryGetOffsets(build, out var offsets))
        {
            // 未知构建号——保持全零，Phase 2 hook 安装前应检查 IsLoaded
            IsLoaded = false;
            return;
        }

        nint udwmBase = NativeMethods.GetModuleHandleW("udwm.dll");
        if (udwmBase == 0)
        {
            IsLoaded = false;
            return;
        }

        CAccent_UpdateAccentPolicy                         = offsets.AccentPolicy        != 0 ? udwmBase + offsets.AccentPolicy        : 0;
        CTopLevelWindow_UpdateNCAreaBackground             = offsets.NCAreaBackground    != 0 ? udwmBase + offsets.NCAreaBackground    : 0;
        CGlassColorizationParameters_AdjustWindowColorization = offsets.GlassColorization != 0 ? udwmBase + offsets.GlassColorization : 0;

        IsLoaded = true;
    }

    // ─────────────────────────────────────────────────────────
    // RVA 表（Phase 2 中替换为从 DWMBlurGlass/OpenGlass 整理的真实值）
    // ─────────────────────────────────────────────────────────
    private static bool TryGetOffsets(uint build, out OffsetEntry offsets)
    {
        offsets = build switch
        {
            // Windows 10 20H1 – 22H2  (19041 – 19045)
            >= 19041 and <= 19045 => new OffsetEntry(
                AccentPolicy:      0,   // TODO Phase 2
                NCAreaBackground:  0,
                GlassColorization: 0),

            // Windows 11 21H2  (22000)
            22000 => new OffsetEntry(
                AccentPolicy:      0,
                NCAreaBackground:  0,
                GlassColorization: 0),

            // Windows 11 22H2  (22621)
            22621 => new OffsetEntry(
                AccentPolicy:      0,
                NCAreaBackground:  0,
                GlassColorization: 0),

            // Windows 11 23H2  (22631)
            22631 => new OffsetEntry(
                AccentPolicy:      0,
                NCAreaBackground:  0,
                GlassColorization: 0),

            // Windows 11 24H2  (26100)
            >= 26100 => new OffsetEntry(
                AccentPolicy:      0,
                NCAreaBackground:  0,
                GlassColorization: 0),

            _ => default
        };

        return build is (>= 19041 and <= 19045) or 22000 or 22621 or 22631 or >= 26100;
    }

    private readonly record struct OffsetEntry(int AccentPolicy, int NCAreaBackground, int GlassColorization);
}
