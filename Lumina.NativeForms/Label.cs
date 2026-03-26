namespace Lumina.NativeForms;

public class Label : Control
{
    protected override string ClassName => "STATIC";

    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE;
}
