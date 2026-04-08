using System.Drawing;
using System.Drawing.Drawing2D;

namespace Lumina.Forms;

/// <summary>
/// Represents a group box used to visually group related controls.
/// </summary>
public class GroupBox : ContainerControlBase
{
    private const int CaptionTextOffset = 8;
    private const int CaptionGapPadding = 2;

    /// <summary>
    /// Initializes a group box with WinForms-compatible default spacing.
    /// </summary>
    public GroupBox()
    {
        Padding = new Padding(3);
        Size = new Size(200, 100);
    }

    /// <inheritdoc />
    protected override string ClassName => "BUTTON";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.BS_GROUPBOX;

    /// <inheritdoc />
    protected override uint ExStyle => base.ExStyle | Win32.WS_EX_CONTROLPARENT;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    protected override bool UseParentBackgroundForTheming => true;

    /// <inheritdoc />
    public override Rectangle DisplayRectangle
    {
        get
        {
            int fontHeight = GetCaptionFontHeight();
            return new Rectangle(
                Padding.Left,
                fontHeight + Padding.Top,
                Math.Max(0, Width - Padding.Horizontal),
                Math.Max(0, Height - fontHeight - Padding.Vertical));
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyNativeThemeState();
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyNativeThemeState();
        ApplyExplorerTheme();
        Invalidate();
    }

    /// <inheritdoc />
    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        switch (message)
        {
            case Win32.WM_ERASEBKGND:
                if (wParam != 0)
                {
                    PaintBackground(wParam);
                    result = (nint)1;
                    return true;
                }

                break;

            case Win32.WM_PAINT:
                PaintGroupBox(0);
                result = 0;
                return true;

            case Win32.WM_PRINTCLIENT:
                if (wParam != 0)
                {
                    PaintGroupBox(wParam);
                    result = 0;
                    return true;
                }

                break;
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }

    private void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
    }

    private void PaintGroupBox(nint targetHdc)
    {
        if (Handle == 0 || Width <= 0 || Height <= 0)
        {
            return;
        }

        bool ownsPaintScope = false;
        Win32.PAINTSTRUCT paintStruct = default;
        nint hdc = targetHdc;

        if (hdc == 0)
        {
            hdc = Win32.BeginPaint(Handle, out paintStruct);
            if (hdc == 0)
            {
                return;
            }

            ownsPaintScope = true;
        }

        try
        {
            PaintBackground(hdc);
            DrawBorderAndCaption(hdc);
        }
        finally
        {
            if (ownsPaintScope)
            {
                _ = Win32.EndPaint(Handle, ref paintStruct);
            }
        }
    }

    private void PaintBackground(nint hdc)
    {
        var bounds = new Win32.RECT
        {
            Left = 0,
            Top = 0,
            Right = Width,
            Bottom = Height,
        };

        if (TryGetThemeColors(out nint backgroundBrush, out _, out _, out _)
            && backgroundBrush != 0)
        {
            _ = Win32.FillRect(hdc, ref bounds, backgroundBrush);
        }
    }

    /// <inheritdoc />
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    private Rectangle MeasureCaptionBounds(nint hdc, int fontHeight)
    {
        if (string.IsNullOrEmpty(Text))
        {
            return Rectangle.Empty;
        }

        int horizontalInset = CaptionTextOffset;
        int maxWidth = Math.Max(0, Width - (horizontalInset * 2));
        if (maxWidth == 0)
        {
            return Rectangle.Empty;
        }

        nint fontHandle = Owner is not null && Owner.UiFontHandle != 0
            ? Owner.UiFontHandle
            : Win32.GetStockObject(Win32.DEFAULT_GUI_FONT);
        nint previousFont = 0;

        try
        {
            if (fontHandle != 0)
            {
                previousFont = Win32.SelectObject(hdc, fontHandle);
            }

            Win32.RECT measureRect = new()
            {
                Left = horizontalInset,
                Top = 0,
                Right = horizontalInset + maxWidth,
                Bottom = Math.Max(fontHeight + 2, Height),
            };

            int drawResult = Win32.DrawTextW(
                hdc,
                Text,
                Text.Length,
                ref measureRect,
                Win32.DT_LEFT | Win32.DT_SINGLELINE | Win32.DT_CALCRECT | Win32.DT_HIDEPREFIX);

            int width = Math.Min(maxWidth, Math.Max(0, measureRect.Width));
            if ((drawResult == 0 || width == 0) && Win32.GetTextExtentPoint32W(hdc, Text, Text.Length, out Win32.SIZE textSize))
            {
                width = Math.Min(maxWidth, Math.Max(0, textSize.cx));
            }

            int height = Math.Max(fontHeight, Math.Max(0, measureRect.Height));
            return new Rectangle(horizontalInset, 0, width, height);
        }
        finally
        {
            if (previousFont != 0)
            {
                _ = Win32.SelectObject(hdc, previousFont);
            }
        }
    }

    private void DrawCaptionText(nint hdc, Rectangle textBounds, uint textArgb)
    {
        if (textBounds.Width <= 0 || textBounds.Height <= 0 || string.IsNullOrEmpty(Text))
        {
            return;
        }

        nint fontHandle = Owner is not null && Owner.UiFontHandle != 0
            ? Owner.UiFontHandle
            : Win32.GetStockObject(Win32.DEFAULT_GUI_FONT);
        nint previousFont = 0;

        try
        {
            if (fontHandle != 0)
            {
                previousFont = Win32.SelectObject(hdc, fontHandle);
            }

            _ = Win32.SetBkMode(hdc, Win32.TRANSPARENT);
            _ = Win32.SetTextColor(hdc, Win32.ToColorRef(textArgb));

            Win32.RECT captionRect = new()
            {
                Left = textBounds.Left,
                Top = textBounds.Top,
                Right = textBounds.Right,
                Bottom = textBounds.Bottom,
            };

            _ = Win32.DrawTextW(
                hdc,
                Text,
                Text.Length,
                ref captionRect,
                Win32.DT_LEFT | Win32.DT_VCENTER | Win32.DT_SINGLELINE | Win32.DT_HIDEPREFIX);
        }
        finally
        {
            if (previousFont != 0)
            {
                _ = Win32.SelectObject(hdc, previousFont);
            }
        }
    }

    private void DrawBorderAndCaption(nint hdc)
    {
        int fontHeight = GetCaptionFontHeight();
        Rectangle textBounds = MeasureCaptionBounds(hdc, fontHeight);
        int borderTop = Math.Clamp(fontHeight / 2, 0, Math.Max(0, Height - 1));
        int gapLeft = textBounds.Width > 0 ? Math.Max(0, textBounds.Left - CaptionGapPadding) : 0;
        int gapRight = textBounds.Width > 0 ? Math.Min(Width - 1, textBounds.Right + CaptionGapPadding) : 0;

        ThemePalette palette = CurrentVisualStyle.Palette;
        uint borderArgb = Enabled
            ? (palette.SurfaceBorder != 0 ? palette.SurfaceBorder : palette.ControlBorder)
            : palette.DisabledBorder;
        uint textArgb = Enabled
            ? (ForeColor.IsEmpty ? palette.SurfaceForeground : unchecked((uint)ForeColor.ToArgb()))
            : palette.DisabledForeground;

        if (ShouldUseClassicFrame())
        {
            DrawClassicFrame(hdc, borderTop, gapLeft, gapRight, textArgb, palette);
        }
        else
        {
            DrawThemedFrame(hdc, borderTop, gapLeft, gapRight, borderArgb);
        }

        DrawCaptionText(hdc, textBounds, textArgb);
    }

    private void DrawThemedFrame(nint hdc, int borderTop, int gapLeft, int gapRight, uint borderArgb)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            return;
        }

        using var graphics = Graphics.FromHdc(hdc);
        graphics.SmoothingMode = SmoothingMode.None;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;

        using var pen = new Pen(Color.FromArgb(unchecked((int)borderArgb)));
        int left = 0;
        int right = Math.Max(0, Width - 1);
        int bottom = Math.Max(borderTop, Height - 1);

        graphics.DrawLine(pen, left, borderTop, left, bottom);
        graphics.DrawLine(pen, left, bottom, right, bottom);
        graphics.DrawLine(pen, right, borderTop, right, bottom);

        if (gapRight > gapLeft)
        {
            if (gapLeft > left)
            {
                graphics.DrawLine(pen, left, borderTop, gapLeft, borderTop);
            }

            if (gapRight < right)
            {
                graphics.DrawLine(pen, gapRight, borderTop, right, borderTop);
            }
        }
        else
        {
            graphics.DrawLine(pen, left, borderTop, right, borderTop);
        }
    }

    private void DrawClassicFrame(nint hdc, int borderTop, int gapLeft, int gapRight, uint textArgb, ThemePalette palette)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            return;
        }

        using var graphics = Graphics.FromHdc(hdc);
        graphics.SmoothingMode = SmoothingMode.None;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;

        int left = 0;
        int right = Math.Max(0, Width - 1);
        int bottom = Math.Max(borderTop, Height - 1);
        int topShadow = Math.Max(0, borderTop - 1);
        bool hasGap = gapRight > gapLeft;

        if (IsHighContrastEnabled())
        {
            uint lineArgb = Enabled ? textArgb : palette.DisabledForeground;
            using var pen = new Pen(Color.FromArgb(unchecked((int)lineArgb)));

            graphics.DrawLine(pen, left, borderTop, left, bottom);
            graphics.DrawLine(pen, left, bottom, right, bottom);
            graphics.DrawLine(pen, right, borderTop, right, bottom);

            if (hasGap)
            {
                if (gapLeft > left)
                {
                    graphics.DrawLine(pen, left, borderTop, gapLeft, borderTop);
                }

                if (gapRight < right)
                {
                    graphics.DrawLine(pen, gapRight, borderTop, right, borderTop);
                }
            }
            else
            {
                graphics.DrawLine(pen, left, borderTop, right, borderTop);
            }

            return;
        }

        uint backgroundArgb = palette.SurfaceBackground;
        uint lightArgb = BlendArgb(backgroundArgb, 0xFF_FF_FF_FF, 0.45f);
        uint darkArgb = BlendArgb(backgroundArgb, 0xFF_00_00_00, 0.35f);

        using var lightPen = new Pen(Color.FromArgb(unchecked((int)lightArgb)));
        using var darkPen = new Pen(Color.FromArgb(unchecked((int)darkArgb)));

        graphics.DrawLine(lightPen, 1, borderTop, 1, bottom);
        graphics.DrawLine(lightPen, left, bottom, right, bottom);
        graphics.DrawLine(lightPen, right, topShadow, right, bottom);

        graphics.DrawLine(darkPen, left, borderTop, left, Math.Max(borderTop, bottom - 1));
        graphics.DrawLine(darkPen, left, Math.Max(borderTop, bottom - 1), Math.Max(0, right - 1), Math.Max(borderTop, bottom - 1));
        graphics.DrawLine(darkPen, Math.Max(0, right - 1), topShadow, Math.Max(0, right - 1), Math.Max(borderTop, bottom - 1));

        if (hasGap)
        {
            if (gapLeft > 1)
            {
                graphics.DrawLine(lightPen, 1, borderTop, gapLeft, borderTop);
            }

            if (gapRight < right)
            {
                graphics.DrawLine(lightPen, gapRight, borderTop, right, borderTop);
            }

            if (gapLeft > left)
            {
                graphics.DrawLine(darkPen, left, topShadow, gapLeft, topShadow);
            }

            if (gapRight < Math.Max(0, right - 1))
            {
                graphics.DrawLine(darkPen, gapRight, topShadow, Math.Max(0, right - 1), topShadow);
            }
        }
        else
        {
            graphics.DrawLine(lightPen, 1, borderTop, right, borderTop);
            graphics.DrawLine(darkPen, left, topShadow, Math.Max(0, right - 1), topShadow);
        }
    }

    private int GetCaptionFontHeight()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            try
            {
                Font? messageBoxFont = SystemFonts.MessageBoxFont;
                if (messageBoxFont is not null && messageBoxFont.Height > 0)
                {
                    return messageBoxFont.Height;
                }
            }
            catch (ArgumentException)
            {
            }
            catch (PlatformNotSupportedException)
            {
            }
        }

        nint fontHandle = Owner is not null && Owner.UiFontHandle != 0
            ? Owner.UiFontHandle
            : Win32.GetStockObject(Win32.DEFAULT_GUI_FONT);

        return Win32.GetFontHeight(fontHandle);
    }

    private bool ShouldUseClassicFrame()
    {
        return CurrentVisualStyle.VisualStyleKind == VisualStyleKind.Classic
            || IsHighContrastEnabled()
            || Width < 10
            || Height < 10;
    }

    private static bool IsHighContrastEnabled()
    {
        var highContrast = new Win32.HIGHCONTRASTW
        {
            cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<Win32.HIGHCONTRASTW>(),
        };

        return Win32.SystemParametersInfoW(Win32.SPI_GETHIGHCONTRAST, highContrast.cbSize, ref highContrast, 0)
            && (highContrast.dwFlags & Win32.HCF_HIGHCONTRASTON) != 0;
    }

    private static uint BlendArgb(uint background, uint foreground, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);

        float backgroundRed = (background >> 16) & 0xFF;
        float backgroundGreen = (background >> 8) & 0xFF;
        float backgroundBlue = background & 0xFF;

        float foregroundRed = (foreground >> 16) & 0xFF;
        float foregroundGreen = (foreground >> 8) & 0xFF;
        float foregroundBlue = foreground & 0xFF;

        byte red = (byte)Math.Clamp((backgroundRed * (1f - amount)) + (foregroundRed * amount), 0f, 255f);
        byte green = (byte)Math.Clamp((backgroundGreen * (1f - amount)) + (foregroundGreen * amount), 0f, 255f);
        byte blue = (byte)Math.Clamp((backgroundBlue * (1f - amount)) + (foregroundBlue * amount), 0f, 255f);

        return 0xFF_00_00_00
            | ((uint)red << 16)
            | ((uint)green << 8)
            | blue;
    }
}
