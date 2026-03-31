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
}
