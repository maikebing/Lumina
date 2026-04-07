using System.Runtime.InteropServices;

namespace Lumina.Forms;

internal static class DarkModeCapabilities
{
    private static readonly Lazy<Snapshot> s_snapshot = new(CreateSnapshot);

    internal static Snapshot Current => s_snapshot.Value;

    internal readonly record struct Snapshot(
        bool IsWindows,
        int Major,
        int Minor,
        int Build)
    {
        internal bool IsWindows7Like => IsWindows && Major == 6 && Minor <= 1;
        internal bool IsLegacyWindows => IsWindows && Major < 10;
        internal bool IsWindows10OrGreater => IsWindows && Major >= 10;
    }

    private static Snapshot CreateSnapshot()
    {
        if (!OperatingSystem.IsWindows())
        {
            return new Snapshot(false, 0, 0, 0);
        }

        (int major, int minor, int build) = GetWindowsVersion();
        return new Snapshot(true, major, minor, build);
    }

    private static (int Major, int Minor, int Build) GetWindowsVersion()
    {
        if (TryGetWindowsVersion(out int major, out int minor, out int build))
        {
            return (major, minor, build);
        }

        Version version = Environment.OSVersion.Version;
        return (version.Major, version.Minor, version.Build);
    }

    private static bool TryGetWindowsVersion(out int major, out int minor, out int build)
    {
        major = 0;
        minor = 0;
        build = 0;

        try
        {
            RtlGetNtVersionNumbers(out uint rtlMajor, out uint rtlMinor, out uint rtlBuild);
            major = unchecked((int)rtlMajor);
            minor = unchecked((int)rtlMinor);
            build = unchecked((int)(rtlBuild & ~0xF0000000u));
            return true;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
    }

    [DllImport("ntdll.dll")]
    private static extern void RtlGetNtVersionNumbers(out uint major, out uint minor, out uint buildNumber);
}
