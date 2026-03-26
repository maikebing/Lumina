using System.Drawing;

namespace Lumina.NativeForms;

/// <summary>
/// Provides the common WinForms-like surface for all NativeForms controls backed by Win32 child windows.
/// </summary>
public abstract class Control
{
    private string _text = string.Empty;
    private int _left;
    private int _top;
    private int _width = 120;
    private int _height = 24;
    private bool _enabled = true;
    private bool _visible = true;

    /// <summary>
    /// Gets or sets the design-time or lookup name of the control.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the native Win32 window handle after the control has been created.
    /// </summary>
    public nint Handle { get; private set; }

    /// <summary>
    /// Gets or sets the text displayed by the control.
    /// </summary>
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

    /// <summary>
    /// Gets the left position of the control in client coordinates.
    /// </summary>
    public int Left => _left;

    /// <summary>
    /// Gets the top position of the control in client coordinates.
    /// </summary>
    public int Top => _top;

    /// <summary>
    /// Gets the width of the control.
    /// </summary>
    public int Width => _width;

    /// <summary>
    /// Gets the height of the control.
    /// </summary>
    public int Height => _height;

    /// <summary>
    /// Gets or sets a value indicating whether the control can receive user input.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the control is visible.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the upper-left location of the control.
    /// </summary>
    public Point Location
    {
        get => new(_left, _top);
        set => SetBounds(value.X, value.Y, _width, _height);
    }

    /// <summary>
    /// Gets or sets the size of the control.
    /// </summary>
    public Size Size
    {
        get => new(_width, _height);
        set => SetBounds(_left, _top, value.Width, value.Height);
    }

    /// <summary>
    /// Gets or sets the bounds of the control.
    /// </summary>
    public Rectangle Bounds
    {
        get => new(_left, _top, _width, _height);
        set => SetBounds(value.X, value.Y, value.Width, value.Height);
    }

    internal Form? Owner { get; private set; }

    internal int Id { get; private set; }

    /// <summary>
    /// Gets the Win32 class name used to create the underlying child window.
    /// </summary>
    protected abstract string ClassName { get; }

    /// <summary>
    /// Gets the Win32 style flags used when creating the underlying child window.
    /// </summary>
    protected abstract uint Style { get; }

    /// <summary>
    /// Gets optional Win32 extended style flags for the underlying child window.
    /// </summary>
    protected virtual uint ExStyle => 0;

    internal void Attach(Form owner, int id)
    {
        Owner = owner;
        Id = id;
    }

    /// <summary>
    /// Updates the control bounds using client coordinates.
    /// </summary>
    /// <param name="left">The left position.</param>
    /// <param name="top">The top position.</param>
    /// <param name="width">The desired width.</param>
    /// <param name="height">The desired height.</param>
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

    /// <summary>
    /// Lets a derived control expand or normalize its native height before the child window is created or resized.
    /// </summary>
    /// <param name="requestedHeight">The requested logical height.</param>
    /// <returns>The native Win32 height to apply.</returns>
    protected virtual int GetNativeHeight(int requestedHeight) => requestedHeight;

    /// <summary>
    /// Called after the native handle is created and themed.
    /// </summary>
    protected virtual void OnHandleCreated()
    {
        ApplyBounds();
    }

    /// <summary>
    /// Called when the parent form routes a <c>WM_COMMAND</c> notification to the control.
    /// </summary>
    /// <param name="notificationCode">The Win32 notification code.</param>
    /// <returns><see langword="true"/> if the notification was handled; otherwise, <see langword="false"/>.</returns>
    protected virtual bool OnCommand(int notificationCode) => false;

    /// <summary>
    /// Applies native theming customizations after the control handle is created.
    /// </summary>
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
