namespace NativeFormsDemo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
#if NET10_0 && !WINDOWS
            Application.ConfigureVisualStyles(settings =>
            {
                settings.ThemeMode = ThemeMode.System;
                settings.PreferredVisualStyle = VisualStyleKind.System;
                settings.ApplyBackdropEffects = true;
            });
#endif
            Application.Run(new frmMain());
        }
    }
}
