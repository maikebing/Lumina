namespace Lumina.NativeForms;

public class Button : Control
{
    public event EventHandler? Click;

    protected override string ClassName => "BUTTON";

    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.BS_PUSHBUTTON;

    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(24, requestedHeight);

    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.BN_CLICKED)
        {
            return false;
        }

        Click?.Invoke(this, EventArgs.Empty);
        return true;
    }

    protected override void ApplyTheme()
    {
        _ = Win32.SetWindowTheme(Handle, "Explorer", null);
    }
}
