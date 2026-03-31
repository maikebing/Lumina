using System.ComponentModel;
using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a shortcut menu associated with a control or component.
/// </summary>
public class ContextMenuStrip : Component
{
    /// <summary>
    /// Initializes an empty context menu.
    /// </summary>
    public ContextMenuStrip()
    {
    }

    /// <summary>
    /// Initializes a context menu and adds it to the specified component container.
    /// </summary>
    /// <param name="container">The component container.</param>
    public ContextMenuStrip(IContainer container)
    {
        container?.Add(this);
    }

    /// <summary>
    /// Gets the items contained in the context menu.
    /// </summary>
    public ToolStripItemCollection Items { get; } = new();

    /// <summary>
    /// Gets or sets the component name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the design-time size of the menu.
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// Suspends layout logic for compatibility with designer-generated code.
    /// </summary>
    public void SuspendLayout()
    {
    }

    /// <summary>
    /// Resumes layout logic for compatibility with designer-generated code.
    /// </summary>
    /// <param name="performLayout">Whether layout should be performed immediately.</param>
    public void ResumeLayout(bool performLayout)
    {
        if (performLayout)
        {
            PerformLayout();
        }
    }

    /// <summary>
    /// Performs layout for compatibility with designer-generated code.
    /// </summary>
    public void PerformLayout()
    {
    }

    /// <summary>
    /// Shows the context menu relative to a control.
    /// </summary>
    /// <param name="control">The control that owns the menu.</param>
    /// <param name="position">The client-relative position.</param>
    public void Show(Control control, Point position)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (control.Handle != 0 && Win32.GetWindowRect(control.Handle, out var rect))
        {
            ShowAtScreenPoint(control.Owner?.Handle ?? control.Handle, new Point(rect.Left + position.X, rect.Top + position.Y));
            return;
        }

        ShowAtScreenPoint(control.Owner?.Handle ?? 0, new Point(control.Left + position.X, control.Top + position.Y));
    }

    internal void ShowAtScreenPoint(nint ownerHandle, Point screenLocation)
    {
        ToolStripPopupMenu.Show(Items, ownerHandle, screenLocation);
    }
}