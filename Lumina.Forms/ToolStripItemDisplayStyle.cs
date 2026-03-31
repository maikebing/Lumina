namespace Lumina.Forms;

/// <summary>
/// Specifies how a tool strip item shows its image and text.
/// </summary>
public enum ToolStripItemDisplayStyle
{
    /// <summary>
    /// The item does not show image or text.
    /// </summary>
    None = 0,

    /// <summary>
    /// The item shows text only.
    /// </summary>
    Text = 1,

    /// <summary>
    /// The item shows an image only.
    /// </summary>
    Image = 2,

    /// <summary>
    /// The item shows both image and text.
    /// </summary>
    ImageAndText = 3,
}