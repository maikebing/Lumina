namespace Lumina.Forms;

/// <summary>
/// Specifies which panel of a split container keeps its size when the container is resized.
/// </summary>
public enum FixedPanel
{
    /// <summary>
    /// Neither panel keeps a fixed size.
    /// </summary>
    None = 0,

    /// <summary>
    /// The first panel keeps its size.
    /// </summary>
    Panel1 = 1,

    /// <summary>
    /// The second panel keeps its size.
    /// </summary>
    Panel2 = 2,
}
