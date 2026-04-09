using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Globalization;

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

    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        => Show(null, text, caption, buttons, MessageBoxIcon.None);

    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        => Show(null, text, caption, buttons, icon);

    public static DialogResult Show(Form? owner, string text, string caption)
        => Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);

    public static DialogResult Show(Form? owner, string text, string caption, MessageBoxButtons buttons)
        => Show(owner, text, caption, buttons, MessageBoxIcon.None);

    /// <summary>
    /// Displays a message box with owner, text, caption, buttons and icon.
    /// </summary>
    public static DialogResult Show(Form? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        if (OperatingSystem.IsWindows() && TryShowTaskDialog(owner, text, caption, buttons, icon, out DialogResult taskDialogResult))
        {
            return taskDialogResult;
        }

        if (!OperatingSystem.IsWindows())
        {
            return DialogResult.None;
        }

        try
        {
            return ShowCustomDialog(owner, text, caption, buttons, icon);
        }
        catch
        {
            DialogResult fallbackResult = ShowMessageBoxCore(owner?.Handle ?? 0, text, caption, buttons, icon);
            if (fallbackResult != DialogResult.None || owner is null)
            {
                return fallbackResult;
            }

            return ShowMessageBoxCore(0, text, caption, buttons, icon);
        }
    }

    private static bool TryShowTaskDialog(Form? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, out DialogResult result)
    {
        result = DialogResult.None;

        if (!OperatingSystem.IsWindows())
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

            if (hr < 0 || buttonId == 0)
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

    private static DialogResult ShowMessageBoxCore(nint ownerHandle, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        uint type = MapButtonsForMessageBox(buttons) | MapIconForMessageBox(icon) | Win32.MB_SETFOREGROUND | Win32.MB_TOPMOST;

        DarkModeNative.RefreshImmersiveState();
        s_pendingDarkMode = Application.CurrentVisualStyle.IsDarkMode;
        s_pendingPalette = Application.CurrentVisualStyle.Palette.Clone();

        unsafe
        {
            nint hookPtr = (nint)(delegate* unmanaged[Stdcall]<int, nint, nint, nint>)&CbtHookProc;
            s_cbtHook = Win32.SetWindowsHookExW(Win32.WH_CBT, hookPtr, 0, Win32.GetCurrentThreadId());
        }

        try
        {
            int result = Win32.MessageBoxW(ownerHandle, text ?? string.Empty, caption ?? string.Empty, type);
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

    private static DialogResult ShowCustomDialog(Form? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        using var dialog = new MessageBoxForm(owner, text, caption, buttons, icon);
        nint ownerHandle = owner?.Handle ?? 0;
        bool ownerDisabled = false;

        try
        {
            if (ownerHandle != 0)
            {
                ownerDisabled = Win32.EnableWindow(ownerHandle, false);
            }

            dialog.Show();
            CenterDialog(ownerHandle, dialog.Handle);
            _ = Win32.SetForegroundWindow(dialog.Handle);

            while (dialog.Handle != 0)
            {
                int getMessageResult = Win32.GetMessage(out Win32.MSG msg, 0, 0, 0);
                if (getMessageResult <= 0)
                {
                    break;
                }

                _ = Win32.TranslateMessage(in msg);
                _ = Win32.DispatchMessage(in msg);
            }

            return dialog.SelectedResult;
        }
        finally
        {
            if (ownerDisabled && ownerHandle != 0)
            {
                _ = Win32.EnableWindow(ownerHandle, true);
                _ = Win32.SetForegroundWindow(ownerHandle);
            }
        }
    }

    private static void CenterDialog(nint ownerHandle, nint dialogHandle)
    {
        if (ownerHandle == 0 || dialogHandle == 0)
        {
            return;
        }

        if (!Win32.GetWindowRect(ownerHandle, out Win32.RECT ownerRect)
            || !Win32.GetWindowRect(dialogHandle, out Win32.RECT dialogRect))
        {
            return;
        }

        int width = Math.Max(1, dialogRect.Width);
        int height = Math.Max(1, dialogRect.Height);
        int left = ownerRect.Left + Math.Max(0, (ownerRect.Width - width) / 2);
        int top = ownerRect.Top + Math.Max(0, (ownerRect.Height - height) / 2);
        _ = Win32.MoveWindow(dialogHandle, left, top, width, height, true);
    }

    private static string GetButtonText(DialogResult result)
    {
        bool isChinese = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("zh", StringComparison.OrdinalIgnoreCase);

        return result switch
        {
            DialogResult.OK => isChinese ? "确定" : "OK",
            DialogResult.Yes => isChinese ? "是" : "Yes",
            DialogResult.No => isChinese ? "否" : "No",
            DialogResult.Cancel => isChinese ? "取消" : "Cancel",
            _ => isChinese ? "关闭" : "Close",
        };
    }

    private static DialogResult[] GetDialogButtons(MessageBoxButtons buttons)
    {
        return buttons switch
        {
            MessageBoxButtons.OKCancel => [DialogResult.OK, DialogResult.Cancel],
            MessageBoxButtons.YesNo => [DialogResult.Yes, DialogResult.No],
            MessageBoxButtons.YesNoCancel => [DialogResult.Yes, DialogResult.No, DialogResult.Cancel],
            _ => [DialogResult.OK],
        };
    }

    private static DialogResult GetCloseResult(MessageBoxButtons buttons)
    {
        return buttons switch
        {
            MessageBoxButtons.OK => DialogResult.OK,
            MessageBoxButtons.YesNo => DialogResult.No,
            _ => DialogResult.Cancel,
        };
    }

    private sealed class MessageBoxForm : Form
    {
        private const int HorizontalPadding = 18;
        private const int VerticalPadding = 16;
        private const int IconSize = 32;
        private const int ButtonWidth = 88;
        private const int ButtonHeight = 28;
        private const int ButtonSpacing = 10;

        private readonly MessageBoxButtons _buttons;
        private readonly Label _messageLabel;
        private readonly Panel _buttonPanel;
        private readonly PictureBox? _iconBox;
        private readonly Bitmap? _iconBitmap;

        public MessageBoxForm(Form? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            _buttons = buttons;
            SelectedResult = DialogResult.None;

            Text = string.IsNullOrWhiteSpace(caption) ? "Message" : caption;
            Name = "LuminaMessageBox";
            Width = 420;
            Height = 180;
            Icon = owner?.Icon;

            if (owner is not null)
            {
                SetThemeMode(owner.CurrentVisualStyle.ThemeMode);
            }

            _buttonPanel = new Panel();
            Controls.Add(_buttonPanel);

            _iconBitmap = icon switch
            {
                MessageBoxIcon.Error => SystemIcons.Error.ToBitmap(),
                MessageBoxIcon.Warning => SystemIcons.Warning.ToBitmap(),
                MessageBoxIcon.Information => SystemIcons.Information.ToBitmap(),
                MessageBoxIcon.Question => SystemIcons.Question.ToBitmap(),
                _ => null,
            };

            if (_iconBitmap is not null)
            {
                _iconBox = new PictureBox
                {
                    Image = _iconBitmap,
                    SizeMode = PictureBoxSizeMode.CenterImage,
                };
                Controls.Add(_iconBox);
            }

            _messageLabel = new Label
            {
                Text = text ?? string.Empty,
            };
            Controls.Add(_messageLabel);

            foreach (DialogResult result in GetDialogButtons(buttons))
            {
                var button = new Button
                {
                    Text = GetButtonText(result),
                };
                button.Click += (_, _) =>
                {
                    SelectedResult = result;
                    Close();
                };
                _buttonPanel.Controls.Add(button);
            }

            FormClosing += MessageBoxForm_FormClosing;
            SizeChanged += (_, _) => UpdateLayout();
            UpdateSizeToFit(text ?? string.Empty);
            UpdateLayout();
        }

        public DialogResult SelectedResult { get; private set; }

        protected override uint WindowStyle => Win32.WS_CAPTION | Win32.WS_SYSMENU | Win32.WS_CLIPCHILDREN;

        protected override uint WindowExStyle => Win32.WS_EX_APPWINDOW | Win32.WS_EX_COMPOSITED;

        protected override void OnClosed()
        {
            _iconBitmap?.Dispose();
            base.OnClosed();
        }

        private void MessageBoxForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (SelectedResult == DialogResult.None)
            {
                SelectedResult = GetCloseResult(_buttons);
            }
        }

        private void UpdateSizeToFit(string text)
        {
            using var bitmap = new Bitmap(1, 1);
            using Graphics graphics = Graphics.FromImage(bitmap);
            using Font font = SystemFonts.MessageBoxFont ?? new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);

            int contentMaxWidth = 360;
            SizeF textSize = graphics.MeasureString(string.IsNullOrWhiteSpace(text) ? " " : text, font, contentMaxWidth);
            int iconOffset = _iconBox is null ? 0 : IconSize + 12;
            int buttonCount = _buttonPanel.Controls.Count;
            int buttonsWidth = buttonCount * ButtonWidth + Math.Max(0, buttonCount - 1) * ButtonSpacing;

            Width = Math.Max(320, Math.Max((int)Math.Ceiling(textSize.Width) + HorizontalPadding * 2 + iconOffset, buttonsWidth + HorizontalPadding * 2 + 20));
            Height = Math.Max(150, VerticalPadding * 3 + Math.Max(IconSize, (int)Math.Ceiling(textSize.Height)) + ButtonHeight + 24);
        }

        private void UpdateLayout()
        {
            int clientWidth = ClientSize.Width;
            int clientHeight = ClientSize.Height;
            int buttonPanelHeight = ButtonHeight + 16;
            int contentTop = VerticalPadding;
            int contentHeight = Math.Max(IconSize, clientHeight - buttonPanelHeight - VerticalPadding * 2);
            int textLeft = HorizontalPadding;

            if (_iconBox is not null)
            {
                int iconTop = contentTop + Math.Max(0, (contentHeight - IconSize) / 2);
                _iconBox.SetBounds(HorizontalPadding, iconTop, IconSize, IconSize);
                textLeft += IconSize + 12;
            }

            _messageLabel.SetBounds(textLeft, contentTop, Math.Max(120, clientWidth - textLeft - HorizontalPadding), contentHeight);

            _buttonPanel.SetBounds(0, Math.Max(0, clientHeight - buttonPanelHeight), clientWidth, buttonPanelHeight);
            int buttonsWidth = _buttonPanel.Controls.Count * ButtonWidth + Math.Max(0, _buttonPanel.Controls.Count - 1) * ButtonSpacing;
            int buttonLeft = Math.Max(HorizontalPadding, clientWidth - HorizontalPadding - buttonsWidth);
            int buttonTop = Math.Max(0, (buttonPanelHeight - ButtonHeight) / 2);

            foreach (Control control in _buttonPanel.Controls)
            {
                control.SetBounds(buttonLeft, buttonTop, ButtonWidth, ButtonHeight);
                buttonLeft += ButtonWidth + ButtonSpacing;
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
