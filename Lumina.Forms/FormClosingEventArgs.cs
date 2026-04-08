namespace Lumina.Forms;

/// <summary>
/// Provides data for the form closing event.
/// </summary>
public sealed class FormClosingEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether to cancel the close operation.
    /// </summary>
    public bool Cancel { get; set; }
}
