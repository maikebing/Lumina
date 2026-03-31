namespace Lumina.Forms;

/// <summary>
/// Represents the interior spacing between a control boundary and its contents.
/// </summary>
public readonly struct Padding : IEquatable<Padding>
{
    /// <summary>
    /// Gets an empty padding value.
    /// </summary>
    public static Padding Empty => default;

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

    /// <summary>
    /// Gets the combined horizontal padding.
    /// </summary>
    public int Horizontal => Left + Right;

    /// <summary>
    /// Gets the combined vertical padding.
    /// </summary>
    public int Vertical => Top + Bottom;

    /// <inheritdoc />
    public bool Equals(Padding other)
        => Left == other.Left
            && Top == other.Top
            && Right == other.Right
            && Bottom == other.Bottom;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is Padding other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Left, Top, Right, Bottom);

    /// <summary>
    /// Determines whether two padding values are equal.
    /// </summary>
    public static bool operator ==(Padding left, Padding right) => left.Equals(right);

    /// <summary>
    /// Determines whether two padding values are different.
    /// </summary>
    public static bool operator !=(Padding left, Padding right) => !left.Equals(right);
}
