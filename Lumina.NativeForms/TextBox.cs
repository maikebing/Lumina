namespace Lumina.NativeForms;

public class TextBox : Control
{
    private bool _multiline;
    private bool _readOnly;

    public TextBox()
    {
    }

    public TextBox(bool multiline, bool readOnly = false)
    {
        _multiline = multiline;
        _readOnly = readOnly;

        if (multiline)
        {
            SetBounds(0, 0, 240, 120);
        }
    }

    public bool Multiline
    {
        get => _multiline;
        set => _multiline = value;
    }

    public bool ReadOnly
    {
        get => _readOnly;
        set => _readOnly = value;
    }

    protected override string ClassName => "EDIT";

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

    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    protected override int GetNativeHeight(int requestedHeight)
        => _multiline ? Math.Max(48, requestedHeight) : Math.Max(24, requestedHeight);

    public void AppendText(string value)
    {
        Text += value;
    }

    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
    }
}
