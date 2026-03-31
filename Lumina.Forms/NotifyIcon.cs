using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lumina.Forms;

/// <summary>
/// Represents a notification area icon component.
/// </summary>
public class NotifyIcon : Component
{
    private const uint NIM_ADD = 0;
    private const uint NIM_MODIFY = 1;
    private const uint NIM_DELETE = 2;
    private const uint NIF_ICON = 0x02;
    private const uint NIF_TIP = 0x04;
    private static readonly Lock s_syncRoot = new();
    private static readonly Win32.WindowProc s_windowProc = WindowProcThunk;
    private static bool s_registered;
    private static int s_nextIconId;

    private readonly uint _iconId = unchecked((uint)Interlocked.Increment(ref s_nextIconId));
    private string _text = string.Empty;
    private bool _visible;
    private bool _addedToTray;
    private nint _windowHandle;

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
            (nint)(-3),
            0,
            instanceHandle,
            0);
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
            uFlags = NIF_ICON | NIF_TIP,
            hIcon = LoadIconW(0, (nint)32512),
            szTip = TrimToolTip(_text),
        };

        _ = Shell_NotifyIconW(message, ref notifyIconData);
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
        return Win32.DefWindowProcW(hwnd, message, wParam, lParam);
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