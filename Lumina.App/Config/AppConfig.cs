using System.Xml;
using System.Xml.Serialization;
using Lumina;

namespace Lumina.App.Config;

/// <summary>
/// Lumina.App 的持久化配置，保存于用户 AppData 目录。
/// </summary>
[XmlRoot("LuminaConfig")]
public sealed class AppConfig
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Lumina", "config.xml");

    private static readonly XmlSerializer s_serializer = new(typeof(AppConfig));

    /// <summary>当前效果预设。</summary>
    public EffectKind ActiveEffect { get; set; } = EffectKind.Mica;

    /// <summary>混合叠加色（ARGB）。</summary>
    public uint BlendColor { get; set; } = 0x80_00_00_00;

    /// <summary>模糊半径。</summary>
    public int BlurRadius { get; set; } = 20;

    /// <summary>整体不透明度。</summary>
    public float Opacity { get; set; } = 0.8f;

    /// <summary>UI 语言代码（如 "zh-CN"、"en-US"）。</summary>
    public string Language { get; set; } = "zh-CN";

    /// <summary>排除窗口的进程名列表。</summary>
    [XmlArray("Exclusions")]
    [XmlArrayItem("Process")]
    public List<string> ExcludedProcesses { get; set; } = [];

    /// <summary>从磁盘加载配置，文件不存在时返回默认值。</summary>
    public static AppConfig Load()
    {
        if (!File.Exists(ConfigPath)) return new AppConfig();
        try
        {
            using var sr = new StreamReader(ConfigPath);
            return (AppConfig?)s_serializer.Deserialize(sr) ?? new AppConfig();
        }
        catch
        {
            return new AppConfig();
        }
    }

    /// <summary>将配置保存到磁盘。</summary>
    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
        var settings = new XmlWriterSettings { Indent = true };
        using var xw = XmlWriter.Create(ConfigPath, settings);
        s_serializer.Serialize(xw, this);
    }

    /// <summary>将配置导出到指定路径。</summary>
    public void Export(string path)
    {
        var settings = new XmlWriterSettings { Indent = true };
        using var xw = XmlWriter.Create(path, settings);
        s_serializer.Serialize(xw, this);
    }

    /// <summary>从指定路径导入配置并覆盖当前实例字段。</summary>
    public static AppConfig Import(string path)
    {
        using var sr = new StreamReader(path);
        return (AppConfig?)s_serializer.Deserialize(sr) ?? new AppConfig();
    }
}
