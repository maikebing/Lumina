namespace Lumina.Forms;

/// <summary>
/// Represents layout metadata for a table layout column.
/// </summary>
public class ColumnStyle
{
    /// <summary>
    /// Initializes an empty column style.
    /// </summary>
    public ColumnStyle()
    {
    }

    /// <summary>
    /// Initializes a column style with the specified sizing behavior.
    /// </summary>
    /// <param name="sizeType">The sizing mode.</param>
    /// <param name="width">The width value.</param>
    public ColumnStyle(SizeType sizeType, float width)
    {
        SizeType = sizeType;
        Width = width;
    }

    /// <summary>
    /// Gets or sets the sizing mode.
    /// </summary>
    public SizeType SizeType { get; set; }

    /// <summary>
    /// Gets or sets the width value.
    /// </summary>
    public float Width { get; set; }
}