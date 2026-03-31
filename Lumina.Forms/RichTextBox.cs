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
}