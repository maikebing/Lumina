using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Displays the operating system about dialog.
/// </summary>
public static class SystemAboutDialog
{
    [ThreadStatic]
    private static nint s_cbtHook;

    [ThreadStatic]
    private static bool s_pendingDarkMode;

    [ThreadStatic]
    private static ThemePalette? s_pendingPalette;

    /// <summary>
    /// Shows the system about dialog.
    /// </summary>
    public static DialogResult Show(string appName, string? additionalInfo = null)
        => Show(null, appName, additionalInfo, null);

    /// <summary>
    /// Shows the system about dialog with owner and icon.
    /// </summary>
    public static DialogResult Show(Form? owner, string appName, string? additionalInfo = null, Icon? icon = null)
    {
        if (string.IsNullOrWhiteSpace(appName))
        {
            appName = "Lumina";
        }

        nint iconHandle = icon?.Handle ?? owner?.Icon?.Handle ?? 0;

        if (!OperatingSystem.IsWindows())
        {
            int fallback = Win32.ShellAboutW(owner?.Handle ?? 0, appName, additionalInfo, iconHandle);
            return fallback > 0 ? DialogResult.OK : DialogResult.Cancel;
        }

        s_pendingDarkMode = owner?.CurrentVisualStyle.IsDarkMode ?? Application.CurrentVisualStyle.IsDarkMode;
        s_pendingPalette = (owner?.CurrentVisualStyle.Palette ?? Application.CurrentVisualStyle.Palette).Clone();

        unsafe
        {
            nint hookPtr = (nint)(delegate* unmanaged[Stdcall]<int, nint, nint, nint>)&CbtHookProc;
            s_cbtHook = Win32.SetWindowsHookExW(Win32.WH_CBT, hookPtr, 0, Win32.GetCurrentThreadId());
        }

        try
        {
            int result = Win32.ShellAboutW(owner?.Handle ?? 0, appName, additionalInfo, iconHandle);
            return result > 0 ? DialogResult.OK : DialogResult.Cancel;
        }
        finally
        {
            if (s_cbtHook != 0)
            {
                _ = Win32.UnhookWindowsHookEx(s_cbtHook);
                s_cbtHook = 0;
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint CbtHookProc(int nCode, nint wParam, nint lParam)
    {
        nint hook = s_cbtHook;

        if (nCode == Win32.HCBT_ACTIVATE && wParam != 0)
        {
            CommonDialogThemeHelper.Apply(wParam, s_pendingDarkMode, s_pendingPalette, uniformBackground: true);
            if (hook != 0)
            {
                _ = Win32.UnhookWindowsHookEx(hook);
                s_cbtHook = 0;
            }
        }

        return Win32.CallNextHookEx(hook, nCode, wParam, lParam);
    }
}
