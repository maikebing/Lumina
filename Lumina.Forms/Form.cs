using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
namespace Lumina.Forms;

/// <summary>
/// Represents a top-level LuminaForms window backed by a native Win32 overlapped window.
/// </summary>
public class Form : IDisposable
{
    private const string WindowClassName = "LuminaFormsWindow";
    private static bool s_registered;

    private readonly List<Control> _controlList = [];
    private readonly Dictionary<int, Control> _controlsById = [];
    private readonly ControlCollection _controls;
    private GCHandle _selfHandle;
    private bool _disposed;
    private bool _shown;
    private int _nextControlId = 1000;
    private bool _effectExplicitlySet;
    private bool _refreshingMainMenuStrip;
    private bool _themeExplicitlySet;
    private EffectKind _pendingEffectKind = EffectKind.None;
    private EffectOptions? _pendingEffectOptions;
    private MenuStrip? _mainMenuStrip;
    private ThemeMode? _requestedThemeMode;
    private NativeTheme? _themeOverride;
    private ThemePalette? _paletteOverride;

    /// <summary>
    /// Initializes a new top-level LuminaForms window.
    /// </summary>
    public Form()
    {
        _controls = new ControlCollection(this);
    }

    /// <summary>
    /// Occurs after the native form handle and attached child controls are created.
    /// </summary>
    public event EventHandler? Created;

    /// <summary>
    /// Occurs before the form is first shown.
    /// </summary>
    public event EventHandler? Load;

    /// <summary>
    /// Occurs after the form becomes visible for the first time.
    /// </summary>
    public event EventHandler? Shown;

    /// <summary>
    /// Occurs when the form is closed.
    /// </summary>
    public event EventHandler? Closed;

    /// <summary>
    /// Occurs when the form performs layout.
    /// </summary>
    public event EventHandler? Layout;

    /// <summary>
    /// Occurs when the form size changes.
    /// </summary>
    public event EventHandler? SizeChanged;

    /// <summary>
    /// Gets or sets the window title text.
    /// </summary>
    public string Text { get; set; } = "Lumina Native Form";

    /// <summary>
    /// Gets or sets the design-time or lookup name of the form.
    /// </summary>
    public string Name { get; set; } = string.Empty;

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
    /// Gets or sets the main menu strip associated with the form.
    /// </summary>
    public MenuStrip? MainMenuStrip
    {
        get => _mainMenuStrip;
        set
        {
            if (ReferenceEquals(_mainMenuStrip, value))
            {
                return;
            }

            MenuStrip? previous = _mainMenuStrip;
            _mainMenuStrip = value;
            RefreshMainMenuStrip(previous);
        }
    }

    /// <summary>
    /// Gets the visual style currently resolved for this form after application defaults and form-level overrides are applied.
    /// </summary>
    public ResolvedVisualStyle CurrentVisualStyle => ResolveCurrentVisualStyle();

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
              int err = Marshal.GetLastWin32Error();
              throw new InvalidOperationException($"Failed to create the native form window. Win32 error: {err}");
        }

        Handle = hwnd;
        Application.EnsureVisualStylesInitialized();
        ApplyApplicationDefaults();
        RefreshMainMenuStrip();

        foreach (var control in CollectionsMarshal.AsSpan(_controlList))
        {
            control.CreateHandleRecursive();
        }

        Application.RegisterOpenForm(this);
        OnCreated();
        OnLoad();
        PerformLayout();

        _ = Win32.ShowWindow(Handle, Win32.SW_SHOW);
        _ = Win32.UpdateWindow(Handle);
        OnShown();
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
    /// Suspends layout logic for compatibility with designer-generated code.
    /// </summary>
    public void SuspendLayout()
    {
    }

    /// <summary>
    /// Resumes layout logic for compatibility with designer-generated code.
    /// </summary>
    /// <param name="performLayout">Whether layout should be performed immediately.</param>
    public void ResumeLayout(bool performLayout)
    {
        if (performLayout)
        {
            PerformLayout();
        }
    }

    /// <summary>
    /// Performs layout for compatibility with designer-generated code.
    /// </summary>
    public void PerformLayout()
    {
        foreach (var control in CollectionsMarshal.AsSpan(_controlList))
        {
            control.PerformLayout();
        }

        OnLayout();
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
    /// Applies a form-level theme override that can supply theme mode, visual style, effect defaults, and palette tokens.
    /// </summary>
    /// <param name="theme">The theme to use for this form.</param>
    public void UseTheme(NativeTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        _themeOverride = theme;
        ApplyApplicationDefaults();
    }

    /// <summary>
    /// Clears the form-level theme override and returns to the application defaults.
    /// </summary>
    public void ResetTheme()
    {
        _themeOverride = null;
        ApplyApplicationDefaults();
    }

    /// <summary>
    /// Applies a form-level semantic palette override without requiring a full theme object.
    /// </summary>
    /// <param name="palette">The palette to use for this form.</param>
    public void SetPalette(ThemePalette palette)
    {
        ArgumentNullException.ThrowIfNull(palette);
        _paletteOverride = palette;
        ApplyApplicationDefaults();
    }

    /// <summary>
    /// Clears the form-level palette override and returns to the application default palette.
    /// </summary>
    public void ResetPalette()
    {
        _paletteOverride = null;
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
        Created?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called before the form is shown for the first time.
    /// </summary>
    protected virtual void OnLoad()
    {
        Load?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called after the form is shown for the first time.
    /// </summary>
    protected virtual void OnShown()
    {
        Shown?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the form should update control layout.
    /// </summary>
    protected virtual void OnLayout()
    {
        Layout?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the form is being closed in response to <c>WM_DESTROY</c>.
    /// </summary>
    protected virtual void OnClosed()
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the form size changes.
    /// </summary>
    protected virtual void OnSizeChanged()
    {
        SizeChanged?.Invoke(this, EventArgs.Empty);
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
            ScaleControlTree(control, scaleX, scaleY);
        }

        AutoScaleDimensions = currentDimensions;
    }

    /// <summary>
    /// Releases the native window and any unmanaged resources owned by the form.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources used by the form.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release both managed and unmanaged resources;
    /// <see langword="false"/> to release unmanaged resources only.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        ReleaseMainMenuStrip();

        if (Handle != 0)
        {
            _ = Win32.DestroyWindow(Handle);
        }

        if (disposing)
        {
            ReleaseControlHandles();
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
        AttachControl(control, parent: null);
        _controlList.Add(control);

        if (Handle != 0)
        {
            control.CreateHandleRecursive();
        }
    }

    internal void AttachControl(Control control, Control? parent)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (control.Owner is not null)
        {
            throw new InvalidOperationException("The control already belongs to a form.");
        }

        control.Attach(this, ++_nextControlId, parent);
        _controlsById[control.Id] = control;

        if (control is ContainerControlBase container)
        {
            container.AttachChildrenToOwner(this);
        }
    }

    internal void RefreshMainMenuStrip()
    {
        RefreshMainMenuStrip(previous: null);
    }

    private void RefreshMainMenuStrip(MenuStrip? previous)
    {
        if (_refreshingMainMenuStrip)
        {
            return;
        }

        _refreshingMainMenuStrip = true;
        if (!OperatingSystem.IsWindows() || Handle == 0)
        {
            try
            {
                previous?.ReleaseNativeMenu();
            }
            finally
            {
                _refreshingMainMenuStrip = false;
            }

            return;
        }

        try
        {
            _ = Win32.SetMenu(Handle, 0);
            previous?.ReleaseNativeMenu();

            if (_mainMenuStrip is not null)
            {
                _mainMenuStrip.SynchronizeNativeMenu();
                _ = Win32.SetMenu(Handle, _mainMenuStrip.GetNativeMenuHandle());
            }

            _ = Win32.DrawMenuBar(Handle);
        }
        finally
        {
            _refreshingMainMenuStrip = false;
        }
    }

    private bool UsesNativeMainMenuBar()
        => OperatingSystem.IsWindows()
            && Handle != 0
            && _mainMenuStrip is not null;

    private void ReleaseMainMenuStrip()
    {
        if (Handle != 0 && OperatingSystem.IsWindows())
        {
            _ = Win32.SetMenu(Handle, 0);
            _ = Win32.DrawMenuBar(Handle);
        }

        _mainMenuStrip?.ReleaseNativeMenu();
    }

    private void ApplyApplicationDefaults()
    {
        ResolvedVisualStyle visualStyle = ResolveCurrentVisualStyle();

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
        ApplyPendingEffect();
    }

    internal void RefreshVisualStyles()
    {
        ApplyApplicationDefaults();
    }

    private ResolvedVisualStyle ResolveCurrentVisualStyle()
    {
        ThemeMode requestedThemeMode = _themeExplicitlySet
            ? _requestedThemeMode ?? ThemeMode.System
            : Application.VisualStyleSettings.ThemeMode;

        EffectKind? preferredEffect = _effectExplicitlySet
            ? _pendingEffectKind
            : Application.VisualStyleSettings.PreferredEffect;

        EffectOptions? preferredEffectOptions = _effectExplicitlySet
            ? _pendingEffectOptions
            : Application.VisualStyleSettings.PreferredEffectOptions;

        NativeTheme? theme = _themeOverride ?? Application.VisualStyleSettings.Theme;
        ThemePalette? palette = _paletteOverride ?? Application.VisualStyleSettings.Palette;

        return Application.ResolveVisualStyle(
            requestedThemeMode,
            Application.VisualStyleSettings.PreferredVisualStyle,
            Application.VisualStyleSettings.ApplyBackdropEffects,
            preferredEffect,
            preferredEffectOptions,
            theme,
            palette);
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

    private static unsafe void EnsureWindowClassRegistered(nint instanceHandle)
    {
        if (s_registered)
        {
            return;
        }

        var windowClass = new Win32.WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<Win32.WNDCLASSEXW>(),
            style = Win32.CS_HREDRAW | Win32.CS_VREDRAW,
                lpfnWndProc = (nint)(delegate* unmanaged[Stdcall]<nint, uint, nint, nint, nint>)&WindowProcThunk,
            hInstance = instanceHandle,
            hCursor = Win32.LoadCursorW(0, (nint)Win32.IDC_ARROW),
            hbrBackground = Win32.GetSysColorBrush(Win32.COLOR_BTNFACE),
            lpszClassName = WindowClassName,
        };

            ushort atom = Win32.RegisterClassExW(ref windowClass);
            int regError = Marshal.GetLastWin32Error();
            if (atom == 0 && regError != 1410 /* ERROR_CLASS_ALREADY_EXISTS */)
            {
                throw new InvalidOperationException($"Failed to register the native window class. Win32 error: {regError}");
            }
            s_registered = true;
    }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
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
                PerformLayout();
                return 0;

            case Win32.WM_KEYDOWN:
            case Win32.WM_SYSKEYDOWN:
                if (TryHandleMenuShortcut(BuildShortcutKeyData(wParam)))
                {
                    return 0;
                }

                if (!UsesNativeMainMenuBar() && (int)(nuint)wParam == Win32.VK_F10 && TryActivateFirstMenuItem())
                {
                    return 0;
                }

                break;

            case Win32.WM_SYSCHAR:
                if (!UsesNativeMainMenuBar() && TryHandleMenuMnemonic((char)(ushort)(nuint)wParam))
                {
                    return 0;
                }

                break;

            case Win32.WM_SETTINGCHANGE:
            case Win32.WM_THEMECHANGED:
                RefreshVisualStyles();
                break;

            case Win32.WM_INITMENUPOPUP:
                ToolStripPopupMenu.NotifyMenuDepthChange(+1);
                break;

            case Win32.WM_UNINITMENUPOPUP:
                ToolStripPopupMenu.NotifyMenuDepthChange(-1);
                break;

            case Win32.WM_MENUSELECT:
            {
                int flags = Win32.HighWord(wParam);
                ToolStripPopupMenu.NotifyMenuSelectionChanged((flags & (int)Win32.MF_POPUP) != 0);
                break;
            }

            case Win32.WM_NOTIFY:
                if (HandleNotify(lParam))
                {
                    return 0;
                }

                return 0;

            case Win32.WM_COMMAND:
                if (HandleCommand(wParam, lParam))
                {
                    return 0;
                }

                return 0;

            case Win32.WM_DESTROY:
                OnClosed();
                return 0;

            case Win32.WM_NCDESTROY:
                ReleaseMainMenuStrip();
                ReleaseControlHandles();
                Handle = 0;
                Win32.SetWindowLongPtrW(hwnd, Win32.GWLP_USERDATA, 0);
                if (Application.UnregisterOpenForm(this))
                {
                    Win32.PostQuitMessage(0);
                }

                if (_selfHandle.IsAllocated)
                {
                    _selfHandle.Free();
                }

                return 0;

            default:
                return Win32.DefWindowProcW(hwnd, message, wParam, lParam);
        }

        return Win32.DefWindowProcW(hwnd, message, wParam, lParam);
    }

    private bool HandleCommand(nint wParam, nint lParam)
    {
        int controlId = Win32.LowWord(wParam);
        int notificationCode = Win32.HighWord(wParam);

        if (_mainMenuStrip is not null && _mainMenuStrip.TryHandleNativeCommand(controlId))
        {
            return true;
        }

        if (_controlsById.TryGetValue(controlId, out var control))
        {
            _ = control.HandleCommand(notificationCode);
        }

        OnCommand(controlId, notificationCode, lParam);
        return true;
    }

    private bool HandleNotify(nint lParam)
    {
        if (lParam == 0)
        {
            return false;
        }

        Win32.NMHDR header = Marshal.PtrToStructure<Win32.NMHDR>(lParam);
        int controlId = unchecked((int)header.idFrom);
        int notificationCode = unchecked((int)header.code);

        return _controlsById.TryGetValue(controlId, out var control)
            && control.HandleNotify(notificationCode, lParam);
    }

    private bool TryHandleMenuShortcut(Keys keyData)
    {
        if (MainMenuStrip is null || keyData == Keys.None)
        {
            return false;
        }

        return TryHandleMenuShortcut(MainMenuStrip.Items, keyData);
    }

    private static bool TryHandleMenuShortcut(IEnumerable<ToolStripItem> items, Keys keyData)
    {
        foreach (ToolStripItem item in items)
        {
            if (!item.Visible || !item.Enabled)
            {
                continue;
            }

            if (item is ToolStripMenuItem menuItem && menuItem.MatchesShortcut(keyData))
            {
                menuItem.PerformClick();
                return true;
            }

            if (item is ToolStripDropDownItem dropDownItem && TryHandleMenuShortcut(dropDownItem.DropDownItems, keyData))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryHandleMenuMnemonic(char mnemonic)
    {
        if (MainMenuStrip is null || char.IsControl(mnemonic))
        {
            return false;
        }

        return MainMenuStrip.TryActivateMnemonic(mnemonic);
    }

    private bool TryActivateFirstMenuItem()
    {
        return MainMenuStrip is not null && MainMenuStrip.TryActivateFirstItem();
    }

    private static Keys BuildShortcutKeyData(nint virtualKey)
    {
        Keys keyData = (Keys)(unchecked((int)(nuint)virtualKey) & (int)Keys.KeyCode);

        if ((Win32.GetKeyState(Win32.VK_CONTROL) & 0x8000) != 0)
        {
            keyData |= Keys.Control;
        }

        if ((Win32.GetKeyState(Win32.VK_SHIFT) & 0x8000) != 0)
        {
            keyData |= Keys.Shift;
        }

        if ((Win32.GetKeyState(Win32.VK_MENU) & 0x8000) != 0)
        {
            keyData |= Keys.Alt;
        }

        return keyData;
    }

    private void ReleaseControlHandles()
    {
        foreach (Control control in CollectionsMarshal.AsSpan(_controlList))
        {
            control.ReleaseHandleRecursive();
        }
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

    private static void ScaleControlTree(Control control, float scaleX, float scaleY)
    {
        Rectangle bounds = control.Bounds;
        control.SetBounds(
            ScaleCoordinate(bounds.X, scaleX),
            ScaleCoordinate(bounds.Y, scaleY),
            ScaleSize(bounds.Width, scaleX),
            ScaleSize(bounds.Height, scaleY));

        if (control is ContainerControlBase container)
        {
            foreach (Control child in container.ChildControls)
            {
                ScaleControlTree(child, scaleX, scaleY);
            }
        }
    }

    private static unsafe Win32.CREATESTRUCTW ReadCreateStruct(nint lParam)
    {
        return MemoryMarshal.Read<Win32.CREATESTRUCTW>(new ReadOnlySpan<byte>((void*)lParam, sizeof(Win32.CREATESTRUCTW)));
    }

    /// <summary>
    /// Represents the child control collection of a LuminaForms form.
    /// </summary>
    public sealed class ControlCollection : IEnumerable<Control>
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

        /// <summary>
        /// Adds a batch of controls to the form.
        /// </summary>
        /// <param name="controls">The controls to add.</param>
        public void AddRange(IEnumerable<Control> controls)
        {
            ArgumentNullException.ThrowIfNull(controls);

            foreach (Control control in controls)
            {
                _owner.AddControl(control);
            }
        }

        /// <summary>
        /// Adds a batch of controls to the form.
        /// </summary>
        /// <param name="controls">The controls to add.</param>
        public void AddRange(params Control[] controls)
        {
            AddRange((IEnumerable<Control>)controls);
        }

        /// <summary>
        /// Gets the number of controls currently attached to the form.
        /// </summary>
        public int Count => _owner._controlList.Count;

        /// <summary>
        /// Gets the control at the specified index.
        /// </summary>
        /// <param name="index">The zero-based control index.</param>
        /// <returns>The control at the requested index.</returns>
        public Control this[int index] => _owner._controlList[index];

        /// <summary>
        /// Determines whether the specified control is already attached to the form.
        /// </summary>
        /// <param name="control">The control to locate.</param>
        /// <returns><see langword="true"/> if the control is attached; otherwise, <see langword="false"/>.</returns>
        public bool Contains(Control control)
        {
            return _owner._controlList.Contains(control);
        }

        /// <summary>
        /// Finds controls by <see cref="Control.Name"/> using case-insensitive matching.
        /// </summary>
        /// <param name="key">The control name to locate.</param>
        /// <param name="searchAllChildren"><see langword="true"/> to search nested containers recursively; otherwise, <see langword="false"/>.</param>
        /// <returns>The matching controls.</returns>
        public Control[] Find(string key, bool searchAllChildren)
        {
            ArgumentNullException.ThrowIfNull(key);

            List<Control> matches = [];
            CollectMatches(_owner._controlList, key, searchAllChildren, matches);
            return [.. matches];
        }

        /// <inheritdoc />
        public IEnumerator<Control> GetEnumerator()
        {
            return _owner._controlList.GetEnumerator();
        }

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static void CollectMatches(IEnumerable<Control> controls, string key, bool searchAllChildren, List<Control> matches)
        {
            foreach (Control control in controls)
            {
                if (string.Equals(control.Name, key, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(control);
                }

                if (searchAllChildren && control is ContainerControlBase container)
                {
                    CollectMatches(container.Controls, key, searchAllChildren, matches);
                }
            }
        }
    }
}
