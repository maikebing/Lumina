using System.Runtime.Versioning;

namespace Lumina.NotepadDemo;

[SupportedOSPlatform("windows6.1")]
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new frmNotepad());
    }
}
