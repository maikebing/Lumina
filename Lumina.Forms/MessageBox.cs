namespace Lumina.Forms;

/// <summary>
/// Displays a message box.
/// </summary>
public static class MessageBox
{
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
        uint type = MapButtons(buttons) | MapIcon(icon);
        int result = Win32.MessageBoxW(owner?.Handle ?? 0, text ?? string.Empty, caption ?? string.Empty, type);
        return MapResult(result);
    }

    private static uint MapButtons(MessageBoxButtons buttons)
    {
        return buttons switch
        {
            MessageBoxButtons.OKCancel => Win32.MB_OKCANCEL,
            MessageBoxButtons.YesNo => Win32.MB_YESNO,
            MessageBoxButtons.YesNoCancel => Win32.MB_YESNOCANCEL,
            _ => Win32.MB_OK,
        };
    }

    private static uint MapIcon(MessageBoxIcon icon)
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
