using System.Drawing;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Provides the common WinForms-like surface for all LuminaForms controls backed by Win32 child windows.
/// </summary>
public abstract class Control : IDisposable
{
    private static readonly Lock s_subclassLock = new();
    private static readonly Dictionary<nint, Control> s_controlsByHandle = [];
    private static readonly Win32.WindowProc s_subclassProc = SubclassWindowProcThunk;

    private string _text = string.Empty;
    private int _left;
    private int _top;
    private int _width = 120;
    private int _height = 24;
    private bool _enabled = true;
    private bool _visible = true;
    private bool _disposed;
    private nint _originalWindowProc;
    private long _lastContextMenuShownTick = long.MinValue;
    private Point _lastContextMenuScreenLocation = new(int.MinValue, int.MinValue);

    /// <summary>
    /// Gets or sets the design-time or lookup name of the control.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the native Win32 window handle after the control has been created.
    /// </summary>
    public nint Handle { get; private set; }

    /// <summary>
    /// Gets or sets an arbitrary user-defined value associated with the control.
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Gets or sets the context menu associated with the control.
    /// </summary>
    public ContextMenuStrip? ContextMenuStrip { get; set; }

    /// <summary>
    /// Gets or sets the tab order index used by designer-style layout code.
    /// </summary>
    public int TabIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control sizes itself to its contents.
    /// </summary>
    public bool AutoSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can move the focus to this control with the Tab key.
    /// </summary>
    public bool TabStop { get; set; }

    /// <summary>
    /// Gets a value indicating whether the control has been disposed.
    /// </summary>
    public bool IsDisposed => _disposed;

    /// <summary>
    /// Gets the parent control, if the control is currently hosted inside a container control.
    /// </summary>
    public Control? Parent { get; private set; }

    /// <summary>
    /// Occurs when the <see cref="Text"/> property changes.
    /// </summary>
    public event EventHandler? TextChanged;

    /// <summary>
    /// Gets or sets the text displayed by the control.
    /// </summary>
    public string Text
    {
        get
        {
            if (Handle != 0)
            {
                _text = Win32.GetText(Handle);
            }

            return _text;
        }
        set
        {
            ThrowIfDisposed();

            string newText = value ?? string.Empty;
            string currentText = Text;
            if (string.Equals(currentText, newText, StringComparison.Ordinal))
            {
                _text = newText;
                return;
            }

            _text = newText;
            if (Handle != 0)
            {
                _ = Win32.SetWindowTextW(Handle, _text);
            }

            OnTextChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets the left position of the control in client coordinates.
    /// </summary>
    public int Left
    {
        get => _left;
        set => SetBounds(value, _top, _width, _height);
    }

    /// <summary>
    /// Gets the top position of the control in client coordinates.
    /// </summary>
    public int Top
    {
        get => _top;
        set => SetBounds(_left, value, _width, _height);
    }

    /// <summary>
    /// Gets the width of the control.
    /// </summary>
    public int Width
    {
        get => _width;
        set => SetBounds(_left, _top, value, _height);
    }

    /// <summary>
    /// Gets the height of the control.
    /// </summary>
    public int Height
    {
        get => _height;
        set => SetBounds(_left, _top, _width, value);
    }

    /// <summary>
    /// Gets or sets the right edge of the control.
    /// </summary>
    public int Right
    {
        get => _left + _width;
        set => SetBounds(value - _width, _top, _width, _height);
    }

    /// <summary>
    /// Gets or sets the bottom edge of the control.
    /// </summary>
    public int Bottom
    {
        get => _top + _height;
        set => SetBounds(_left, value - _height, _width, _height);
    }

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

    /// <summary>
    /// Gets a value indicating whether this control should materialize a native child window.
    /// </summary>
    protected virtual bool ShouldCreateNativeHandle => true;

    internal void Attach(Form owner, int id, Control? parent)
    {
        ThrowIfDisposed();
        Owner = owner;
        Id = id;
        Parent = parent;
    }

    internal void SetParent(Control parent)
    {
        Parent = parent;
    }

    internal void ClearParent()
    {
        Parent = null;
    }

    internal void DetachFromOwner()
    {
        Owner = null;
        Id = 0;
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
        ThrowIfDisposed();

        bool boundsChanged = _left != left
            || _top != top
            || _width != Math.Max(1, width)
            || _height != Math.Max(1, height);

        _left = left;
        _top = top;
        _width = Math.Max(1, width);
        _height = Math.Max(1, height);

        if (Handle != 0)
        {
            ApplyBounds();
        }

        if (boundsChanged)
        {
            OnBoundsChanged();
        }
    }

    /// <summary>
    /// Invalidates the control so the underlying native window repaints.
    /// </summary>
    public void Invalidate()
    {
        if (Handle != 0)
        {
            _ = Win32.InvalidateRect(Handle, 0, true);
        }
    }

    /// <summary>
    /// Forces the control to repaint immediately.
    /// </summary>
    public void Refresh()
    {
        if (Handle != 0)
        {
            Invalidate();
            _ = Win32.UpdateWindow(Handle);
        }
    }

    /// <summary>
    /// Suspends layout logic for compatibility with designer-generated code.
    /// </summary>
    public virtual void SuspendLayout()
    {
    }

    /// <summary>
    /// Resumes layout logic for compatibility with designer-generated code.
    /// </summary>
    /// <param name="performLayout">Whether layout should be performed immediately.</param>
    public virtual void ResumeLayout(bool performLayout)
    {
        if (performLayout)
        {
            PerformLayout();
        }
    }

    /// <summary>
    /// Performs layout for compatibility with designer-generated code.
    /// </summary>
    public virtual void PerformLayout()
    {
    }

    /// <summary>
    /// Releases the native child window and marks the control as disposed.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        OnDisposing();

        if (Handle != 0)
        {
            _ = Win32.DestroyWindow(Handle);
        }

        ReleaseHandleRecursive();
    }

    internal void CreateHandleRecursive()
    {
        CreateHandle();

        if (Handle != 0 && this is ContainerControlBase container)
        {
            container.CreateChildHandles();
        }
    }

    internal void ReleaseHandleRecursive()
    {
        if (this is ContainerControlBase container)
        {
            foreach (Control child in container.ChildControls)
            {
                child.ReleaseHandleRecursive();
            }
        }

        Handle = 0;
    }

    internal void CreateHandle()
    {
        ThrowIfDisposed();

        if (Owner is null)
        {
            throw new InvalidOperationException("The control is not attached to a form.");
        }

        if (Handle != 0)
        {
            return;
        }

        if (!ShouldCreateNativeHandle)
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
            Parent?.Handle ?? Owner.Handle,
            (nint)Id,
            Owner.InstanceHandle,
            0);

        if (Handle == 0)
        {
            throw new InvalidOperationException($"Failed to create control '{ClassName}'.");
        }

        RegisterSubclass();

        if (Owner.UiFontHandle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.WM_SETFONT, Owner.UiFontHandle, (nint)1);
        }

        _ = Win32.EnableWindow(Handle, _enabled);
        _ = Win32.ShowWindow(Handle, _visible ? Win32.SW_SHOW : Win32.SW_HIDE);
        ApplyTheme();
        OnHandleCreated();
    }

    internal static bool TryGetControlByHandle(nint handle, out Control? control)
    {
        lock (s_subclassLock)
        {
            return s_controlsByHandle.TryGetValue(handle, out control);
        }
    }

    internal bool HandleCommand(int notificationCode)
        => OnCommand(notificationCode);

    internal bool HandleNotify(int notificationCode, nint lParam)
        => OnNotify(notificationCode, lParam);

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
    /// Called when the parent form routes a <c>WM_NOTIFY</c> notification to the control.
    /// </summary>
    /// <param name="notificationCode">The Win32 notification code.</param>
    /// <param name="lParam">The raw notification payload.</param>
    /// <returns><see langword="true"/> if the notification was handled; otherwise, <see langword="false"/>.</returns>
    protected virtual bool OnNotify(int notificationCode, nint lParam) => false;

    /// <summary>
    /// Called after the control bounds change.
    /// </summary>
    protected virtual void OnBoundsChanged()
    {
    }

    /// <summary>
    /// Called before the control destroys its native resources during disposal.
    /// </summary>
    protected virtual void OnDisposing()
    {
    }

    /// <summary>
    /// Applies native theming customizations after the control handle is created.
    /// </summary>
    protected virtual void ApplyTheme()
    {
    }

    /// <summary>
    /// Raises the <see cref="TextChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnTextChanged(EventArgs e)
    {
        TextChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Updates the cached <see cref="Text"/> value from the native child window and raises <see cref="TextChanged"/>
    /// if the value changed.
    /// </summary>
    /// <returns><see langword="true"/> if the text changed; otherwise, <see langword="false"/>.</returns>
    protected bool UpdateTextFromHandle()
    {
        if (Handle == 0)
        {
            return false;
        }

        string currentText = Win32.GetText(Handle);
        if (string.Equals(_text, currentText, StringComparison.Ordinal))
        {
            return false;
        }

        _text = currentText;
        OnTextChanged(EventArgs.Empty);
        return true;
    }

    private void RegisterSubclass()
    {
        if (Handle == 0 || _originalWindowProc != 0)
        {
            return;
        }

        nint subclassPointer = Marshal.GetFunctionPointerForDelegate(s_subclassProc);
        nint previousWindowProc = Win32.SetWindowLongPtrW(Handle, Win32.GWLP_WNDPROC, subclassPointer);
        if (previousWindowProc == 0)
        {
            return;
        }

        _originalWindowProc = previousWindowProc;
        lock (s_subclassLock)
        {
            s_controlsByHandle[Handle] = this;
        }
    }

    private static nint SubclassWindowProcThunk(nint hwnd, uint message, nint wParam, nint lParam)
    {
        Control? control;
        lock (s_subclassLock)
        {
            _ = s_controlsByHandle.TryGetValue(hwnd, out control);
        }

        return control is null
            ? Win32.DefWindowProcW(hwnd, message, wParam, lParam)
            : control.SubclassWindowProc(hwnd, message, wParam, lParam);
    }

    private nint SubclassWindowProc(nint hwnd, uint message, nint wParam, nint lParam)
    {
        nint result = _originalWindowProc != 0
            ? Win32.CallWindowProcW(_originalWindowProc, hwnd, message, wParam, lParam)
            : Win32.DefWindowProcW(hwnd, message, wParam, lParam);

        if (message == Win32.WM_CONTEXTMENU && ContextMenuStrip is not null && TryShowAttachedContextMenu(hwnd, lParam))
        {
            return 0;
        }

        if (message == Win32.WM_RBUTTONUP && ContextMenuStrip is not null)
        {
            _ = TryShowAttachedContextMenuFromClientPoint(hwnd, lParam);
        }

        if (message == Win32.WM_NCDESTROY)
        {
            lock (s_subclassLock)
            {
                _ = s_controlsByHandle.Remove(hwnd);
            }

            _originalWindowProc = 0;
        }

        return result;
    }

    internal bool TryShowAttachedContextMenu(nint hwnd, nint lParam)
    {
        if (!TryResolveContextMenuScreenLocation(hwnd, lParam, out Point screenLocation))
        {
            return false;
        }

        return TryShowAttachedContextMenuAtScreenPoint(hwnd, screenLocation);
    }

    internal bool TryShowAttachedContextMenuFromClientPoint(nint hwnd, nint lParam)
    {
        if (!TryResolveClientContextMenuScreenLocation(hwnd, lParam, out Point screenLocation))
        {
            return false;
        }

        return TryShowAttachedContextMenuAtScreenPoint(hwnd, screenLocation);
    }

    internal bool TryShowAttachedContextMenuAtScreenPoint(nint hwnd, Point screenLocation)
    {
        ContextMenuStrip? contextMenuStrip = ContextMenuStrip;
        if (contextMenuStrip is null)
        {
            return false;
        }

        if (ShouldSuppressDuplicateContextMenu(screenLocation))
        {
            return true;
        }

        _lastContextMenuShownTick = Environment.TickCount64;
        _lastContextMenuScreenLocation = screenLocation;

        nint ownerHandle = Owner?.Handle ?? Parent?.Handle ?? hwnd;
        contextMenuStrip.ShowAtScreenPoint(ownerHandle, screenLocation);
        return true;
    }

    private bool TryResolveContextMenuScreenLocation(nint hwnd, nint lParam, out Point screenLocation)
    {
        if (lParam == (nint)(-1))
        {
            if (Win32.GetWindowRect(hwnd, out var rect))
            {
                screenLocation = new Point(rect.Left + Math.Min(16, Math.Max(0, rect.Width - 1)), rect.Top + Math.Min(16, Math.Max(0, rect.Height - 1)));
                return true;
            }

            if (Win32.GetCursorPos(out var cursor))
            {
                screenLocation = new Point(cursor.x, cursor.y);
                return true;
            }

            screenLocation = default;
            return false;
        }

        screenLocation = ExtractPoint(lParam);
        return true;
    }

    private static bool TryResolveClientContextMenuScreenLocation(nint hwnd, nint lParam, out Point screenLocation)
    {
        var point = new Win32.POINT
        {
            x = unchecked((short)(nuint)lParam),
            y = unchecked((short)(((nuint)lParam >> 16) & 0xFFFF)),
        };

        if (!Win32.ClientToScreen(hwnd, ref point))
        {
            screenLocation = default;
            return false;
        }

        screenLocation = new Point(point.x, point.y);
        return true;
    }

    private bool ShouldSuppressDuplicateContextMenu(Point screenLocation)
    {
        long elapsed = Environment.TickCount64 - _lastContextMenuShownTick;
        return elapsed is >= 0 and <= 250
            && Math.Abs(_lastContextMenuScreenLocation.X - screenLocation.X) <= 4
            && Math.Abs(_lastContextMenuScreenLocation.Y - screenLocation.Y) <= 4;
    }

    internal static Point ExtractPoint(nint lParam)
    {
        return new Point(
            unchecked((short)(nuint)lParam),
            unchecked((short)(((nuint)lParam >> 16) & 0xFFFF)));
    }

    private void ApplyBounds()
    {
        if (Handle != 0)
        {
            _ = Win32.MoveWindow(Handle, _left, _top, _width, GetNativeHeight(_height), true);
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
