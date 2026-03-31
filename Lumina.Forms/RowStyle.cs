namespace Lumina.Forms;

/// <summary>
/// Represents layout metadata for a table layout row.
/// </summary>
public class RowStyle
{
    /// <summary>
    /// Initializes an empty row style.
    /// </summary>
    public RowStyle()
    {
    }

    /// <summary>
    /// Initializes a row style with the specified sizing behavior.
    /// </summary>
    /// <param name="sizeType">The sizing mode.</param>
    /// <param name="height">The height value.</param>
    public RowStyle(SizeType sizeType, float height)
    {
        SizeType = sizeType;
        Height = height;
    }

    /// <summary>
    /// Gets or sets the sizing mode.
    /// </summary>
    public SizeType SizeType { get; set; }

    /// <summary>
    /// Gets or sets the height value.
    /// </summary>
    public float Height { get; set; }
}