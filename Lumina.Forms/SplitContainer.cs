using System.ComponentModel;

namespace Lumina.Forms;

/// <summary>
/// Represents a two-panel split container.
/// </summary>
public class SplitContainer : ContainerControlBase, ISupportInitialize
{
    private bool _initializing;
    private int _splitterDistance = 100;

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
    /// Gets or sets the splitter distance measured from the left edge.
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

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE;

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
    public override void PerformLayout()
    {
        LayoutPanels();
        base.PerformLayout();
    }

    private void LayoutPanels()
    {
        if (_initializing)
        {
            return;
        }

        int splitterWidth = 4;
        int firstWidth = Math.Clamp(_splitterDistance, 0, Math.Max(0, Width - splitterWidth));
        int secondLeft = Math.Min(Width, firstWidth + splitterWidth);
        int secondWidth = Math.Max(0, Width - secondLeft);

        Panel1.SetBounds(0, 0, Math.Max(1, firstWidth), Math.Max(1, Height));
        Panel2.SetBounds(secondLeft, 0, Math.Max(1, secondWidth), Math.Max(1, Height));
        Panel1.PerformLayout();
        Panel2.PerformLayout();
    }
}