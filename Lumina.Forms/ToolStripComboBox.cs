namespace Lumina.Forms;

/// <summary>
/// Represents a combo box hosted in a tool strip.
/// </summary>
public class ToolStripComboBox : ToolStripItem
{
    /// <summary>
    /// Gets the logical items stored in the combo box.
    /// </summary>
    public List<object?> Items { get; } = [];

    /// <summary>
    /// Gets or sets the selected index.
    /// </summary>
    public int SelectedIndex { get; set; } = -1;
}