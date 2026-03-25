using System.Runtime.InteropServices;

namespace Lumina.Advanced;

/// <summary>
/// 高级模式入口：通过注入 <c>dwm.exe</c> 解锁深度效果，需要管理员权限。
/// </summary>
public static partial class LuminaAdvanced
{
    private static bool _injected;
    private static readonly HashSet<nint> _excluded = [];

    /// <summary>当前进程是否以管理员权限运行。</summary>
    public static bool IsElevated
    {
        get
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }

    /// <summary>是否已完成注入。</summary>
    public static bool IsInjected => _injected;

    /// <summary>
    /// 将 <c>Lumina.Ext.dll</c> 注入到 <c>dwm.exe</c>，激活高级效果。
    /// </summary>
    /// <param name="extDllPath">
    /// <c>Lumina.Ext.dll</c> 的完整路径。传 <c>null</c> 则从调用程序集所在目录查找。
    /// </param>
    /// <exception cref="InvalidOperationException">未以管理员权限运行，或已注入。</exception>
    /// <exception cref="FileNotFoundException"><c>Lumina.Ext.dll</c> 不存在。</exception>
    public static void Inject(string? extDllPath = null)
    {
        if (!IsElevated)
            throw new InvalidOperationException("Lumina.Advanced.Inject() 需要管理员权限。");
        if (_injected)
            throw new InvalidOperationException("已注入，请勿重复调用。");

        extDllPath ??= Path.Combine(
            AppContext.BaseDirectory, "Lumina.Ext.dll");

        if (!File.Exists(extDllPath))
            throw new FileNotFoundException("找不到 Lumina.Ext.dll。", extDllPath);

        uint pid = FindDwmPid();
        if (pid == 0)
            throw new InvalidOperationException("找不到 dwm.exe 进程。");

        InjectDll(pid, extDllPath);
        _injected = true;
    }

    /// <summary>
    /// 从排除列表中添加一个窗口句柄，使该窗口不受 Hook 影响。
    /// </summary>
    public static void Exclude(nint hwnd)
    {
        if (hwnd != 0) _excluded.Add(hwnd);
    }

    /// <summary>从排除列表中移除窗口句柄。</summary>
    public static void Include(nint hwnd) => _excluded.Remove(hwnd);

    /// <summary>检查窗口是否在排除列表中。</summary>
    public static bool IsExcluded(nint hwnd) => _excluded.Contains(hwnd);

    // ── 注入实现 ─────────────────────────────────────────────────

    private static uint FindDwmPid()
    {
        nint snap = CreateToolhelp32Snapshot(2 /* TH32CS_SNAPPROCESS */, 0);
        if (snap == -1) return 0;
        try
        {
            var entry = new PROCESSENTRY32W { dwSize = (uint)Marshal.SizeOf<PROCESSENTRY32W>() };
            if (!Process32FirstW(snap, ref entry)) return 0;
            do
            {
                if (entry.szExeFile.Equals("dwm.exe", StringComparison.OrdinalIgnoreCase))
                    return entry.th32ProcessID;
            }
            while (Process32NextW(snap, ref entry));
            return 0;
        }
        finally { CloseHandle(snap); }
    }

    private static void InjectDll(uint pid, string dllPath)
    {
        nint hProcess = OpenProcess(0x001F0FFF /* PROCESS_ALL_ACCESS */, false, pid);
        if (hProcess == 0)
            throw new InvalidOperationException($"OpenProcess 失败，错误码 {Marshal.GetLastWin32Error()}。");
        try
        {
            byte[] pathBytes = System.Text.Encoding.Unicode.GetBytes(dllPath + '\0');
            nuint size = (nuint)pathBytes.Length;

            nint mem = VirtualAllocEx(hProcess, 0, size, 0x3000 /* MEM_COMMIT|RESERVE */, 0x04 /* PAGE_READWRITE */);
            if (mem == 0)
                throw new InvalidOperationException($"VirtualAllocEx 失败，错误码 {Marshal.GetLastWin32Error()}。");
            try
            {
                if (!WriteProcessMemory(hProcess, mem, pathBytes, size, out _))
                    throw new InvalidOperationException($"WriteProcessMemory 失败，错误码 {Marshal.GetLastWin32Error()}。");

                nint k32  = GetModuleHandleW("kernel32.dll");
                nint load = GetProcAddress(k32, "LoadLibraryW");

                nint thread = CreateRemoteThread(hProcess, 0, 0, load, mem, 0, out _);
                if (thread == 0)
                    throw new InvalidOperationException($"CreateRemoteThread 失败，错误码 {Marshal.GetLastWin32Error()}。");

                WaitForSingleObject(thread, 0xFFFFFFFF);
                CloseHandle(thread);
            }
            finally { VirtualFreeEx(hProcess, mem, 0, 0x8000 /* MEM_RELEASE */); }
        }
        finally { CloseHandle(hProcess); }
    }

    // ── P/Invoke ─────────────────────────────────────────────────

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool Process32FirstW(nint hSnapshot, ref PROCESSENTRY32W lppe);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool Process32NextW(nint hSnapshot, ref PROCESSENTRY32W lppe);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint OpenProcess(uint dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint VirtualAllocEx(nint hProcess, nint lpAddress,
        nuint dwSize, uint flAllocationType, uint flProtect);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool WriteProcessMemory(nint hProcess, nint lpBaseAddress,
        byte[] lpBuffer, nuint nSize, out nuint lpNumberOfBytesWritten);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes,
        nuint dwStackSize, nint lpStartAddress, nint lpParameter,
        uint dwCreationFlags, out uint lpThreadId);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial uint WaitForSingleObject(nint hHandle, uint dwMilliseconds);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool VirtualFreeEx(nint hProcess, nint lpAddress,
        nuint dwSize, uint dwFreeType);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(nint hObject);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandleW(string lpModuleName);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint GetProcAddress(nint hModule, string lpProcName);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct PROCESSENTRY32W
    {
        internal uint   dwSize;
        internal uint   cntUsage;
        internal uint   th32ProcessID;
        internal nuint  th32DefaultHeapID;
        internal uint   th32ModuleID;
        internal uint   cntThreads;
        internal uint   th32ParentProcessID;
        internal int    pcPriClassBase;
        internal uint   dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string szExeFile;
    }
}
