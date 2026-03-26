using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lumina;

/// <summary>
/// Provides source-generated JSON serialization metadata for <see cref="EffectProfile"/>.
/// </summary>
[JsonSerializable(typeof(EffectProfile))]
public partial class EffectProfileJsonContext : JsonSerializerContext { }

/// <summary>
/// 效果配置档案，可序列化为 JSON，用于持久化和导入导出。
/// </summary>
public sealed class EffectProfile
{
    /// <summary>配置档案名称。</summary>
    public string Name { get; set; } = "Default";

    /// <summary>效果类型。</summary>
    public EffectKind Kind { get; set; } = EffectKind.Mica;

    /// <summary>混合叠加色（ARGB，0xAARRGGBB）。</summary>
    public uint BlendColor { get; set; } = 0x80_00_00_00;

    /// <summary>模糊半径（0–100）。</summary>
    public int BlurRadius { get; set; } = 20;

    /// <summary>整体不透明度（0.0–1.0）。</summary>
    public float Opacity { get; set; } = 0.8f;

    /// <summary>将此档案转换为 <see cref="EffectOptions"/>。</summary>
    public EffectOptions ToOptions() => new()
    {
        BlendColor = BlendColor,
        BlurRadius = BlurRadius,
        Opacity    = Opacity,
    };

    /// <summary>将档案序列化为 JSON 字符串。</summary>
    public string ToJson() =>
        JsonSerializer.Serialize(this, EffectProfileJsonContext.Default.EffectProfile);

    /// <summary>从 JSON 字符串反序列化档案。</summary>
    /// <exception cref="JsonException">JSON 格式无效。</exception>
    public static EffectProfile FromJson(string json) =>
        JsonSerializer.Deserialize(json, EffectProfileJsonContext.Default.EffectProfile)
        ?? throw new JsonException("反序列化结果为 null。");

    /// <summary>保存到 JSON 文件。</summary>
    public void SaveJson(string path) => File.WriteAllText(path, ToJson());

    /// <summary>从 JSON 文件加载。</summary>
    public static EffectProfile LoadJson(string path) => FromJson(File.ReadAllText(path));
}
