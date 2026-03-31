namespace Lumina.Forms;

/// <summary>
/// Specifies how a form scales itself and its child controls for the current environment.
/// </summary>
public enum AutoScaleMode
{
    /// <summary>
    /// Disables automatic scaling.
    /// </summary>
    None = 0,

    /// <summary>
    /// Scales using the current UI font metrics.
    /// </summary>
    Font = 1,

    /// <summary>
    /// Scales using the current system DPI.
    /// </summary>
    Dpi = 2,

    /// <summary>
    /// Inherits the scaling mode from the parent container. Top-level LuminaForms windows treat this as <see cref="Font"/>.
    /// </summary>
    Inherit = 3,
}
