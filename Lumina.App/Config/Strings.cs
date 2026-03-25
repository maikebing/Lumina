using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lumina.App.Config;

[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class StringsJsonContext : JsonSerializerContext { }

/// <summary>
/// 从 JSON 语言文件加载本地化字符串。
/// 语言文件位于程序目录 <c>lang/{code}.json</c>。
/// 文件命名规则：zh-CN / zh-TW 保留区域后缀，其余语言只用主标签（en / de / fr / ru / ja / ko）。
/// </summary>
public static class Strings
{
    private static Dictionary<string, string> _table = [];

    /// <summary>当前已加载的语言代码。</summary>
    public static string CurrentLanguage { get; private set; } = "en";

    /// <summary>
    /// 按操作系统当前 UI 语言自动加载。找不到匹配文件时回落到英语内置表。
    /// </summary>
    public static void LoadFromSystem() =>
        Load(ResolveCode(CultureInfo.CurrentUICulture.Name));

    /// <summary>
    /// 加载指定语言代码。找不到文件时回落到英语内置表。
    /// </summary>
    public static void Load(string languageCode)
    {
        string path = Path.Combine(
            AppContext.BaseDirectory, "lang", languageCode + ".json");

        if (!File.Exists(path))
        {
            LoadBuiltinEnUS();
            return;
        }

        try
        {
            using var fs = File.OpenRead(path);
            _table = JsonSerializer.Deserialize(fs,
                StringsJsonContext.Default.DictionaryStringString) ?? [];
            CurrentLanguage = languageCode;
        }
        catch
        {
            LoadBuiltinEnUS();
        }
    }

    /// <summary>获取本地化字符串，键不存在时返回键名本身。</summary>
    public static string Get(string key) =>
        _table.TryGetValue(key, out var v) ? v : key;

    /// <summary>
    /// 将 OS 语言代码映射到语言文件名。
    /// zh-CN / zh-TW 保留完整代码，其余取主标签。
    /// </summary>
    private static string ResolveCode(string osCode)
    {
        // 精确匹配优先（zh-TW / zh-HK / zh-MO = 繁体）
        if (osCode.Equals("zh-TW", StringComparison.OrdinalIgnoreCase) ||
            osCode.Equals("zh-HK", StringComparison.OrdinalIgnoreCase) ||
            osCode.Equals("zh-MO", StringComparison.OrdinalIgnoreCase) ||
            osCode.StartsWith("zh-Hant", StringComparison.OrdinalIgnoreCase))
            return "zh-TW";

        // 其余 zh-* = 简体
        if (osCode.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
            return "zh-CN";

        // 取主标签（"de-AT" → "de"）
        string primary = osCode.Contains('-')
            ? osCode[..osCode.IndexOf('-')].ToLowerInvariant()
            : osCode.ToLowerInvariant();

        return primary switch
        {
            "de" => "de",
            "fr" => "fr",
            "ru" => "ru",
            "ja" => "ja",
            "ko" => "ko",
            "en" => "en",
            _    => "en",  // 未知语言回落英语
        };
    }

    private static void LoadBuiltinEnUS()
    {
        CurrentLanguage = "en";
        _table = new Dictionary<string, string>
        {
            ["tray.tooltip"]        = "Lumina — Window Visual Enhancer",
            ["tray.settings"]       = "Settings",
            ["tray.toggle.enable"]  = "Enable Effects",
            ["tray.toggle.disable"] = "Disable Effects",
            ["tray.exit"]           = "Exit",
            ["settings.title"]      = "Lumina Settings",
            ["settings.preset"]     = "Effect Preset",
            ["settings.color"]      = "Blend Color",
            ["settings.apply"]      = "Apply",
            ["settings.close"]      = "Close",
            ["error.noAdmin"]       = "Administrator privileges required. Please run as administrator.",
            ["error.noDwm"]         = "Cannot find dwm.exe process.",
            ["error.noExt"]         = "Cannot find Lumina.Ext.dll.",
        };
    }
}
