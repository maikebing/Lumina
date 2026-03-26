namespace Lumina.NativeForms;

/// <summary>
/// Represents a standard radio button control.
/// </summary>
public class RadioButton : Control
{
    private bool _checked;

    /// <summary>
    /// Occurs when the <see cref="Checked"/> state changes.
    /// </summary>
    public event EventHandler? CheckedChanged;

    /// <summary>
    /// Gets or sets a value indicating whether the radio button is selected.
    /// </summary>
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

    /// <inheritdoc />
    protected override string ClassName => "BUTTON";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.BS_AUTORADIOBUTTON;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        Checked = _checked;
    }

    /// <inheritdoc />
    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.BN_CLICKED)
        {
            return false;
        }

        bool currentValue = Win32.SendMessageW(Handle, Win32.BM_GETCHECK, 0, 0) == (nint)Win32.BST_CHECKED;
        if (_checked != currentValue)
        {
            _checked = currentValue;
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        return true;
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
    }
}
