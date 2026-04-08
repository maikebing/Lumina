using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
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
    private static readonly Dictionary<MenuIconKind, Bitmap> s_icons = [];

    public static Image GetIcon(MenuIconKind kind)
    {
        if (!s_icons.TryGetValue(kind, out Bitmap? icon))
        {
            icon = CreateIcon(kind);
            s_icons[kind] = icon;
        }

        return icon;
    }

    private static Bitmap CreateIcon(MenuIconKind kind)
    {
        var bitmap = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        switch (kind)
        {
            case MenuIconKind.FileMenu:
                DrawFolder(graphics, Color.FromArgb(88, 166, 255), Color.FromArgb(245, 197, 66));
                DrawDocumentBadge(graphics, Color.FromArgb(245, 248, 252));
                break;

            case MenuIconKind.EditMenu:
                DrawPencil(graphics, Color.FromArgb(255, 194, 102));
                break;

            case MenuIconKind.FormatMenu:
                DrawLetter(graphics, "A", Color.FromArgb(102, 217, 239));
                DrawUnderline(graphics, Color.FromArgb(240, 113, 120));
                break;

            case MenuIconKind.HelpMenu:
                DrawInfoCircle(graphics, Color.FromArgb(129, 230, 217), '?');
                break;

            case MenuIconKind.NewFile:
                DrawDocument(graphics, Color.FromArgb(226, 232, 240), Color.FromArgb(75, 192, 132));
                DrawPlusBadge(graphics, Color.FromArgb(75, 192, 132));
                break;

            case MenuIconKind.OpenFile:
                DrawFolder(graphics, Color.FromArgb(100, 181, 246), Color.FromArgb(255, 202, 40));
                DrawArrow(graphics, Color.FromArgb(100, 181, 246), directionRight: false);
                break;

            case MenuIconKind.OpenFolder:
                DrawFolder(graphics, Color.FromArgb(255, 179, 71), Color.FromArgb(255, 213, 79));
                break;

            case MenuIconKind.Save:
                DrawDisk(graphics, Color.FromArgb(77, 182, 172), Color.FromArgb(226, 232, 240));
                break;

            case MenuIconKind.SaveAs:
                DrawDisk(graphics, Color.FromArgb(126, 214, 223), Color.FromArgb(226, 232, 240));
                DrawPencilBadge(graphics, Color.FromArgb(255, 194, 102));
                break;

            case MenuIconKind.Exit:
                DrawDoor(graphics, Color.FromArgb(244, 67, 54));
                DrawArrow(graphics, Color.FromArgb(255, 138, 128), directionRight: true);
                break;

            case MenuIconKind.Find:
                DrawMagnifier(graphics, Color.FromArgb(144, 202, 249));
                break;

            case MenuIconKind.Replace:
                DrawSwap(graphics, Color.FromArgb(129, 199, 132));
                break;

            case MenuIconKind.DateTime:
                DrawClock(graphics, Color.FromArgb(255, 202, 40));
                break;

            case MenuIconKind.WordCount:
                DrawBars(graphics, Color.FromArgb(186, 104, 200));
                break;

            case MenuIconKind.WrapText:
                DrawWrap(graphics, Color.FromArgb(77, 208, 225));
                break;

            case MenuIconKind.Font:
                DrawLetter(graphics, "A", Color.FromArgb(144, 202, 249));
                break;

            case MenuIconKind.Color:
                DrawPalette(graphics);
                break;

            case MenuIconKind.About:
                DrawInfoCircle(graphics, Color.FromArgb(129, 230, 217), 'i');
                break;

            case MenuIconKind.Gitee:
                DrawExternalLink(graphics, Color.FromArgb(255, 112, 67));
                break;

            case MenuIconKind.GitHub:
                DrawExternalLink(graphics, Color.FromArgb(160, 174, 192));
                break;
        }

        return bitmap;
    }

    private static void DrawDocument(Graphics graphics, Color outlineColor, Color accentColor)
    {
        using var paperBrush = new SolidBrush(Color.FromArgb(245, 248, 252));
        using var outlinePen = new Pen(outlineColor, 1.4f);
        using var accentBrush = new SolidBrush(accentColor);

        graphics.FillRectangle(paperBrush, 3, 2, 8, 11);
        graphics.DrawRectangle(outlinePen, 3, 2, 8, 11);
        graphics.FillPolygon(accentBrush, [new PointF(8, 2), new PointF(11, 2), new PointF(11, 5)]);
        graphics.FillRectangle(accentBrush, 5, 6, 4, 1);
        graphics.FillRectangle(accentBrush, 5, 8, 4, 1);
    }

    private static void DrawDocumentBadge(Graphics graphics, Color badgeColor)
    {
        using var brush = new SolidBrush(badgeColor);
        using var pen = new Pen(Color.FromArgb(88, 166, 255), 1.2f);
        graphics.FillEllipse(brush, 9, 8, 5, 5);
        graphics.DrawEllipse(pen, 9, 8, 5, 5);
    }

    private static void DrawPlusBadge(Graphics graphics, Color accentColor)
    {
        using var brush = new SolidBrush(accentColor);
        graphics.FillEllipse(brush, 9, 8, 5, 5);
        using var pen = new Pen(Color.FromArgb(245, 248, 252), 1.2f);
        graphics.DrawLine(pen, 11.5f, 9.5f, 11.5f, 11.5f);
        graphics.DrawLine(pen, 10.5f, 10.5f, 12.5f, 10.5f);
    }

    private static void DrawFolder(Graphics graphics, Color outlineColor, Color fillColor)
    {
        using var fillBrush = new SolidBrush(fillColor);
        using var outlinePen = new Pen(outlineColor, 1.3f);

        graphics.FillRectangle(fillBrush, 2, 5, 11, 7);
        graphics.FillRectangle(fillBrush, 4, 3, 4, 3);
        graphics.DrawRectangle(outlinePen, 2, 5, 11, 7);
        graphics.DrawLine(outlinePen, 2, 5, 4, 3);
        graphics.DrawLine(outlinePen, 8, 3, 13, 3);
    }

    private static void DrawDisk(Graphics graphics, Color bodyColor, Color labelColor)
    {
        using var bodyBrush = new SolidBrush(bodyColor);
        using var labelBrush = new SolidBrush(labelColor);
        using var outlinePen = new Pen(Color.FromArgb(32, 41, 56), 1.2f);

        graphics.FillRectangle(bodyBrush, 2, 2, 11, 11);
        graphics.DrawRectangle(outlinePen, 2, 2, 11, 11);
        graphics.FillRectangle(labelBrush, 4, 3, 5, 3);
        graphics.FillRectangle(labelBrush, 4, 8, 6, 3);
        graphics.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 56)), 9, 3, 2, 3);
    }

    private static void DrawPencil(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 2.1f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };
        using var tipBrush = new SolidBrush(Color.FromArgb(255, 231, 166));

        graphics.DrawLine(pen, 3, 11, 10.5f, 3.5f);
        graphics.FillPolygon(tipBrush, [new PointF(10.2f, 3.2f), new PointF(12.5f, 2.5f), new PointF(11.8f, 4.8f)]);
    }

    private static void DrawPencilBadge(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 1.8f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };
        graphics.DrawLine(pen, 10f, 10.5f, 13f, 7.5f);
    }

    private static void DrawDoor(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 1.3f);
        graphics.DrawRectangle(pen, 2, 2, 5, 11);
        graphics.DrawEllipse(pen, 5.5f, 7f, 0.8f, 0.8f);
    }

    private static void DrawArrow(Graphics graphics, Color accentColor, bool directionRight)
    {
        using var pen = new Pen(accentColor, 1.8f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };

        if (directionRight)
        {
            graphics.DrawLine(pen, 6, 8, 13, 8);
            graphics.DrawLine(pen, 10.5f, 5.5f, 13, 8);
            graphics.DrawLine(pen, 10.5f, 10.5f, 13, 8);
        }
        else
        {
            graphics.DrawLine(pen, 13, 8, 6, 8);
            graphics.DrawLine(pen, 8.5f, 5.5f, 6, 8);
            graphics.DrawLine(pen, 8.5f, 10.5f, 6, 8);
        }
    }

    private static void DrawMagnifier(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 1.8f);
        graphics.DrawEllipse(pen, 2.5f, 2.5f, 6.5f, 6.5f);
        graphics.DrawLine(pen, 8.2f, 8.2f, 12.5f, 12.5f);
    }

    private static void DrawSwap(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 1.6f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };

        graphics.DrawLine(pen, 3, 5, 11, 5);
        graphics.DrawLine(pen, 8.5f, 3, 11, 5);
        graphics.DrawLine(pen, 8.5f, 7, 11, 5);
        graphics.DrawLine(pen, 13, 11, 5, 11);
        graphics.DrawLine(pen, 7.5f, 9, 5, 11);
        graphics.DrawLine(pen, 7.5f, 13, 5, 11);
    }

    private static void DrawClock(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 1.6f);
        graphics.DrawEllipse(pen, 2.5f, 2.5f, 10, 10);
        graphics.DrawLine(pen, 7.5f, 4.5f, 7.5f, 8f);
        graphics.DrawLine(pen, 7.5f, 8f, 10f, 9.5f);
    }

    private static void DrawBars(Graphics graphics, Color accentColor)
    {
        using var brush = new SolidBrush(accentColor);
        graphics.FillRectangle(brush, 3, 9, 2, 4);
        graphics.FillRectangle(brush, 7, 6, 2, 7);
        graphics.FillRectangle(brush, 11, 4, 2, 9);
    }

    private static void DrawWrap(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 1.5f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };

        graphics.DrawLine(pen, 2.5f, 4f, 12.5f, 4f);
        graphics.DrawLine(pen, 2.5f, 8f, 10f, 8f);
        graphics.DrawLine(pen, 10f, 8f, 10f, 11f);
        graphics.DrawLine(pen, 10f, 11f, 7.5f, 8.5f);
        graphics.DrawLine(pen, 10f, 11f, 12.5f, 8.5f);
    }

    private static void DrawLetter(Graphics graphics, string text, Color accentColor)
    {
        using var font = new Font("Segoe UI", 8.5f, FontStyle.Bold, GraphicsUnit.Point);
        using var brush = new SolidBrush(accentColor);
        graphics.DrawString(text, font, brush, new PointF(2f, 1.5f));
    }

    private static void DrawUnderline(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 1.6f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };
        graphics.DrawLine(pen, 3, 12, 13, 12);
    }

    private static void DrawPalette(Graphics graphics)
    {
        using var outlinePen = new Pen(Color.FromArgb(226, 232, 240), 1.2f);
        using var blueBrush = new SolidBrush(Color.FromArgb(88, 166, 255));
        using var orangeBrush = new SolidBrush(Color.FromArgb(255, 166, 77));
        using var greenBrush = new SolidBrush(Color.FromArgb(104, 211, 145));
        using var pinkBrush = new SolidBrush(Color.FromArgb(236, 72, 153));

        graphics.DrawEllipse(outlinePen, 2, 2, 11, 11);
        graphics.FillEllipse(blueBrush, 4, 4, 2, 2);
        graphics.FillEllipse(orangeBrush, 7, 4, 2, 2);
        graphics.FillEllipse(greenBrush, 5, 7, 2, 2);
        graphics.FillEllipse(pinkBrush, 8, 8, 2, 2);
    }

    private static void DrawInfoCircle(Graphics graphics, Color accentColor, char glyph)
    {
        using var pen = new Pen(accentColor, 1.5f);
        using var font = new Font("Segoe UI", 8f, FontStyle.Bold, GraphicsUnit.Point);
        using var brush = new SolidBrush(accentColor);
        graphics.DrawEllipse(pen, 2.5f, 2.5f, 10, 10);
        graphics.DrawString(glyph.ToString(), font, brush, new PointF(5f, 2.5f));
    }

    private static void DrawExternalLink(Graphics graphics, Color accentColor)
    {
        using var pen = new Pen(accentColor, 1.5f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
        };

        graphics.DrawRectangle(pen, 2.5f, 5.5f, 7.5f, 7f);
        graphics.DrawLine(pen, 7f, 3f, 12.5f, 3f);
        graphics.DrawLine(pen, 12.5f, 3f, 12.5f, 8.5f);
        graphics.DrawLine(pen, 6.5f, 9.5f, 12.5f, 3.5f);
    }
}