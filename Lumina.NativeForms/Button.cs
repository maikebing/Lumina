namespace Lumina.NativeForms;

/// <summary>
/// Represents a standard push button control.
/// </summary>
public class Button : Control
{
    /// <summary>
    /// Occurs when the user activates the button.
    /// </summary>
    public event EventHandler? Click;

    /// <inheritdoc />
    protected override string ClassName => "BUTTON";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.BS_PUSHBUTTON;

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(24, requestedHeight);

    /// <inheritdoc />
    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.BN_CLICKED)
        {
            return false;
        }

        Click?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
    }
}
