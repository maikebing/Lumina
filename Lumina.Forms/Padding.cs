namespace Lumina.Forms;

/// <summary>
/// Represents the interior spacing between a control boundary and its contents.
/// </summary>
public readonly struct Padding
{
    /// <summary>
    /// Initializes uniform padding on all sides.
    /// </summary>
    /// <param name="all">The value to apply to all sides.</param>
    public Padding(int all)
        : this(all, all, all, all)
    {
    }

    /// <summary>
    /// Initializes padding with individual side values.
    /// </summary>
    /// <param name="left">The left padding.</param>
    /// <param name="top">The top padding.</param>
    /// <param name="right">The right padding.</param>
    /// <param name="bottom">The bottom padding.</param>
    public Padding(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Gets the left padding.
    /// </summary>
    public int Left { get; }

    /// <summary>
    /// Gets the top padding.
    /// </summary>
    public int Top { get; }

    /// <summary>
    /// Gets the right padding.
    /// </summary>
    public int Right { get; }

    /// <summary>
    /// Gets the bottom padding.
    /// </summary>
    public int Bottom { get; }
}