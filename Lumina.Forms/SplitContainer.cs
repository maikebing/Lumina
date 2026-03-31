using System.ComponentModel;
using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a two-panel split container.
/// </summary>
public class SplitContainer : ContainerControlBase, ISupportInitialize
{
    private bool _initializing;
    private bool _draggingSplitter;
    private int _dragOffset;
    private int _splitterDistance = 100;
    private int _splitterWidth = 6;
    private int _panel1MinSize = 25;
    private int _panel2MinSize = 25;
    private Orientation _orientation = Orientation.Vertical;

    /// <summary>
    /// Initializes a split container with two child panels.
    /// </summary>
    public SplitContainer()
    {
        Panel1 = new Panel();
        Panel2 = new Panel();
        Controls.AddRange(Panel1, Panel2);
    }

    /// <summary>
    /// Gets the first panel.
    /// </summary>
    public Panel Panel1 { get; }

    /// <summary>
    /// Gets the second panel.
    /// </summary>
    public Panel Panel2 { get; }

    /// <summary>
    /// Gets or sets the splitter distance measured from the leading edge of the control.
    /// </summary>
    public int SplitterDistance
    {
        get => _splitterDistance;
        set
        {
            _splitterDistance = Math.Max(0, value);
            LayoutPanels();
        }
    }

    /// <summary>
    /// Gets or sets the splitter orientation.
    /// </summary>
    public Orientation Orientation
    {
        get => _orientation;
        set
        {
            if (_orientation == value)
            {
                return;
            }

            _orientation = value;
            LayoutPanels();
        }
    }

    /// <summary>
    /// Gets or sets the thickness of the splitter.
    /// </summary>
    public int SplitterWidth
    {
        get => _splitterWidth;
        set
        {
            int normalized = Math.Max(4, value);
            if (_splitterWidth == normalized)
            {
                return;
            }

            _splitterWidth = normalized;
            LayoutPanels();
        }
    }

    /// <summary>
    /// Gets or sets the minimum size of the first panel.
    /// </summary>
    public int Panel1MinSize
    {
        get => _panel1MinSize;
        set
        {
            _panel1MinSize = Math.Max(0, value);
            LayoutPanels();
        }
    }

    /// <summary>
    /// Gets or sets the minimum size of the second panel.
    /// </summary>
    public int Panel2MinSize
    {
        get => _panel2MinSize;
        set
        {
            _panel2MinSize = Math.Max(0, value);
            LayoutPanels();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the splitter is fixed in place.
    /// </summary>
    public bool IsSplitterFixed { get; set; }

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_CLIPCHILDREN | Win32.WS_CLIPSIBLINGS;

    /// <inheritdoc />
    public void BeginInit()
    {
        _initializing = true;
    }

    /// <inheritdoc />
    public void EndInit()
    {
        _initializing = false;
        LayoutPanels();
    }

    /// <inheritdoc />
    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        Point point = ExtractPoint(lParam);

        switch (message)
        {
            case Win32.WM_LBUTTONDOWN:
                if (!IsSplitterFixed && GetSplitterBounds().Contains(point))
                {
                    Rectangle splitterBounds = GetSplitterBounds();
                    _draggingSplitter = true;
                    _dragOffset = IsVerticalLayout
                        ? point.X - splitterBounds.Left
                        : point.Y - splitterBounds.Top;
                    _ = Win32.SetCapture(Handle);
                    result = 0;
                    return true;
                }

                break;

            case Win32.WM_MOUSEMOVE:
                if (_draggingSplitter)
                {
                    SplitterDistance = ResolveDraggedSplitterDistance(point);
                    result = 0;
                    return true;
                }

                break;

            case Win32.WM_LBUTTONUP:
                if (_draggingSplitter)
                {
                    StopDraggingSplitter();
                    result = 0;
                    return true;
                }

                break;

            case Win32.WM_SETCURSOR:
                if (!IsSplitterFixed
                    && Win32.LowWord(lParam) == Win32.HTCLIENT
                    && Handle != 0
                    && Win32.GetCursorPos(out var cursor)
                    && IsSplitterHot(new Point(cursor.x, cursor.y)))
                {
                    nint cursorHandle = Win32.LoadCursorW(0, (nint)(IsVerticalLayout ? Win32.IDC_SIZEWE : Win32.IDC_SIZENS));
                    if (cursorHandle != 0)
                    {
                        _ = Win32.SetCursor(cursorHandle);
                        result = (nint)1;
                        return true;
                    }
                }

                break;
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        LayoutPanels();
    }

    /// <inheritdoc />
    protected override void OnBoundsChanged()
    {
        LayoutPanels();
    }

    /// <inheritdoc />
    protected override void OnDisposing()
    {
        StopDraggingSplitter();
        base.OnDisposing();
    }

    /// <inheritdoc />
    public override void PerformLayout()
    {
        LayoutPanels();
        base.PerformLayout();
    }

    private bool IsVerticalLayout => Orientation == Orientation.Vertical;

    private void LayoutPanels()
    {
        if (_initializing)
        {
            return;
        }

        Rectangle contentBounds = new(
            Padding.Left,
            Padding.Top,
            Math.Max(1, Width - Padding.Horizontal),
            Math.Max(1, Height - Padding.Vertical));

        int splitterWidth = Math.Max(4, SplitterWidth);
        int primarySize = IsVerticalLayout ? contentBounds.Width : contentBounds.Height;
        int firstSize = ClampSplitterDistance(_splitterDistance, primarySize, splitterWidth);
        _splitterDistance = firstSize;
        int secondSize = Math.Max(0, primarySize - firstSize - splitterWidth);

        if (IsVerticalLayout)
        {
            Panel1.SetBounds(contentBounds.Left, contentBounds.Top, Math.Max(1, firstSize), Math.Max(1, contentBounds.Height));
            Panel2.SetBounds(contentBounds.Left + firstSize + splitterWidth, contentBounds.Top, Math.Max(1, secondSize), Math.Max(1, contentBounds.Height));
        }
        else
        {
            Panel1.SetBounds(contentBounds.Left, contentBounds.Top, Math.Max(1, contentBounds.Width), Math.Max(1, firstSize));
            Panel2.SetBounds(contentBounds.Left, contentBounds.Top + firstSize + splitterWidth, Math.Max(1, contentBounds.Width), Math.Max(1, secondSize));
        }

        Invalidate();
        Panel1.PerformLayout();
        Panel2.PerformLayout();
    }

    private Rectangle GetSplitterBounds()
    {
        Rectangle contentBounds = new(
            Padding.Left,
            Padding.Top,
            Math.Max(1, Width - Padding.Horizontal),
            Math.Max(1, Height - Padding.Vertical));

        return IsVerticalLayout
            ? new Rectangle(contentBounds.Left + _splitterDistance, contentBounds.Top, Math.Max(4, SplitterWidth), Math.Max(1, contentBounds.Height))
            : new Rectangle(contentBounds.Left, contentBounds.Top + _splitterDistance, Math.Max(1, contentBounds.Width), Math.Max(4, SplitterWidth));
    }

    private bool IsSplitterHot(Point screenPoint)
    {
        Rectangle splitterBounds = GetSplitterBounds();
        if (Handle != 0 && Win32.GetWindowRect(Handle, out var rect))
        {
            splitterBounds.Offset(rect.Left, rect.Top);
        }

        return splitterBounds.Contains(screenPoint);
    }

    private int ResolveDraggedSplitterDistance(Point point)
    {
        int coordinate = IsVerticalLayout ? point.X : point.Y;
        int availablePrimarySize = IsVerticalLayout
            ? Math.Max(0, Width - Padding.Horizontal)
            : Math.Max(0, Height - Padding.Vertical);
        return ClampSplitterDistance(coordinate - _dragOffset, availablePrimarySize, Math.Max(4, SplitterWidth));
    }

    private int ClampSplitterDistance(int proposedDistance, int availablePrimarySize, int splitterWidth)
    {
        int usablePrimarySize = Math.Max(0, availablePrimarySize);
        int maxDistance = Math.Max(0, usablePrimarySize - splitterWidth - Panel2MinSize);
        int minDistance = Math.Min(Panel1MinSize, Math.Max(0, usablePrimarySize - splitterWidth));

        if (maxDistance < minDistance)
        {
            minDistance = Math.Max(0, usablePrimarySize - splitterWidth);
            maxDistance = minDistance;
        }

        return Math.Clamp(proposedDistance, minDistance, maxDistance);
    }

    private void StopDraggingSplitter()
    {
        _draggingSplitter = false;
        if (Handle != 0 && Win32.GetCapture() == Handle)
        {
            _ = Win32.ReleaseCapture();
        }
    }
}
