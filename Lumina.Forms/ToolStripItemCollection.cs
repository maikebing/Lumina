namespace Lumina.Forms;

/// <summary>
/// Represents a collection of tool strip items.
/// </summary>
public sealed class ToolStripItemCollection : IEnumerable<ToolStripItem>
{
    private readonly List<ToolStripItem> _items = [];
    private readonly Action? _itemsChanged;

    internal ToolStripItemCollection(Action? itemsChanged = null)
    {
        _itemsChanged = itemsChanged;
    }

    /// <summary>
    /// Gets the number of items in the collection.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Gets the item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    public ToolStripItem this[int index] => _items[index];

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(ToolStripItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
        item.Changed += OnItemChanged;
        _itemsChanged?.Invoke();
    }

    /// <summary>
    /// Adds a batch of items to the collection.
    /// </summary>
    /// <param name="items">The items to add.</param>
    public void AddRange(IEnumerable<ToolStripItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        foreach (ToolStripItem item in items)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Adds a batch of items to the collection.
    /// </summary>
    /// <param name="items">The items to add.</param>
    public void AddRange(params ToolStripItem[] items)
    {
        AddRange((IEnumerable<ToolStripItem>)items);
    }

    /// <inheritdoc />
    public IEnumerator<ToolStripItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void OnItemChanged(object? sender, EventArgs e)
    {
        _itemsChanged?.Invoke();
    }
}
