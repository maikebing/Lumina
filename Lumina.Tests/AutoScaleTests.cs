using System.Drawing;
using System.Runtime.InteropServices;
using Lumina.Forms;
using Xunit;

namespace Lumina.Tests;

public class AutoScaleTests
{
    [Fact]
    public void PerformAutoScale_ScalesFormAndAttachedControls()
    {
        using var form = new Form
        {
            AutoScaleMode = AutoScaleMode.Dpi,
            AutoScaleDimensions = new SizeF(48f, 48f),
            ClientSize = new Size(100, 80),
        };

        var button = new Button();
        button.SetBounds(10, 12, 30, 20);
        form.Controls.Add(button);

        form.PerformAutoScale();

        Assert.True(form.Width >= 200);
        Assert.True(form.Height >= 160);
        Assert.True(button.Left >= 20);
        Assert.True(button.Top >= 24);
        Assert.True(button.Width >= 60);
        Assert.True(button.Height >= 40);
        Assert.Equal(form.CurrentAutoScaleDimensions, form.AutoScaleDimensions);
    }

    [Fact]
    public void PerformAutoScale_WithNoneMode_DoesNotChangeBounds()
    {
        using var form = new Form
        {
            AutoScaleMode = AutoScaleMode.None,
            ClientSize = new Size(100, 80),
        };

        var label = new Label();
        label.SetBounds(10, 12, 30, 20);
        form.Controls.Add(label);

        form.PerformAutoScale();

        Assert.Equal(100, form.Width);
        Assert.Equal(80, form.Height);
        Assert.Equal(new Rectangle(10, 12, 30, 20), label.Bounds);
    }

    [Fact]
    public void CurrentAutoScaleDimensions_UseSystemMessageBoxFontMetrics()
    {
        using var form = new Form
        {
            AutoScaleMode = AutoScaleMode.Font,
        };

        Font? messageBoxFont = SystemFonts.MessageBoxFont;
        Assert.NotNull(messageBoxFont);

        SizeF expected = GetFontScaleDimensions(messageBoxFont!);

        Assert.Equal(expected.Width, form.CurrentAutoScaleDimensions.Width);
        Assert.Equal(expected.Height, form.CurrentAutoScaleDimensions.Height);
    }

    private static SizeF GetFontScaleDimensions(Font font)
    {
        nint screenDc = GetDC(0);
        Assert.NotEqual(0, screenDc);

        int dpiY = GetDeviceCaps(screenDc, 90);
        int height = -Math.Max(1, (int)Math.Round(font.SizeInPoints * (dpiY > 0 ? dpiY : 96f) / 72f, MidpointRounding.AwayFromZero));
        nint fontHandle = CreateFontW(
            height,
            0,
            0,
            0,
            font.Style.HasFlag(FontStyle.Bold) ? 700 : 400,
            font.Style.HasFlag(FontStyle.Italic) ? 1u : 0u,
            font.Style.HasFlag(FontStyle.Underline) ? 1u : 0u,
            font.Style.HasFlag(FontStyle.Strikeout) ? 1u : 0u,
            font.GdiCharSet,
            0,
            0,
            5,
            0,
            font.Name);

        Assert.NotEqual(0, fontHandle);
        nint previous = SelectObject(screenDc, fontHandle);

        try
        {
            Assert.True(GetTextMetricsW(screenDc, out TEXTMETRICW metrics));
            return new SizeF(metrics.tmAveCharWidth, metrics.tmHeight);
        }
        finally
        {
            if (previous != 0)
            {
                _ = SelectObject(screenDc, previous);
            }

            _ = DeleteObject(fontHandle);
            _ = ReleaseDC(0, screenDc);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct TEXTMETRICW
    {
        public int tmHeight;
        public int tmAscent;
        public int tmDescent;
        public int tmInternalLeading;
        public int tmExternalLeading;
        public int tmAveCharWidth;
        public int tmMaxCharWidth;
        public int tmWeight;
        public int tmOverhang;
        public int tmDigitizedAspectX;
        public int tmDigitizedAspectY;
        public char tmFirstChar;
        public char tmLastChar;
        public char tmDefaultChar;
        public char tmBreakChar;
        public byte tmItalic;
        public byte tmUnderlined;
        public byte tmStruckOut;
        public byte tmPitchAndFamily;
        public byte tmCharSet;
    }

    [DllImport("user32.dll")]
    private static extern nint GetDC(nint hwnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(nint hwnd, nint hdc);

    [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
    private static extern nint CreateFontW(
        int cHeight,
        int cWidth,
        int cEscapement,
        int cOrientation,
        int cWeight,
        uint bItalic,
        uint bUnderline,
        uint bStrikeOut,
        uint iCharSet,
        uint iOutPrecision,
        uint iClipPrecision,
        uint iQuality,
        uint iPitchAndFamily,
        string pszFaceName);

    [DllImport("gdi32.dll")]
    private static extern nint SelectObject(nint hdc, nint obj);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetTextMetricsW(nint hdc, out TEXTMETRICW metrics);

    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(nint hdc, int index);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(nint hObject);
}
