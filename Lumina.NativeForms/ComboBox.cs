using System.Runtime.InteropServices;

namespace Lumina.NativeForms;

/// <summary>
/// Represents a Win32 combo box with a WinForms-like API surface.
/// </summary>
public class ComboBox : Control
{
    private readonly List<string> _items = [];
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
    /// Gets the item collection used to add or clear list entries.
    /// </summary>
    public ItemCollection Items => _itemsCollection;

    /// <summary>
    /// Gets or sets the zero-based selected item index.
    /// </summary>
    public int SelectedIndex
    {
        get => Handle != 0 ? (int)Win32.SendMessageW(Handle, Win32.CB_GETCURSEL, 0, 0) : _selectedIndex;
        set
        {
            _selectedIndex = value;
            if (Handle != 0)
            {
                _ = Win32.SendMessageW(Handle, Win32.CB_SETCURSEL, (nint)value, 0);
            }
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
    public void AddItem(string value)
    {
        _items.Add(value);
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_ADDSTRING, 0, value);
        }
    }

    /// <summary>
    /// Removes all items from the combo box.
    /// </summary>
    public void ClearItems()
    {
        _items.Clear();
        _selectedIndex = -1;
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_RESETCONTENT, 0, 0);
        }
    }

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(requestedHeight, DropDownHeight);

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();

        foreach (string item in CollectionsMarshal.AsSpan(_items))
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_ADDSTRING, 0, item);
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

        SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
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

    /// <summary>
    /// Represents the item collection for a <see cref="ComboBox"/>.
    /// </summary>
    public sealed class ItemCollection
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
        public void Add(string value)
        {
            _owner.AddItem(value);
        }

        /// <summary>
        /// Removes all items from the owning combo box.
        /// </summary>
        public void Clear()
        {
            _owner.ClearItems();
        }
    }
}
