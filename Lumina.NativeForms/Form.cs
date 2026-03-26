using System.Drawing;
using System.Runtime.InteropServices;

namespace Lumina.NativeForms;

/// <summary>
/// Represents a top-level NativeForms window backed by a native Win32 overlapped window.
/// </summary>
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

    /// <summary>
    /// Initializes a new top-level NativeForms window.
    /// </summary>
    public Form()
    {
        _controls = new ControlCollection(this);
    }

    /// <summary>
    /// Gets or sets the window title text.
    /// </summary>
    public string Text { get; set; } = "Lumina Native Form";

    /// <summary>
    /// Gets or sets how the form should scale itself and its child controls.
    /// </summary>
    public AutoScaleMode AutoScaleMode { get; set; }

    /// <summary>
    /// Gets or sets the design-time scaling dimensions used as the baseline for automatic scaling.
    /// </summary>
    public SizeF AutoScaleDimensions { get; set; }

    /// <summary>
    /// Gets the current runtime scaling dimensions for the configured <see cref="AutoScaleMode"/>.
    /// </summary>
    public SizeF CurrentAutoScaleDimensions => GetCurrentAutoScaleDimensions();

    /// <summary>
    /// Gets or sets the initial window width.
    /// </summary>
    public int Width { get; set; } = 960;

    /// <summary>
    /// Gets or sets the initial window height.
    /// </summary>
    public int Height { get; set; } = 640;

    /// <summary>
    /// Gets the native window handle after the form has been created.
    /// </summary>
    public nint Handle { get; private set; }

    internal nint InstanceHandle { get; private set; }

    internal nint UiFontHandle { get; private set; }

    /// <summary>
    /// Gets or sets the client size used when creating the form.
    /// </summary>
    public Size ClientSize
    {
        get => new(Width, Height);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    /// <summary>
    /// Gets the collection of child controls owned by the form.
    /// </summary>
    public ControlCollection Controls => _controls;

    /// <summary>
    /// Creates the native window, applies the current application defaults, and shows the form.
    /// </summary>
    public void Show()
    {
        ThrowIfDisposed();
        if (_shown)
        {
            return;
        }

        PerformAutoScale();

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

        foreach (var control in CollectionsMarshal.AsSpan(_controlList))
        {
            control.CreateHandle();
        }

        OnCreated();
        ApplyPendingEffect();
        OnLayout();

        _ = Win32.ShowWindow(Handle, Win32.SW_SHOW);
        _ = Win32.UpdateWindow(Handle);
    }

    /// <summary>
    /// Closes the form if its native window has already been created.
    /// </summary>
    public void Close()
    {
        if (Handle != 0)
        {
            _ = Win32.DestroyWindow(Handle);
        }
    }

    /// <summary>
    /// Applies a Lumina effect to the form and marks it as explicitly overridden from the application default.
    /// </summary>
    /// <param name="kind">The effect kind to apply.</param>
    /// <param name="options">Optional effect parameters.</param>
    public void SetEffect(EffectKind kind, EffectOptions? options = null)
    {
        _effectExplicitlySet = true;
        _pendingEffectKind = kind;
        _pendingEffectOptions = options;
        ApplyPendingEffect();
    }

    /// <summary>
    /// Overrides the form theme mode independently of the application default.
    /// </summary>
    /// <param name="themeMode">The theme mode to apply to the window.</param>
    public void SetThemeMode(ThemeMode themeMode)
    {
        _themeExplicitlySet = true;
        _requestedThemeMode = themeMode;
        ApplyPendingThemeMode();
    }

    /// <summary>
    /// Clears any form-level theme override and returns to the application default theme mode.
    /// </summary>
    public void ResetThemeMode()
    {
        _themeExplicitlySet = false;
        _requestedThemeMode = null;
        ApplyApplicationDefaults();
    }

    /// <summary>
    /// Applies the Mica backdrop to the form.
    /// </summary>
    public void SetMica() => SetEffect(EffectKind.Mica);

    /// <summary>
    /// Applies the Mica Alt backdrop to the form.
    /// </summary>
    public void SetMicaAlt() => SetEffect(EffectKind.MicaAlt);

    /// <summary>
    /// Applies the Acrylic backdrop to the form.
    /// </summary>
    /// <param name="blendColor">The ARGB blend color used by the Acrylic backdrop.</param>
    public void SetAcrylic(uint blendColor = 0x80_00_00_00)
        => SetEffect(EffectKind.Acrylic, new EffectOptions { BlendColor = blendColor });

    /// <summary>
    /// Applies the Aero-style backdrop to the form.
    /// </summary>
    public void SetAero() => SetEffect(EffectKind.Aero);

    /// <summary>
    /// Applies a blur effect to the form.
    /// </summary>
    /// <param name="radius">The blur radius to use.</param>
    public void SetBlur(int radius = 20)
        => SetEffect(EffectKind.Blur, new EffectOptions { BlurRadius = radius });

    /// <summary>
    /// Removes the currently applied Lumina effect from the form.
    /// </summary>
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

    /// <summary>
    /// Called after the native window and child controls have been created.
    /// </summary>
    protected virtual void OnCreated()
    {
    }

    /// <summary>
    /// Called when the form should update control layout.
    /// </summary>
    protected virtual void OnLayout()
    {
    }

    /// <summary>
    /// Called when the form is being closed in response to <c>WM_DESTROY</c>.
    /// </summary>
    protected virtual void OnClosed()
    {
    }

    /// <summary>
    /// Called when the form size changes.
    /// </summary>
    protected virtual void OnSizeChanged()
    {
    }

    /// <summary>
    /// Called after a child control command is dispatched.
    /// </summary>
    /// <param name="controlId">The numeric child control identifier.</param>
    /// <param name="notificationCode">The Win32 notification code.</param>
    /// <param name="sourceHandle">The originating child window handle.</param>
    protected virtual void OnCommand(int controlId, int notificationCode, nint sourceHandle)
    {
    }

    /// <summary>
    /// Tries to read the current client size from the native window.
    /// </summary>
    /// <param name="width">Receives the client width.</param>
    /// <param name="height">Receives the client height.</param>
    /// <returns><see langword="true"/> if the size was retrieved; otherwise, <see langword="false"/>.</returns>
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

    /// <summary>
    /// Applies automatic scaling to the form and all attached controls using the configured <see cref="AutoScaleMode"/>.
    /// </summary>
    public void PerformAutoScale()
    {
        AutoScaleMode effectiveMode = ResolveAutoScaleMode();
        if (effectiveMode == AutoScaleMode.None)
        {
            return;
        }

        SizeF currentDimensions = GetAutoScaleDimensions(effectiveMode);
        if (currentDimensions.Width <= 0 || currentDimensions.Height <= 0)
        {
            return;
        }

        if (AutoScaleDimensions.Width <= 0 || AutoScaleDimensions.Height <= 0)
        {
            AutoScaleDimensions = currentDimensions;
            return;
        }

        float scaleX = currentDimensions.Width / AutoScaleDimensions.Width;
        float scaleY = currentDimensions.Height / AutoScaleDimensions.Height;

        if (IsApproximatelyOne(scaleX) && IsApproximatelyOne(scaleY))
        {
            AutoScaleDimensions = currentDimensions;
            return;
        }

        Width = ScaleSize(Width, scaleX);
        Height = ScaleSize(Height, scaleY);

        foreach (var control in CollectionsMarshal.AsSpan(_controlList))
        {
            Rectangle bounds = control.Bounds;
            control.SetBounds(
                ScaleCoordinate(bounds.X, scaleX),
                ScaleCoordinate(bounds.Y, scaleY),
                ScaleSize(bounds.Width, scaleX),
                ScaleSize(bounds.Height, scaleY));
        }

        AutoScaleDimensions = currentDimensions;
    }

    /// <summary>
    /// Releases the native window and any unmanaged resources owned by the form.
    /// </summary>
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
        _ = Win32.DwmSetWindowAttribute(Handle, Win32.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
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
            var createStruct = ReadCreateStruct(lParam);
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

    private SizeF GetCurrentAutoScaleDimensions()
        => GetAutoScaleDimensions(ResolveAutoScaleMode());

    private AutoScaleMode ResolveAutoScaleMode()
    {
        return AutoScaleMode == AutoScaleMode.Inherit
            ? AutoScaleMode.Font
            : AutoScaleMode;
    }

    private static SizeF GetAutoScaleDimensions(AutoScaleMode autoScaleMode)
    {
        return autoScaleMode switch
        {
            AutoScaleMode.Dpi => Win32.GetSystemDpiScaleDimensions(),
            AutoScaleMode.Font => Win32.GetDefaultFontScaleDimensions(),
            _ => SizeF.Empty,
        };
    }

    private static bool IsApproximatelyOne(float value)
        => Math.Abs(value - 1f) < 0.001f;

    private static int ScaleCoordinate(int value, float factor)
        => (int)Math.Round(value * factor, MidpointRounding.AwayFromZero);

    private static int ScaleSize(int value, float factor)
        => Math.Max(1, (int)Math.Round(value * factor, MidpointRounding.AwayFromZero));

    private static unsafe Win32.CREATESTRUCTW ReadCreateStruct(nint lParam)
    {
        return MemoryMarshal.Read<Win32.CREATESTRUCTW>(new ReadOnlySpan<byte>((void*)lParam, sizeof(Win32.CREATESTRUCTW)));
    }

    /// <summary>
    /// Represents the child control collection of a NativeForms form.
    /// </summary>
    public sealed class ControlCollection
    {
        private readonly Form _owner;

        internal ControlCollection(Form owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Adds a control to the form.
        /// </summary>
        /// <param name="control">The control to add.</param>
        public void Add(Control control)
        {
            _owner.AddControl(control);
        }
    }
}
