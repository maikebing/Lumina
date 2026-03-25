namespace Lumina;

/// <summary>应用于窗口的视觉效果类型。</summary>
public enum EffectKind
{
    /// <summary>移除所有效果，恢复默认窗口外观。</summary>
    None = 0,

    /// <summary>亚克力模糊（Acrylic Blur）。</summary>
    Acrylic,

    /// <summary>Mica 材质（需要 Windows 11）。</summary>
    Mica,

    /// <summary>Mica Alt 材质（需要 Windows 11 22H2+）。</summary>
    MicaAlt,

    /// <summary>经典 Aero 玻璃效果（含反射与视差）。</summary>
    Aero,

    /// <summary>自定义模糊，通过 <see cref="EffectOptions.BlurRadius"/> 控制半径。</summary>
    Blur,
}
