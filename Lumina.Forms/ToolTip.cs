using System.ComponentModel;

namespace Lumina.Forms;

/// <summary>
/// Represents a tooltip component.
/// </summary>
public class ToolTip : Component
{
    private readonly Dictionary<Control, string> _toolTips = [];

    /// <summary>
    /// Initializes a new tooltip component.
    /// </summary>
    public ToolTip()
    {
    }

    /// <summary>
    /// Initializes a new tooltip component and adds it to the specified container.
    /// </summary>
    /// <param name="container">The container that owns the component.</param>
    public ToolTip(IContainer container)
    {
        container?.Add(this);
    }

    /// <summary>
    /// Associates tooltip text with the specified control.
    /// </summary>
    /// <param name="control">The target control.</param>
    /// <param name="caption">The tooltip text.</param>
    public void SetToolTip(Control control, string? caption)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (string.IsNullOrEmpty(caption))
        {
            _ = _toolTips.Remove(control);
            return;
        }

        _toolTips[control] = caption;
    }

    /// <summary>
    /// Gets the tooltip text associated with the specified control, if any.
    /// </summary>
    /// <param name="control">The target control.</param>
    /// <returns>The tooltip text, or an empty string when none is registered.</returns>
    public string GetToolTip(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        return _toolTips.TryGetValue(control, out string? caption)
            ? caption
            : string.Empty;
    }
}