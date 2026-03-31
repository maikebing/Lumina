namespace Lumina.Forms;

/// <summary>
/// Represents a simple child-control container panel.
/// </summary>
public class Panel : ContainerControlBase
{
    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_CLIPCHILDREN | Win32.WS_CLIPSIBLINGS;
}
