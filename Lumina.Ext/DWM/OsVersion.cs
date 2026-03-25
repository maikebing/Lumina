namespace Lumina.Ext.DWM;

internal static class OsVersion
{
    internal static readonly uint BuildNumber;

    static OsVersion()
    {
        var info = new OsVersionInfo { dwOSVersionInfoSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<OsVersionInfo>() };
        NativeMethods.RtlGetVersion(ref info);
        BuildNumber = info.dwBuildNumber;
    }

    internal static bool IsWindows11 => BuildNumber >= 22000;
    internal static bool SupportsSystemBackdrop => BuildNumber >= 22621;
}
