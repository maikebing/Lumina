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
        Dock = DockStyle.Bottom;
        CanOverflow = false;
        GripStyle = ToolStripGripStyle.Hidden;
        LayoutStyle = ToolStripLayoutStyle.Table;
        ShowItemToolTips = false;
        Stretch = true;
        SizingGrip = true;
        Padding = new Padding(1, 0, 1, 0);
    }

    /// <summary>
    /// Gets or sets whether a sizing grip is shown.
    /// </summary>
    public bool SizingGrip { get; set; }

    /// <inheritdoc />
    protected override string ClassName => base.ClassName;

    /// <inheritdoc />
    protected override uint Style => base.Style | Win32.WS_CLIPSIBLINGS;

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => requestedHeight;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyNativeThemeState();
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        base.ApplyTheme();
        ApplyNativeThemeState();
    }

    /// <inheritdoc />
    private protected override void ApplyNativeThemeState()
    {
        base.ApplyNativeThemeState();

        if (Handle == 0)
        {
            return;
        }

        DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
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
            base.PerformLayout();
        }
        finally
        {
            _performingStatusLayout = false;
        }
    }

    private protected override void LayoutItemHosts()
    {
        int availableHeight = Math.Max(1, Height - Padding.Vertical);
        int x = Padding.Left;

        int springItemCount = 0;
        int fixedWidth = 0;
        foreach (ToolStripItem item in Items)
        {
            if (!TryGetItemHost(item, out Control? host) || host is null)
            {
                continue;
            }

            ApplyItemState(host, item);
            host.Visible = item.Visible;
            if (!item.Visible)
            {
                continue;
            }

            Size hostSize = ResolveHostSize(item, availableHeight);
            if (item is ToolStripStatusLabel { Spring: true })
            {
                springItemCount++;
            }
            else
            {
                fixedWidth += hostSize.Width;
            }
        }

        int springWidth = 0;
        int springRemainder = 0;
        if (springItemCount > 0)
        {
            int remainingWidth = Math.Max(0, Width - Padding.Horizontal - fixedWidth);
            springWidth = remainingWidth / springItemCount;
            springRemainder = remainingWidth % springItemCount;
        }

        foreach (ToolStripItem item in Items)
        {
            if (!TryGetItemHost(item, out Control? host) || host is null)
            {
                continue;
            }

            ApplyItemState(host, item);
            host.Visible = item.Visible;
            if (!item.Visible)
            {
                continue;
            }

            Size hostSize = ResolveHostSize(item, availableHeight);
            int width = hostSize.Width;
            if (item is ToolStripStatusLabel { Spring: true })
            {
                width = Math.Max(width, springWidth + (springRemainder > 0 ? 1 : 0));
                if (springRemainder > 0)
                {
                    springRemainder--;
                }
            }

            int y = Padding.Top + Math.Max(0, (availableHeight - hostSize.Height) / 2);
            host.SetBounds(x, y, width, hostSize.Height);
            x += width;
        }
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

        int height = Math.Max(1, Height);
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

}
