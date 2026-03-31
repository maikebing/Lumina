namespace Lumina.Forms;

/// <summary>
/// Represents a single page hosted by a tab control.
/// </summary>
public class TabPage : Panel
{
    /// <summary>
    /// Gets or sets the page padding.
    /// </summary>
    public Padding Padding { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether visual styles are used for the page background.
    /// </summary>
    public bool UseVisualStyleBackColor { get; set; }
}