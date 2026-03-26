namespace Lumina.NativeForms.Demo;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        return Application.Run(new WdsScaleSimulatorDemoForm());
    }
}
