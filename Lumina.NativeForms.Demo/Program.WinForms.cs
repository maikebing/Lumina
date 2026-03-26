using WinFormsApplication = System.Windows.Forms.Application;

namespace Lumina.NativeForms.Demo;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        WinFormsApplication.EnableVisualStyles();
        WinFormsApplication.SetCompatibleTextRenderingDefault(false);
        WinFormsApplication.Run(new WdsScaleSimulatorDemoForm());
    }
}
