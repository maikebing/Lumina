namespace Lumina.Forms;

/// <summary>
/// Represents a masked text box.
/// </summary>
public class MaskedTextBox : TextBox
{
    /// <summary>
    /// Gets or sets the mask pattern.
    /// </summary>
    public string Mask { get; set; } = string.Empty;
}