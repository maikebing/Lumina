using System.Runtime.InteropServices;

namespace Lumina;

/// <summary>
/// 标准模式入口：基于公开 DWM API，无需管理员权限。
/// </summary>
public static class LuminaWindow
{
    /// <summary>
    /// 为指定窗口句柄应用视觉效果。
    /// </summary>
    /// <param name="hwnd">目标窗口句柄（HWND）。</param>
    /// <param name="kind">效果类型。</param>
    /// <param name="options">效果参数，传 <c>null</c> 使用默认值。</param>
    /// <exception cref="ArgumentException"><paramref name="hwnd"/> 为零。</exception>
    public static void SetEffect(nint hwnd, EffectKind kind, EffectOptions? options = null)
    {
        if (hwnd == 0) throw new ArgumentException("hwnd 不能为零。", nameof(hwnd));
        options ??= EffectOptions.Default;

        // 先清除旧效果
        ClearBackdrop(hwnd);

        switch (kind)
        {
            case EffectKind.None:
                break;

            case EffectKind.Mica:
                SetSystemBackdrop(hwnd, DwmSystemBackdropType.Mica);
                break;

            case EffectKind.MicaAlt:
                SetSystemBackdrop(hwnd, DwmSystemBackdropType.MicaAlt);
                break;

            case EffectKind.Acrylic:
                SetSystemBackdrop(hwnd, DwmSystemBackdropType.Acrylic);
                break;

            case EffectKind.Blur:
            case EffectKind.Aero:
                // 标准模式下退回到 Acrylic（Hook 版在 Lumina.Core.Advanced）
                SetBlurBehind(hwnd, options);
                break;
        }
    }

    /// <summary>移除窗口上的所有 Lumina 效果。</summary>
    public static void Clear(nint hwnd)
    {
        if (hwnd == 0) return;
        ClearBackdrop(hwnd);
    }

    // ── 内部实现 ─────────────────────────────────────────────────

    private static void SetSystemBackdrop(nint hwnd, DwmSystemBackdropType type)
    {
        int value = (int)type;
        DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref value, sizeof(int));
    }

    private static void ClearBackdrop(nint hwnd)
    {
        int none = 0;
        DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref none, sizeof(int));

        var bb = new DWM_BLURBEHIND { dwFlags = 0, fEnable = false };
        DwmEnableBlurBehindWindow(hwnd, ref bb);
    }

    private static void SetBlurBehind(nint hwnd, EffectOptions options)
    {
        var bb = new DWM_BLURBEHIND
        {
            dwFlags  = DWM_BB_ENABLE,
            fEnable  = true,
            hRgnBlur = 0,
        };
        DwmEnableBlurBehindWindow(hwnd, ref bb);
    }

    // ── P/Invoke ─────────────────────────────────────────────────

    private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
    private const uint DWM_BB_ENABLE            = 0x00000001;

    private enum DwmSystemBackdropType : int
    {
        None    = 0,
        Auto    = 1,
        Mica    = 2,
        Acrylic = 3,
        MicaAlt = 4,
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DWM_BLURBEHIND
    {
        public uint dwFlags;
        [MarshalAs(UnmanagedType.Bool)] public bool fEnable;
        public nint hRgnBlur;
        [MarshalAs(UnmanagedType.Bool)] public bool fTransitionOnMaximized;
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        nint hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll")]
    private static extern int DwmEnableBlurBehindWindow(
        nint hwnd, ref DWM_BLURBEHIND pBlurBehind);
}
