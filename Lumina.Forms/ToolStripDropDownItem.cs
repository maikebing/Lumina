namespace Lumina.Forms;

/// <summary>
/// Represents a tool strip item that can own a drop-down item collection.
/// </summary>
public class ToolStripDropDownItem : ToolStripItem
{
    private readonly ToolStripItemCollection _dropDownItems;

    /// <summary>
    /// Initializes a drop-down item.
    /// </summary>
    public ToolStripDropDownItem()
    {
        _dropDownItems = new ToolStripItemCollection(NotifyChanged);
    }

    /// <summary>
    /// Gets the items contained in the drop-down.
    /// </summary>
    public ToolStripItemCollection DropDownItems => _dropDownItems;
}
