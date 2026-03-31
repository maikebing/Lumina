namespace Lumina.Forms;

/// <summary>
/// Represents a lightweight table layout container.
/// </summary>
public class TableLayoutPanel : Panel
{
    private readonly Dictionary<Control, (int Column, int Row)> _cellPositions = [];

    /// <summary>
    /// Gets or sets the number of logical columns.
    /// </summary>
    public int ColumnCount { get; set; }

    /// <summary>
    /// Gets or sets the number of logical rows.
    /// </summary>
    public int RowCount { get; set; }

    /// <summary>
    /// Gets the logical column styles.
    /// </summary>
    public List<ColumnStyle> ColumnStyles { get; } = [];

    /// <summary>
    /// Gets the logical row styles.
    /// </summary>
    public List<RowStyle> RowStyles { get; } = [];

    internal void AddCell(Control control, int column, int row)
    {
        _cellPositions[control] = (column, row);
        AddChild(control);
    }

    /// <inheritdoc />
    public override void PerformLayout()
    {
        int columns = Math.Max(1, ColumnCount);
        int rows = Math.Max(1, RowCount);
        int[] columnWidths = CalculateColumnWidths(columns);
        int[] rowHeights = CalculateRowHeights(rows);

        foreach ((Control control, (int column, int row)) in _cellPositions)
        {
            if (column < 0 || column >= columns || row < 0 || row >= rows)
            {
                continue;
            }

            int x = 0;
            for (int index = 0; index < column; index++)
            {
                x += columnWidths[index];
            }

            int y = 0;
            for (int index = 0; index < row; index++)
            {
                y += rowHeights[index];
            }

            control.SetBounds(x, y, Math.Max(1, columnWidths[column]), Math.Max(1, rowHeights[row]));
        }

        base.PerformLayout();
    }

    private int[] CalculateColumnWidths(int columns)
    {
        int[] widths = new int[columns];
        int remainingWidth = Width;
        float totalPercent = 0;

        for (int index = 0; index < columns; index++)
        {
            ColumnStyle? style = index < ColumnStyles.Count ? ColumnStyles[index] : null;
            if (style is null)
            {
                continue;
            }

            switch (style.SizeType)
            {
                case SizeType.Absolute:
                    widths[index] = (int)Math.Round(style.Width, MidpointRounding.AwayFromZero);
                    remainingWidth -= widths[index];
                    break;
                case SizeType.AutoSize:
                    widths[index] = GetAutoColumnWidth(index);
                    remainingWidth -= widths[index];
                    break;
                case SizeType.Percent:
                    totalPercent += style.Width;
                    break;
            }
        }

        remainingWidth = Math.Max(0, remainingWidth);
        for (int index = 0; index < columns; index++)
        {
            if (widths[index] > 0)
            {
                continue;
            }

            ColumnStyle? style = index < ColumnStyles.Count ? ColumnStyles[index] : null;
            if (style?.SizeType == SizeType.Percent && totalPercent > 0)
            {
                widths[index] = Math.Max(1, (int)Math.Round(remainingWidth * (style.Width / totalPercent), MidpointRounding.AwayFromZero));
            }
            else
            {
                widths[index] = Math.Max(1, remainingWidth / Math.Max(1, columns - index));
            }
        }

        NormalizeSizes(widths, Width);
        return widths;
    }

    private int[] CalculateRowHeights(int rows)
    {
        int[] heights = new int[rows];
        int remainingHeight = Height;
        float totalPercent = 0;

        for (int index = 0; index < rows; index++)
        {
            RowStyle? style = index < RowStyles.Count ? RowStyles[index] : null;
            if (style is null)
            {
                continue;
            }

            switch (style.SizeType)
            {
                case SizeType.Absolute:
                    heights[index] = (int)Math.Round(style.Height, MidpointRounding.AwayFromZero);
                    remainingHeight -= heights[index];
                    break;
                case SizeType.AutoSize:
                    heights[index] = GetAutoRowHeight(index);
                    remainingHeight -= heights[index];
                    break;
                case SizeType.Percent:
                    totalPercent += style.Height;
                    break;
            }
        }

        remainingHeight = Math.Max(0, remainingHeight);
        for (int index = 0; index < rows; index++)
        {
            if (heights[index] > 0)
            {
                continue;
            }

            RowStyle? style = index < RowStyles.Count ? RowStyles[index] : null;
            if (style?.SizeType == SizeType.Percent && totalPercent > 0)
            {
                heights[index] = Math.Max(1, (int)Math.Round(remainingHeight * (style.Height / totalPercent), MidpointRounding.AwayFromZero));
            }
            else
            {
                heights[index] = Math.Max(1, remainingHeight / Math.Max(1, rows - index));
            }
        }

        NormalizeSizes(heights, Height);
        return heights;
    }

    private int GetAutoColumnWidth(int column)
    {
        int maxWidth = 1;
        foreach ((Control control, (int itemColumn, _)) in _cellPositions)
        {
            if (itemColumn == column)
            {
                maxWidth = Math.Max(maxWidth, control.Width);
            }
        }

        return maxWidth;
    }

    private int GetAutoRowHeight(int row)
    {
        int maxHeight = 1;
        foreach ((Control control, (_, int itemRow)) in _cellPositions)
        {
            if (itemRow == row)
            {
                maxHeight = Math.Max(maxHeight, control.Height);
            }
        }

        return maxHeight;
    }

    private static void NormalizeSizes(int[] sizes, int target)
    {
        int total = sizes.Sum();
        if (total == target)
        {
            return;
        }

        int delta = target - total;
        if (sizes.Length == 0)
        {
            return;
        }

        sizes[^1] = Math.Max(1, sizes[^1] + delta);
    }
}