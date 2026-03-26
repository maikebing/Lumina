using System.Runtime.InteropServices;

namespace Lumina.NativeForms;

/// <summary>
/// Represents a simple single-selection list box control.
/// </summary>
public class ListBox : Control
{
    private readonly List<string> _items = [];
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
    /// Gets or sets the zero-based selected item index.
    /// </summary>
    public int SelectedIndex
    {
        get => Handle != 0 ? (int)Win32.SendMessageW(Handle, Win32.LB_GETCURSEL, 0, 0) : _selectedIndex;
        set
        {
            _selectedIndex = value;
            if (Handle != 0)
            {
                _ = Win32.SendMessageW(Handle, Win32.LB_SETCURSEL, (nint)value, 0);
            }
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
    public void AddItem(string value)
    {
        _items.Add(value);
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.LB_ADDSTRING, 0, value);
        }
    }

    /// <summary>
    /// Removes all items from the list box.
    /// </summary>
    public void ClearItems()
    {
        _items.Clear();
        _selectedIndex = -1;
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.LB_RESETCONTENT, 0, 0);
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();

        foreach (string item in CollectionsMarshal.AsSpan(_items))
        {
            _ = Win32.SendMessageW(Handle, Win32.LB_ADDSTRING, 0, item);
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

        SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
    }

    /// <summary>
    /// Represents the item collection for a <see cref="ListBox"/>.
    /// </summary>
    public sealed class ObjectCollection
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
        public void Add(string value)
        {
            _owner.AddItem(value);
        }

        /// <summary>
        /// Removes all items from the owning list box.
        /// </summary>
        public void Clear()
        {
            _owner.ClearItems();
        }
    }
}
