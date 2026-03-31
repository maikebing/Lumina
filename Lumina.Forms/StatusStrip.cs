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
            return 8;
        }

        Size preferredSize = ResolveHostSize(item, availableHeight);
        return item switch
        {
            ToolStripStatusLabel => Math.Max(48, preferredSize.Width),
            ToolStripProgressBar => Math.Max(72, preferredSize.Width + 6),
            ToolStripComboBox => Math.Max(96, preferredSize.Width + 6),
            ToolStripTextBox => Math.Max(72, preferredSize.Width + 6),
            _ => Math.Max(22, preferredSize.Width + 4),
        };
    }

    private void LayoutHostInPart(Control host, ToolStripItem item, int partIndex, int availableHeight)
    {
        var nativeRect = new Win32.RECT();
        _ = Win32.SendMessageW(Handle, Win32.SB_GETRECT, (nint)partIndex, ref nativeRect);

        int innerWidth = Math.Max(1, nativeRect.Width - 4);
        int innerHeight = Math.Max(1, nativeRect.Height - 4);
        Size preferredSize = ResolveHostSize(item, availableHeight);

        int hostWidth = Math.Max(1, Math.Min(innerWidth, preferredSize.Width));
        int hostHeight = Math.Max(1, Math.Min(innerHeight, preferredSize.Height));
        int x = nativeRect.Left + 2;
        int y = nativeRect.Top + Math.Max(0, (nativeRect.Height - hostHeight) / 2);

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

    private readonly record struct PartLayoutPlan(int[] PartRightEdges, int[] ItemPartIndices);
}
