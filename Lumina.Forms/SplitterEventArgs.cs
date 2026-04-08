namespace Lumina.Forms;

/// <summary>
/// Provides data for splitter move notifications.
/// </summary>
public sealed class SplitterEventArgs : EventArgs
{
    /// <summary>
    /// Initializes splitter move event data.
    /// </summary>
    public SplitterEventArgs(int x, int y, int splitX, int splitY)
    {
        X = x;
        Y = y;
        SplitX = splitX;
        SplitY = splitY;
    }

    /// <summary>
    /// Gets the current pointer X coordinate.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Gets the current pointer Y coordinate.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Gets the proposed splitter X coordinate.
    /// </summary>
    public int SplitX { get; }

    /// <summary>
    /// Gets the proposed splitter Y coordinate.
    /// </summary>
    public int SplitY { get; }
}

/// <summary>
/// Represents the method that handles splitter notifications.
/// </summary>
/// <param name="sender">The splitter source.</param>
/// <param name="e">The event data.</param>
public delegate void SplitterEventHandler(object? sender, SplitterEventArgs e);
