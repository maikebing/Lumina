namespace Lumina;

/// <summary>效果参数，所有字段均为可选，未设置时使用默认值。</summary>
public sealed class EffectOptions
{
    /// <summary>默认参数（模糊半径 20，混合色半透明黑，透明度 0.8）。</summary>
    public static EffectOptions Default { get; } = new();

    /// <summary>
    /// 混合叠加色，格式为 ARGB（0xAARRGGBB）。
    /// 默认值 <c>0x80000000</c>（半透明黑）。
    /// </summary>
    public uint BlendColor { get; init; } = 0x80_00_00_00;

    /// <summary>
    /// 模糊半径，仅对 <see cref="EffectKind.Blur"/> 和 <see cref="EffectKind.Acrylic"/> 有效。
    /// 范围 0–100，默认 20。
    /// </summary>
    public int BlurRadius { get; init; } = 20;

    /// <summary>
    /// 整体不透明度，范围 0.0–1.0，默认 0.8。
    /// </summary>
    public float Opacity { get; init; } = 0.8f;
}
