using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Displays the operating system about dialog.
/// </summary>
public static class SystemAboutDialog
{
    /// <summary>
    /// Shows the system about dialog.
    /// </summary>
    public static DialogResult Show(string appName, string? additionalInfo = null)
        => Show(null, appName, additionalInfo, null);

    /// <summary>
    /// Shows the system about dialog with owner and icon.
    /// </summary>
    public static DialogResult Show(Form? owner, string appName, string? additionalInfo = null, Icon? icon = null)
    {
        if (string.IsNullOrWhiteSpace(appName))
        {
            appName = "Lumina";
        }

        nint iconHandle = icon?.Handle ?? owner?.Icon?.Handle ?? 0;
        int result = Win32.ShellAboutW(owner?.Handle ?? 0, appName, additionalInfo, iconHandle);
        return result > 0 ? DialogResult.OK : DialogResult.Cancel;
    }
}
