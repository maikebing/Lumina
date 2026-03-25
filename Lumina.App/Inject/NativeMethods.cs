using System.Runtime.InteropServices;

namespace Lumina.App.Inject;

internal static partial class NativeMethods
{
    // ── 进程枚举 (Toolhelp32) ─────────────────────────────────
    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial nint CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool Process32FirstW(nint hSnapshot, ref PROCESSENTRY32W lppe);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool Process32NextW(nint hSnapshot, ref PROCESSENTRY32W lppe);

    // ── 远程进程 ──────────────────────────────────────────────
    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial nint OpenProcess(uint dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial nint VirtualAllocEx(nint hProcess, nint lpAddress,
        nuint dwSize, uint flAllocationType, uint flProtect);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WriteProcessMemory(nint hProcess, nint lpBaseAddress,
        byte[] lpBuffer, nuint nSize, out nuint lpNumberOfBytesWritten);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes,
        nuint dwStackSize, nint lpStartAddress, nint lpParameter,
        uint dwCreationFlags, out uint lpThreadId);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial uint WaitForSingleObject(nint hHandle, uint dwMilliseconds);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool VirtualFreeEx(nint hProcess, nint lpAddress,
        nuint dwSize, uint dwFreeType);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CloseHandle(nint hObject);

    // ── 模块/符号 ─────────────────────────────────────────────
    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandleW(string lpModuleName);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint GetProcAddress(nint hModule, string lpProcName);

    // ── 权限检查 ──────────────────────────────────────────────
    [LibraryImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool OpenProcessToken(nint ProcessHandle,
        uint DesiredAccess, out nint TokenHandle);

    [LibraryImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetTokenInformation(nint TokenHandle,
        uint TokenInformationClass, out uint TokenInformation,
        uint TokenInformationLength, out uint ReturnLength);

    // ── 常量 ──────────────────────────────────────────────────
    internal const uint TH32CS_SNAPPROCESS   = 0x00000002;
    internal const uint PROCESS_ALL_ACCESS   = 0x1F0FFF;
    internal const uint MEM_COMMIT           = 0x1000;
    internal const uint MEM_RESERVE          = 0x2000;
    internal const uint MEM_RELEASE          = 0x8000;
    internal const uint PAGE_READWRITE       = 0x04;
    internal const uint INFINITE             = 0xFFFFFFFF;
    internal const uint WAIT_TIMEOUT         = 0x00000102;
    internal const uint TOKEN_QUERY          = 0x0008;
    internal const uint TOKEN_ELEVATION      = 20;
    internal const nint INVALID_HANDLE_VALUE = -1;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct PROCESSENTRY32W
{
    internal uint  dwSize;
    internal uint  cntUsage;
    internal uint  th32ProcessID;
    internal nuint th32DefaultHeapID;
    internal uint  th32ModuleID;
    internal uint  cntThreads;
    internal uint  th32ParentProcessID;
    internal int   pcPriClassBase;
    internal uint  dwFlags;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    internal string szExeFile;
}
