namespace Lumina.Forms;

/// <summary>
/// Specifies which mouse button was pressed.
/// </summary>
[Flags]
public enum MouseButtons
{
    /// <summary>
    /// No mouse button was pressed.
    /// </summary>
    None = 0,

    /// <summary>
    /// The left mouse button was pressed.
    /// </summary>
    Left = 0x00100000,

    /// <summary>
    /// The right mouse button was pressed.
    /// </summary>
    Right = 0x00200000,

    /// <summary>
    /// The middle mouse button was pressed.
    /// </summary>
    Middle = 0x00400000,
}