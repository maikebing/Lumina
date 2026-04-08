using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Displays a message box.
/// </summary>
public static class MessageBox
{
    private static readonly bool s_isTaskDialogSupported = IsTaskDialogSupported();

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
        if (OperatingSystem.IsWindows() && TryShowTaskDialog(owner, text, caption, buttons, icon, out DialogResult taskDialogResult))
        {
            return taskDialogResult;
        }

        uint type = MapButtonsForMessageBox(buttons) | MapIconForMessageBox(icon);

        if (!OperatingSystem.IsWindows())
        {
            return DialogResult.None;
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

    private static bool TryShowTaskDialog(Form? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, out DialogResult result)
    {
        result = DialogResult.None;

        if (!s_isTaskDialogSupported)
        {
            return false;
        }

        try
        {
            int hr = Win32.TaskDialog(
                owner?.Handle ?? 0,
                0,
                string.IsNullOrEmpty(caption) ? null : caption,
                null,
                text ?? string.Empty,
                MapButtonsForTaskDialog(buttons),
                MapIconForTaskDialog(icon),
                out int buttonId);

            if (hr < 0)
            {
                return false;
            }

            result = MapResult(buttonId);
            return true;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsTaskDialogSupported()
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        nint comctl32 = Win32.GetModuleHandleW("comctl32.dll");
        if (comctl32 == 0)
        {
            comctl32 = Win32.LoadLibraryExW("comctl32.dll", 0, Win32.LOAD_LIBRARY_SEARCH_SYSTEM32);
        }

        return comctl32 != 0 && Win32.GetProcAddress(comctl32, "TaskDialog") != 0;
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

    private static uint MapButtonsForTaskDialog(MessageBoxButtons buttons)
    {
        return buttons switch
        {
            MessageBoxButtons.OKCancel => Win32.TDCBF_OK_BUTTON | Win32.TDCBF_CANCEL_BUTTON,
            MessageBoxButtons.YesNo => Win32.TDCBF_YES_BUTTON | Win32.TDCBF_NO_BUTTON,
            MessageBoxButtons.YesNoCancel => Win32.TDCBF_YES_BUTTON | Win32.TDCBF_NO_BUTTON | Win32.TDCBF_CANCEL_BUTTON,
            _ => Win32.TDCBF_OK_BUTTON,
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

    private static nint MapIconForTaskDialog(MessageBoxIcon icon)
    {
        return icon switch
        {
            MessageBoxIcon.Warning => Win32.TD_WARNING_ICON,
            MessageBoxIcon.Error => Win32.TD_ERROR_ICON,
            MessageBoxIcon.Information => Win32.TD_INFORMATION_ICON,
            MessageBoxIcon.Question => Win32.TD_INFORMATION_ICON,
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
