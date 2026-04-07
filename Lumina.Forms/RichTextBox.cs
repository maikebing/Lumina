namespace Lumina.Forms;

/// <summary>
/// Represents a multi-line rich text edit control.
/// </summary>
public class RichTextBox : TextBox
{
    /// <summary>
    /// Initializes a multi-line rich text box.
    /// </summary>
    public RichTextBox()
        : base(multiline: true)
    {
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyTheme();
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        base.ApplyTheme();

        if (Handle == 0)
        {
            return;
        }

        ThemePalette palette = CurrentVisualStyle.Palette;
        _ = Win32.SendMessageW(Handle, Win32.EM_SETBKGNDCOLOR, 0, unchecked((nint)Win32.ToColorRef(palette.ControlBackground)));
    }
}