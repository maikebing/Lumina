using System.Runtime.InteropServices;

namespace Lumina.App;

/// <summary>
/// 注册未处理异常处理器，崩溃时在 AppData 写入 minidump 文件。
/// </summary>
internal static partial class CrashHandler
{
    private static readonly string DumpDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Lumina", "crashes");

    internal static unsafe void Register()
    {
        SetUnhandledExceptionFilter(&OnUnhandledException);
    }

    [UnmanagedCallersOnly]
    private static uint OnUnhandledException(nint pExceptionInfo)
    {
        try
        {
            Directory.CreateDirectory(DumpDir);
            string path = Path.Combine(DumpDir,
                $"lumina_{DateTime.UtcNow:yyyyMMdd_HHmmss}.dmp");

            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            nint hProcess = GetCurrentProcess();
            uint pid      = GetCurrentProcessId();
            // MiniDumpWithDataSegs | MiniDumpWithHandleData | MiniDumpWithThreadInfo
            const uint dumpType = 0x0001 | 0x0004 | 0x1000;
            MiniDumpWriteDump(hProcess, pid, fs.SafeFileHandle.DangerousGetHandle(),
                dumpType, pExceptionInfo, 0, 0);
        }
        catch { /* 写转储失败时静默，避免递归崩溃 */ }

        return 0; // EXCEPTION_CONTINUE_SEARCH
    }

    [LibraryImport("kernel32.dll")]
    private static partial nint GetCurrentProcess();

    [LibraryImport("kernel32.dll")]
    private static partial uint GetCurrentProcessId();

    [LibraryImport("kernel32.dll")]
    private static unsafe partial nint SetUnhandledExceptionFilter(
        delegate* unmanaged<nint, uint> lpTopLevelExceptionFilter);

    [LibraryImport("dbghelp.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool MiniDumpWriteDump(
        nint hProcess, uint processId, nint hFile,
        uint dumpType, nint exceptionParam,
        nint userStreamParam, nint callbackParam);
}
