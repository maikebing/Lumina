namespace Lumina.Forms;

/// <summary>
/// Specifies which border of a container a control is bound to.
/// </summary>
public enum DockStyle
{
    /// <summary>
    /// The control is not docked.
    /// </summary>
    None = 0,

    /// <summary>
    /// The control is docked to the top edge.
    /// </summary>
    Top = 1,

    /// <summary>
    /// The control is docked to the bottom edge.
    /// </summary>
    Bottom = 2,

    /// <summary>
    /// The control is docked to the left edge.
    /// </summary>
    Left = 3,

    /// <summary>
    /// The control is docked to the right edge.
    /// </summary>
    Right = 4,

    /// <summary>
    /// The control fills the remaining client area.
    /// </summary>
    Fill = 5,
}
