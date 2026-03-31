namespace Lumina.Forms;

/// <summary>
/// Represents a WinForms-compatible list view backed by the native common control.
/// </summary>
public class ListView : Control
{
    /// <summary>
    /// Gets or sets a value indicating whether the control should preserve legacy image behavior.
    /// </summary>
    public bool UseCompatibleStateImageBehavior { get; set; } = true;

    /// <inheritdoc />
    protected override string ClassName => "SysListView32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_VSCROLL;

    /// <inheritdoc />
    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
    }
}