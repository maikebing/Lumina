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
        Height = 30;
    }

    /// <inheritdoc />
    protected override string ClassName => base.ClassName;

    /// <inheritdoc />
    protected override uint Style => base.Style | Win32.WS_CLIPSIBLINGS;

    /// <inheritdoc />
    private protected override bool ShouldCreateHostControl(ToolStripItem item)
        => item is not ToolStripStatusLabel && item is not ToolStripSeparator;

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(18, requestedHeight);

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
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

}
