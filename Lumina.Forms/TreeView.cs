namespace Lumina.Forms;

/// <summary>
/// Represents a tree view backed by the native common control.
/// </summary>
public class TreeView : Control
{
    private const uint ModernExtendedStyles =
        Win32.TVS_EX_DOUBLEBUFFER |
        Win32.TVS_EX_AUTOHSCROLL |
        Win32.TVS_EX_FADEINOUTEXPANDOS;

    /// <summary>
    /// Initializes a tree view.
    /// </summary>
    public TreeView()
    {
        Margin = new Padding(6);
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
        ApplyNativeColors();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyExtendedStyles();
        ApplyNativeColors();
    }

    private void ApplyExtendedStyles()
    {
        if (Handle == 0)
        {
            return;
        }

        _ = Win32.SendMessageW(
            Handle,
            Win32.TVM_SETEXTENDEDSTYLE,
            (nint)ModernExtendedStyles,
            (nint)ModernExtendedStyles);
    }

    private void ApplyNativeColors()
    {
        if (Handle == 0)
        {
            return;
        }

        ThemePalette palette = CurrentVisualStyle.Palette;
        uint background = Win32.ToColorRef(palette.ControlBackground);
        uint foreground = Win32.ToColorRef(palette.ControlForeground);

        _ = Win32.SendMessageW(Handle, Win32.TVM_SETBKCOLOR, 0, unchecked((nint)background));
        _ = Win32.SendMessageW(Handle, Win32.TVM_SETTEXTCOLOR, 0, unchecked((nint)foreground));
    }
}
