using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Lumina;

/// <summary>
/// 效果配置档案，可序列化为 JSON 或 XML，用于持久化和导入导出。
/// </summary>
[XmlRoot("LuminaProfile")]
public sealed class EffectProfile
{
    /// <summary>配置档案名称。</summary>
    [XmlAttribute]
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

    // ── JSON ──────────────────────────────────────────────────────

    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented        = true,
        Converters           = { new JsonStringEnumConverter() },
    };

    /// <summary>将档案序列化为 JSON 字符串。</summary>
    public string ToJson() => JsonSerializer.Serialize(this, s_jsonOptions);

    /// <summary>从 JSON 字符串反序列化档案。</summary>
    /// <exception cref="JsonException">JSON 格式无效。</exception>
    public static EffectProfile FromJson(string json) =>
        JsonSerializer.Deserialize<EffectProfile>(json, s_jsonOptions)
        ?? throw new JsonException("反序列化结果为 null。");

    /// <summary>保存到 JSON 文件。</summary>
    public void SaveJson(string path) => File.WriteAllText(path, ToJson());

    /// <summary>从 JSON 文件加载。</summary>
    public static EffectProfile LoadJson(string path) => FromJson(File.ReadAllText(path));

    // ── XML ───────────────────────────────────────────────────────

    private static readonly XmlSerializer s_xmlSerializer = new(typeof(EffectProfile));

    /// <summary>将档案序列化为 XML 字符串。</summary>
    public string ToXml()
    {
        var settings = new XmlWriterSettings { Indent = true };
        using var sw = new StringWriter();
        using var xw = XmlWriter.Create(sw, settings);
        s_xmlSerializer.Serialize(xw, this);
        return sw.ToString();
    }

    /// <summary>从 XML 字符串反序列化档案。</summary>
    public static EffectProfile FromXml(string xml)
    {
        using var sr = new StringReader(xml);
        return (EffectProfile?)s_xmlSerializer.Deserialize(sr)
               ?? throw new InvalidOperationException("反序列化结果为 null。");
    }

    /// <summary>保存到 XML 文件。</summary>
    public void SaveXml(string path) => File.WriteAllText(path, ToXml());

    /// <summary>从 XML 文件加载。</summary>
    public static EffectProfile LoadXml(string path) => FromXml(File.ReadAllText(path));
}
