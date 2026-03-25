using System.Runtime.InteropServices;
using Lumina.App.GUI;
using Lumina.App.Inject;

if (!IsElevated())
{
    Console.Error.WriteLine("[Lumina] 需要管理员权限。请以管理员身份运行。");
    return 1;
}

string extDll = Path.Combine(AppContext.BaseDirectory, "Lumina.Ext.dll");
if (!File.Exists(extDll))
{
    Console.Error.WriteLine($"[Lumina] 找不到 {extDll}");
    return 1;
}

uint dwmPid = FindProcessId("dwm.exe");
if (dwmPid == 0)
{
    Console.Error.WriteLine("[Lumina] 找不到 dwm.exe 进程。");
    return 1;
}

Console.WriteLine($"[Lumina] dwm.exe PID = {dwmPid}");

nint hProcess = NativeMethods.OpenProcess(NativeMethods.PROCESS_ALL_ACCESS, false, dwmPid);
if (hProcess == 0)
{
    Console.Error.WriteLine($"[Lumina] OpenProcess 失败，错误码 {Marshal.GetLastWin32Error()}");
    return 1;
}

try
{
    int result = Inject(hProcess, extDll);
    if (result != 0) return result;
}
finally
{
    NativeMethods.CloseHandle(hProcess);
}

Console.WriteLine("[Lumina] 注入成功，进入托盘模式。");
TrayIcon.Run();
return 0;

// ── 注入逻辑 ──────────────────────────────────────────────────
static int Inject(nint hProcess, string dllPath)
{
    byte[] pathBytes = System.Text.Encoding.Unicode.GetBytes(dllPath + '\0');
    nuint size = (nuint)pathBytes.Length;

    nint remoteMem = NativeMethods.VirtualAllocEx(
        hProcess, 0, size,
        NativeMethods.MEM_COMMIT | NativeMethods.MEM_RESERVE,
        NativeMethods.PAGE_READWRITE);

    if (remoteMem == 0)
    {
        Console.Error.WriteLine($"[Lumina] VirtualAllocEx 失败，错误码 {Marshal.GetLastWin32Error()}");
        return 1;
    }

    try
    {
        if (!NativeMethods.WriteProcessMemory(hProcess, remoteMem, pathBytes, size, out _))
        {
            Console.Error.WriteLine($"[Lumina] WriteProcessMemory 失败，错误码 {Marshal.GetLastWin32Error()}");
            return 1;
        }

        nint kernel32 = NativeMethods.GetModuleHandleW("kernel32.dll");
        if (kernel32 == 0)
        {
            Console.Error.WriteLine("[Lumina] 获取 kernel32.dll 句柄失败。");
            return 1;
        }

        nint loadLibrary = NativeMethods.GetProcAddress(kernel32, "LoadLibraryW");
        if (loadLibrary == 0)
        {
            Console.Error.WriteLine("[Lumina] 获取 LoadLibraryW 地址失败。");
            return 1;
        }

        nint hThread = NativeMethods.CreateRemoteThread(
            hProcess, 0, 0, loadLibrary, remoteMem, 0, out _);

        if (hThread == 0)
        {
            Console.Error.WriteLine($"[Lumina] CreateRemoteThread 失败，错误码 {Marshal.GetLastWin32Error()}");
            return 1;
        }

        try
        {
            uint result = NativeMethods.WaitForSingleObject(hThread, 10_000);
            if (result == NativeMethods.WAIT_TIMEOUT)
            {
                Console.Error.WriteLine("[Lumina] 等待注入线程超时。");
                return 1;
            }

            Console.WriteLine("[Lumina] 注入成功。");
            return 0;
        }
        finally
        {
            NativeMethods.CloseHandle(hThread);
        }
    }
    finally
    {
        NativeMethods.VirtualFreeEx(hProcess, remoteMem, 0, NativeMethods.MEM_RELEASE);
    }
}

// ── 进程查找 ──────────────────────────────────────────────────
static uint FindProcessId(string name)
{
    nint snap = NativeMethods.CreateToolhelp32Snapshot(NativeMethods.TH32CS_SNAPPROCESS, 0);
    if (snap == NativeMethods.INVALID_HANDLE_VALUE) return 0;

    try
    {
        var entry = new PROCESSENTRY32W { dwSize = (uint)Marshal.SizeOf<PROCESSENTRY32W>() };
        if (!NativeMethods.Process32FirstW(snap, ref entry)) return 0;

        do
        {
            if (string.Equals(entry.szExeFile, name, StringComparison.OrdinalIgnoreCase))
                return entry.th32ProcessID;
        }
        while (NativeMethods.Process32NextW(snap, ref entry));

        return 0;
    }
    finally
    {
        NativeMethods.CloseHandle(snap);
    }
}

// ── 权限检查 ──────────────────────────────────────────────────
static bool IsElevated()
{
    if (!NativeMethods.OpenProcessToken((nint)(-1), // GetCurrentProcess pseudo-handle
        NativeMethods.TOKEN_QUERY, out nint token))
        return false;

    try
    {
        NativeMethods.GetTokenInformation(token, NativeMethods.TOKEN_ELEVATION,
            out uint elevated, sizeof(uint), out _);
        return elevated != 0;
    }
    finally
    {
        NativeMethods.CloseHandle(token);
    }
}
