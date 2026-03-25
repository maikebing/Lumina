using System.Runtime.InteropServices;
using Lumina.Ext.DWM;

namespace Lumina.Ext.Hooks;

/// <summary>
/// x64 inline hook，使用 mov rax, addr; jmp rax 跳板（14字节）
/// 替代 minhook，纯 C# AOT unsafe 实现
/// </summary>
internal sealed unsafe class InlineHook : IDisposable
{
    private readonly nint _target;
    private readonly byte[] _originalBytes = new byte[14];
    private nint _trampoline;
    private bool _installed;

    internal InlineHook(nint target)
    {
        _target = target;
    }

    /// <summary>安装 Hook，detour 必须是 [UnmanagedCallersOnly] 函数指针</summary>
    internal void Install(nint detour)
    {
        if (_installed) return;

        // 保存原始字节
        Marshal.Copy(_target, _originalBytes, 0, 14);

        // 分配 trampoline（原始字节 + jmp back）
        _trampoline = NativeMethods.VirtualAlloc(
            0, 32,
            NativeMethods.MEM_COMMIT | NativeMethods.MEM_RESERVE,
            NativeMethods.PAGE_EXECUTE_READWRITE);

        if (_trampoline != 0)
        {
            // 写入原始 14 字节
            Marshal.Copy(_originalBytes, 0, _trampoline, 14);
            // jmp back to target+14
            WriteJmp(_trampoline + 14, _target + 14);
        }

        // 写入跳转补丁
        Patch(_target, detour);
        _installed = true;
    }

    /// <summary>卸载 Hook，恢复原始字节</summary>
    internal void Uninstall()
    {
        if (!_installed) return;
        NativeMethods.VirtualProtect(_target, 14, NativeMethods.PAGE_EXECUTE_READWRITE, out var old);
        Marshal.Copy(_originalBytes, 0, _target, 14);
        NativeMethods.VirtualProtect(_target, 14, old, out _);
        _installed = false;
    }

    /// <summary>原始函数 trampoline 指针（调用原始逻辑用）</summary>
    internal nint Trampoline => _trampoline;

    private static void Patch(nint target, nint dest)
    {
        NativeMethods.VirtualProtect(target, 14, NativeMethods.PAGE_EXECUTE_READWRITE, out var old);
        byte* p = (byte*)target;
        // mov rax, dest (48 B8 <8bytes>)
        p[0] = 0x48; p[1] = 0xB8;
        *(nint*)(p + 2) = dest;
        // jmp rax (FF E0)
        p[10] = 0xFF; p[11] = 0xE0;
        // nop nop (填充对齐)
        p[12] = 0x90; p[13] = 0x90;
        NativeMethods.VirtualProtect(target, 14, old, out _);
    }

    private static void WriteJmp(nint from, nint to)
    {
        byte* p = (byte*)from;
        p[0] = 0x48; p[1] = 0xB8;
        *(nint*)(p + 2) = to;
        p[10] = 0xFF; p[11] = 0xE0;
    }

    public void Dispose()
    {
        Uninstall();
        if (_trampoline != 0)
        {
            NativeMethods.VirtualFree(_trampoline, 0, NativeMethods.MEM_RELEASE);
            _trampoline = 0;
        }
    }
}
