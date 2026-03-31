namespace Lumina.Forms;

/// <summary>
/// Specifies how a picture box positions and scales its image.
/// </summary>
public enum PictureBoxSizeMode
{
    /// <summary>
    /// The image is placed in the upper-left corner at its native size.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// The image is stretched to fill the control bounds.
    /// </summary>
    StretchImage = 1,

    /// <summary>
    /// The control resizes itself to the image size.
    /// </summary>
    AutoSize = 2,

    /// <summary>
    /// The image is centered without scaling.
    /// </summary>
    CenterImage = 3,

    /// <summary>
    /// The image is scaled proportionally to fit inside the control bounds.
    /// </summary>
    Zoom = 4,
}