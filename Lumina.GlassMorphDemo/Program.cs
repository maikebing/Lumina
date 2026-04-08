using System.Runtime.Versioning;

namespace Lumina.GlassMorphDemo;

[SupportedOSPlatform("windows6.1")]
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new frmGlassMorphDemo());
    }
}
