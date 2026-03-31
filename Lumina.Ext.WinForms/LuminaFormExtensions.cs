
using Lumina;

namespace System.Windows.Forms;

/// <summary>
/// WinForms 扩展方法，为 <see cref="Form"/> 提供一行式效果 API。
/// </summary>
public static class LuminaFormExtensions
{
    /// <summary>应用 Mica 效果。</summary>
    public static void SetMica(this Form form)
        => LuminaWindow.SetEffect(form.Handle, EffectKind.Mica);

    /// <summary>应用 Mica Alt 效果。</summary>
    public static void SetMicaAlt(this Form form)
        => LuminaWindow.SetEffect(form.Handle, EffectKind.MicaAlt);

    /// <summary>应用亚克力模糊效果。</summary>
    /// <param name="form">目标窗体。</param>
    /// <param name="blendColor">混合色，格式 0xAARRGGBB。</param>
    public static void SetAcrylic(this Form form, uint blendColor = 0x80_00_00_00)
        => LuminaWindow.SetEffect(form.Handle, EffectKind.Acrylic,
            new EffectOptions { BlendColor = blendColor });

    /// <summary>应用 Aero 玻璃效果。</summary>
    public static void SetAero(this Form form)
        => LuminaWindow.SetEffect(form.Handle, EffectKind.Aero);

    /// <summary>应用自定义模糊效果。</summary>
    /// <param name="form">目标窗体。</param>
    /// <param name="radius">模糊半径（0–100）。</param>
    public static void SetBlur(this Form form, int radius = 20)
        => LuminaWindow.SetEffect(form.Handle, EffectKind.Blur,
            new EffectOptions { BlurRadius = radius });

    /// <summary>移除所有 Lumina 效果。</summary>
    public static void ClearLuminaEffect(this Form form)
        => LuminaWindow.Clear(form.Handle);
}
