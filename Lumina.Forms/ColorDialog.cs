using System.Drawing;
using System.Runtime.CompilerServices;
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
    [ThreadStatic]
    private static bool s_pendingDarkMode;

    [ThreadStatic]
    private static ThemePalette? s_pendingPalette;

    /// <summary>
    /// Shows the dialog with an owner.
    /// </summary>
    /// <param name="owner">The owner window for the dialog, or <see langword="null"/>.</param>
    /// <returns>The result of the dialog interaction.</returns>
    public DialogResult ShowDialog(Form? owner)
    {
        nint customColorsPtr = 0;

        try
        {
            customColorsPtr = Marshal.AllocHGlobal(sizeof(int) * CustomColorCount);
            Marshal.Copy(CustomColors, 0, customColorsPtr, CustomColorCount);

            s_pendingDarkMode = owner?.CurrentVisualStyle.IsDarkMode ?? Application.CurrentVisualStyle.IsDarkMode;
            s_pendingPalette = (owner?.CurrentVisualStyle.Palette ?? Application.CurrentVisualStyle.Palette).Clone();

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
                lpfnHook = GetHookProc(),
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
            if (customColorsPtr != 0)
            {
                Marshal.FreeHGlobal(customColorsPtr);
            }
        }
    }

    private static unsafe nint GetHookProc()
        => (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&HookProc;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint HookProc(nint hWnd, uint message, nint wParam, nint lParam)
    {
        try
        {
            if (message == Win32.WM_INITDIALOG)
            {
                CommonDialogThemeHelper.Apply(hWnd, s_pendingDarkMode, s_pendingPalette);
            }
        }
        catch
        {
            // Never propagate exceptions across unmanaged boundary.
        }

        return 0;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
