using System.Drawing;

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
        _cellPositions[control] = (Math.Max(0, column), Math.Max(0, row));
        AddChild(control);
    }

    /// <inheritdoc />
    internal override void OnChildRemoved(Control control)
    {
        _ = _cellPositions.Remove(control);
        base.OnChildRemoved(control);
    }

    /// <inheritdoc />
    public override void PerformLayout()
    {
        int columns = ResolveColumnCount();
        int rows = ResolveInitialRowCount(columns);
        List<CellAssignment> assignments = BuildAssignments(columns, ref rows);

        Rectangle displayBounds = new(
            Padding.Left,
            Padding.Top,
            Math.Max(1, Width - Padding.Horizontal),
            Math.Max(1, Height - Padding.Vertical));

        int[] columnWidths = CalculateColumnWidths(columns, assignments, displayBounds.Width);
        int[] rowHeights = CalculateRowHeights(rows, assignments, displayBounds.Height);
        int[] columnOffsets = BuildOffsets(columnWidths, displayBounds.Left);
        int[] rowOffsets = BuildOffsets(rowHeights, displayBounds.Top);

        foreach (CellAssignment assignment in assignments)
        {
            Rectangle cellBounds = new(
                columnOffsets[assignment.Column],
                rowOffsets[assignment.Row],
                Math.Max(1, columnWidths[assignment.Column]),
                Math.Max(1, rowHeights[assignment.Row]));

            LayoutControlInCell(assignment.Control, cellBounds);
        }

        base.PerformLayout();
    }

    private int ResolveColumnCount()
    {
        int maxAssignedColumn = -1;
        foreach ((_, (int column, _)) in _cellPositions)
        {
            maxAssignedColumn = Math.Max(maxAssignedColumn, column);
        }

        return Math.Max(1, Math.Max(ColumnCount, maxAssignedColumn + 1));
    }

    private int ResolveInitialRowCount(int columns)
    {
        int maxAssignedRow = -1;
        foreach ((_, (_, int row)) in _cellPositions)
        {
            maxAssignedRow = Math.Max(maxAssignedRow, row);
        }

        int childCount = ChildControls.Length;
        int sequentialRows = (int)Math.Ceiling(Math.Max(1, childCount) / (double)Math.Max(1, columns));
        return Math.Max(1, Math.Max(RowCount, Math.Max(maxAssignedRow + 1, sequentialRows)));
    }

    private List<CellAssignment> BuildAssignments(int columns, ref int rows)
    {
        var assignments = new List<CellAssignment>(ChildControls.Length);
        var autoControls = new List<Control>();
        bool[,] occupied = new bool[rows, columns];

        foreach (Control control in ChildControls)
        {
            if (_cellPositions.TryGetValue(control, out (int Column, int Row) position))
            {
                while (position.Row >= rows)
                {
                    occupied = GrowRows(occupied, ++rows, columns);
                }

                if (!occupied[position.Row, position.Column])
                {
                    occupied[position.Row, position.Column] = true;
                    assignments.Add(new CellAssignment(control, position.Column, position.Row));
                    continue;
                }
            }

            autoControls.Add(control);
        }

        int autoRow = 0;
        int autoColumn = 0;
        foreach (Control control in autoControls)
        {
            FindNextAvailableCell(ref occupied, ref rows, columns, ref autoColumn, ref autoRow);
            occupied[autoRow, autoColumn] = true;
            assignments.Add(new CellAssignment(control, autoColumn, autoRow));
            AdvanceAutoCell(ref autoColumn, ref autoRow, columns);
        }

        return assignments;
    }

    private int[] CalculateColumnWidths(int columns, List<CellAssignment> assignments, int availableWidth)
    {
        int[] widths = new int[columns];
        int fixedWidth = 0;
        float totalPercent = 0;
        List<int> percentColumns = [];
        List<int> fillColumns = [];

        for (int index = 0; index < columns; index++)
        {
            ColumnStyle? style = index < ColumnStyles.Count ? ColumnStyles[index] : null;
            if (style is null)
            {
                fillColumns.Add(index);
                continue;
            }

            switch (style.SizeType)
            {
                case SizeType.Absolute:
                    widths[index] = Math.Max(1, (int)Math.Round(style.Width, MidpointRounding.AwayFromZero));
                    fixedWidth += widths[index];
                    break;

                case SizeType.AutoSize:
                    widths[index] = GetAutoColumnWidth(index, assignments);
                    fixedWidth += widths[index];
                    break;

                case SizeType.Percent when style.Width > 0:
                    percentColumns.Add(index);
                    totalPercent += style.Width;
                    break;

                default:
                    fillColumns.Add(index);
                    break;
            }
        }

        int remainingWidth = Math.Max(0, availableWidth - fixedWidth);
        if (percentColumns.Count > 0 && totalPercent > 0)
        {
            foreach (int column in percentColumns)
            {
                float weight = ColumnStyles[column].Width / totalPercent;
                widths[column] = Math.Max(1, (int)Math.Round(remainingWidth * weight, MidpointRounding.AwayFromZero));
            }
        }

        int assignedWidth = widths.Sum();
        int fillWidth = Math.Max(0, availableWidth - assignedWidth);
        if (fillColumns.Count > 0)
        {
            int remainingFillColumns = fillColumns.Count;
            foreach (int column in fillColumns)
            {
                int width = Math.Max(1, fillWidth / Math.Max(1, remainingFillColumns));
                widths[column] = width;
                fillWidth -= width;
                remainingFillColumns--;
            }
        }

        NormalizeSizes(widths, availableWidth);
        return widths;
    }

    private int[] CalculateRowHeights(int rows, List<CellAssignment> assignments, int availableHeight)
    {
        int[] heights = new int[rows];
        int fixedHeight = 0;
        float totalPercent = 0;
        List<int> percentRows = [];
        List<int> fillRows = [];

        for (int index = 0; index < rows; index++)
        {
            RowStyle? style = index < RowStyles.Count ? RowStyles[index] : null;
            if (style is null)
            {
                fillRows.Add(index);
                continue;
            }

            switch (style.SizeType)
            {
                case SizeType.Absolute:
                    heights[index] = Math.Max(1, (int)Math.Round(style.Height, MidpointRounding.AwayFromZero));
                    fixedHeight += heights[index];
                    break;

                case SizeType.AutoSize:
                    heights[index] = GetAutoRowHeight(index, assignments);
                    fixedHeight += heights[index];
                    break;

                case SizeType.Percent when style.Height > 0:
                    percentRows.Add(index);
                    totalPercent += style.Height;
                    break;

                default:
                    fillRows.Add(index);
                    break;
            }
        }

        int remainingHeight = Math.Max(0, availableHeight - fixedHeight);
        if (percentRows.Count > 0 && totalPercent > 0)
        {
            foreach (int row in percentRows)
            {
                float weight = RowStyles[row].Height / totalPercent;
                heights[row] = Math.Max(1, (int)Math.Round(remainingHeight * weight, MidpointRounding.AwayFromZero));
            }
        }

        int assignedHeight = heights.Sum();
        int fillHeight = Math.Max(0, availableHeight - assignedHeight);
        if (fillRows.Count > 0)
        {
            int remainingFillRows = fillRows.Count;
            foreach (int row in fillRows)
            {
                int height = Math.Max(1, fillHeight / Math.Max(1, remainingFillRows));
                heights[row] = height;
                fillHeight -= height;
                remainingFillRows--;
            }
        }

        NormalizeSizes(heights, availableHeight);
        return heights;
    }

    private int GetAutoColumnWidth(int column, List<CellAssignment> assignments)
    {
        int maxWidth = 1;
        foreach (CellAssignment assignment in assignments)
        {
            if (assignment.Column == column)
            {
                maxWidth = Math.Max(maxWidth, Math.Max(1, assignment.Control.Width) + assignment.Control.Margin.Horizontal);
            }
        }

        return maxWidth;
    }

    private int GetAutoRowHeight(int row, List<CellAssignment> assignments)
    {
        int maxHeight = 1;
        foreach (CellAssignment assignment in assignments)
        {
            if (assignment.Row == row)
            {
                maxHeight = Math.Max(maxHeight, Math.Max(1, assignment.Control.Height) + assignment.Control.Margin.Vertical);
            }
        }

        return maxHeight;
    }

    private static int[] BuildOffsets(int[] sizes, int start)
    {
        int[] offsets = new int[sizes.Length];
        int current = start;
        for (int index = 0; index < sizes.Length; index++)
        {
            offsets[index] = current;
            current += sizes[index];
        }

        return offsets;
    }

    private static void LayoutControlInCell(Control control, Rectangle cellBounds)
    {
        Rectangle contentBounds = new(
            cellBounds.Left + control.Margin.Left,
            cellBounds.Top + control.Margin.Top,
            Math.Max(1, cellBounds.Width - control.Margin.Horizontal),
            Math.Max(1, cellBounds.Height - control.Margin.Vertical));

        int width = Math.Max(1, Math.Min(Math.Max(1, control.Width), contentBounds.Width));
        int height = Math.Max(1, Math.Min(Math.Max(1, control.Height), contentBounds.Height));
        control.SetBounds(contentBounds.Left, contentBounds.Top, width, height);
    }

    private static bool[,] GrowRows(bool[,] occupied, int newRowCount, int columns)
    {
        bool[,] expanded = new bool[newRowCount, columns];
        for (int row = 0; row < occupied.GetLength(0); row++)
        {
            for (int column = 0; column < occupied.GetLength(1); column++)
            {
                expanded[row, column] = occupied[row, column];
            }
        }

        return expanded;
    }

    private static void FindNextAvailableCell(ref bool[,] occupied, ref int rows, int columns, ref int column, ref int row)
    {
        while (true)
        {
            while (row >= rows)
            {
                occupied = GrowRows(occupied, ++rows, columns);
            }

            if (!occupied[row, column])
            {
                return;
            }

            AdvanceAutoCell(ref column, ref row, columns);
        }
    }

    private static void AdvanceAutoCell(ref int column, ref int row, int columns)
    {
        column++;
        if (column >= columns)
        {
            column = 0;
            row++;
        }
    }

    private static void NormalizeSizes(int[] sizes, int target)
    {
        if (sizes.Length == 0)
        {
            return;
        }

        int total = sizes.Sum();
        if (total < target)
        {
            sizes[^1] += target - total;
            return;
        }

        int overflow = total - target;
        for (int index = sizes.Length - 1; index >= 0 && overflow > 0; index--)
        {
            int reducible = Math.Max(0, sizes[index] - 1);
            int reduction = Math.Min(reducible, overflow);
            sizes[index] -= reduction;
            overflow -= reduction;
        }
    }

    private readonly record struct CellAssignment(Control Control, int Column, int Row);
}
