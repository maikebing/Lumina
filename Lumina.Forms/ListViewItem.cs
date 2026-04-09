using System.Collections;
using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents an item displayed by a <see cref="ListView"/>.
/// </summary>
public class ListViewItem
{
    /// <summary>
    /// Initializes a new list view item with the specified main text.
    /// </summary>
    /// <param name="text">The primary item text.</param>
    public ListViewItem(string text)
    {
        Text = text ?? string.Empty;
        SubItems = new ListViewSubItemCollection(this);
        SubItems.Add(Text);
    }

    /// <summary>
    /// Gets or sets the primary text of the item.
    /// </summary>
    public string Text
    {
        get => SubItems.Count == 0 ? string.Empty : SubItems[0].Text;
        set
        {
            string normalized = value ?? string.Empty;
            if (SubItems.Count == 0)
            {
                SubItems.Add(normalized);
            }
            else
            {
                SubItems[0].Text = normalized;
            }
        }
    }

    /// <summary>
    /// Gets the subitems belonging to the item.
    /// </summary>
    public ListViewSubItemCollection SubItems { get; }

    /// <summary>
    /// Gets or sets the item foreground color.
    /// </summary>
    public Color ForeColor { get; set; } = Color.Empty;

    /// <summary>
    /// Represents a single list view subitem.
    /// </summary>
    public sealed class ListViewSubItem
    {
        internal ListViewSubItem(string text)
        {
            Text = text ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the displayed subitem text.
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// Represents the subitem collection for a <see cref="ListViewItem"/>.
    /// </summary>
    public sealed class ListViewSubItemCollection : IList<ListViewSubItem>
    {
        private readonly List<ListViewSubItem> _items = [];
        private readonly ListViewItem _owner;

        internal ListViewSubItemCollection(ListViewItem owner)
        {
            _owner = owner;
        }

        public ListViewSubItem this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public void Add(ListViewSubItem item)
        {
            _items.Add(item);
        }

        public ListViewSubItem Add(string text)
        {
            var item = new ListViewSubItem(text);
            _items.Add(item);
            return item;
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(ListViewSubItem item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(ListViewSubItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ListViewSubItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(ListViewSubItem item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, ListViewSubItem item)
        {
            _items.Insert(index, item);
        }

        public bool Remove(ListViewSubItem item)
        {
            return _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
