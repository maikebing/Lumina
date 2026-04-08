using System.Drawing;
using System.Reflection;
using System.Runtime.Versioning;

namespace Lumina.Forms;

internal static class DefaultAppIconProvider
{
    private const string DefaultIconResourceName = "Lumina.Forms.Assets.DefaultAppIcon.ico";
    private static readonly Lock s_syncRoot = new();
    private static MemoryStream? s_defaultIconStream;
    private static Icon? s_defaultIcon;

    internal static Icon? GetIcon()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            return null;
        }

        lock (s_syncRoot)
        {
            s_defaultIcon ??= LoadDefaultIcon();
            return s_defaultIcon;
        }
    }

    internal static nint GetIconHandle()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            return 0;
        }

        return GetIconHandleCore();
    }

    [SupportedOSPlatform("windows6.1")]
    private static Icon LoadDefaultIcon()
    {
        Assembly assembly = typeof(DefaultAppIconProvider).Assembly;
        Stream? resourceStream = assembly.GetManifestResourceStream(DefaultIconResourceName);
        if (resourceStream is null)
        {
            return SystemIcons.Application;
        }

        using (resourceStream)
        {
            s_defaultIconStream = new MemoryStream();
            resourceStream.CopyTo(s_defaultIconStream);
            s_defaultIconStream.Position = 0;
            return new Icon(s_defaultIconStream);
        }
    }

    [SupportedOSPlatform("windows6.1")]
    private static nint GetIconHandleCore()
        => GetIcon()?.Handle ?? 0;
}
