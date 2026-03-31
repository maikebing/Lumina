namespace Lumina.Forms;

/// <summary>
/// Specifies how a table layout row or column is sized.
/// </summary>
public enum SizeType
{
    /// <summary>
    /// The size is fixed.
    /// </summary>
    Absolute = 0,

    /// <summary>
    /// The size is determined automatically.
    /// </summary>
    AutoSize = 1,

    /// <summary>
    /// The size is proportional.
    /// </summary>
    Percent = 2,
}