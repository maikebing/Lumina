using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents a Win32 combo box with a WinForms-like API surface.
/// </summary>
public class ComboBox : Control
{
    private readonly List<object?> _items = [];
    private int _selectedIndex = -1;
    private readonly ItemCollection _itemsCollection;

    /// <summary>
    /// Initializes an empty combo box.
    /// </summary>
    public ComboBox()
    {
        _itemsCollection = new ItemCollection(this);
    }

    /// <summary>
    /// Occurs when <see cref="SelectedIndex"/> changes.
    /// </summary>
    public event EventHandler? SelectedIndexChanged;

    /// <summary>
    /// Gets or sets the native drop-down height used when the list is opened.
    /// </summary>
    public int DropDownHeight { get; set; } = 240;

    /// <summary>
    /// Gets or sets the behavior of the combo box edit/list presentation.
    /// </summary>
    public ComboBoxStyle DropDownStyle { get; set; } = ComboBoxStyle.DropDownList;

    /// <summary>
    /// Gets or sets a value indicating whether formatting is enabled.
    /// </summary>
    public bool FormattingEnabled { get; set; }

    /// <summary>
    /// Gets the item collection used to add or clear list entries.
    /// </summary>
    public ItemCollection Items => _itemsCollection;

    /// <summary>
    /// Gets or sets the zero-based selected item index.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (Handle != 0)
            {
                _selectedIndex = (int)Win32.SendMessageW(Handle, Win32.CB_GETCURSEL, 0, 0);
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
                    _ = Win32.SendMessageW(Handle, Win32.CB_SETCURSEL, (nint)value, 0);
                }

                return;
            }

            _selectedIndex = value;
            if (Handle != 0)
            {
                _ = Win32.SendMessageW(Handle, Win32.CB_SETCURSEL, (nint)value, 0);
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
                throw new ArgumentException("The specified item does not exist in the combo box.", nameof(value));
            }

            SelectedIndex = index;
        }
    }

    /// <inheritdoc />
    protected override string ClassName => "COMBOBOX";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_VSCROLL | GetDropDownStyle();

    /// <inheritdoc />
    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    /// <summary>
    /// Adds an item to the combo box.
    /// </summary>
    /// <param name="value">The item text to add.</param>
    public int AddItem(object? value)
    {
        _items.Add(value);
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_ADDSTRING, 0, value?.ToString() ?? string.Empty);
        }

        return _items.Count - 1;
    }

    /// <summary>
    /// Removes all items from the combo box.
    /// </summary>
    public void ClearItems()
    {
        bool selectionChanged = _selectedIndex != -1;
        _items.Clear();
        _selectedIndex = -1;
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_RESETCONTENT, 0, 0);
        }

        if (selectionChanged)
        {
            OnSelectedIndexChanged(EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(requestedHeight, DropDownHeight);

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();

        foreach (object? item in CollectionsMarshal.AsSpan(_items))
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_ADDSTRING, 0, item?.ToString() ?? string.Empty);
        }

        if (_selectedIndex >= 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_SETCURSEL, (nint)_selectedIndex, 0);
        }
    }

    /// <inheritdoc />
    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.CBN_SELCHANGE)
        {
            return false;
        }

        int currentIndex = (int)Win32.SendMessageW(Handle, Win32.CB_GETCURSEL, 0, 0);
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

    private uint GetDropDownStyle()
    {
        return DropDownStyle switch
        {
            ComboBoxStyle.Simple => Win32.CBS_SIMPLE,
            ComboBoxStyle.DropDown => Win32.CBS_DROPDOWN,
            _ => Win32.CBS_DROPDOWNLIST,
        };
    }

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
    /// Represents the item collection for a <see cref="ComboBox"/>.
    /// </summary>
    public sealed class ItemCollection : IEnumerable<object?>
    {
        private readonly ComboBox _owner;

        internal ItemCollection(ComboBox owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Adds an item to the owning combo box.
        /// </summary>
        /// <param name="value">The item text to add.</param>
        /// <returns>The zero-based index of the newly added item.</returns>
        public int Add(object? value)
        {
            return _owner.AddItem(value);
        }

        /// <summary>
        /// Adds a batch of items to the owning combo box.
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
        /// Adds a batch of items to the owning combo box.
        /// </summary>
        /// <param name="values">The item texts to add.</param>
        public void AddRange(params object?[] values)
        {
            AddRange((IEnumerable<object?>)values);
        }

        /// <summary>
        /// Removes all items from the owning combo box.
        /// </summary>
        public void Clear()
        {
            _owner.ClearItems();
        }

        /// <summary>
        /// Gets the number of items currently stored in the combo box.
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
