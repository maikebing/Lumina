namespace Lumina.Forms;

/// <summary>
/// Specifies how items on a tool strip are arranged.
/// </summary>
public enum ToolStripLayoutStyle
{
    /// <summary>
    /// Items are arranged in a split-stack layout with overflow support.
    /// </summary>
    StackWithOverflow = 0,

    /// <summary>
    /// Items are arranged horizontally with overflow support.
    /// </summary>
    HorizontalStackWithOverflow = 1,

    /// <summary>
    /// Items are arranged vertically with overflow support.
    /// </summary>
    VerticalStackWithOverflow = 2,

    /// <summary>
    /// Items are arranged in a table layout.
    /// </summary>
    Table = 3,
}
