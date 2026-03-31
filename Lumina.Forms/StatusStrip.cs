using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a status bar hosted on a form.
/// </summary>
public class StatusStrip : ToolStrip
{
    private bool _performingStatusLayout;

    /// <summary>
    /// Initializes a status strip with the standard status-bar height.
    /// </summary>
    public StatusStrip()
    {
        Height = 22;
    }

    /// <inheritdoc />
    protected override string ClassName => OperatingSystem.IsWindows()
        ? "msctls_statusbar32"
        : base.ClassName;

    /// <inheritdoc />
    protected override uint Style => OperatingSystem.IsWindows()
        ? Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_CLIPSIBLINGS
        : base.Style;

    /// <inheritdoc />
    private protected override bool ShouldCreateHostControl(ToolStripItem item)
        => item is not ToolStripStatusLabel && item is not ToolStripSeparator;

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(18, requestedHeight);

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        ApplyNativeMetrics();
        base.OnHandleCreated();
        ApplyNativeMetrics();
    }

    /// <inheritdoc />
    protected override void OnBoundsChanged()
    {
        if (_performingStatusLayout)
        {
            return;
        }

        PerformLayout();
    }

    /// <inheritdoc />
    protected override bool HandleWindowMessage(uint message, nint wParam, nint lParam, out nint result)
    {
        if (ContextMenuStrip is not null)
        {
            if (message == Win32.WM_CONTEXTMENU
                && ShouldHandleBlankAreaContextMenu(lParam)
                && TryShowAttachedContextMenu(Handle, lParam))
            {
                result = 0;
                return true;
            }

            if (message == Win32.WM_RBUTTONUP
                && IsBlankAreaClientPoint(ExtractPoint(lParam))
                && TryShowAttachedContextMenuFromClientPoint(Handle, lParam))
            {
                result = 0;
                return true;
            }
        }

        return base.HandleWindowMessage(message, wParam, lParam, out result);
    }

    /// <inheritdoc />
    public override void PerformLayout()
    {
        if (_performingStatusLayout)
        {
            return;
        }

        _performingStatusLayout = true;
        try
        {
            DockToBottom();
            EnsureItemHosts();

            if (!OperatingSystem.IsWindows() || Handle == 0)
            {
                base.PerformLayout();
                return;
            }

            ApplyNativeMetrics();
            LayoutNativeStatusBar();

            foreach (Control child in ChildControls)
            {
                child.PerformLayout();
            }
        }
        finally
        {
            _performingStatusLayout = false;
        }
    }

    private void LayoutNativeStatusBar()
    {
        List<ToolStripItem> visibleItems = [];
        foreach (ToolStripItem item in Items)
        {
            if (!item.Visible)
            {
                if (TryGetItemHost(item, out Control? hiddenHost) && hiddenHost is not null)
                {
                    hiddenHost.Visible = false;
                }

                continue;
            }

            visibleItems.Add(item);
        }

        int availableHeight = Math.Max(1, Height - 6);
        PartLayoutPlan layoutPlan = CreatePartLayoutPlan(visibleItems, availableHeight);

        _ = Win32.SendMessageW(Handle, Win32.SB_SETPARTS, (nint)layoutPlan.PartRightEdges.Length, layoutPlan.PartRightEdges);

        for (int partIndex = 0; partIndex < layoutPlan.ItemPartIndices.Length; partIndex++)
        {
            ToolStripItem item = visibleItems[partIndex];
            int nativePartIndex = layoutPlan.ItemPartIndices[partIndex];

            if (item is ToolStripStatusLabel)
            {
                _ = Win32.SendMessageW(Handle, Win32.SB_SETTEXTW, (nint)nativePartIndex, item.Text ?? string.Empty);
            }
            else
            {
                _ = Win32.SendMessageW(Handle, Win32.SB_SETTEXTW, (nint)nativePartIndex, string.Empty);
            }

            if (!TryGetItemHost(item, out Control? host) || host is null)
            {
                continue;
            }

            ApplyItemState(host, item);
            host.ContextMenuStrip = ContextMenuStrip;
            LayoutHostInPart(host, item, nativePartIndex, availableHeight);
        }

        for (int fillerIndex = layoutPlan.ItemPartIndices.Length; fillerIndex < layoutPlan.PartRightEdges.Length; fillerIndex++)
        {
            _ = Win32.SendMessageW(Handle, Win32.SB_SETTEXTW, (nint)fillerIndex, string.Empty);
        }
    }

    private PartLayoutPlan CreatePartLayoutPlan(List<ToolStripItem> visibleItems, int availableHeight)
    {
        if (visibleItems.Count == 0)
        {
            return new PartLayoutPlan([-1], []);
        }

        List<int> rightEdges = [];
        List<int> itemPartIndices = new(visibleItems.Count);
        int runningRight = 0;

        foreach (ToolStripItem item in visibleItems)
        {
            runningRight += ResolvePartWidth(item, availableHeight);
            rightEdges.Add(runningRight);
            itemPartIndices.Add(rightEdges.Count - 1);
        }

        bool lastItemUsesHostedControl = TryGetItemHost(visibleItems[^1], out Control? lastHost)
            && lastHost is not null;

        if (!lastItemUsesHostedControl)
        {
            rightEdges[^1] = -1;
        }
        else if (runningRight < Math.Max(1, Width))
        {
            rightEdges.Add(-1);
        }

        return new PartLayoutPlan([.. rightEdges], [.. itemPartIndices]);
    }

    private int ResolvePartWidth(ToolStripItem item, int availableHeight)
    {
        if (item is ToolStripSeparator)
        {
            return 10;
        }

        Size preferredSize = ResolveHostSize(item, availableHeight);
        return item switch
        {
            ToolStripStatusLabel => Math.Max(56, preferredSize.Width + 10),
            ToolStripProgressBar => Math.Max(84, preferredSize.Width + 10),
            ToolStripComboBox => Math.Max(104, preferredSize.Width + 10),
            ToolStripTextBox => Math.Max(84, preferredSize.Width + 10),
            _ when IsImageOnlyItem(item)
                => Math.Max(28, preferredSize.Width + 10),
            _ => Math.Max(26, preferredSize.Width + 8),
        };
    }

    private void LayoutHostInPart(Control host, ToolStripItem item, int partIndex, int availableHeight)
    {
        var nativeRect = new Win32.RECT();
        _ = Win32.SendMessageW(Handle, Win32.SB_GETRECT, (nint)partIndex, ref nativeRect);

        int leftInset;
        int rightInset;
        int topInset;
        int bottomInset;

        switch (item)
        {
            case ToolStripProgressBar:
            case ToolStripComboBox:
            case ToolStripTextBox:
                leftInset = 4;
                rightInset = 4;
                topInset = 2;
                bottomInset = 2;
                break;

            default:
                if (IsImageOnlyItem(item))
                {
                    leftInset = 5;
                    rightInset = 5;
                    topInset = 2;
                    bottomInset = 2;
                }
                else
                {
                    leftInset = 3;
                    rightInset = 3;
                    topInset = 2;
                    bottomInset = 2;
                }

                break;
        }

        int innerWidth = Math.Max(1, nativeRect.Width - leftInset - rightInset);
        int innerHeight = Math.Max(1, nativeRect.Height - topInset - bottomInset);
        Size preferredSize = ResolveHostSize(item, availableHeight);

        int hostWidth = Math.Max(1, Math.Min(innerWidth, preferredSize.Width));
        int hostHeight = Math.Max(1, Math.Min(innerHeight, preferredSize.Height));
        int x = nativeRect.Left + leftInset;
        if (IsImageOnlyItem(item))
        {
            x = nativeRect.Left + Math.Max(leftInset, (nativeRect.Width - hostWidth) / 2);
        }

        int y = nativeRect.Top + topInset + Math.Max(0, (innerHeight - hostHeight) / 2);

        host.SetBounds(x, y, hostWidth, hostHeight);
        host.Visible = item.Visible;
    }

    private void ApplyNativeMetrics()
    {
        if (!OperatingSystem.IsWindows() || Handle == 0)
        {
            return;
        }

        int minHeight = Math.Max(18, Height - 2);
        _ = Win32.SendMessageW(Handle, Win32.SB_SETMINHEIGHT, (nint)minHeight, 0);
        _ = Win32.SendMessageW(Handle, Win32.WM_SIZE, 0, 0);
    }

    private void DockToBottom()
    {
        if (Owner is null)
        {
            return;
        }

        int clientWidth = Owner.ClientSize.Width;
        int clientHeight = Owner.ClientSize.Height;
        if (Owner.Handle != 0 && Win32.GetClientRect(Owner.Handle, out var clientRect))
        {
            clientWidth = clientRect.Width;
            clientHeight = clientRect.Height;
        }

        int height = Math.Max(18, Height);
        int width = Math.Max(1, clientWidth);
        int top = Math.Max(0, clientHeight - height);

        if (Left == 0 && Top == top && Width == width && Height == height)
        {
            return;
        }

        SetBounds(0, top, width, height);
    }

    private bool ShouldHandleBlankAreaContextMenu(nint lParam)
    {
        if (lParam == (nint)(-1))
        {
            return true;
        }

        return !IsPointOverHostedControl(ExtractPoint(lParam), useScreenCoordinates: true);
    }

    private bool IsBlankAreaClientPoint(Point clientPoint)
        => !IsPointOverHostedControl(clientPoint, useScreenCoordinates: false);

    private bool IsPointOverHostedControl(Point point, bool useScreenCoordinates)
    {
        foreach (Control child in ChildControls)
        {
            if (!child.Visible)
            {
                continue;
            }

            Rectangle bounds = child.Bounds;
            if (useScreenCoordinates && child.Handle != 0 && Win32.GetWindowRect(child.Handle, out var rect))
            {
                bounds = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
            }

            if (bounds.Contains(point))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsImageOnlyItem(ToolStripItem item)
        => OperatingSystem.IsWindows()
            && item.DisplayStyle == ToolStripItemDisplayStyle.Image
            && item.Image is not null;

    private readonly record struct PartLayoutPlan(int[] PartRightEdges, int[] ItemPartIndices);
}
