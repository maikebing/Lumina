namespace Lumina.Forms;

/// <summary>
/// Provides mouse event data.
/// </summary>
public sealed class MouseEventArgs : EventArgs
{
    /// <summary>
    /// Initializes mouse event data.
    /// </summary>
    /// <param name="button">The pressed button.</param>
    /// <param name="clicks">The number of clicks.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="delta">The wheel delta.</param>
    public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
    {
        Button = button;
        Clicks = clicks;
        X = x;
        Y = y;
        Delta = delta;
    }

    /// <summary>
    /// Gets which mouse button changed state.
    /// </summary>
    public MouseButtons Button { get; }

    /// <summary>
    /// Gets the number of clicks.
    /// </summary>
    public int Clicks { get; }

    /// <summary>
    /// Gets the horizontal coordinate.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets the vertical coordinate.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Gets the mouse wheel delta.
    /// </summary>
    public int Delta { get; }
}