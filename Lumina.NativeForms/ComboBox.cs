namespace Lumina.NativeForms;

public class ComboBox : Control
{
    private readonly List<string> _items = [];
    private int _selectedIndex = -1;
    private readonly ItemCollection _itemsCollection;

    public ComboBox()
    {
        _itemsCollection = new ItemCollection(this);
    }

    public event EventHandler? SelectedIndexChanged;

    public int DropDownHeight { get; set; } = 240;

    public ComboBoxStyle DropDownStyle { get; set; } = ComboBoxStyle.DropDownList;

    public ItemCollection Items => _itemsCollection;

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

    protected override string ClassName => "COMBOBOX";

    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_VSCROLL | GetDropDownStyle();

    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    public void AddItem(string value)
    {
        _items.Add(value);
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_ADDSTRING, 0, value);
        }
    }

    public void ClearItems()
    {
        _items.Clear();
        _selectedIndex = -1;
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_RESETCONTENT, 0, 0);
        }
    }

    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(requestedHeight, DropDownHeight);

    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();

        foreach (var item in _items)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_ADDSTRING, 0, item);
        }

        if (_selectedIndex >= 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.CB_SETCURSEL, (nint)_selectedIndex, 0);
        }
    }

    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.CBN_SELCHANGE)
        {
            return false;
        }

        SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

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

    public sealed class ItemCollection
    {
        private readonly ComboBox _owner;

        internal ItemCollection(ComboBox owner)
        {
            _owner = owner;
        }

        public void Add(string value)
        {
            _owner.AddItem(value);
        }

        public void Clear()
        {
            _owner.ClearItems();
        }
    }
}
