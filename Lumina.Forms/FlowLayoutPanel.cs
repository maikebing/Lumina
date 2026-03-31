namespace Lumina.Forms;

/// <summary>
/// Represents a lightweight flow layout container.
/// </summary>
public class FlowLayoutPanel : Panel
{
	/// <inheritdoc />
	public override void PerformLayout()
	{
		int x = 0;
		int y = 0;
		int rowHeight = 0;
		int maxWidth = Math.Max(1, Width);

		foreach (Control child in ChildControls)
		{
			if (!child.Visible)
			{
				continue;
			}

			if (x > 0 && x + child.Width > maxWidth)
			{
				x = 0;
				y += rowHeight + 4;
				rowHeight = 0;
			}

			child.SetBounds(x, y, child.Width, child.Height);
			x += child.Width + 4;
			rowHeight = Math.Max(rowHeight, child.Height);
		}

		base.PerformLayout();
	}
}