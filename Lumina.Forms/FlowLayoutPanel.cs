using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a lightweight flow layout container.
/// </summary>
public class FlowLayoutPanel : Panel
{
    private FlowDirection _flowDirection = FlowDirection.LeftToRight;

    /// <summary>
    /// Gets or sets the direction used to place child controls.
    /// </summary>
    public FlowDirection FlowDirection
    {
        get => _flowDirection;
        set
        {
            if (_flowDirection == value)
            {
                return;
            }

            _flowDirection = value;
            PerformLayout();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether controls wrap when they reach the container edge.
    /// </summary>
    public bool WrapContents { get; set; } = true;

    /// <inheritdoc />
    public override void PerformLayout()
    {
        Rectangle displayBounds = new(
            Padding.Left,
            Padding.Top,
            Math.Max(1, Width - Padding.Horizontal),
            Math.Max(1, Height - Padding.Vertical));

        switch (FlowDirection)
        {
            case FlowDirection.RightToLeft:
                LayoutHorizontal(displayBounds, leftToRight: false);
                break;

            case FlowDirection.TopDown:
                LayoutVertical(displayBounds, topDown: true);
                break;

            case FlowDirection.BottomUp:
                LayoutVertical(displayBounds, topDown: false);
                break;

            default:
                LayoutHorizontal(displayBounds, leftToRight: true);
                break;
        }

        base.PerformLayout();
    }

    private void LayoutHorizontal(Rectangle displayBounds, bool leftToRight)
    {
        int x = leftToRight ? displayBounds.Left : displayBounds.Right;
        int y = displayBounds.Top;
        int lineHeight = 0;
        bool firstInLine = true;

        foreach (Control child in ChildControls)
        {
            if (!child.Visible)
            {
                continue;
            }

            int childWidth = Math.Max(1, child.Width);
            int childHeight = Math.Max(1, child.Height);
            int outerWidth = childWidth + child.Margin.Horizontal;
            int outerHeight = childHeight + child.Margin.Vertical;

            bool shouldWrap = WrapContents && !firstInLine && leftToRight
                ? x + outerWidth > displayBounds.Right
                : WrapContents && !firstInLine && x - outerWidth < displayBounds.Left;

            if (shouldWrap)
            {
                x = leftToRight ? displayBounds.Left : displayBounds.Right;
                y += lineHeight;
                lineHeight = 0;
                firstInLine = true;
            }

            int childLeft = leftToRight
                ? x + child.Margin.Left
                : x - outerWidth + child.Margin.Left;
            int childTop = y + child.Margin.Top;

            child.SetBounds(childLeft, childTop, childWidth, childHeight);

            x = leftToRight ? x + outerWidth : x - outerWidth;
            lineHeight = Math.Max(lineHeight, outerHeight);
            firstInLine = false;
        }
    }

    private void LayoutVertical(Rectangle displayBounds, bool topDown)
    {
        int x = displayBounds.Left;
        int y = topDown ? displayBounds.Top : displayBounds.Bottom;
        int lineWidth = 0;
        bool firstInLine = true;

        foreach (Control child in ChildControls)
        {
            if (!child.Visible)
            {
                continue;
            }

            int childWidth = Math.Max(1, child.Width);
            int childHeight = Math.Max(1, child.Height);
            int outerWidth = childWidth + child.Margin.Horizontal;
            int outerHeight = childHeight + child.Margin.Vertical;

            bool shouldWrap = WrapContents && !firstInLine && topDown
                ? y + outerHeight > displayBounds.Bottom
                : WrapContents && !firstInLine && y - outerHeight < displayBounds.Top;

            if (shouldWrap)
            {
                y = topDown ? displayBounds.Top : displayBounds.Bottom;
                x += lineWidth;
                lineWidth = 0;
                firstInLine = true;
            }

            int childLeft = x + child.Margin.Left;
            int childTop = topDown
                ? y + child.Margin.Top
                : y - outerHeight + child.Margin.Top;

            child.SetBounds(childLeft, childTop, childWidth, childHeight);

            y = topDown ? y + outerHeight : y - outerHeight;
            lineWidth = Math.Max(lineWidth, outerWidth);
            firstInLine = false;
        }
    }
}
