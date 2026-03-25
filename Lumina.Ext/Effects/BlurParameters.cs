namespace Lumina.Ext.Effects;

internal struct BlurParameters
{
    internal int   BlurRadius;   // 0–100
    internal uint  BlendColor;   // AABBGGRR packed
    internal float Opacity;      // 0.0–1.0

    internal static BlurParameters Default => new() { BlurRadius = 20, BlendColor = 0x80000000, Opacity = 0.8f };
    internal static BlurParameters Dark    => new() { BlurRadius = 30, BlendColor = 0xCC000000, Opacity = 0.9f };
}
