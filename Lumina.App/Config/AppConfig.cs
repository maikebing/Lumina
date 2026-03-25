using System.Text.Json;
using System.Text.Json.Serialization;
using Lumina;

namespace Lumina.App.Config;

[JsonSerializable(typeof(AppConfig))]
internal partial class AppConfigJsonContext : JsonSerializerContext { }

/// <summary>
/// Lumina.App 的持久化配置，保存于用户 AppData 目录。
/// </summary>
public sealed class AppConfig
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Lumina", "config.json");

    private static readonly JsonSerializerOptions s_options =
        AppConfigJsonContext.Default.Options;

    public EffectKind ActiveEffect { get; set; } = EffectKind.Mica;
    public uint BlendColor { get; set; } = 0x80_00_00_00;
    public int BlurRadius { get; set; } = 20;
    public float Opacity { get; set; } = 0.8f;
    public string Language { get; set; } = "zh-CN";
    public List<string> ExcludedProcesses { get; set; } = [];

    public static AppConfig Load()
    {
        if (!File.Exists(ConfigPath)) return new AppConfig();
        try
        {
            using var fs = File.OpenRead(ConfigPath);
            return JsonSerializer.Deserialize(fs, AppConfigJsonContext.Default.AppConfig)
                   ?? new AppConfig();
        }
        catch
        {
            return new AppConfig();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
        using var fs = File.Create(ConfigPath);
        JsonSerializer.Serialize(fs, this, AppConfigJsonContext.Default.AppConfig);
    }

    public void Export(string path)
    {
        using var fs = File.Create(path);
        JsonSerializer.Serialize(fs, this, AppConfigJsonContext.Default.AppConfig);
    }

    public static AppConfig Import(string path)
    {
        using var fs = File.OpenRead(path);
        return JsonSerializer.Deserialize(fs, AppConfigJsonContext.Default.AppConfig)
               ?? new AppConfig();
    }
}
