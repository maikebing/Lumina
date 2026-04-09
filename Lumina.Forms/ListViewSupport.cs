namespace Lumina.Forms;

public enum View
{
    LargeIcon = 0,
    Details = 1,
    SmallIcon = 2,
    List = 3,
    Tile = 4
}

public sealed class ColumnHeader
{
    public string Text { get; set; } = string.Empty;

    public int Width { get; set; }

    public HorizontalAlignment TextAlign { get; set; } = HorizontalAlignment.Left;
}

public sealed class ColumnHeaderCollection : IEnumerable<ColumnHeader>
{
    private readonly List<ColumnHeader> _headers = [];

    public int Count => _headers.Count;

    public ColumnHeader this[int index] => _headers[index];

    public void Clear()
    {
        _headers.Clear();
    }

    public ColumnHeader Add(string text, int width, HorizontalAlignment alignment)
    {
        var header = new ColumnHeader
        {
            Text = text ?? string.Empty,
            Width = width,
            TextAlign = alignment,
        };

        _headers.Add(header);
        return header;
    }

    public IEnumerator<ColumnHeader> GetEnumerator()
    {
        return _headers.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public sealed class ListViewItemCollection : IEnumerable<ListViewItem>
{
    private readonly List<ListViewItem> _items = [];

    public int Count => _items.Count;

    public ListViewItem this[int index] => _items[index];

    public void Add(ListViewItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public ListViewItem Add(string text)
    {
        var item = new ListViewItem(text);
        _items.Add(item);
        return item;
    }

    public void Clear()
    {
        _items.Clear();
    }

    public IEnumerator<ListViewItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
