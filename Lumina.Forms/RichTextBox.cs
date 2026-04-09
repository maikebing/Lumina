using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a multi-line rich text edit control.
/// </summary>
public class RichTextBox : TextBox
{
    private Color _selectionColor = Color.Empty;

    /// <summary>
    /// Initializes a multi-line rich text box.
    /// </summary>
    public RichTextBox()
        : base(multiline: true)
    {
        _ = DarkModeNative.TryEnableDarkScrollBar();
    }

    public bool HideSelection { get; set; } = true;

    public Color SelectionColor
    {
        get => _selectionColor;
        set => _selectionColor = value;
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