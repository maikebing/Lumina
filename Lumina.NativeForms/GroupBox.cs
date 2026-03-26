namespace Lumina.NativeForms;

/// <summary>
/// Represents a group box used to visually group related controls.
/// </summary>
public class GroupBox : Control
{
    /// <inheritdoc />
    protected override string ClassName => "BUTTON";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.BS_GROUPBOX;
}
