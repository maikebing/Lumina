using System.Runtime.InteropServices;

namespace Lumina.Ext.DWM;

internal static partial class NativeMethods
{
    // ── 内存保护 ──────────────────────────────────────────────
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool VirtualProtect(
        nint lpAddress, nuint dwSize,
        uint flNewProtect, out uint lpflOldProtect);

    [LibraryImport("kernel32.dll")]
    internal static partial nint VirtualAlloc(
        nint lpAddress, nuint dwSize,
        uint flAllocationType, uint flProtect);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool VirtualFree(nint lpAddress, nuint dwSize, uint dwFreeType);

    // ── 模块/进程 ────────────────────────────────────────────
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandleW(string lpModuleName);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint GetProcAddress(nint hModule, string lpProcName);

    [LibraryImport("kernel32.dll")]
    internal static partial nint GetCurrentProcess();

    // ── 窗口枚举 ──────────────────────────────────────────────
    internal delegate bool EnumWindowsProc(nint hwnd, nint lParam);

    [DllImport("user32.dll")]
    internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindowVisible(nint hWnd);

    // ── DWM API ──────────────────────────────────────────────
    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmGetWindowAttribute(
        nint hwnd, uint dwAttribute,
        out int pvAttribute, int cbAttribute);

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmSetWindowAttribute(
        nint hwnd, uint dwAttribute,
        in int pvAttribute, int cbAttribute);

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmGetColorizationColor(
        out uint pcrColorization,
        [MarshalAs(UnmanagedType.Bool)] out bool pfOpaqueBlend);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmEnableBlurBehindWindow(
        nint hwnd, ref Backdrops.DWM_BLURBEHIND pBlurBehind);

    // ── 版本 ─────────────────────────────────────────────────
    [DllImport("ntdll.dll")]
    internal static extern int RtlGetVersion(ref OsVersionInfo lpVersionInformation);

    // ── 注册表 ────────────────────────────────────────────────
    internal static readonly nint HKEY_CURRENT_USER = (nint)0x80000001;
    internal const uint KEY_READ = 0x20019;

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern int RegOpenKeyExW(nint hKey, string lpSubKey, uint ulOptions, uint samDesired, out nint phkResult);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern int RegQueryValueExW(nint hKey, string lpValueName, uint lpReserved, out uint lpType, ref uint lpData, ref uint lpcbData);

    [DllImport("advapi32.dll")]
    internal static extern int RegCloseKey(nint hKey);

    // ── 线程/同步 ──────────────────────────────────────────────
    [LibraryImport("kernel32.dll")]
    internal static partial nint CreateThread(nint lpAttr, nuint dwStackSize,
        nint lpStartAddress, nint lpParameter, uint dwCreationFlags, out uint lpThreadId);

    [LibraryImport("kernel32.dll")]
    internal static partial void Sleep(uint dwMilliseconds);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CloseHandle(nint hObject);

    [LibraryImport("kernel32.dll")]
    internal static partial uint WaitForSingleObject(nint hHandle, uint dwMilliseconds);

    // ── 常量 ─────────────────────────────────────────────────
    internal const uint PAGE_EXECUTE_READWRITE = 0x40;
    internal const uint PAGE_EXECUTE_READ      = 0x20;
    internal const uint MEM_COMMIT             = 0x1000;
    internal const uint MEM_RESERVE            = 0x2000;
    internal const uint MEM_RELEASE            = 0x8000;

    // DWMWA
    internal const uint DWMWA_NCRENDERING_ENABLED  = 1;
    internal const uint DWMWA_SYSTEMBACKDROP_TYPE  = 38;
    internal const uint DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    // DWMSBT
    internal const int DWMSBT_NONE         = 0;
    internal const int DWMSBT_MAINWINDOW   = 2; // Mica
    internal const int DWMSBT_TRANSIENTWINDOW = 3; // Acrylic
    internal const int DWMSBT_TABBEDWINDOW = 4; // Mica Alt
}

[StructLayout(LayoutKind.Sequential)]
internal struct OsVersionInfo
{
    internal uint dwOSVersionInfoSize;
    internal uint dwMajorVersion;
    internal uint dwMinorVersion;
    internal uint dwBuildNumber;
    internal uint dwPlatformId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    internal string szCSDVersion;
}
