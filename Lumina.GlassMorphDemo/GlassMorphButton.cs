using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Lumina.GlassMorphDemo;

[SupportedOSPlatform("windows6.1")]
internal sealed class GlassMorphButton : Label
{
    private const uint AnimationTimerId = 1;
    private const int HorizontalInset = 12;
    private const int VerticalInset = 16;
    private const float HoverLerpFactor = 0.18f;
    private const float LeaveLerpFactor = 0.24f;

    private Point _hoverPoint;
    private bool _hovered;
    private bool _pressed;
    private bool _timerRunning;
    private float _hoverProgress;
    private float _sheenProgress = -0.35f;

    public event EventHandler? Click;

    public Color AccentColor { get; set; } = Color.FromArgb(87, 179, 255);

    protected override uint Style => base.Style | NativeMethods.SS_NOTIFY;

    protected override bool UseParentBackgroundForTheming => false;

    protected override int GetNativeHeight(int requestedHeight)
        => requestedHeight;

    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        Invalidate();
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
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
                PaintButton();
                result = 0;
                return true;

            case NativeMethods.WM_MOUSEMOVE:
            {
                Point hoverPoint = ExtractPoint(lParam);
                bool hoverPointChanged = !_hoverPoint.Equals(hoverPoint);
                _hoverPoint = hoverPoint;

                if (Enabled && !_hovered)
                {
                    _hovered = true;
                    _sheenProgress = -0.28f;
                    TrackMouseLeave();
                    StartAnimationTimer();
                    Invalidate();
                }

                if (hoverPointChanged && _hoverProgress > 0.08f)
                {
                    Invalidate();
                }

                result = 0;
                return true;
            }

            case NativeMethods.WM_MOUSELEAVE:
                _hovered = false;
                _pressed = false;
                StartAnimationTimer();
                Invalidate();
                result = 0;
                return true;

            case NativeMethods.WM_LBUTTONDOWN:
                _pressed = true;
                Invalidate();
                result = 0;
                return true;

            case NativeMethods.WM_LBUTTONUP:
                if (_pressed)
                {
                    _pressed = false;
                    Invalidate();
                    Click?.Invoke(this, EventArgs.Empty);
                }

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

    private void AdvanceAnimation()
    {
        float target = _hovered ? 1f : 0f;
        float factor = _hovered ? HoverLerpFactor : LeaveLerpFactor;
        _hoverProgress += (target - _hoverProgress) * factor;

        if (_hovered)
        {
            _sheenProgress = Math.Min(1.35f, _sheenProgress + 0.12f);
        }
        else
        {
            _sheenProgress = MathF.Max(-0.35f, _sheenProgress - 0.16f);
        }

        if (Math.Abs(target - _hoverProgress) < 0.015f)
        {
            _hoverProgress = target;
        }

        bool animationSettled = !_hovered && _hoverProgress <= 0.01f;
        bool sheenSettled = !_hovered && _sheenProgress <= -0.30f;
        if (animationSettled && sheenSettled)
        {
            StopAnimationTimer();
        }

        Invalidate();
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

    private void PaintButton()
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
            Rectangle buttonBounds = Rectangle.FromLTRB(
                clientBounds.Left + HorizontalInset,
                clientBounds.Top + VerticalInset + (_pressed ? 1 : 0),
                clientBounds.Right - HorizontalInset,
                clientBounds.Bottom - VerticalInset + (_pressed ? 1 : 0));

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

            DrawDropShadow(graphics, buttonBounds);
            DrawAmbientGlow(graphics, buttonBounds);

            using GraphicsPath buttonPath = CreateRoundedRectanglePath(buttonBounds, Math.Max(12, buttonBounds.Height / 2));
            DrawGlassSurface(graphics, buttonBounds, buttonPath);
            DrawSweep(graphics, buttonBounds, buttonPath);
            DrawContent(graphics, buttonBounds);

            using var targetGraphics = Graphics.FromHdc(hdc);
            targetGraphics.DrawImageUnscaled(surface, 0, 0);
        }
        finally
        {
            _ = NativeMethods.EndPaint(Handle, ref paintStruct);
        }
    }

    private void DrawDropShadow(Graphics graphics, Rectangle buttonBounds)
    {
        Rectangle shadowBounds = buttonBounds;
        shadowBounds.Offset(0, 8);
        using GraphicsPath shadowPath = CreateRoundedRectanglePath(shadowBounds, Math.Max(12, shadowBounds.Height / 2));
        using var shadowPen = new Pen(Color.FromArgb((int)Lerp(54f, 72f, _hoverProgress), 26, 8, 36), 8f);
        graphics.DrawPath(shadowPen, shadowPath);
    }

    private void DrawAmbientGlow(Graphics graphics, Rectangle buttonBounds)
    {
        int glowWidth = (int)Math.Round(Lerp(buttonBounds.Width * 0.18f, buttonBounds.Width * 0.72f, _hoverProgress));
        int glowHeight = (int)Math.Round(Lerp(14f, 36f, _hoverProgress));
        int glowX = buttonBounds.Left + ((buttonBounds.Width - glowWidth) / 2);
        int glowY = buttonBounds.Bottom - (glowHeight / 2);
        var glowBounds = new Rectangle(glowX, glowY, Math.Max(24, glowWidth), Math.Max(10, glowHeight));

        using var glowPath = CreateEllipsePath(glowBounds);
        using var glowBrush = new PathGradientBrush(glowPath)
        {
            CenterColor = Color.FromArgb((int)Lerp(118f, 220f, _hoverProgress), AccentColor),
            SurroundColors = [Color.FromArgb(0, AccentColor)],
        };

        graphics.FillPath(glowBrush, glowPath);
    }

    private void DrawGlassSurface(Graphics graphics, Rectangle buttonBounds, GraphicsPath buttonPath)
    {
        Color idleTop = Color.FromArgb(112, 79, 143);
        Color idleMid = Color.FromArgb(92, 63, 124);
        Color idleBottom = Color.FromArgb(70, 42, 101);

        using (var fillBrush = new LinearGradientBrush(
            buttonBounds,
            Color.FromArgb((int)Lerp(150f, 170f, _hoverProgress), idleTop),
            Color.FromArgb((int)Lerp(128f, 144f, _hoverProgress), idleBottom),
            LinearGradientMode.Vertical))
        {
            var blend = new ColorBlend
            {
                Colors =
                [
                    Color.FromArgb((int)Lerp(154f, 174f, _hoverProgress), idleTop),
                    Color.FromArgb((int)Lerp(144f, 164f, _hoverProgress), idleMid),
                    Color.FromArgb((int)Lerp(130f, 150f, _hoverProgress), idleBottom),
                ],
                Positions = [0f, 0.55f, 1f],
            };

            fillBrush.InterpolationColors = blend;
            graphics.FillPath(fillBrush, buttonPath);
        }

        DrawCenterExpansion(graphics, buttonBounds, buttonPath);

        Rectangle innerGlowBounds = BuildInnerGlowBounds(buttonBounds);
        using (var innerGlowPath = CreateEllipsePath(innerGlowBounds))
        using (var innerGlowBrush = new PathGradientBrush(innerGlowPath))
        {
            innerGlowBrush.CenterColor = Color.FromArgb((int)Lerp(36f, 162f, _hoverProgress), Blend(Color.White, AccentColor, 0.22f));
            innerGlowBrush.SurroundColors = [Color.FromArgb(0, Color.White)];
            graphics.FillPath(innerGlowBrush, innerGlowPath);
        }

        Rectangle topSheenBounds = Rectangle.FromLTRB(
            buttonBounds.Left + 12,
            buttonBounds.Top + 5,
            buttonBounds.Right - 12,
            buttonBounds.Top + Math.Max(12, buttonBounds.Height / 2));
        using GraphicsPath topSheenPath = CreateRoundedRectanglePath(topSheenBounds, Math.Max(10, topSheenBounds.Height / 2));
        using (var sheenBrush = new LinearGradientBrush(
            topSheenBounds,
            Color.FromArgb((int)Lerp(32f, 88f, _hoverProgress), Color.White),
            Color.FromArgb(0, Color.White),
            LinearGradientMode.Vertical))
        {
            graphics.FillPath(sheenBrush, topSheenPath);
        }

        if (_hoverProgress > 0.02f)
        {
            using var haloPen = new Pen(Color.FromArgb((int)Lerp(0f, 42f, _hoverProgress), AccentColor), Lerp(6f, 12f, _hoverProgress));
            graphics.DrawPath(haloPen, buttonPath);
        }

        using var borderPen = new Pen(Color.FromArgb((int)Lerp(144f, 198f, _hoverProgress), Blend(Color.White, AccentColor, 0.20f)));
        using var innerPen = new Pen(Color.FromArgb((int)Lerp(70f, 96f, _hoverProgress), Color.White));
        graphics.DrawPath(borderPen, buttonPath);

        Rectangle innerBounds = Rectangle.Inflate(buttonBounds, -2, -2);
        using GraphicsPath innerPath = CreateRoundedRectanglePath(innerBounds, Math.Max(10, innerBounds.Height / 2));
        graphics.DrawPath(innerPen, innerPath);
    }

    private void DrawCenterExpansion(Graphics graphics, Rectangle buttonBounds, GraphicsPath buttonPath)
    {
        if (_hoverProgress <= 0.001f)
        {
            return;
        }

        int spreadHeight = Math.Max(18, buttonBounds.Height - 8);
        int minWidth = spreadHeight;
        int maxWidth = Math.Max(minWidth, buttonBounds.Width - 10);
        int spreadWidth = (int)Math.Round(Lerp(minWidth, maxWidth, _hoverProgress));
        int spreadLeft = buttonBounds.Left + ((buttonBounds.Width - spreadWidth) / 2);
        int spreadTop = buttonBounds.Top + ((buttonBounds.Height - spreadHeight) / 2);
        var spreadBounds = new Rectangle(spreadLeft, spreadTop, spreadWidth, spreadHeight);

        GraphicsState state = graphics.Save();
        using var clipRegion = new Region(buttonPath);
        graphics.SetClip(clipRegion, CombineMode.Intersect);

        int spreadRadius = Math.Max(3, Math.Min(6, spreadBounds.Height / 6));
        using var spreadPath = CreateRoundedRectanglePath(spreadBounds, spreadRadius);
        using var spreadBrush = new PathGradientBrush(spreadPath)
        {
            CenterColor = Color.FromArgb((int)Lerp(40f, 168f, _hoverProgress), Blend(Color.White, AccentColor, 0.62f)),
            SurroundColors = [Color.FromArgb(0, AccentColor)],
        };

        graphics.FillPath(spreadBrush, spreadPath);
        graphics.Restore(state);
    }

    private void DrawSweep(Graphics graphics, Rectangle buttonBounds, GraphicsPath buttonPath)
    {
        if (_hoverProgress <= 0.02f || _sheenProgress < -0.1f || _sheenProgress > 1.12f)
        {
            return;
        }

        int sweepWidth = Math.Max(26, buttonBounds.Width / 5);
        float sweepX = Lerp(buttonBounds.Left - sweepWidth, buttonBounds.Right + sweepWidth, _sheenProgress);
        PointF[] polygon =
        [
            new PointF(sweepX - 18, buttonBounds.Top + 3),
            new PointF(sweepX + 18, buttonBounds.Top + 3),
            new PointF(sweepX + sweepWidth, buttonBounds.Bottom - 3),
            new PointF(sweepX + sweepWidth - 36, buttonBounds.Bottom - 3),
        ];

        using var sweepPath = new GraphicsPath();
        sweepPath.AddPolygon(polygon);
        using var region = new Region(buttonPath);
        GraphicsState state = graphics.Save();
        graphics.SetClip(region, CombineMode.Intersect);
        using var sweepBrush = new LinearGradientBrush(
            new PointF(sweepX - 18, buttonBounds.Top),
            new PointF(sweepX + sweepWidth, buttonBounds.Bottom),
            Color.FromArgb((int)Lerp(0f, 44f, _hoverProgress), Color.White),
            Color.FromArgb((int)Lerp(0f, 88f, _hoverProgress), Color.White));
        graphics.FillPath(sweepBrush, sweepPath);
        graphics.Restore(state);
    }

    private void DrawContent(Graphics graphics, Rectangle buttonBounds)
    {
        Rectangle textBounds = Rectangle.Inflate(buttonBounds, -18, -8);
        if (_pressed)
        {
            textBounds.Offset(0, 1);
        }

        using var textShadowBrush = new SolidBrush(Color.FromArgb((int)Lerp(80f, 104f, _hoverProgress), 18, 10, 28));
        using var textBrush = new SolidBrush(Color.FromArgb(248, 250, 255));
        using var font = new Font("Segoe UI", 10.2f, FontStyle.Bold, GraphicsUnit.Point);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        Rectangle shadowBounds = textBounds;
        shadowBounds.Offset(0, 1);
        graphics.DrawString(Text ?? string.Empty, font, textShadowBrush, shadowBounds, format);
        graphics.DrawString(Text ?? string.Empty, font, textBrush, textBounds, format);
    }

    private Color ResolveParentColor()
    {
        if (Parent is not null && !Parent.BackColor.IsEmpty)
        {
            return Parent.BackColor;
        }

        return Color.FromArgb(69, 24, 95);
    }

    private Rectangle BuildInnerGlowBounds(Rectangle buttonBounds)
    {
        float cursorRatio = buttonBounds.Width <= 0
            ? 0.5f
            : Math.Clamp((_hoverPoint.X - buttonBounds.Left) / (float)buttonBounds.Width, 0.18f, 0.82f);
        cursorRatio = Lerp(0.5f, cursorRatio, 0.24f);

        int width = (int)Math.Round(Lerp(buttonBounds.Width * 0.28f, buttonBounds.Width * 0.78f, _hoverProgress));
        int height = (int)Math.Round(Lerp(buttonBounds.Height * 0.34f, buttonBounds.Height * 0.82f, _hoverProgress));
        int centerX = buttonBounds.Left + (int)Math.Round(buttonBounds.Width * cursorRatio);
        int x = centerX - (width / 2);
        int y = buttonBounds.Top + ((buttonBounds.Height - height) / 2);

        x = Math.Min(Math.Max(buttonBounds.Left + 4, x), buttonBounds.Right - width - 4);
        return new Rectangle(x, y, Math.Max(42, width), Math.Max(16, height));
    }

    private static Point ExtractPoint(nint lParam)
    {
        return new Point(
            unchecked((short)(nuint)lParam),
            unchecked((short)(((nuint)lParam >> 16) & 0xFFFF)));
    }

    private static GraphicsPath CreateEllipsePath(Rectangle bounds)
    {
        var path = new GraphicsPath();
        path.AddEllipse(bounds);
        return path;
    }

    private static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
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

    private static float Lerp(float from, float to, float amount)
        => from + ((to - from) * Math.Clamp(amount, 0f, 1f));

    private static Color Blend(Color from, Color to, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return Color.FromArgb(
            (int)Math.Round(from.A + ((to.A - from.A) * amount)),
            (int)Math.Round(from.R + ((to.R - from.R) * amount)),
            (int)Math.Round(from.G + ((to.G - from.G) * amount)),
            (int)Math.Round(from.B + ((to.B - from.B) * amount)));
    }

    private static class NativeMethods
    {
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_PAINT = 0x000F;
        public const int WM_TIMER = 0x0113;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
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
}
