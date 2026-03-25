using System.Runtime.InteropServices;

namespace Lumina.App.GUI;

/// <summary>
/// 系统托盘图标，提供快速启用/禁用切换菜单。
/// </summary>
internal static partial class TrayIcon
{
    // ── Shell_NotifyIcon 结构 ────────────────────────────────────
    private const uint NIM_ADD     = 0;
    private const uint NIM_MODIFY  = 1;
    private const uint NIM_DELETE  = 2;
    private const uint NIF_MESSAGE = 0x01;
    private const uint NIF_ICON    = 0x02;
    private const uint NIF_TIP     = 0x04;
    private const uint NIS_HIDDEN  = 0x01;

    private const uint WM_APP_TRAY = 0x8001;
    private const uint WM_COMMAND  = 0x0111;
    private const uint WM_DESTROY  = 0x0002;
    private const uint WM_CLOSE    = 0x0010;

    // 托盘右键菜单命令 ID
    private const int IDM_SETTINGS  = 1001;
    private const int IDM_TOGGLE    = 1002;
    private const int IDM_AUTOSTART = 1003;
    private const int IDM_EXIT      = 1004;

    private const uint MF_STRING  = 0x00000000;
    private const uint MF_CHECKED = 0x00000008;
    private const uint MF_GRAYED  = 0x00000001;
    private const uint MF_SEPARATOR = 0x00000800;
    private const uint TPM_RIGHTBUTTON = 0x0002;
    private const uint TPM_RETURNCMD   = 0x0100;

    private static bool   _enabled = true;
    private static nint   _hwnd;
    private static NOTIFYICONDATAW _nid;

    internal static void Run()
    {
        const string className = "LuminaTrayHost";

        var wc = new WNDCLASSEXW
        {
            cbSize        = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            lpfnWndProc   = &WndProc,
            hInstance     = GetModuleHandleW(null),
            lpszClassName = className,
        };
        RegisterClassExW(ref wc);

        _hwnd = CreateWindowExW(0, className, "Lumina", 0,
            0, 0, 0, 0, -3 /* HWND_MESSAGE */, 0,
            wc.hInstance, 0);

        AddTrayIcon();

        MSG msg;
        while (GetMessageW(out msg, 0, 0, 0) > 0)
        {
            TranslateMessage(ref msg);
            DispatchMessageW(ref msg);
        }

        RemoveTrayIcon();
    }

    private static void AddTrayIcon()
    {
        _nid = new NOTIFYICONDATAW
        {
            cbSize           = (uint)Marshal.SizeOf<NOTIFYICONDATAW>(),
            hWnd             = _hwnd,
            uID              = 1,
            uFlags           = NIF_MESSAGE | NIF_TIP,
            uCallbackMessage = WM_APP_TRAY,
            szTip            = "Lumina — 窗口特效",
        };
        // 尝试加载内嵌图标，失败则使用应用默认图标
        nint hIcon = LoadIconW(GetModuleHandleW(null), (nint)32512 /* IDI_APPLICATION */);
        if (hIcon != 0) { _nid.hIcon = hIcon; _nid.uFlags |= NIF_ICON; }
        Shell_NotifyIconW(NIM_ADD, ref _nid);
    }

    private static void RemoveTrayIcon() => Shell_NotifyIconW(NIM_DELETE, ref _nid);

    [UnmanagedCallersOnly]
    private static nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        if (msg == WM_APP_TRAY)
        {
            uint mouseMsg = (uint)(lParam & 0xFFFF);
            // 右键弹出菜单
            if (mouseMsg == 0x0205 /* WM_RBUTTONUP */ || mouseMsg == 0x0204 /* WM_RBUTTONDOWN */)
            {
                ShowContextMenu(hWnd);
                return 0;
            }
            // 左键双击打开设置
            if (mouseMsg == 0x0203 /* WM_LBUTTONDBLCLK */)
            {
                SettingsWindow.Show();
                return 0;
            }
        }
        else if (msg == WM_DESTROY)
        {
            PostQuitMessage(0);
            return 0;
        }
        return DefWindowProcW(hWnd, msg, wParam, lParam);
    }

    private static void ShowContextMenu(nint hWnd)
    {
        nint hMenu = CreatePopupMenu();
        AppendMenuW(hMenu, MF_STRING, IDM_SETTINGS, "设置(&S)");
        AppendMenuW(hMenu, MF_SEPARATOR, 0, null);
        uint toggleFlags    = MF_STRING | (_enabled ? MF_CHECKED : 0);
        AppendMenuW(hMenu, toggleFlags, IDM_TOGGLE, "启用特效(&E)");
        uint autoStartFlags = MF_STRING | (Config.AutoStart.IsEnabled ? MF_CHECKED : 0);
        AppendMenuW(hMenu, autoStartFlags, IDM_AUTOSTART, "开机自启(&A)");
        AppendMenuW(hMenu, MF_SEPARATOR, 0, null);
        AppendMenuW(hMenu, MF_STRING, IDM_EXIT, "退出(&X)");

        POINT pt;
        GetCursorPos(out pt);
        SetForegroundWindow(hWnd);

        uint cmd = TrackPopupMenu(hMenu, TPM_RIGHTBUTTON | TPM_RETURNCMD,
            pt.x, pt.y, 0, hWnd, nint.Zero);
        DestroyMenu(hMenu);

        switch (cmd)
        {
            case IDM_SETTINGS:
                SettingsWindow.Show();
                break;
            case IDM_TOGGLE:
                _enabled = !_enabled;
                // TODO: 通知 Lumina.Ext 启用/禁用
                UpdateTrayTip();
                break;
            case IDM_AUTOSTART:
                Config.AutoStart.Toggle();
                break;
            case IDM_EXIT:
                SendMessageW(hWnd, WM_DESTROY, 0, 0);
                break;
        }
    }

    private static void UpdateTrayTip()
    {
        _nid.szTip = _enabled ? "Lumina — 窗口特效" : "Lumina — 已禁用";
        Shell_NotifyIconW(NIM_MODIFY, ref _nid);
    }

    // ── P/Invoke ─────────────────────────────────────────────────
    [DllImport("shell32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool Shell_NotifyIconW(uint dwMessage, ref NOTIFYICONDATAW lpData);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern ushort RegisterClassExW(ref WNDCLASSEXW lpwcx);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowExW(uint dwExStyle, string lpClassName,
        string lpWindowName, uint dwStyle, int X, int Y, int nWidth, int nHeight,
        nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

    [LibraryImport("user32.dll")]
    private static partial int GetMessageW(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(ref MSG lpMsg);

    [LibraryImport("user32.dll")]
    private static partial nint DispatchMessageW(ref MSG lpmsg);

    [LibraryImport("user32.dll")]
    private static partial nint DefWindowProcW(nint hWnd, uint Msg, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial void PostQuitMessage(int nExitCode);

    [LibraryImport("user32.dll")]
    private static partial nint CreatePopupMenu();

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AppendMenuW(nint hMenu, uint uFlags, uint uIDNewItem, string? lpNewItem);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyMenu(nint hMenu);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetCursorPos(out POINT lpPoint);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    private static partial uint TrackPopupMenu(nint hMenu, uint uFlags,
        int x, int y, int nReserved, nint hWnd, nint prcRect);

    [LibraryImport("user32.dll")]
    private static partial nint SendMessageW(nint hWnd, uint Msg, nint wParam, nint lParam);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandleW(string? lpModuleName);

    [LibraryImport("user32.dll")]
    private static partial nint LoadIconW(nint hInstance, nint lpIconName);

    // ── 结构体 ───────────────────────────────────────────────────
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NOTIFYICONDATAW
    {
        public uint   cbSize;
        public nint   hWnd;
        public uint   uID;
        public uint   uFlags;
        public uint   uCallbackMessage;
        public nint   hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public uint   dwState;
        public uint   dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public uint   uTimeoutOrVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public uint   dwInfoFlags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private unsafe struct WNDCLASSEXW
    {
        public uint   cbSize;
        public uint   style;
        public delegate* unmanaged<nint, uint, nint, nint, nint> lpfnWndProc;
        public int    cbClsExtra;
        public int    cbWndExtra;
        public nint   hInstance;
        public nint   hIcon;
        public nint   hCursor;
        public nint   hbrBackground;
        public nint   lpszMenuName;
        public string lpszClassName;
        public nint   hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public nint  hwnd;
        public uint  message;
        public nint  wParam;
        public nint  lParam;
        public uint  time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int x; public int y; }
}
