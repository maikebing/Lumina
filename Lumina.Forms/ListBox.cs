using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents a simple single-selection list box control.
/// </summary>
public class ListBox : Control
{
    private readonly List<object?> _items = [];
    private readonly ObjectCollection _itemsCollection;
    private int _selectedIndex = -1;

    /// <summary>
    /// Initializes an empty list box.
    /// </summary>
    public ListBox()
    {
        _itemsCollection = new ObjectCollection(this);
    }

    /// <summary>
    /// Occurs when <see cref="SelectedIndex"/> changes.
    /// </summary>
    public event EventHandler? SelectedIndexChanged;

    /// <summary>
    /// Gets the collection used to add and clear list items.
    /// </summary>
    public ObjectCollection Items => _itemsCollection;

    /// <summary>
    /// Gets or sets a value indicating whether formatting is enabled.
    /// </summary>
    public bool FormattingEnabled { get; set; }

    /// <summary>
    /// Gets or sets the zero-based selected item index.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (Handle != 0)
            {
                _selectedIndex = (int)Win32.SendMessageW(Handle, Win32.LB_GETCURSEL, 0, 0);
            }

            return _selectedIndex;
        }
        set
        {
            ValidateSelectedIndex(value);

            if (_selectedIndex == value)
            {
                if (Handle != 0)
                {
                    _ = Win32.SendMessageW(Handle, Win32.LB_SETCURSEL, (nint)value, 0);
                }

                return;
            }

            _selectedIndex = value;
            if (Handle != 0)
            {
                _ = Win32.SendMessageW(Handle, Win32.LB_SETCURSEL, (nint)value, 0);
            }

            OnSelectedIndexChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the currently selected item.
    /// </summary>
    public object? SelectedItem
    {
        get
        {
            int selectedIndex = SelectedIndex;
            return selectedIndex >= 0 && selectedIndex < _items.Count
                ? _items[selectedIndex]
                : null;
        }
        set
        {
            if (value is null)
            {
                SelectedIndex = -1;
                return;
            }

            int index = _items.FindIndex(item => Equals(item, value));
            if (index < 0)
            {
                throw new ArgumentException("The specified item does not exist in the list box.", nameof(value));
            }

            SelectedIndex = index;
        }
    }

    /// <inheritdoc />
    protected override string ClassName => "LISTBOX";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_VSCROLL | Win32.LBS_NOTIFY;

    /// <inheritdoc />
    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    /// <summary>
    /// Adds an item to the list box.
    /// </summary>
    /// <param name="value">The item text to add.</param>
    public int AddItem(object? value)
    {
        _items.Add(value);
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.LB_ADDSTRING, 0, value?.ToString() ?? string.Empty);
        }

        return _items.Count - 1;
    }

    /// <summary>
    /// Removes all items from the list box.
    /// </summary>
    public void ClearItems()
    {
        bool selectionChanged = _selectedIndex != -1;
        _items.Clear();
        _selectedIndex = -1;
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.LB_RESETCONTENT, 0, 0);
        }

        if (selectionChanged)
        {
            OnSelectedIndexChanged(EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();

        foreach (object? item in CollectionsMarshal.AsSpan(_items))
        {
            _ = Win32.SendMessageW(Handle, Win32.LB_ADDSTRING, 0, item?.ToString() ?? string.Empty);
        }

        if (_selectedIndex >= 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.LB_SETCURSEL, (nint)_selectedIndex, 0);
        }
    }

    /// <inheritdoc />
    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.LBN_SELCHANGE)
        {
            return false;
        }

        int currentIndex = (int)Win32.SendMessageW(Handle, Win32.LB_GETCURSEL, 0, 0);
        if (_selectedIndex != currentIndex)
        {
            _selectedIndex = currentIndex;
            OnSelectedIndexChanged(EventArgs.Empty);
        }

        return true;
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyExplorerTheme();
    }

    /// <inheritdoc />
    protected override string GetPreferredThemeClass(ResolvedVisualStyle visualStyle)
        => visualStyle.IsDarkMode && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763)
            ? "DarkMode_CFD"
            : "CFD";

    /// <inheritdoc />
    protected override string GetFallbackThemeClass(ResolvedVisualStyle visualStyle)
        => base.GetPreferredThemeClass(visualStyle);

    private void ValidateSelectedIndex(int value)
    {
        if (value < -1 || value >= _items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    /// <summary>
    /// Raises the <see cref="SelectedIndexChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnSelectedIndexChanged(EventArgs e)
    {
        SelectedIndexChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Represents the item collection for a <see cref="ListBox"/>.
    /// </summary>
    public sealed class ObjectCollection : IEnumerable<object?>
    {
        private readonly ListBox _owner;

        internal ObjectCollection(ListBox owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Adds an item to the owning list box.
        /// </summary>
        /// <param name="value">The item text to add.</param>
        /// <returns>The zero-based index of the newly added item.</returns>
        public int Add(object? value)
        {
            return _owner.AddItem(value);
        }

        /// <summary>
        /// Adds a batch of items to the owning list box.
        /// </summary>
        /// <param name="values">The item texts to add.</param>
        public void AddRange(IEnumerable<object?> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            foreach (object? value in values)
            {
                _owner.AddItem(value);
            }
        }

        /// <summary>
        /// Adds a batch of items to the owning list box.
        /// </summary>
        /// <param name="values">The item texts to add.</param>
        public void AddRange(params object?[] values)
        {
            AddRange((IEnumerable<object?>)values);
        }

        /// <summary>
        /// Removes all items from the owning list box.
        /// </summary>
        public void Clear()
        {
            _owner.ClearItems();
        }

        /// <summary>
        /// Gets the number of items currently stored in the list box.
        /// </summary>
        public int Count => _owner._items.Count;

        /// <summary>
        /// Gets the item at the requested index.
        /// </summary>
        /// <param name="index">The zero-based item index.</param>
        /// <returns>The stored item.</returns>
        public object? this[int index] => _owner._items[index];

        /// <inheritdoc />
        public IEnumerator<object?> GetEnumerator()
        {
            return _owner._items.GetEnumerator();
        }

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
