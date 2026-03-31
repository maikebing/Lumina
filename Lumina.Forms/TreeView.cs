namespace Lumina.Forms;

/// <summary>
/// Represents a tree view backed by the native common control.
/// </summary>
public class TreeView : Control
{
    /// <summary>
    /// Initializes a tree view.
    /// </summary>
    public TreeView()
    {
        Nodes = new TreeNodeCollection();
    }

    /// <summary>
    /// Gets the root nodes displayed by the tree view.
    /// </summary>
    public TreeNodeCollection Nodes { get; }

    /// <inheritdoc />
    protected override string ClassName => "SysTreeView32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_VSCROLL;

    /// <inheritdoc />
    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyExplorerTheme();
    }
}
