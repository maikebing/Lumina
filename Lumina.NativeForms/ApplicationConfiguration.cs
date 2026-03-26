namespace Lumina.NativeForms;

/// <summary>
/// Provides a WinForms-style application bootstrap entry point for NativeForms.
/// </summary>
public static class ApplicationConfiguration
{
    /// <summary>
    /// Initializes NativeForms application defaults in a WinForms-compatible way.
    /// </summary>
    /// <remarks>
    /// This method currently enables NativeForms visual styles and applies the default
    /// text-rendering compatibility setting so older WinForms startup code can migrate
    /// with minimal changes.
    /// </remarks>
    public static void Initialize()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
    }
}
