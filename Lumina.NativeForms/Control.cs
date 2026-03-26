using System.Drawing;

namespace Lumina.NativeForms;

public abstract class Control
{
    private string _text = string.Empty;
    private int _left;
    private int _top;
    private int _width = 120;
    private int _height = 24;
    private bool _enabled = true;
    private bool _visible = true;

    public string Name { get; set; } = string.Empty;

    public nint Handle { get; private set; }

    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            if (Handle != 0)
            {
                _ = Win32.SetWindowTextW(Handle, _text);
            }
        }
    }

    public int Left => _left;

    public int Top => _top;

    public int Width => _width;

    public int Height => _height;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            if (Handle != 0)
            {
                _ = Win32.EnableWindow(Handle, value);
            }
        }
    }

    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            if (Handle != 0)
            {
                _ = Win32.ShowWindow(Handle, value ? Win32.SW_SHOW : Win32.SW_HIDE);
            }
        }
    }

    public Point Location
    {
        get => new(_left, _top);
        set => SetBounds(value.X, value.Y, _width, _height);
    }

    public Size Size
    {
        get => new(_width, _height);
        set => SetBounds(_left, _top, value.Width, value.Height);
    }

    public Rectangle Bounds
    {
        get => new(_left, _top, _width, _height);
        set => SetBounds(value.X, value.Y, value.Width, value.Height);
    }

    internal Form? Owner { get; private set; }

    internal int Id { get; private set; }

    protected abstract string ClassName { get; }

    protected abstract uint Style { get; }

    protected virtual uint ExStyle => 0;

    internal void Attach(Form owner, int id)
    {
        Owner = owner;
        Id = id;
    }

    public void SetBounds(int left, int top, int width, int height)
    {
        _left = left;
        _top = top;
        _width = Math.Max(1, width);
        _height = Math.Max(1, height);

        if (Handle != 0)
        {
            ApplyBounds();
        }
    }

    internal void CreateHandle()
    {
        if (Owner is null)
        {
            throw new InvalidOperationException("The control is not attached to a form.");
        }

        if (Handle != 0)
        {
            return;
        }

        int nativeHeight = GetNativeHeight(_height);

        Handle = Win32.CreateWindowExW(
            ExStyle,
            ClassName,
            _text,
            Style,
            _left,
            _top,
            _width,
            nativeHeight,
            Owner.Handle,
            (nint)Id,
            Owner.InstanceHandle,
            0);

        if (Handle == 0)
        {
            throw new InvalidOperationException($"Failed to create control '{ClassName}'.");
        }

        if (Owner.UiFontHandle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.WM_SETFONT, Owner.UiFontHandle, (nint)1);
        }

        _ = Win32.EnableWindow(Handle, _enabled);
        _ = Win32.ShowWindow(Handle, _visible ? Win32.SW_SHOW : Win32.SW_HIDE);
        ApplyTheme();
        OnHandleCreated();
    }

    internal bool HandleCommand(int notificationCode)
        => OnCommand(notificationCode);

    protected virtual int GetNativeHeight(int requestedHeight) => requestedHeight;

    protected virtual void OnHandleCreated()
    {
        ApplyBounds();
    }

    protected virtual bool OnCommand(int notificationCode) => false;

    protected virtual void ApplyTheme()
    {
    }

    private void ApplyBounds()
    {
        if (Handle != 0)
        {
            _ = Win32.MoveWindow(Handle, _left, _top, _width, GetNativeHeight(_height), true);
        }
    }
}
