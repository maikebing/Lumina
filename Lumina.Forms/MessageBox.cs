using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Displays a message box.
/// </summary>
public static class MessageBox
{
    [ThreadStatic]
    private static nint s_cbtHook;

    [ThreadStatic]
    private static bool s_pendingDarkMode;

    [ThreadStatic]
    private static ThemePalette? s_pendingPalette;

    /// <summary>
    /// Displays a message box with specified text.
    /// </summary>
    public static DialogResult Show(string text)
        => Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);

    /// <summary>
    /// Displays a message box with specified text and caption.
    /// </summary>
    public static DialogResult Show(string text, string caption)
        => Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);

    /// <summary>
    /// Displays a message box with owner, text, caption, buttons and icon.
    /// </summary>
    public static DialogResult Show(Form? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        uint type = MapButtonsForMessageBox(buttons) | MapIconForMessageBox(icon);

        if (!OperatingSystem.IsWindows())
        {
            int fallbackResult = Win32.MessageBoxW(owner?.Handle ?? 0, text ?? string.Empty, caption ?? string.Empty, type);
            return MapResult(fallbackResult);
        }

        DarkModeNative.RefreshImmersiveState();
        s_pendingDarkMode = owner?.CurrentVisualStyle.IsDarkMode ?? Application.CurrentVisualStyle.IsDarkMode;
        s_pendingPalette = (owner?.CurrentVisualStyle.Palette ?? Application.CurrentVisualStyle.Palette).Clone();

        unsafe
        {
            nint hookPtr = (nint)(delegate* unmanaged[Stdcall]<int, nint, nint, nint>)&CbtHookProc;
            s_cbtHook = Win32.SetWindowsHookExW(Win32.WH_CBT, hookPtr, 0, Win32.GetCurrentThreadId());
        }

        try
        {
            int result = Win32.MessageBoxW(owner?.Handle ?? 0, text ?? string.Empty, caption ?? string.Empty, type);
            return MapResult(result);
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

    private static uint MapButtonsForMessageBox(MessageBoxButtons buttons)
    {
        return buttons switch
        {
            MessageBoxButtons.OKCancel => Win32.MB_OKCANCEL,
            MessageBoxButtons.YesNo => Win32.MB_YESNO,
            MessageBoxButtons.YesNoCancel => Win32.MB_YESNOCANCEL,
            _ => Win32.MB_OK,
        };
    }

    private static uint MapIconForMessageBox(MessageBoxIcon icon)
    {
        return icon switch
        {
            MessageBoxIcon.Information => Win32.MB_ICONINFORMATION,
            MessageBoxIcon.Warning => Win32.MB_ICONWARNING,
            MessageBoxIcon.Error => Win32.MB_ICONERROR,
            MessageBoxIcon.Question => Win32.MB_ICONQUESTION,
            _ => 0,
        };
    }

    private static DialogResult MapResult(int result)
    {
        return result switch
        {
            Win32.IDOK => DialogResult.OK,
            Win32.IDYES => DialogResult.Yes,
            Win32.IDNO => DialogResult.No,
            Win32.IDCANCEL => DialogResult.Cancel,
            _ => DialogResult.None,
        };
    }
}
