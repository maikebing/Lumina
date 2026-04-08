using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a single-line or multi-line edit control.
/// </summary>
public class TextBox : Control
{
    private bool _multiline;
    private bool _readOnly;
    private int _selectionStart;
    private int _selectionLength;
    private Font? _font = CreateDefaultFont();
    private nint _fontHandle;
    private bool _ownsFontHandle;

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

    /// <summary>
    /// Gets the starting position of text selected in the text box.
    /// </summary>
    public int SelectionStart
    {
        get
        {
            UpdateSelectionFromHandle();
            return _selectionStart;
        }
    }

    /// <summary>
    /// Gets the number of characters selected in the text box.
    /// </summary>
    public int SelectionLength
    {
        get
        {
            UpdateSelectionFromHandle();
            return _selectionLength;
        }
    }

    /// <summary>
    /// Occurs when the selection changes.
    /// </summary>
    public event EventHandler? SelectionChanged;

    /// <summary>
    /// Gets or sets the font of the text box.
    /// </summary>
    public Font Font
    {
        get => _font ??= CreateDefaultFont();
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _font?.Dispose();
            _font = (Font)value.Clone();
            ApplyFontToHandle();
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

    /// <summary>
    /// Selects a range of text in the text box.
    /// </summary>
    /// <param name="start">The starting position.</param>
    /// <param name="length">The number of characters to select.</param>
    public void Select(int start, int length)
    {
        int safeStart = Math.Max(0, start);
        int safeLength = Math.Max(0, length);

        if (Handle == 0)
        {
            return;
        }

        _ = Win32.SendMessageW(Handle, Win32.EM_SETSEL, (nint)safeStart, (nint)(safeStart + safeLength));
        if (_selectionStart != safeStart || _selectionLength != safeLength)
        {
            _selectionStart = safeStart;
            _selectionLength = safeLength;
            OnSelectionChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Scrolls the text box so that the caret is visible.
    /// </summary>
    public void ScrollToCaret()
    {
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.EM_SCROLLCARET, 0, 0);
        }
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
    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        if (message is Win32.WM_KEYUP or Win32.WM_LBUTTONUP)
        {
            _ = UpdateSelectionFromHandle();
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyNativeThemeState();
        ApplyExplorerTheme();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyNativeThemeState();
        ApplyFontToHandle();
    }

    /// <inheritdoc />
    protected override void OnDisposing()
    {
        if (_fontHandle != 0 && _ownsFontHandle)
        {
            _ = Win32.DeleteObject(_fontHandle);
        }

        _fontHandle = 0;
        _ownsFontHandle = false;
        _font?.Dispose();
        _font = null;
        base.OnDisposing();
    }

    private void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
    }

    /// <inheritdoc />
    protected override string GetPreferredThemeClass(ResolvedVisualStyle visualStyle)
        => visualStyle.IsDarkMode && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763)
            ? "DarkMode_CFD"
            : "CFD";

    /// <inheritdoc />
    protected override string GetFallbackThemeClass(ResolvedVisualStyle visualStyle)
        => base.GetPreferredThemeClass(visualStyle);

    /// <summary>
    /// Raises the <see cref="SelectionChanged"/> event.
    /// </summary>
    protected virtual void OnSelectionChanged(EventArgs e)
    {
        SelectionChanged?.Invoke(this, e);
    }

    private static Font CreateDefaultFont()
        => SystemFonts.MessageBoxFont is { } font
            ? (Font)font.Clone()
            : new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);

    private bool UpdateSelectionFromHandle()
    {
        if (Handle == 0)
        {
            return false;
        }

        _ = Win32.SendMessageW(Handle, Win32.EM_GETSEL, out int start, out int end);
        int normalizedStart = Math.Max(0, start);
        int normalizedLength = Math.Max(0, end - start);

        if (_selectionStart == normalizedStart && _selectionLength == normalizedLength)
        {
            return false;
        }

        _selectionStart = normalizedStart;
        _selectionLength = normalizedLength;
        OnSelectionChanged(EventArgs.Empty);
        return true;
    }

    private void ApplyFontToHandle()
    {
        if (Handle == 0)
        {
            return;
        }

        if (_fontHandle != 0 && _ownsFontHandle)
        {
            _ = Win32.DeleteObject(_fontHandle);
            _fontHandle = 0;
            _ownsFontHandle = false;
        }

        if (_font is null)
        {
            return;
        }

        _fontHandle = Win32.CreateFontFromManagedFont(_font);
        _ownsFontHandle = _fontHandle != 0;
        if (_fontHandle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.WM_SETFONT, _fontHandle, (nint)1);
        }
    }
}
