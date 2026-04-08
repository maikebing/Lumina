using System.Drawing;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents a common dialog box that prompts the user to choose a font.
/// </summary>
public sealed class FontDialog : IDisposable
{
    /// <summary>
    /// Gets or sets the selected font.
    /// </summary>
    public Font Font { get; set; } = GetDefaultDialogFont();

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color Color { get; set; } = Color.Black;

    /// <summary>
    /// Gets or sets a value indicating whether to show the color controls.
    /// </summary>
    public bool ShowColor { get; set; } = true;

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    public DialogResult ShowDialog() => ShowDialog(null);

    /// <summary>
    /// Shows the dialog with an owner.
    /// </summary>
    public DialogResult ShowDialog(Form? owner)
    {
        nint logFontPtr = 0;
        GCHandle themeContext = default;
        Win32.CommonDialogHookProc? hookProc = null;

        try
        {
            Win32.LOGFONTW logFont = CreateLogFont(Font);
            logFontPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Win32.LOGFONTW>());
            Marshal.StructureToPtr(logFont, logFontPtr, false);

            bool useDarkMode = owner?.CurrentVisualStyle.IsDarkMode ?? Application.CurrentVisualStyle.IsDarkMode;
            themeContext = GCHandle.Alloc(useDarkMode);
            hookProc = FontDialogHook;

            var chooseFont = new Win32.CHOOSEFONTW
            {
                lStructSize = (uint)Marshal.SizeOf<Win32.CHOOSEFONTW>(),
                hwndOwner = owner?.Handle ?? 0,
                lpLogFont = logFontPtr,
                Flags = Win32.CF_SCREENFONTS | Win32.CF_FORCEFONTEXIST | Win32.CF_INITTOLOGFONTSTRUCT | Win32.CF_ENABLEHOOK,
                rgbColors = Win32.ToColorRef(unchecked((uint)Color.ToArgb())),
                lCustData = (nuint)GCHandle.ToIntPtr(themeContext),
                lpfnHook = Marshal.GetFunctionPointerForDelegate(hookProc),
            };

            if (ShowColor)
            {
                chooseFont.Flags |= Win32.CF_EFFECTS;
            }

            if (!Win32.ChooseFontW(ref chooseFont))
            {
                int error = Win32.CommDlgExtendedError();
                return error == 0 ? DialogResult.Cancel : DialogResult.Cancel;
            }

            Win32.LOGFONTW selectedLogFont = Marshal.PtrToStructure<Win32.LOGFONTW>(logFontPtr);
            Font = CreateManagedFont(selectedLogFont);
            if (ShowColor)
            {
                Color = Color.FromArgb(unchecked((int)Win32.FromColorRef(chooseFont.rgbColors)));
            }

            return DialogResult.OK;
        }
        finally
        {
            if (themeContext.IsAllocated)
            {
                themeContext.Free();
            }

            if (logFontPtr != 0)
            {
                Marshal.FreeHGlobal(logFontPtr);
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }

    private static Win32.LOGFONTW CreateLogFont(Font font)
    {
        float dpiY = Win32.GetSystemDpiScaleDimensions().Height;
        int lfHeight = -Math.Max(1, (int)Math.Round(font.SizeInPoints * dpiY / 72f, MidpointRounding.AwayFromZero));

        return new Win32.LOGFONTW
        {
            lfFaceName = font.Name,
            lfHeight = lfHeight,
            lfWeight = font.Bold ? Win32.FW_BOLD : Win32.FW_NORMAL,
            lfItalic = font.Italic ? (byte)1 : (byte)0,
            lfUnderline = font.Underline ? (byte)1 : (byte)0,
            lfStrikeOut = font.Strikeout ? (byte)1 : (byte)0,
            lfCharSet = font.GdiCharSet,
            lfOutPrecision = (byte)Win32.OUT_DEFAULT_PRECIS,
            lfClipPrecision = (byte)Win32.CLIP_DEFAULT_PRECIS,
            lfQuality = (byte)Win32.CLEARTYPE_QUALITY,
            lfPitchAndFamily = (byte)Win32.DEFAULT_PITCH,
        };
    }

    private static Font CreateManagedFont(Win32.LOGFONTW logFont)
    {
        nint screenDc = Win32.GetDC(0);
        int dpiY = 96;
        if (screenDc != 0)
        {
            dpiY = Math.Max(1, Win32.GetDeviceCaps(screenDc, Win32.LOGPIXELSY));
            _ = Win32.ReleaseDC(0, screenDc);
        }

        float pointSize = Math.Max(1f, Math.Abs(logFont.lfHeight) * 72f / dpiY);
        FontStyle style = FontStyle.Regular;
        if (logFont.lfWeight >= Win32.FW_BOLD)
        {
            style |= FontStyle.Bold;
        }

        if (logFont.lfItalic != 0)
        {
            style |= FontStyle.Italic;
        }

        if (logFont.lfUnderline != 0)
        {
            style |= FontStyle.Underline;
        }

        if (logFont.lfStrikeOut != 0)
        {
            style |= FontStyle.Strikeout;
        }

        string name = string.IsNullOrWhiteSpace(logFont.lfFaceName) ? GetDefaultDialogFontName() : logFont.lfFaceName;
        return new Font(name, pointSize, style, GraphicsUnit.Point, logFont.lfCharSet);
    }

    private static nint FontDialogHook(nint hWnd, uint message, nint wParam, nint lParam)
    {
        if (message != Win32.WM_INITDIALOG || lParam == 0)
        {
            return 0;
        }

        bool useDarkMode = false;
        try
        {
            Win32.CHOOSEFONTW initData = Marshal.PtrToStructure<Win32.CHOOSEFONTW>(lParam);
            nint contextPtr = (nint)initData.lCustData;
            if (contextPtr != 0)
            {
                GCHandle contextHandle = GCHandle.FromIntPtr(contextPtr);
                if (contextHandle.Target is bool target)
                {
                    useDarkMode = target;
                }
            }
        }
        catch
        {
            useDarkMode = false;
        }

        DarkModeNative.ApplyThemeToWindow(hWnd, useDarkMode);
        _ = Win32.SetWindowTheme(hWnd, useDarkMode ? "DarkMode_Explorer" : "Explorer", null);
        return 0;
    }

    private static Font GetDefaultDialogFont()
        => SystemFonts.MessageBoxFont ?? new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);

    private static string GetDefaultDialogFontName()
    {
        Font? font = SystemFonts.MessageBoxFont;
        return string.IsNullOrWhiteSpace(font?.Name) ? "Segoe UI" : font.Name;
    }
}
