using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Provides WinForms-compatible user resizing for controls arranged beside a splitter.
/// </summary>
public class Splitter : Control
{
    private int _minSize = 25;
    private int _minExtra = 25;
    private int _splitterThickness = 3;
    private bool _dragging;
    private Point _dragAnchor;
    private int _initialTargetSize;

    /// <summary>
    /// Initializes a splitter docked to the left edge.
    /// </summary>
    public Splitter()
    {
        Dock = DockStyle.Left;
        TabStop = false;
        SetBounds(0, 0, 3, 3);
    }

    /// <summary>
    /// Gets or sets the border style of the splitter.
    /// </summary>
    public BorderStyle BorderStyle { get; set; }

    /// <summary>
    /// Gets or sets the minimum size of the resized target control.
    /// </summary>
    public int MinSize
    {
        get => _minSize;
        set => _minSize = Math.Max(0, value);
    }

    /// <summary>
    /// Gets or sets the minimum size of the remaining area beside the splitter.
    /// </summary>
    public int MinExtra
    {
        get => _minExtra;
        set => _minExtra = Math.Max(0, value);
    }

    /// <summary>
    /// Gets or sets the current split position.
    /// </summary>
    public int SplitPosition
    {
        get => GetCurrentTargetSize();
        set => ApplySplitSize(value, raiseMoved: true, pointerLocation: default);
    }

    /// <summary>
    /// Occurs while the splitter is being moved.
    /// </summary>
    public event SplitterEventHandler? SplitterMoving;

    /// <summary>
    /// Occurs after the splitter finished moving.
    /// </summary>
    public event SplitterEventHandler? SplitterMoved;

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.SS_NOTIFY;

    /// <inheritdoc />
    public override DockStyle Dock
    {
        get => base.Dock;
        set
        {
            if (value is not DockStyle.Top and not DockStyle.Bottom and not DockStyle.Left and not DockStyle.Right)
            {
                throw new ArgumentException("Splitter only supports Top, Bottom, Left, or Right docking.", nameof(value));
            }

            int thickness = _splitterThickness;
            base.Dock = value;

            if (IsHorizontal)
            {
                SetBounds(Left, Top, Math.Max(1, thickness), Height);
            }
            else
            {
                SetBounds(Left, Top, Width, Math.Max(1, thickness));
            }
        }
    }

    /// <inheritdoc />
    protected override void OnBoundsChanged()
    {
        if (IsHorizontal)
        {
            _splitterThickness = Math.Max(1, Width);
        }
        else
        {
            _splitterThickness = Math.Max(1, Height);
        }
    }

    /// <inheritdoc />
    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        Point point = ExtractPoint(lParam);

        switch (message)
        {
            case Win32.WM_LBUTTONDOWN:
                if (TryBeginSplit(point))
                {
                    result = 0;
                    return true;
                }

                break;

            case Win32.WM_MOUSEMOVE:
                if (_dragging)
                {
                    ApplyDraggedPosition(point, raiseMoved: false);
                    result = 0;
                    return true;
                }

                break;

            case Win32.WM_LBUTTONUP:
                if (_dragging)
                {
                    ApplyDraggedPosition(point, raiseMoved: true);
                    EndSplit();
                    result = 0;
                    return true;
                }

                break;

            case Win32.WM_SETCURSOR:
                if (Win32.LowWord(lParam) == Win32.HTCLIENT)
                {
                    nint cursorHandle = Win32.LoadCursorW(0, (nint)(IsHorizontal ? Win32.IDC_SIZEWE : Win32.IDC_SIZENS));
                    if (cursorHandle != 0)
                    {
                        _ = Win32.SetCursor(cursorHandle);
                        result = (nint)1;
                        return true;
                    }
                }

                break;
        }

        result = 0;
        return false;
    }

    private bool IsHorizontal => Dock is DockStyle.Left or DockStyle.Right;

    private bool TryBeginSplit(Point point)
    {
        if (ResolveLayoutControls() is not { Target: { } target })
        {
            return false;
        }

        _dragging = true;
        _dragAnchor = point;
        _initialTargetSize = IsHorizontal ? target.Width : target.Height;
        _ = Win32.SetCapture(Handle);
        return true;
    }

    private void EndSplit()
    {
        _dragging = false;
        if (Handle != 0 && Win32.GetCapture() == Handle)
        {
            _ = Win32.ReleaseCapture();
        }
    }

    private void ApplyDraggedPosition(Point point, bool raiseMoved)
    {
        if (ResolveLayoutControls() is not { } layout || layout.Target is null)
        {
            return;
        }

        int delta = IsHorizontal
            ? point.X - _dragAnchor.X
            : point.Y - _dragAnchor.Y;

        int proposedSize = Dock switch
        {
            DockStyle.Left or DockStyle.Top => _initialTargetSize + delta,
            DockStyle.Right or DockStyle.Bottom => _initialTargetSize - delta,
            _ => _initialTargetSize,
        };

        ApplySplitSize(proposedSize, raiseMoved, point);
    }

    private void ApplySplitSize(int proposedSize, bool raiseMoved, Point pointerLocation)
    {
        if (ResolveLayoutControls() is not { } layout || layout.Target is null)
        {
            return;
        }

        int maxSize = ResolveMaximumSplitSize(layout);
        int newSize = Math.Clamp(proposedSize, MinSize, Math.Max(MinSize, maxSize));
        if (newSize == GetCurrentTargetSize(layout.Target))
        {
            return;
        }

        SplitterEventArgs eventArgs = CreateEventArgs(layout.Target, newSize, pointerLocation);
        OnSplitterMoving(eventArgs);
        ApplyLayout(layout, newSize);

        if (raiseMoved)
        {
            OnSplitterMoved(eventArgs);
        }
    }

    private int ResolveMaximumSplitSize(SplitterLayout layout)
    {
        int parentPrimarySize = GetParentPrimarySize();
        if (layout.Target is null)
        {
            return parentPrimarySize;
        }

        int remainingSpace = layout.FillTarget is not null
            ? (IsHorizontal ? layout.FillTarget.Width : layout.FillTarget.Height)
            : parentPrimarySize - _splitterThickness - GetCurrentTargetSize(layout.Target);

        return GetCurrentTargetSize(layout.Target) + Math.Max(0, remainingSpace - MinExtra);
    }

    private int GetParentPrimarySize()
    {
        if (Parent is not null)
        {
            return IsHorizontal ? Parent.Width : Parent.Height;
        }

        return Owner is null
            ? 0
            : IsHorizontal ? Owner.ClientSize.Width : Owner.ClientSize.Height;
    }

    private SplitterEventArgs CreateEventArgs(Control target, int newSize, Point pointerLocation)
    {
        int pointerX = pointerLocation == default ? Left : Left + pointerLocation.X;
        int pointerY = pointerLocation == default ? Top : Top + pointerLocation.Y;

        return Dock switch
        {
            DockStyle.Left => new SplitterEventArgs(pointerX, pointerY, target.Left + newSize, Top),
            DockStyle.Right => new SplitterEventArgs(pointerX, pointerY, Right - _splitterThickness, Top),
            DockStyle.Top => new SplitterEventArgs(pointerX, pointerY, Left, target.Top + newSize),
            DockStyle.Bottom => new SplitterEventArgs(pointerX, pointerY, Left, Bottom - _splitterThickness),
            _ => new SplitterEventArgs(pointerX, pointerY, Left, Top),
        };
    }

    private void ApplyLayout(SplitterLayout layout, int newTargetSize)
    {
        Control target = layout.Target!;
        Control? fillTarget = layout.FillTarget;

        switch (Dock)
        {
            case DockStyle.Left:
            {
                int delta = newTargetSize - target.Width;
                target.SetBounds(target.Left, target.Top, newTargetSize, target.Height);
                SetBounds(target.Right, Top, Width, Height);
                if (fillTarget is not null)
                {
                    fillTarget.SetBounds(fillTarget.Left + delta, fillTarget.Top, Math.Max(1, fillTarget.Width - delta), fillTarget.Height);
                }

                break;
            }

            case DockStyle.Right:
            {
                int delta = newTargetSize - target.Width;
                target.SetBounds(target.Left - delta, target.Top, newTargetSize, target.Height);
                SetBounds(target.Left - Width, Top, Width, Height);
                if (fillTarget is not null)
                {
                    fillTarget.SetBounds(fillTarget.Left, fillTarget.Top, Math.Max(1, fillTarget.Width - delta), fillTarget.Height);
                }

                break;
            }

            case DockStyle.Top:
            {
                int delta = newTargetSize - target.Height;
                target.SetBounds(target.Left, target.Top, target.Width, newTargetSize);
                SetBounds(Left, target.Bottom, Width, Height);
                if (fillTarget is not null)
                {
                    fillTarget.SetBounds(fillTarget.Left, fillTarget.Top + delta, fillTarget.Width, Math.Max(1, fillTarget.Height - delta));
                }

                break;
            }

            case DockStyle.Bottom:
            {
                int delta = newTargetSize - target.Height;
                target.SetBounds(target.Left, target.Top - delta, target.Width, newTargetSize);
                SetBounds(Left, target.Top - Height, Width, Height);
                if (fillTarget is not null)
                {
                    fillTarget.SetBounds(fillTarget.Left, fillTarget.Top, fillTarget.Width, Math.Max(1, fillTarget.Height - delta));
                }

                break;
            }
        }

        Parent?.PerformLayout();
        Owner?.PerformLayout();
    }

    private int GetCurrentTargetSize()
        => ResolveLayoutControls() is { Target: { } target }
            ? GetCurrentTargetSize(target)
            : -1;

    private int GetCurrentTargetSize(Control target)
        => IsHorizontal ? target.Width : target.Height;

    private SplitterLayout? ResolveLayoutControls()
    {
        List<Control> siblings = [];
        if (Parent is ContainerControlBase container)
        {
            siblings.AddRange(container.Controls);
        }
        else if (Owner is not null)
        {
            siblings.AddRange(Owner.Controls);
        }

        Control? target = null;
        Control? fillTarget = null;
        foreach (Control sibling in siblings)
        {
            if (ReferenceEquals(sibling, this))
            {
                continue;
            }

            switch (Dock)
            {
                case DockStyle.Left:
                    if (sibling.Right == Left)
                    {
                        target = sibling;
                    }
                    else if (sibling.Left == Right)
                    {
                        fillTarget = sibling;
                    }

                    break;

                case DockStyle.Right:
                    if (sibling.Left == Right)
                    {
                        target = sibling;
                    }
                    else if (sibling.Right == Left)
                    {
                        fillTarget = sibling;
                    }

                    break;

                case DockStyle.Top:
                    if (sibling.Bottom == Top)
                    {
                        target = sibling;
                    }
                    else if (sibling.Top == Bottom)
                    {
                        fillTarget = sibling;
                    }

                    break;

                case DockStyle.Bottom:
                    if (sibling.Top == Bottom)
                    {
                        target = sibling;
                    }
                    else if (sibling.Bottom == Top)
                    {
                        fillTarget = sibling;
                    }

                    break;
            }
        }

        return target is null
            ? null
            : new SplitterLayout(target, fillTarget);
    }

    private void OnSplitterMoving(SplitterEventArgs e)
        => SplitterMoving?.Invoke(this, e);

    private void OnSplitterMoved(SplitterEventArgs e)
        => SplitterMoved?.Invoke(this, e);

    private sealed record SplitterLayout(Control Target, Control? FillTarget);
}
