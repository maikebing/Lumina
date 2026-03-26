namespace Lumina.NativeForms;

public class CheckBox : Control
{
    private bool _checked;

    public event EventHandler? CheckedChanged;

    public bool Checked
    {
        get => Handle != 0
            ? Win32.SendMessageW(Handle, Win32.BM_GETCHECK, 0, 0) == (nint)Win32.BST_CHECKED
            : _checked;
        set
        {
            _checked = value;
            if (Handle != 0)
            {
                _ = Win32.SendMessageW(Handle, Win32.BM_SETCHECK, (nint)(value ? Win32.BST_CHECKED : Win32.BST_UNCHECKED), 0);
            }
        }
    }

    protected override string ClassName => "BUTTON";

    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.BS_AUTOCHECKBOX;

    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        Checked = _checked;
    }

    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.BN_CLICKED)
        {
            return false;
        }

        _checked = Win32.SendMessageW(Handle, Win32.BM_GETCHECK, 0, 0) == (nint)Win32.BST_CHECKED;
        CheckedChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
    }
}
