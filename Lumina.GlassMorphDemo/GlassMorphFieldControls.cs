using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Lumina.GlassMorphDemo;

[SupportedOSPlatform("windows6.1")]
internal abstract class GlassMorphShellPanel : Panel
{
    private const uint AnimationTimerId = 1;
    private bool _hovered;
    private bool _active;
    private bool _timerRunning;
    private float _hoverProgress;
    private float _activeProgress;

    public Color AccentColor { get; set; } = Color.FromArgb(88, 206, 255);

    protected float HoverProgress => _hoverProgress;

    protected float ActiveProgress => _activeProgress;

    protected float SurfaceProgress => Math.Max(_hoverProgress, _activeProgress);

    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        switch (message)
        {
            case NativeMethods.WM_ERASEBKGND:
                result = 1;
                return true;

            case NativeMethods.WM_PAINT:
                PaintShell();
                result = 0;
                return true;

            case NativeMethods.WM_MOUSEMOVE:
                if (!_hovered)
                {
                    _hovered = true;
                    StartAnimationTimer();
                    TrackMouseLeave();
                    Invalidate();
                }

                result = 0;
                return true;

            case NativeMethods.WM_MOUSELEAVE:
                _hovered = false;
                StartAnimationTimer();
                Invalidate();
                result = 0;
                return true;

            case NativeMethods.WM_TIMER:
                if ((nuint)wParam == AnimationTimerId)
                {
                    AdvanceAnimation();
                    result = 0;
                    return true;
                }

                break;
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }

    protected override void OnDisposing()
    {
        StopAnimationTimer();
        base.OnDisposing();
    }

    protected void SetSurfaceActive(bool active)
    {
        if (_active == active)
        {
            return;
        }

        _active = active;
        StartAnimationTimer();
        OnAnimationProgressChanged();
        Invalidate();
    }

    protected virtual void PaintForeground(Graphics graphics, Rectangle shellBounds)
    {
    }

    protected virtual void OnAnimationProgressChanged()
    {
    }

    private void AdvanceAnimation()
    {
        _hoverProgress += ((_hovered ? 1f : 0f) - _hoverProgress) * (_hovered ? 0.20f : 0.28f);
        _activeProgress += ((_active ? 1f : 0f) - _activeProgress) * (_active ? 0.22f : 0.26f);

        if (Math.Abs((_hovered ? 1f : 0f) - _hoverProgress) < 0.015f)
        {
            _hoverProgress = _hovered ? 1f : 0f;
        }

        if (Math.Abs((_active ? 1f : 0f) - _activeProgress) < 0.015f)
        {
            _activeProgress = _active ? 1f : 0f;
        }

        if (!_hovered && !_active && _hoverProgress <= 0.01f && _activeProgress <= 0.01f)
        {
            StopAnimationTimer();
        }

        OnAnimationProgressChanged();
        Invalidate();
    }

    private void PaintShell()
    {
        if (Handle == 0)
        {
            return;
        }

        nint hdc = NativeMethods.BeginPaint(Handle, out NativeMethods.PAINTSTRUCT paintStruct);
        if (hdc == 0)
        {
            return;
        }

        try
        {
            if (!NativeMethods.GetClientRect(Handle, out NativeMethods.RECT clientRect))
            {
                return;
            }

            Rectangle clientBounds = Rectangle.FromLTRB(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom);
            Rectangle shellBounds = Rectangle.Inflate(clientBounds, -1, -1);

            using var surface = new Bitmap(Math.Max(1, clientBounds.Width), Math.Max(1, clientBounds.Height));
            using var graphics = Graphics.FromImage(surface);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var backgroundBrush = new SolidBrush(ResolveParentColor()))
            {
                graphics.FillRectangle(backgroundBrush, clientBounds);
            }

            Rectangle glowBounds = new(
                shellBounds.Left + shellBounds.Width / 4,
                shellBounds.Bottom - 10,
                Math.Max(24, shellBounds.Width / 2),
                (int)Math.Round(Lerp(10f, 22f, Math.Max(_hoverProgress, _activeProgress))));
            using (var glowPath = CreateEllipsePath(glowBounds))
            using (var glowBrush = new PathGradientBrush(glowPath))
            {
                glowBrush.CenterColor = Color.FromArgb((int)Lerp(54f, 160f, Math.Max(_hoverProgress, _activeProgress)), AccentColor);
                glowBrush.SurroundColors = [Color.FromArgb(0, AccentColor)];
                graphics.FillPath(glowBrush, glowPath);
            }

            using GraphicsPath shellPath = CreateRoundedRectanglePath(shellBounds, Math.Max(12, shellBounds.Height / 2));
            DrawBaseSurface(graphics, shellBounds, shellPath);
            PaintForeground(graphics, shellBounds);

            using var targetGraphics = Graphics.FromHdc(hdc);
            targetGraphics.DrawImageUnscaled(surface, 0, 0);
        }
        finally
        {
            _ = NativeMethods.EndPaint(Handle, ref paintStruct);
        }
    }

    private void DrawBaseSurface(Graphics graphics, Rectangle shellBounds, GraphicsPath shellPath)
    {
        Color top = Blend(Color.FromArgb(114, 78, 145), AccentColor, _activeProgress * 0.26f);
        Color bottom = Blend(Color.FromArgb(74, 42, 108), AccentColor, Math.Max(_hoverProgress, _activeProgress) * 0.18f);

        using (var fillBrush = new LinearGradientBrush(
            shellBounds,
            Color.FromArgb((int)Lerp(148f, 172f, Math.Max(_hoverProgress, _activeProgress)), top),
            Color.FromArgb((int)Lerp(132f, 154f, Math.Max(_hoverProgress, _activeProgress)), bottom),
            LinearGradientMode.Vertical))
        {
            graphics.FillPath(fillBrush, shellPath);
        }

        if (Math.Max(_hoverProgress, _activeProgress) > 0.02f)
        {
            Rectangle centerBounds = new(
                shellBounds.Left + (shellBounds.Width / 2) - (int)Lerp(14f, shellBounds.Width / 2f - 6f, Math.Max(_hoverProgress, _activeProgress)),
                shellBounds.Top + 4,
                (int)Lerp(28f, shellBounds.Width - 12f, Math.Max(_hoverProgress, _activeProgress)),
                Math.Max(18, shellBounds.Height - 8));
            int centerRadius = Math.Max(3, Math.Min(6, centerBounds.Height / 6));
            GraphicsState state = graphics.Save();
            using var clipRegion = new Region(shellPath);
            graphics.SetClip(clipRegion, CombineMode.Intersect);
            using var centerPath = CreateRoundedRectanglePath(centerBounds, centerRadius);
            using var centerBrush = new PathGradientBrush(centerPath)
            {
                CenterColor = Color.FromArgb((int)Lerp(24f, 120f, Math.Max(_hoverProgress, _activeProgress)), Blend(Color.White, AccentColor, 0.52f)),
                SurroundColors = [Color.FromArgb(0, AccentColor)],
            };
            graphics.FillPath(centerBrush, centerPath);
            graphics.Restore(state);
        }

        using var borderPen = new Pen(Color.FromArgb((int)Lerp(136f, 186f, Math.Max(_hoverProgress, _activeProgress)), Blend(Color.White, AccentColor, 0.16f)));
        using var innerPen = new Pen(Color.FromArgb((int)Lerp(62f, 92f, Math.Max(_hoverProgress, _activeProgress)), Color.White));
        graphics.DrawPath(borderPen, shellPath);

        Rectangle innerBounds = Rectangle.Inflate(shellBounds, -2, -2);
        using GraphicsPath innerPath = CreateRoundedRectanglePath(innerBounds, Math.Max(10, innerBounds.Height / 2));
        graphics.DrawPath(innerPen, innerPath);
    }

    private void TrackMouseLeave()
    {
        if (Handle == 0)
        {
            return;
        }

        var track = new NativeMethods.TRACKMOUSEEVENT
        {
            cbSize = (uint)Marshal.SizeOf<NativeMethods.TRACKMOUSEEVENT>(),
            dwFlags = NativeMethods.TME_LEAVE,
            hwndTrack = Handle,
        };

        _ = NativeMethods.TrackMouseEvent(ref track);
    }

    private void StartAnimationTimer()
    {
        if (_timerRunning || Handle == 0)
        {
            return;
        }

        _ = NativeMethods.SetTimer(Handle, AnimationTimerId, 16, 0);
        _timerRunning = true;
    }

    private void StopAnimationTimer()
    {
        if (!_timerRunning || Handle == 0)
        {
            _timerRunning = false;
            return;
        }

        _ = NativeMethods.KillTimer(Handle, AnimationTimerId);
        _timerRunning = false;
    }

    private Color ResolveParentColor()
        => Parent is not null && !Parent.BackColor.IsEmpty
            ? Parent.BackColor
            : Color.FromArgb(69, 24, 95);

    internal static GraphicsPath CreateEllipsePath(Rectangle bounds)
    {
        var path = new GraphicsPath();
        path.AddEllipse(bounds);
        return path;
    }

    internal static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
    {
        int diameter = Math.Max(1, radius * 2);
        var path = new GraphicsPath();

        if (bounds.Width <= diameter || bounds.Height <= diameter)
        {
            path.AddRectangle(bounds);
            path.CloseFigure();
            return path;
        }

        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    internal static float Lerp(float from, float to, float amount)
        => from + ((to - from) * Math.Clamp(amount, 0f, 1f));

    internal static Color Blend(Color from, Color to, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return Color.FromArgb(
            (int)Math.Round(from.A + ((to.A - from.A) * amount)),
            (int)Math.Round(from.R + ((to.R - from.R) * amount)),
            (int)Math.Round(from.G + ((to.G - from.G) * amount)),
            (int)Math.Round(from.B + ((to.B - from.B) * amount)));
    }
}

[SupportedOSPlatform("windows6.1")]
internal sealed class FocusAwareTextBox : TextBox
{
    public event EventHandler? FocusStateChanged;

    public bool IsFocused { get; private set; }

    protected override uint ExStyle => 0;

    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        if (message == NativeMethods.WM_SETFOCUS)
        {
            IsFocused = true;
            FocusStateChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (message == NativeMethods.WM_KILLFOCUS)
        {
            IsFocused = false;
            FocusStateChanged?.Invoke(this, EventArgs.Empty);
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }
}

[SupportedOSPlatform("windows6.1")]
internal sealed class FocusAwareComboBox : ComboBox
{
    public event EventHandler? FocusStateChanged;

    public bool IsFocused { get; private set; }

    protected override uint ExStyle => 0;

    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        if (message == NativeMethods.WM_SETFOCUS)
        {
            IsFocused = true;
            FocusStateChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (message == NativeMethods.WM_KILLFOCUS)
        {
            IsFocused = false;
            FocusStateChanged?.Invoke(this, EventArgs.Empty);
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }
}

[SupportedOSPlatform("windows6.1")]
internal sealed class GlassMorphTextBox : GlassMorphShellPanel
{
    private readonly FocusAwareTextBox _editor;

    public GlassMorphTextBox()
    {
        _editor = new FocusAwareTextBox
        {
            BackColor = Color.FromArgb(88, 52, 120),
            ForeColor = Color.FromArgb(244, 247, 255),
            Text = "Search glass morph",
        };

        _editor.FocusStateChanged += (_, _) => SetSurfaceActive(_editor.IsFocused);
        Controls.Add(_editor);
        Size = new Size(260, 42);
    }

    public FocusAwareTextBox Editor => _editor;

    public override void PerformLayout()
    {
        _editor.SetBounds(14, 10, Math.Max(60, Width - 28), Math.Max(20, Height - 20));
        base.PerformLayout();
    }
}

[SupportedOSPlatform("windows6.1")]
internal sealed class GlassMorphComboBox : GlassMorphShellPanel
{
    private readonly FocusAwareComboBox _comboBox;

    public GlassMorphComboBox()
    {
        _comboBox = new FocusAwareComboBox
        {
            BackColor = Color.FromArgb(88, 52, 120),
            ForeColor = Color.FromArgb(244, 247, 255),
        };

        _comboBox.FocusStateChanged += (_, _) => SetSurfaceActive(_comboBox.IsFocused);
        Controls.Add(_comboBox);
        Size = new Size(260, 42);
    }

    public FocusAwareComboBox ComboBox => _comboBox;

    public override void PerformLayout()
    {
        _comboBox.SetBounds(14, 8, Math.Max(60, Width - 28), Height);
        base.PerformLayout();
    }
}

[SupportedOSPlatform("windows6.1")]
internal abstract class GlassMorphToggleBase : Label
{
    private const uint AnimationTimerId = 1;
    private bool _hovered;
    private bool _timerRunning;
    private float _hoverProgress;
    private float _checkedProgress;
    private bool _checked;

    public event EventHandler? CheckedChanged;

    public Color AccentColor { get; set; } = Color.FromArgb(255, 64, 148);

    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked == value)
            {
                return;
            }

            _checked = value;
            StartAnimationTimer();
            Invalidate();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    protected override uint Style => base.Style | NativeMethods.SS_NOTIFY;

    protected abstract bool CircularGlyph { get; }

    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        Invalidate();
    }

    protected override void OnDisposing()
    {
        StopAnimationTimer();
        base.OnDisposing();
    }

    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        switch (message)
        {
            case NativeMethods.WM_ERASEBKGND:
                result = 1;
                return true;

            case NativeMethods.WM_PAINT:
                PaintToggle();
                result = 0;
                return true;

            case NativeMethods.WM_MOUSEMOVE:
                if (!_hovered)
                {
                    _hovered = true;
                    StartAnimationTimer();
                    TrackMouseLeave();
                    Invalidate();
                }

                result = 0;
                return true;

            case NativeMethods.WM_MOUSELEAVE:
                _hovered = false;
                StartAnimationTimer();
                Invalidate();
                result = 0;
                return true;

            case NativeMethods.WM_LBUTTONUP:
                OnToggleClicked();
                result = 0;
                return true;

            case NativeMethods.WM_TIMER:
                if ((nuint)wParam == AnimationTimerId)
                {
                    AdvanceAnimation();
                    result = 0;
                    return true;
                }

                break;
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }

    protected virtual void OnToggleClicked()
    {
        Checked = !Checked;
    }

    private void AdvanceAnimation()
    {
        _hoverProgress += ((_hovered ? 1f : 0f) - _hoverProgress) * (_hovered ? 0.2f : 0.28f);
        _checkedProgress += ((_checked ? 1f : 0f) - _checkedProgress) * (_checked ? 0.22f : 0.28f);

        if (!_hovered && !_checked && _hoverProgress <= 0.01f && _checkedProgress <= 0.01f)
        {
            StopAnimationTimer();
        }

        Invalidate();
    }

    private void PaintToggle()
    {
        if (Handle == 0)
        {
            return;
        }

        nint hdc = NativeMethods.BeginPaint(Handle, out NativeMethods.PAINTSTRUCT paintStruct);
        if (hdc == 0)
        {
            return;
        }

        try
        {
            if (!NativeMethods.GetClientRect(Handle, out NativeMethods.RECT clientRect))
            {
                return;
            }

            Rectangle clientBounds = Rectangle.FromLTRB(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom);
            using var surface = new Bitmap(Math.Max(1, clientBounds.Width), Math.Max(1, clientBounds.Height));
            using var graphics = Graphics.FromImage(surface);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var backgroundBrush = new SolidBrush(Parent is not null && !Parent.BackColor.IsEmpty ? Parent.BackColor : Color.FromArgb(76, 24, 104)))
            {
                graphics.FillRectangle(backgroundBrush, clientBounds);
            }

            Rectangle glyphBounds = new(2, Math.Max(0, (clientBounds.Height - 20) / 2), 20, 20);
            int radius = CircularGlyph ? 10 : 5;
            using GraphicsPath glyphPath = GlassMorphShellPanel.CreateRoundedRectanglePath(glyphBounds, radius);
            using var glyphFill = new SolidBrush(Color.FromArgb((int)GlassMorphShellPanel.Lerp(120f, 168f, _hoverProgress + (_checkedProgress * 0.3f)), Color.FromArgb(88, 52, 120)));
            graphics.FillPath(glyphFill, glyphPath);

            if (_checkedProgress > 0.02f)
            {
                Rectangle centerBounds = new(
                    glyphBounds.Left + ((glyphBounds.Width - (int)GlassMorphShellPanel.Lerp(8f, 14f, _checkedProgress)) / 2),
                    glyphBounds.Top + ((glyphBounds.Height - (int)GlassMorphShellPanel.Lerp(8f, 14f, _checkedProgress)) / 2),
                    (int)GlassMorphShellPanel.Lerp(8f, 14f, _checkedProgress),
                    (int)GlassMorphShellPanel.Lerp(8f, 14f, _checkedProgress));
                int centerRadius = CircularGlyph ? centerBounds.Width / 2 : 3;
                using GraphicsPath centerPath = GlassMorphShellPanel.CreateRoundedRectanglePath(centerBounds, centerRadius);
                using var centerBrush = new PathGradientBrush(centerPath)
                {
                    CenterColor = Color.FromArgb((int)GlassMorphShellPanel.Lerp(40f, 210f, _checkedProgress), AccentColor),
                    SurroundColors = [Color.FromArgb(0, AccentColor)],
                };
                graphics.FillPath(centerBrush, centerPath);
            }

            using var borderPen = new Pen(Color.FromArgb((int)GlassMorphShellPanel.Lerp(140f, 188f, Math.Max(_hoverProgress, _checkedProgress)), GlassMorphShellPanel.Blend(Color.White, AccentColor, 0.18f)));
            graphics.DrawPath(borderPen, glyphPath);

            using var textShadowBrush = new SolidBrush(Color.FromArgb(72, 18, 10, 28));
            using var textBrush = new SolidBrush(Color.FromArgb(244, 247, 255));
            using var font = new Font("Segoe UI", 9.5f, FontStyle.Bold, GraphicsUnit.Point);
            Rectangle textBounds = new(32, 0, Math.Max(10, clientBounds.Width - 34), clientBounds.Height);
            graphics.DrawString(Text ?? string.Empty, font, textShadowBrush, new Rectangle(textBounds.X, textBounds.Y + 1, textBounds.Width, textBounds.Height), StringFormat.GenericDefault);
            graphics.DrawString(Text ?? string.Empty, font, textBrush, textBounds, StringFormat.GenericDefault);

            using var targetGraphics = Graphics.FromHdc(hdc);
            targetGraphics.DrawImageUnscaled(surface, 0, 0);
        }
        finally
        {
            _ = NativeMethods.EndPaint(Handle, ref paintStruct);
        }
    }

    private void TrackMouseLeave()
    {
        if (Handle == 0)
        {
            return;
        }

        var track = new NativeMethods.TRACKMOUSEEVENT
        {
            cbSize = (uint)Marshal.SizeOf<NativeMethods.TRACKMOUSEEVENT>(),
            dwFlags = NativeMethods.TME_LEAVE,
            hwndTrack = Handle,
        };

        _ = NativeMethods.TrackMouseEvent(ref track);
    }

    private void StartAnimationTimer()
    {
        if (_timerRunning || Handle == 0)
        {
            return;
        }

        _ = NativeMethods.SetTimer(Handle, AnimationTimerId, 16, 0);
        _timerRunning = true;
    }

    private void StopAnimationTimer()
    {
        if (!_timerRunning || Handle == 0)
        {
            _timerRunning = false;
            return;
        }

        _ = NativeMethods.KillTimer(Handle, AnimationTimerId);
        _timerRunning = false;
    }
}

[SupportedOSPlatform("windows6.1")]
internal sealed class GlassMorphCheckBox : GlassMorphToggleBase
{
    protected override bool CircularGlyph => false;
}

[SupportedOSPlatform("windows6.1")]
internal sealed class GlassMorphRadioButton : GlassMorphToggleBase
{
    protected override bool CircularGlyph => true;

    protected override void OnToggleClicked()
    {
        Checked = true;
    }
}

[SupportedOSPlatform("windows6.1")]
internal sealed class GlassMorphProgressBar : Label
{
    private const uint AnimationTimerId = 1;
    private bool _timerRunning;
    private float _displayValue;
    private int _value;

    public Color AccentColor { get; set; } = Color.FromArgb(88, 206, 255);

    public int Maximum { get; set; } = 100;

    public int Value
    {
        get => _value;
        set
        {
            _value = Math.Clamp(value, 0, Maximum);
            StartAnimationTimer();
            Invalidate();
        }
    }

    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        switch (message)
        {
            case NativeMethods.WM_ERASEBKGND:
                result = 1;
                return true;

            case NativeMethods.WM_PAINT:
                PaintProgress();
                result = 0;
                return true;

            case NativeMethods.WM_TIMER:
                if ((nuint)wParam == AnimationTimerId)
                {
                    _displayValue += (_value - _displayValue) * 0.16f;
                    if (Math.Abs(_value - _displayValue) < 0.2f)
                    {
                        _displayValue = _value;
                        StopAnimationTimer();
                    }

                    Invalidate();
                    result = 0;
                    return true;
                }

                break;
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }

    protected override void OnDisposing()
    {
        StopAnimationTimer();
        base.OnDisposing();
    }

    private void PaintProgress()
    {
        if (Handle == 0)
        {
            return;
        }

        nint hdc = NativeMethods.BeginPaint(Handle, out NativeMethods.PAINTSTRUCT paintStruct);
        if (hdc == 0)
        {
            return;
        }

        try
        {
            if (!NativeMethods.GetClientRect(Handle, out NativeMethods.RECT clientRect))
            {
                return;
            }

            Rectangle clientBounds = Rectangle.FromLTRB(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom);
            using var surface = new Bitmap(Math.Max(1, clientBounds.Width), Math.Max(1, clientBounds.Height));
            using var graphics = Graphics.FromImage(surface);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var backgroundBrush = new SolidBrush(Parent is not null && !Parent.BackColor.IsEmpty ? Parent.BackColor : Color.FromArgb(76, 24, 104)))
            {
                graphics.FillRectangle(backgroundBrush, clientBounds);
            }

            Rectangle trackBounds = Rectangle.Inflate(clientBounds, -2, -4);
            using GraphicsPath trackPath = GlassMorphShellPanel.CreateRoundedRectanglePath(trackBounds, Math.Max(10, trackBounds.Height / 2));
            using var trackBrush = new SolidBrush(Color.FromArgb(140, 88, 52, 120));
            graphics.FillPath(trackBrush, trackPath);

            float progressRatio = Maximum <= 0 ? 0f : Math.Clamp(_displayValue / Maximum, 0f, 1f);
            int fillWidth = Math.Max(18, (int)Math.Round((trackBounds.Width - 4) * progressRatio));
            Rectangle fillBounds = new(trackBounds.Left + 2, trackBounds.Top + 2, Math.Min(trackBounds.Width - 4, fillWidth), Math.Max(10, trackBounds.Height - 4));
            using GraphicsPath fillPath = GlassMorphShellPanel.CreateRoundedRectanglePath(fillBounds, Math.Max(8, fillBounds.Height / 2));
            using var fillBrush = new LinearGradientBrush(fillBounds, Color.FromArgb(188, GlassMorphShellPanel.Blend(Color.FromArgb(126, 94, 164), AccentColor, 0.62f)), Color.FromArgb(166, AccentColor), LinearGradientMode.Horizontal);
            graphics.FillPath(fillBrush, fillPath);

            Rectangle glowBounds = new(fillBounds.Right - Math.Max(20, fillBounds.Height), fillBounds.Top, Math.Max(20, fillBounds.Height), fillBounds.Height);
            using var glowPath = GlassMorphShellPanel.CreateEllipsePath(glowBounds);
            using var glowBrush = new PathGradientBrush(glowPath)
            {
                CenterColor = Color.FromArgb(180, AccentColor),
                SurroundColors = [Color.FromArgb(0, AccentColor)],
            };
            graphics.FillPath(glowBrush, glowPath);

            using var borderPen = new Pen(Color.FromArgb(172, GlassMorphShellPanel.Blend(Color.White, AccentColor, 0.14f)));
            graphics.DrawPath(borderPen, trackPath);

            using var targetGraphics = Graphics.FromHdc(hdc);
            targetGraphics.DrawImageUnscaled(surface, 0, 0);
        }
        finally
        {
            _ = NativeMethods.EndPaint(Handle, ref paintStruct);
        }
    }

    private void StartAnimationTimer()
    {
        if (_timerRunning || Handle == 0)
        {
            return;
        }

        _ = NativeMethods.SetTimer(Handle, AnimationTimerId, 16, 0);
        _timerRunning = true;
    }

    private void StopAnimationTimer()
    {
        if (!_timerRunning || Handle == 0)
        {
            _timerRunning = false;
            return;
        }

        _ = NativeMethods.KillTimer(Handle, AnimationTimerId);
        _timerRunning = false;
    }
}

internal static class NativeMethods
{
    public const int WM_ERASEBKGND = 0x0014;
    public const int WM_PAINT = 0x000F;
    public const int WM_TIMER = 0x0113;
    public const int WM_MOUSEMOVE = 0x0200;
    public const int WM_MOUSELEAVE = 0x02A3;
    public const int WM_LBUTTONUP = 0x0202;
    public const int WM_SETFOCUS = 0x0007;
    public const int WM_KILLFOCUS = 0x0008;
    public const uint SS_NOTIFY = 0x00000100;
    public const uint TME_LEAVE = 0x00000002;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        public nint hdc;
        public int fErase;
        public RECT rcPaint;
        public int fRestore;
        public int fIncUpdate;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[]? rgbReserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TRACKMOUSEEVENT
    {
        public uint cbSize;
        public uint dwFlags;
        public nint hwndTrack;
        public uint dwHoverTime;
    }

    [DllImport("user32.dll", SetLastError = true)]
    public static extern nint BeginPaint(nint hWnd, out PAINTSTRUCT lpPaint);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EndPaint(nint hWnd, [In] ref PAINTSTRUCT lpPaint);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetClientRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern nuint SetTimer(nint hWnd, uint nIDEvent, uint uElapse, nint lpTimerFunc);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool KillTimer(nint hWnd, uint uIDEvent);
}
