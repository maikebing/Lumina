using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace Lumina.Forms;

/// <summary>
/// Represents a notification area icon component.
/// </summary>
[SupportedOSPlatform("windows")]
public class NotifyIcon : Component
{
    private const uint NIM_ADD = 0;
    private const uint NIM_MODIFY = 1;
    private const uint NIM_DELETE = 2;
    private const uint WM_TRAYICON = 0x8001;
    private const uint WM_LBUTTONUP = 0x0202;
    private const uint WM_LBUTTONDBLCLK = 0x0203;
    private const uint WM_RBUTTONUP = 0x0205;
    private const uint WM_RBUTTONDBLCLK = 0x0206;
    private const uint NIF_MESSAGE = 0x01;
    private const uint NIF_ICON = 0x02;
    private const uint NIF_TIP = 0x04;
    private static readonly Lock s_syncRoot = new();
    private static readonly Win32.WindowProc s_windowProc = WindowProcThunk;
    private static bool s_registered;
    private static int s_nextIconId;
    private static readonly Dictionary<nint, NotifyIcon> s_instances = [];

    private readonly uint _iconId = unchecked((uint)Interlocked.Increment(ref s_nextIconId));
    private Icon? _icon;
    private string? _iconLocation;
    private string _text = string.Empty;
    private bool _visible;
    private bool _addedToTray;
    private nint _iconHandle;
    private nint _loadedIconHandle;
    private string? _loadedIconPath;
    private nint _windowHandle;

    /// <summary>
    /// Occurs when the tray icon is clicked.
    /// </summary>
    public event EventHandler? Click;

    /// <summary>
    /// Occurs when the tray icon is double-clicked.
    /// </summary>
    public event EventHandler? DoubleClick;

    /// <summary>
    /// Occurs when the tray icon receives a mouse click.
    /// </summary>
    public event MouseEventHandler? MouseClick;

    /// <summary>
    /// Occurs when the tray icon receives a mouse double-click.
    /// </summary>
    public event MouseEventHandler? MouseDoubleClick;

    /// <summary>
    /// Gets or sets the context menu shown for the tray icon.
    /// </summary>
    public ContextMenuStrip? ContextMenuStrip { get; set; }

    /// <summary>
    /// Initializes a new notify icon component.
    /// </summary>
    public NotifyIcon()
    {
    }

    /// <summary>
    /// Initializes a new notify icon component and adds it to the specified container.
    /// </summary>
    /// <param name="container">The container that owns the component.</param>
    public NotifyIcon(IContainer container)
    {
        container?.Add(this);
    }

    /// <summary>
    /// Gets or sets the tooltip text.
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            if (_addedToTray)
            {
                UpdateTrayIcon(NIM_MODIFY);
            }
        }
    }

    /// <summary>
    /// Gets or sets the icon shown in the notification area.
    /// </summary>
    public Icon? Icon
    {
        get => _icon;
        set
        {
            if (ReferenceEquals(_icon, value))
            {
                return;
            }

            _icon = value;
            _iconHandle = 0;
            if (value is not null)
            {
                _iconLocation = null;
            }

            ReleaseLoadedIcon();

            if (_addedToTray)
            {
                UpdateTrayIcon(NIM_MODIFY);
            }
        }
    }

    /// <summary>
    /// Gets or sets the file path used to load the tray icon.
    /// </summary>
    public string? IconLocation
    {
        get => _iconLocation;
        set
        {
            string? normalizedValue = string.IsNullOrWhiteSpace(value) ? null : value;
            if (string.Equals(_iconLocation, normalizedValue, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _iconLocation = normalizedValue;
            if (normalizedValue is not null)
            {
                _icon = null;
                _iconHandle = 0;
            }

            ReleaseLoadedIcon();

            if (_addedToTray)
            {
                UpdateTrayIcon(NIM_MODIFY);
            }
        }
    }

    /// <summary>
    /// Gets or sets a native HICON handle used by the tray icon.
    /// </summary>
    public nint IconHandle
    {
        get => _iconHandle;
        set
        {
            if (_iconHandle == value)
            {
                return;
            }

            _iconHandle = value;
            if (value != 0)
            {
                _icon = null;
                _iconLocation = null;
            }

            ReleaseLoadedIcon();

            if (_addedToTray)
            {
                UpdateTrayIcon(NIM_MODIFY);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the notify icon is visible.
    /// </summary>
    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible == value)
            {
                return;
            }

            _visible = value;
            if (_visible)
            {
                ShowTrayIcon();
            }
            else
            {
                HideTrayIcon();
            }
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            HideTrayIcon();
            DestroyMessageWindow();
            ReleaseLoadedIcon();
        }

        base.Dispose(disposing);
    }

    private void ShowTrayIcon()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        EnsureMessageWindow();
        if (!_addedToTray)
        {
            UpdateTrayIcon(NIM_ADD);
            _addedToTray = true;
        }
    }

    private void HideTrayIcon()
    {
        if (!_addedToTray)
        {
            return;
        }

        UpdateTrayIcon(NIM_DELETE);
        _addedToTray = false;
    }

    private void EnsureMessageWindow()
    {
        if (_windowHandle != 0)
        {
            return;
        }

        nint instanceHandle = Win32.GetModuleHandleW(null);
        lock (s_syncRoot)
        {
            if (!s_registered)
            {
                var windowClass = new Win32.WNDCLASSEXW
                {
                    cbSize = (uint)Marshal.SizeOf<Win32.WNDCLASSEXW>(),
                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate(s_windowProc),
                    hInstance = instanceHandle,
                    lpszClassName = "LuminaFormsNotifyIconHost",
                };

                _ = Win32.RegisterClassExW(ref windowClass);
                s_registered = true;
            }
        }

        _windowHandle = Win32.CreateWindowExW(
            0,
            "LuminaFormsNotifyIconHost",
            "LuminaFormsNotifyIconHost",
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            instanceHandle,
            0);

        if (_windowHandle != 0)
        {
            lock (s_syncRoot)
            {
                s_instances[_windowHandle] = this;
            }
        }
    }

    private void DestroyMessageWindow()
    {
        if (_windowHandle == 0)
        {
            return;
        }

        _ = Win32.DestroyWindow(_windowHandle);
        _windowHandle = 0;
    }

    private void UpdateTrayIcon(uint message)
    {
        if (_windowHandle == 0)
        {
            return;
        }

        var notifyIconData = new NOTIFYICONDATAW
        {
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW>(),
            hWnd = _windowHandle,
            uID = _iconId,
            uFlags = message == NIM_DELETE ? 0u : NIF_MESSAGE | NIF_ICON | NIF_TIP,
            uCallbackMessage = WM_TRAYICON,
            hIcon = ResolveIconHandle(),
            szTip = TrimToolTip(_text),
        };

        _ = Shell_NotifyIconW(message, ref notifyIconData);
    }

    private nint ResolveIconHandle()
    {
        if (_icon is not null)
        {
            return _icon.Handle;
        }

        if (_iconHandle != 0)
        {
            return _iconHandle;
        }

        if (!string.IsNullOrWhiteSpace(_iconLocation) && File.Exists(_iconLocation))
        {
            if (!string.Equals(_loadedIconPath, _iconLocation, StringComparison.OrdinalIgnoreCase) || _loadedIconHandle == 0)
            {
                ReleaseLoadedIcon();
                _loadedIconHandle = Win32.LoadImageW(0, _iconLocation, Win32.IMAGE_ICON, 0, 0, Win32.LR_DEFAULTSIZE | Win32.LR_LOADFROMFILE);
                _loadedIconPath = _iconLocation;
            }

            if (_loadedIconHandle != 0)
            {
                return _loadedIconHandle;
            }
        }

        return LoadIconW(0, (nint)32512);
    }

    private void ReleaseLoadedIcon()
    {
        if (_loadedIconHandle == 0)
        {
            _loadedIconPath = null;
            return;
        }

        _ = Win32.DestroyIcon(_loadedIconHandle);
        _loadedIconHandle = 0;
        _loadedIconPath = null;
    }

    private static string TrimToolTip(string text)
    {
        if (text.Length <= 127)
        {
            return text;
        }

        return text[..127];
    }

    private static nint WindowProcThunk(nint hwnd, uint message, nint wParam, nint lParam)
    {
        NotifyIcon? instance;
        lock (s_syncRoot)
        {
            _ = s_instances.TryGetValue(hwnd, out instance);
        }

        return instance is null
            ? Win32.DefWindowProcW(hwnd, message, wParam, lParam)
            : instance.WindowProc(hwnd, message, wParam, lParam);
    }

    private nint WindowProc(nint hwnd, uint message, nint wParam, nint lParam)
    {
        if (message == WM_TRAYICON && unchecked((uint)wParam) == _iconId)
        {
            HandleTrayMouseMessage(unchecked((uint)lParam));
            return 0;
        }

        if (message == Win32.WM_NCDESTROY)
        {
            lock (s_syncRoot)
            {
                _ = s_instances.Remove(hwnd);
            }
        }

        return Win32.DefWindowProcW(hwnd, message, wParam, lParam);
    }

    private void HandleTrayMouseMessage(uint mouseMessage)
    {
        switch (mouseMessage)
        {
            case WM_LBUTTONUP:
                OnClick(EventArgs.Empty);
                OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                break;
            case WM_RBUTTONUP:
                ShowContextMenu();
                OnClick(EventArgs.Empty);
                OnMouseClick(new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
                break;
            case WM_LBUTTONDBLCLK:
                OnDoubleClick(EventArgs.Empty);
                OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
                break;
            case WM_RBUTTONDBLCLK:
                OnDoubleClick(EventArgs.Empty);
                OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0));
                break;
        }
    }

    private void ShowContextMenu()
    {
        if (ContextMenuStrip is null || _windowHandle == 0)
        {
            return;
        }

        if (!Win32.GetCursorPos(out var cursorPoint))
        {
            return;
        }

        nint ownerHandle = Application.GetAnyOpenFormHandle();
        if (ownerHandle == 0)
        {
            ownerHandle = _windowHandle;
        }

        ContextMenuStrip.ShowAtScreenPoint(ownerHandle, new System.Drawing.Point(cursorPoint.x, cursorPoint.y));
    }

    /// <summary>
    /// Raises the <see cref="Click"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnClick(EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="DoubleClick"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnDoubleClick(EventArgs e)
    {
        DoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="MouseClick"/> event.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected virtual void OnMouseClick(MouseEventArgs e)
    {
        MouseClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="MouseDoubleClick"/> event.
    /// </summary>
    /// <param name="e">The mouse event data.</param>
    protected virtual void OnMouseDoubleClick(MouseEventArgs e)
    {
        MouseDoubleClick?.Invoke(this, e);
    }

    [DllImport("shell32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool Shell_NotifyIconW(uint dwMessage, ref NOTIFYICONDATAW lpData);

    [DllImport("user32.dll")]
    private static extern nint LoadIconW(nint hInstance, nint lpIconName);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NOTIFYICONDATAW
    {
        public uint cbSize;
        public nint hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public nint hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public uint dwState;
        public uint dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public uint uTimeoutOrVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public uint dwInfoFlags;
    }
}
