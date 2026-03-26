namespace Lumina.NativeForms;

public static class Application
{
    public static void EnableVisualStyles()
    {
    }

    public static void SetCompatibleTextRenderingDefault(bool defaultValue)
    {
    }

    public static int Run(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        form.Show();
        return RunMessageLoop();
    }

    public static int RunMessageLoop()
    {
        while (Win32.GetMessage(out var message, 0, 0, 0) > 0)
        {
            _ = Win32.TranslateMessage(in message);
            _ = Win32.DispatchMessage(in message);
        }

        return 0;
    }
}
