namespace Lumina.NativeForms;

public class GroupBox : Control
{
    protected override string ClassName => "BUTTON";

    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.BS_GROUPBOX;
}
