using System.Drawing;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents a common dialog box that displays available colors.
/// </summary>
public sealed class ColorDialog : IDisposable
{
    private const int CustomColorCount = 16;

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color Color { get; set; } = Color.Black;

    /// <summary>
    /// Gets or sets whether all basic colors are displayed in the dialog.
    /// </summary>
    public bool AnyColor { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the dialog opens with full controls visible.
    /// </summary>
    public bool FullOpen { get; set; } = true;

    /// <summary>
    /// Gets the custom colors.
    /// </summary>
    public int[] CustomColors { get; } = new int[CustomColorCount];

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    public DialogResult ShowDialog() => ShowDialog(null);

    /// <summary>
    /// Shows the dialog with an owner.
    /// </summary>
    public DialogResult ShowDialog(Form? owner)
    {
        nint customColorsPtr = 0;
        GCHandle themeContext = default;
        Win32.CommonDialogHookProc? hookProc = null;

        try
        {
            customColorsPtr = Marshal.AllocHGlobal(sizeof(int) * CustomColorCount);
            Marshal.Copy(CustomColors, 0, customColorsPtr, CustomColorCount);

            bool useDarkMode = owner?.CurrentVisualStyle.IsDarkMode ?? Application.CurrentVisualStyle.IsDarkMode;
            themeContext = GCHandle.Alloc(useDarkMode);
            hookProc = ColorDialogHook;

            uint flags = Win32.CC_RGBINIT | Win32.CC_ENABLEHOOK;
            if (AnyColor)
            {
                flags |= Win32.CC_ANYCOLOR;
            }

            if (FullOpen)
            {
                flags |= Win32.CC_FULLOPEN;
            }

            var chooseColor = new Win32.CHOOSECOLORW
            {
                lStructSize = (uint)Marshal.SizeOf<Win32.CHOOSECOLORW>(),
                hwndOwner = owner?.Handle ?? 0,
                rgbResult = Win32.ToColorRef(unchecked((uint)Color.ToArgb())),
                lpCustColors = customColorsPtr,
                Flags = flags,
                lCustData = (nuint)GCHandle.ToIntPtr(themeContext),
                lpfnHook = Marshal.GetFunctionPointerForDelegate(hookProc),
            };

            if (!Win32.ChooseColorW(ref chooseColor))
            {
                int error = Win32.CommDlgExtendedError();
                return error == 0 ? DialogResult.Cancel : DialogResult.Cancel;
            }

            Marshal.Copy(customColorsPtr, CustomColors, 0, CustomColorCount);
            Color = Color.FromArgb(unchecked((int)Win32.FromColorRef(chooseColor.rgbResult)));
            return DialogResult.OK;
        }
        finally
        {
            if (themeContext.IsAllocated)
            {
                themeContext.Free();
            }

            if (customColorsPtr != 0)
            {
                Marshal.FreeHGlobal(customColorsPtr);
            }
        }
    }

    private static nint ColorDialogHook(nint hWnd, uint message, nint wParam, nint lParam)
    {
        if (message != Win32.WM_INITDIALOG || lParam == 0)
        {
            return 0;
        }

        bool useDarkMode = false;
        try
        {
            Win32.CHOOSECOLORW initData = Marshal.PtrToStructure<Win32.CHOOSECOLORW>(lParam);
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

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
