using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;

namespace Lumina.NotepadDemo;

[SupportedOSPlatform("windows6.1")]
public partial class frmNotepad
{
    private void ApplyMenuIcons()
    {
        文件ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.FileMenu);
        编辑ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.EditMenu);
        格式ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.FormatMenu);
        帮助ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.HelpMenu);

        新建ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.NewFile);
        打开ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.OpenFile);
        打开文件夹ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.OpenFolder);
        保存ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.Save);
        另存为ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.SaveAs);
        退出ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.Exit);

        查找ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.Find);
        替换ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.Replace);
        插入时间日期ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.DateTime);
        统计ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.WordCount);

        自动换行ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.WrapText);
        字体ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.Font);
        颜色ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.Color);

        系统关于ToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.About);
        GiteeToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.Gitee);
        GitHubToolStripMenuItem.Image = NotepadMenuIconFactory.GetIcon(MenuIconKind.GitHub);
    }
}

internal enum MenuIconKind
{
    FileMenu,
    EditMenu,
    FormatMenu,
    HelpMenu,
    NewFile,
    OpenFile,
    OpenFolder,
    Save,
    SaveAs,
    Exit,
    Find,
    Replace,
    DateTime,
    WordCount,
    WrapText,
    Font,
    Color,
    About,
    Gitee,
    GitHub,
}

[SupportedOSPlatform("windows6.1")]
internal static class NotepadMenuIconFactory
{
    private static readonly Dictionary<MenuIconKind, Image> s_icons = [];
    private static readonly Lock s_syncRoot = new();

    public static Image GetIcon(MenuIconKind kind)
    {
        lock (s_syncRoot)
        {
            if (!s_icons.TryGetValue(kind, out Image? icon))
            {
                icon = LoadIcon(kind);
                s_icons[kind] = icon;
            }

            return icon;
        }
    }

    private static Image LoadIcon(MenuIconKind kind)
    {
        Bitmap bitmap = kind switch
        {
            MenuIconKind.FileMenu => Properties.Resources.FileMenu,
            MenuIconKind.EditMenu => Properties.Resources.EditMenu,
            MenuIconKind.FormatMenu => Properties.Resources.FormatMenu,
            MenuIconKind.HelpMenu => Properties.Resources.HelpMenu,
            MenuIconKind.NewFile => Properties.Resources.NewFile,
            MenuIconKind.OpenFile => Properties.Resources.OpenFile,
            MenuIconKind.OpenFolder => Properties.Resources.OpenFolder,
            MenuIconKind.Save => Properties.Resources.Save,
            MenuIconKind.SaveAs => Properties.Resources.SaveAs,
            MenuIconKind.Exit => Properties.Resources.Exit,
            MenuIconKind.Find => Properties.Resources.Find,
            MenuIconKind.Replace => Properties.Resources.Replace,
            MenuIconKind.DateTime => Properties.Resources.DateTime,
            MenuIconKind.WordCount => Properties.Resources.WordCount,
            MenuIconKind.WrapText => Properties.Resources.WrapText,
            MenuIconKind.Font => Properties.Resources.Font,
            MenuIconKind.Color => Properties.Resources.Color,
            MenuIconKind.About => Properties.Resources.About,
            MenuIconKind.Gitee => Properties.Resources.Gitee,
            MenuIconKind.GitHub => Properties.Resources.GitHub,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };

        return new Bitmap(bitmap);
    }
}