using System.Drawing;
using System.Runtime.InteropServices;

namespace Lumina.NativeForms;

public class Form : IDisposable
{
    private const string WindowClassName = "LuminaNativeFormsWindow";
    private static readonly Win32.WindowProc s_windowProc = WindowProcThunk;
    private static bool s_registered;

    private readonly List<Control> _controlList = [];
    private readonly Dictionary<int, Control> _controlsById = [];
    private readonly ControlCollection _controls;
    private GCHandle _selfHandle;
    private bool _disposed;
    private bool _shown;
    private int _nextControlId = 1000;
    private bool _effectExplicitlySet;
    private bool _themeExplicitlySet;
    private EffectKind _pendingEffectKind = EffectKind.None;
    private EffectOptions? _pendingEffectOptions;
    private ThemeMode? _requestedThemeMode;

    public Form()
    {
        _controls = new ControlCollection(this);
    }

    public string Text { get; set; } = "Lumina Native Form";

    public int Width { get; set; } = 960;

    public int Height { get; set; } = 640;

    public nint Handle { get; private set; }

    internal nint InstanceHandle { get; private set; }

    internal nint UiFontHandle { get; private set; }

    public Size ClientSize
    {
        get => new(Width, Height);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    public ControlCollection Controls => _controls;

    public void Show()
    {
        ThrowIfDisposed();
        if (_shown)
        {
            return;
        }

        _shown = true;
        InstanceHandle = Win32.GetModuleHandleW(null);
        EnsureWindowClassRegistered(InstanceHandle);

        UiFontHandle = Win32.GetStockObject(Win32.DEFAULT_GUI_FONT);
        _selfHandle = GCHandle.Alloc(this);

        nint hwnd = Win32.CreateWindowExW(
            Win32.WS_EX_APPWINDOW,
            WindowClassName,
            Text,
            Win32.WS_OVERLAPPEDWINDOW | Win32.WS_VISIBLE | Win32.WS_CLIPCHILDREN,
            Win32.CW_USEDEFAULT,
            Win32.CW_USEDEFAULT,
            Width,
            Height,
            0,
            0,
            InstanceHandle,
            GCHandle.ToIntPtr(_selfHandle));

        if (hwnd == 0)
        {
            _selfHandle.Free();
            throw new InvalidOperationException("Failed to create the native form window.");
        }

        Handle = hwnd;
        Application.EnsureVisualStylesInitialized();
        ApplyApplicationDefaults();

        foreach (var control in _controlList)
        {
            control.CreateHandle();
        }

        OnCreated();
        ApplyPendingEffect();
        OnLayout();

        _ = Win32.ShowWindow(Handle, Win32.SW_SHOW);
        _ = Win32.UpdateWindow(Handle);
    }

    public void Close()
    {
        if (Handle != 0)
        {
            _ = Win32.DestroyWindow(Handle);
        }
    }

    public void SetEffect(EffectKind kind, EffectOptions? options = null)
    {
        _effectExplicitlySet = true;
        _pendingEffectKind = kind;
        _pendingEffectOptions = options;
        ApplyPendingEffect();
    }

    public void SetThemeMode(ThemeMode themeMode)
    {
        _themeExplicitlySet = true;
        _requestedThemeMode = themeMode;
        ApplyPendingThemeMode();
    }

    public void ResetThemeMode()
    {
        _themeExplicitlySet = false;
        _requestedThemeMode = null;
        ApplyApplicationDefaults();
    }

    public void SetMica() => SetEffect(EffectKind.Mica);

    public void SetMicaAlt() => SetEffect(EffectKind.MicaAlt);

    public void SetAcrylic(uint blendColor = 0x80_00_00_00)
        => SetEffect(EffectKind.Acrylic, new EffectOptions { BlendColor = blendColor });

    public void SetAero() => SetEffect(EffectKind.Aero);

    public void SetBlur(int radius = 20)
        => SetEffect(EffectKind.Blur, new EffectOptions { BlurRadius = radius });

    public void ClearLuminaEffect()
    {
        _effectExplicitlySet = true;
        _pendingEffectKind = EffectKind.None;
        _pendingEffectOptions = null;
        if (Handle != 0)
        {
            LuminaWindow.Clear(Handle);
        }
    }

    protected virtual void OnCreated()
    {
    }

    protected virtual void OnLayout()
    {
    }

    protected virtual void OnClosed()
    {
    }

    protected virtual void OnSizeChanged()
    {
    }

    protected virtual void OnCommand(int controlId, int notificationCode, nint sourceHandle)
    {
    }

    protected bool TryGetClientSize(out int width, out int height)
    {
        if (Handle != 0 && Win32.GetClientRect(Handle, out var rect))
        {
            width = rect.Width;
            height = rect.Height;
            return true;
        }

        width = 0;
        height = 0;
        return false;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (Handle != 0)
        {
            _ = Win32.DestroyWindow(Handle);
        }

        if (_selfHandle.IsAllocated)
        {
            _selfHandle.Free();
        }

        Handle = 0;
        UiFontHandle = 0;
    }

    private void AddControl(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (control.Owner is not null)
        {
            throw new InvalidOperationException("The control already belongs to a form.");
        }

        control.Attach(this, ++_nextControlId);
        _controlList.Add(control);
        _controlsById[control.Id] = control;

        if (Handle != 0)
        {
            control.CreateHandle();
        }
    }

    private void ApplyApplicationDefaults()
    {
        var visualStyle = Application.GetResolvedVisualStyle();

        if (!_effectExplicitlySet)
        {
            _pendingEffectKind = visualStyle.EffectKind;
            _pendingEffectOptions = visualStyle.EffectOptions;
        }

        if (!_themeExplicitlySet)
        {
            _requestedThemeMode = visualStyle.ThemeMode;
        }

        ApplyPendingThemeMode();
    }

    private void ApplyPendingEffect()
    {
        if (Handle == 0)
        {
            return;
        }

        LuminaWindow.SetEffect(Handle, _pendingEffectKind, _pendingEffectOptions);
    }

    private void ApplyPendingThemeMode()
    {
        if (Handle == 0 || !OperatingSystem.IsWindows())
        {
            return;
        }

        ThemeMode resolvedThemeMode = Application.ResolveThemeMode(_requestedThemeMode ?? ThemeMode.System);
        int useDarkMode = resolvedThemeMode == ThemeMode.Dark ? 1 : 0;
        _ = Win32.DwmSetWindowAttribute(Handle, Win32.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf<int>());
    }

    private static void EnsureWindowClassRegistered(nint instanceHandle)
    {
        if (s_registered)
        {
            return;
        }

        var windowClass = new Win32.WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<Win32.WNDCLASSEXW>(),
            style = Win32.CS_HREDRAW | Win32.CS_VREDRAW,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(s_windowProc),
            hInstance = instanceHandle,
            hCursor = Win32.LoadCursorW(0, (nint)Win32.IDC_ARROW),
            hbrBackground = Win32.GetSysColorBrush(Win32.COLOR_WINDOW),
            lpszClassName = WindowClassName,
        };

        _ = Win32.RegisterClassExW(ref windowClass);
        s_registered = true;
    }

    private static nint WindowProcThunk(nint hwnd, uint message, nint wParam, nint lParam)
    {
        if (message == Win32.WM_NCCREATE)
        {
            var createStruct = Marshal.PtrToStructure<Win32.CREATESTRUCTW>(lParam);
            Win32.SetWindowLongPtrW(hwnd, Win32.GWLP_USERDATA, createStruct.lpCreateParams);
        }

        nint userData = Win32.GetWindowLongPtrW(hwnd, Win32.GWLP_USERDATA);
        if (userData != 0)
        {
            var self = (Form?)GCHandle.FromIntPtr(userData).Target;
            if (self is not null)
            {
                return self.WindowProc(hwnd, message, wParam, lParam);
            }
        }

        return Win32.DefWindowProcW(hwnd, message, wParam, lParam);
    }

    private nint WindowProc(nint hwnd, uint message, nint wParam, nint lParam)
    {
        switch (message)
        {
            case Win32.WM_SIZE:
                OnSizeChanged();
                OnLayout();
                return 0;

            case Win32.WM_COMMAND:
                HandleCommand(wParam, lParam);
                return 0;

            case Win32.WM_DESTROY:
                OnClosed();
                Win32.PostQuitMessage(0);
                return 0;

            case Win32.WM_NCDESTROY:
                Handle = 0;
                Win32.SetWindowLongPtrW(hwnd, Win32.GWLP_USERDATA, 0);
                if (_selfHandle.IsAllocated)
                {
                    _selfHandle.Free();
                }
                return 0;

            default:
                return Win32.DefWindowProcW(hwnd, message, wParam, lParam);
        }
    }

    private void HandleCommand(nint wParam, nint lParam)
    {
        int controlId = Win32.LowWord(wParam);
        int notificationCode = Win32.HighWord(wParam);

        if (_controlsById.TryGetValue(controlId, out var control))
        {
            _ = control.HandleCommand(notificationCode);
        }

        OnCommand(controlId, notificationCode, lParam);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public sealed class ControlCollection
    {
        private readonly Form _owner;

        internal ControlCollection(Form owner)
        {
            _owner = owner;
        }

        public void Add(Control control)
        {
            _owner.AddControl(control);
        }
    }
}
