using System.Xml;
using System.Xml.Serialization;

namespace Lumina.App.Config;

/// <summary>
/// 从 XML 语言文件加载本地化字符串。
/// 语言文件位于程序目录 <c>lang\{code}.xml</c>。
/// </summary>
public static class Strings
{
    private static Dictionary<string, string> _table = [];

    /// <summary>当前已加载的语言代码。</summary>
    public static string CurrentLanguage { get; private set; } = "zh-CN";

    /// <summary>
    /// 加载指定语言文件。文件不存在时静默回落到内置中文。
    /// </summary>
    public static void Load(string languageCode)
    {
        string path = Path.Combine(
            AppContext.BaseDirectory, "lang", languageCode + ".xml");

        if (!File.Exists(path))
        {
            LoadBuiltinZhCN();
            return;
        }

        try
        {
            var doc = new XmlDocument();
            doc.Load(path);
            _table = [];
            foreach (XmlElement node in doc.DocumentElement!.ChildNodes.OfType<XmlElement>())
                _table[node.GetAttribute("key")] = node.InnerText;
            CurrentLanguage = languageCode;
        }
        catch
        {
            LoadBuiltinZhCN();
        }
    }

    /// <summary>获取本地化字符串，键不存在时返回键名本身。</summary>
    public static string Get(string key) =>
        _table.TryGetValue(key, out var v) ? v : key;

    // ── 内置中文回落 ─────────────────────────────────────────────

    private static void LoadBuiltinZhCN()
    {
        CurrentLanguage = "zh-CN";
        _table = new Dictionary<string, string>
        {
            ["tray.tooltip"]        = "Lumina — 窗口视觉增强",
            ["tray.settings"]       = "设置",
            ["tray.toggle.enable"]  = "启用效果",
            ["tray.toggle.disable"] = "禁用效果",
            ["tray.exit"]           = "退出",
            ["settings.title"]      = "Lumina 设置",
            ["settings.preset"]     = "效果预设",
            ["settings.color"]      = "混合色",
            ["settings.apply"]      = "应用",
            ["settings.close"]      = "关闭",
            ["error.noAdmin"]       = "需要管理员权限，请以管理员身份运行。",
            ["error.noDwm"]         = "找不到 dwm.exe 进程。",
            ["error.noExt"]         = "找不到 Lumina.Ext.dll。",
        };
    }
}
