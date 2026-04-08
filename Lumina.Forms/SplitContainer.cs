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
    private int _splitterDistance = 50;
    private int _splitterWidth = 4;
    private int _splitterIncrement = 1;
    private int _panel1MinSize = 25;
    private int _panel2MinSize = 25;
    private Orientation _orientation = Orientation.Vertical;
    private FixedPanel _fixedPanel;
    private bool _panel1Collapsed;
    private bool _panel2Collapsed;
    private int _lastPrimarySize = -1;
    private int _lastPanel1Size;
    private int _lastPanel2Size;
    private double _splitRatio = 0.5d;

    /// <summary>
    /// Initializes a split container with two child panels.
    /// </summary>
    public SplitContainer()
    {
        Panel1 = new Panel();
        Panel2 = new Panel();
        TabStop = true;
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
            _splitterDistance = Math.Max(0, NormalizeSplitterDistance(value));
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
            int normalized = Math.Max(1, value);
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

    /// <summary>
    /// Gets or sets which panel remains fixed while the control is resized.
    /// </summary>
    public FixedPanel FixedPanel
    {
        get => _fixedPanel;
        set
        {
            if (_fixedPanel == value)
            {
                return;
            }

            _fixedPanel = value;
            LayoutPanels();
        }
    }

    /// <summary>
    /// Gets or sets whether the first panel is collapsed.
    /// </summary>
    public bool Panel1Collapsed
    {
        get => _panel1Collapsed;
        set
        {
            if (_panel1Collapsed == value)
            {
                return;
            }

            _panel1Collapsed = value;
            if (value)
            {
                _panel2Collapsed = false;
            }

            LayoutPanels();
        }
    }

    /// <summary>
    /// Gets or sets whether the second panel is collapsed.
    /// </summary>
    public bool Panel2Collapsed
    {
        get => _panel2Collapsed;
        set
        {
            if (_panel2Collapsed == value)
            {
                return;
            }

            _panel2Collapsed = value;
            if (value)
            {
                _panel1Collapsed = false;
            }

            LayoutPanels();
        }
    }

    /// <summary>
    /// Gets or sets the step interval used while the splitter moves.
    /// </summary>
    public int SplitterIncrement
    {
        get => _splitterIncrement;
        set => _splitterIncrement = Math.Max(1, value);
    }

    /// <summary>
    /// Gets the current bounds of the splitter bar.
    /// </summary>
    public Rectangle SplitterRectangle => GetSplitterBounds();

    /// <summary>
    /// Occurs while the splitter is being moved.
    /// </summary>
    public event SplitterEventHandler? SplitterMoving;

    /// <summary>
    /// Occurs after the splitter has moved.
    /// </summary>
    public event SplitterEventHandler? SplitterMoved;

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_CLIPCHILDREN | Win32.WS_CLIPSIBLINGS | Win32.SS_NOTIFY;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

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
                    int proposedDistance = ResolveDraggedSplitterDistance(point);
                    SplitterDistance = proposedDistance;
                    SplitterMoving?.Invoke(this, CreateSplitterEventArgs(point, proposedDistance));
                    result = 0;
                    return true;
                }

                break;

            case Win32.WM_LBUTTONUP:
                if (_draggingSplitter)
                {
                    int proposedDistance = ResolveDraggedSplitterDistance(point);
                    SplitterDistance = proposedDistance;
                    StopDraggingSplitter();
                    SplitterMoved?.Invoke(this, CreateSplitterEventArgs(point, _splitterDistance));
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
        ApplyNativeThemeState();
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

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyNativeThemeState();
    }

    private void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
        if (Panel1.Handle != 0)
        {
            DarkModeNative.ApplyThemeToWindow(Panel1.Handle, CurrentVisualStyle.IsDarkMode);
        }

        if (Panel2.Handle != 0)
        {
            DarkModeNative.ApplyThemeToWindow(Panel2.Handle, CurrentVisualStyle.IsDarkMode);
        }
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

        int splitterWidth = Math.Max(1, SplitterWidth);
        int primarySize = IsVerticalLayout ? contentBounds.Width : contentBounds.Height;
        if (_panel1Collapsed || _panel2Collapsed)
        {
            LayoutCollapsedPanels(contentBounds);
            _lastPrimarySize = primarySize;
            return;
        }

        Panel1.Visible = true;
        Panel2.Visible = true;

        int desiredDistance = ResolveSplitterDistanceForResize(primarySize, splitterWidth);
        int firstSize = ClampSplitterDistance(desiredDistance, primarySize, splitterWidth);
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

        _lastPrimarySize = primarySize;
        _lastPanel1Size = firstSize;
        _lastPanel2Size = secondSize;
        int usablePrimary = Math.Max(1, primarySize - splitterWidth);
        _splitRatio = Math.Clamp(firstSize / (double)usablePrimary, 0d, 1d);

        Invalidate();
        Panel1.PerformLayout();
        Panel2.PerformLayout();
    }

    private Rectangle GetSplitterBounds()
    {
        if (_panel1Collapsed || _panel2Collapsed)
        {
            return Rectangle.Empty;
        }

        Rectangle contentBounds = new(
            Padding.Left,
            Padding.Top,
            Math.Max(1, Width - Padding.Horizontal),
            Math.Max(1, Height - Padding.Vertical));

        return IsVerticalLayout
            ? new Rectangle(contentBounds.Left + _splitterDistance, contentBounds.Top, Math.Max(1, SplitterWidth), Math.Max(1, contentBounds.Height))
            : new Rectangle(contentBounds.Left, contentBounds.Top + _splitterDistance, Math.Max(1, contentBounds.Width), Math.Max(1, SplitterWidth));
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
        return ClampSplitterDistance(NormalizeSplitterDistance(coordinate - _dragOffset), availablePrimarySize, Math.Max(1, SplitterWidth));
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

    private int NormalizeSplitterDistance(int distance)
    {
        int increment = Math.Max(1, SplitterIncrement);
        return Math.Max(0, (int)Math.Round(distance / (double)increment) * increment);
    }

    private int ResolveSplitterDistanceForResize(int primarySize, int splitterWidth)
    {
        if (_lastPrimarySize <= 0 || primarySize == _lastPrimarySize || _draggingSplitter)
        {
            return _splitterDistance;
        }

        return FixedPanel switch
        {
            FixedPanel.Panel1 => _lastPanel1Size,
            FixedPanel.Panel2 => Math.Max(0, primarySize - splitterWidth - _lastPanel2Size),
            _ => (int)Math.Round(Math.Max(0, primarySize - splitterWidth) * _splitRatio),
        };
    }

    private void LayoutCollapsedPanels(Rectangle contentBounds)
    {
        if (_panel1Collapsed)
        {
            Panel1.Visible = false;
            Panel2.Visible = true;
            Panel2.SetBounds(contentBounds.Left, contentBounds.Top, contentBounds.Width, contentBounds.Height);
            Panel2.PerformLayout();
            return;
        }

        Panel1.Visible = true;
        Panel2.Visible = false;
        Panel1.SetBounds(contentBounds.Left, contentBounds.Top, contentBounds.Width, contentBounds.Height);
        Panel1.PerformLayout();
    }

    private SplitterEventArgs CreateSplitterEventArgs(Point point, int distance)
    {
        return IsVerticalLayout
            ? new SplitterEventArgs(point.X, point.Y, distance, 0)
            : new SplitterEventArgs(point.X, point.Y, 0, distance);
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
