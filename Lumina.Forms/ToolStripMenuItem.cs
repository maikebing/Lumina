namespace Lumina.Forms;

/// <summary>
/// Represents a selectable menu command item.
/// </summary>
public class ToolStripMenuItem : ToolStripDropDownItem
{
	/// <summary>
	/// Gets or sets a value indicating whether the menu item is checked.
	/// </summary>
	public bool Checked { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the checked state toggles when the item is clicked.
	/// </summary>
	public bool CheckOnClick { get; set; }

	/// <inheritdoc />
	protected override void OnClick(EventArgs e)
	{
		if (CheckOnClick)
		{
			Checked = !Checked;
		}

		base.OnClick(e);
	}
}