namespace Lumina.Forms;

/// <summary>
/// Represents a tool strip item that can own a drop-down item collection.
/// </summary>
public class ToolStripDropDownItem : ToolStripItem
{
    /// <summary>
    /// Gets the items contained in the drop-down.
    /// </summary>
    public ToolStripItemCollection DropDownItems { get; } = new();
}