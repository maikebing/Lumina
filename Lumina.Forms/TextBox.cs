namespace Lumina.Forms;

/// <summary>
/// Represents a single-line or multi-line edit control.
/// </summary>
public class TextBox : Control
{
    private bool _multiline;
    private bool _readOnly;

    /// <summary>
    /// Initializes a single-line editable text box.
    /// </summary>
    public TextBox()
    {
    }

    /// <summary>
    /// Initializes a text box with the requested multi-line and read-only behavior.
    /// </summary>
    /// <param name="multiline">Whether the control should use a multi-line edit window.</param>
    /// <param name="readOnly">Whether the control should reject user edits.</param>
    public TextBox(bool multiline, bool readOnly = false)
    {
        _multiline = multiline;
        _readOnly = readOnly;

        if (multiline)
        {
            SetBounds(0, 0, 240, 120);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the text box is multi-line.
    /// </summary>
    public bool Multiline
    {
        get => _multiline;
        set => _multiline = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the text box is read-only.
    /// </summary>
    public bool ReadOnly
    {
        get => _readOnly;
        set
        {
            _readOnly = value;
            if (Handle != 0)
            {
                _ = Win32.SendMessageW(Handle, Win32.EM_SETREADONLY, value ? (nint)1 : 0, 0);
            }
        }
    }

    /// <inheritdoc />
    protected override string ClassName => "EDIT";

    /// <inheritdoc />
    protected override uint Style
    {
        get
        {
            var style = Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.ES_LEFT;
            if (_multiline)
            {
                style |= Win32.WS_VSCROLL | Win32.ES_MULTILINE | Win32.ES_AUTOVSCROLL | Win32.ES_WANTRETURN;
            }
            else
            {
                style |= Win32.ES_AUTOHSCROLL;
            }

            if (_readOnly)
            {
                style |= Win32.ES_READONLY;
            }

            return style;
        }
    }

    /// <inheritdoc />
    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => _multiline ? Math.Max(48, requestedHeight) : Math.Max(24, requestedHeight);

    /// <summary>
    /// Appends text to the current text box contents.
    /// </summary>
    /// <param name="value">The text to append.</param>
    public void AppendText(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (Handle == 0)
        {
            Text += value;
            return;
        }

        _ = Win32.SendMessageW(Handle, Win32.EM_SETSEL, (nint)int.MaxValue, (nint)int.MaxValue);
        _ = Win32.SendMessageW(Handle, Win32.EM_REPLACESEL, 0, value);
        _ = UpdateTextFromHandle();
    }

    /// <inheritdoc />
    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.EN_CHANGE)
        {
            return false;
        }

        return UpdateTextFromHandle();
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
    }
}
