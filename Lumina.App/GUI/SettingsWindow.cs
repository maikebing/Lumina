using System.Runtime.InteropServices;

namespace Lumina.App.GUI;

/// <summary>
/// 主设置窗口：效果预设切换、混合色调整、排除列表入口。
/// 纯 Win32 API，兼容 Native AOT。
/// </summary>
internal static partial class SettingsWindow
{
    private const string ClassName = "LuminaSettings";

    // 控件 ID
    private const int IDC_PRESET_LABEL  = 2001;
    private const int IDC_PRESET_COMBO  = 2002;
    private const int IDC_COLOR_LABEL   = 2003;
    private const int IDC_COLOR_BTN     = 2004;
    private const int IDC_EXCLUDE_BTN   = 2005;
    private const int IDC_APPLY_BTN     = 2006;
    private const int IDC_CLOSE_BTN     = 2007;
    private const int IDC_IMPORT_BTN    = 2008;
    private const int IDC_EXPORT_BTN    = 2009;

    private const uint WS_OVERLAPPED   = 0x00000000;
    private const uint WS_CAPTION       = 0x00C00000;
    private const uint WS_SYSMENU       = 0x00080000;
    private const uint WS_MINIMIZEBOX   = 0x00020000;
    private const uint WS_VISIBLE       = 0x10000000;
    private const uint WS_CHILD         = 0x40000000;
    private const uint WS_TABSTOP       = 0x00010000;
    private const uint WS_GROUP         = 0x00020000;
    private const uint CBS_DROPDOWNLIST = 0x0003;
    private const uint BS_PUSHBUTTON    = 0x00000000;
    private const uint BS_DEFPUSHBUTTON = 0x00000001;
    private const uint WM_DESTROY       = 0x0002;
    private const uint WM_COMMAND       = 0x0111;
    private const uint WM_CTLCOLORSTATIC = 0x0138;
    private const uint CB_ADDSTRING     = 0x0143;
    private const uint CB_GETCURSEL     = 0x0147;
    private const uint CB_SETCURSEL     = 0x014E;
    private const int  CW_USEDEFAULT    = unchecked((int)0x80000000);

    // 效果预设顺序与 SystemBackdropType 枚举对应
    private static readonly string[] PresetNames =
        ["无", "模糊 (Acrylic)", "Mica", "Mica Alt", "亚克力(旧)"];

    private static nint _hwnd;
    private static bool _registered;
    private static uint _blendColor = 0x00_80_80_80; // 默认灰色
    private static int  _pendingPreset;
    private static uint _pendingBlendColor;

    internal static void Show()
    {
        if (_hwnd != 0 && IsWindowVisible(_hwnd))
        {
            SetForegroundWindow(_hwnd);
            return;
        }

        EnsureRegistered();

        nint hInst = GetModuleHandleW(null);
        _hwnd = CreateWindowExW(
            0, ClassName, "Lumina 设置",
            WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX | WS_VISIBLE,
            CW_USEDEFAULT, CW_USEDEFAULT, 380, 300,
            0, 0, hInst, 0);

        if (_hwnd == 0) return;

        BuildControls(_hwnd, hInst);
        ShowWindow(_hwnd, 1 /* SW_SHOWNORMAL */);
        UpdateWindow(_hwnd);
    }

    private static void EnsureRegistered()
    {
        if (_registered) return;
        _registered = true;

        var wc = new WNDCLASSEXW
        {
            cbSize        = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            lpfnWndProc   = &WndProc,
            hInstance     = GetModuleHandleW(null),
            hbrBackground = (nint)6, // COLOR_WINDOW+1
            lpszClassName = ClassName,
        };
        RegisterClassExW(ref wc);
    }

    private static void BuildControls(nint hWnd, nint hInst)
    {
        // 标签：预设
        CreateWindowExW(0, "STATIC", "效果预设：",
            WS_CHILD | WS_VISIBLE, 16, 20, 90, 20, hWnd, IDC_PRESET_LABEL, hInst, 0);

        // 下拉框：预设
        nint hCombo = CreateWindowExW(0, "COMBOBOX", null,
            WS_CHILD | WS_VISIBLE | WS_TABSTOP | CBS_DROPDOWNLIST,
            110, 16, 220, 140, hWnd, IDC_PRESET_COMBO, hInst, 0);
        foreach (string name in PresetNames)
            SendMessageW(hCombo, CB_ADDSTRING, 0, name);
        SendMessageW(hCombo, CB_SETCURSEL, 2, 0); // 默认 Mica

        // 标签：混合色
        CreateWindowExW(0, "STATIC", "混合颜色：",
            WS_CHILD | WS_VISIBLE, 16, 60, 90, 20, hWnd, IDC_COLOR_LABEL, hInst, 0);

        // 按钮：选色
        CreateWindowExW(0, "BUTTON", "选择颜色...",
            WS_CHILD | WS_VISIBLE | WS_TABSTOP | BS_PUSHBUTTON,
            110, 56, 120, 26, hWnd, IDC_COLOR_BTN, hInst, 0);

        // 按钮：排除列表
        CreateWindowExW(0, "BUTTON", "窗口排除列表...",
            WS_CHILD | WS_VISIBLE | WS_TABSTOP | BS_PUSHBUTTON,
            16, 100, 140, 26, hWnd, IDC_EXCLUDE_BTN, hInst, 0);

        // 按钮：应用
        CreateWindowExW(0, "BUTTON", "应用",
            WS_CHILD | WS_VISIBLE | WS_TABSTOP | BS_DEFPUSHBUTTON,
            190, 180, 80, 28, hWnd, IDC_APPLY_BTN, hInst, 0);

        // 按钮：关闭
        CreateWindowExW(0, "BUTTON", "关闭",
            WS_CHILD | WS_VISIBLE | WS_TABSTOP | BS_PUSHBUTTON,
            280, 180, 80, 28, hWnd, IDC_CLOSE_BTN, hInst, 0);

        // 按钮：导入配置
        CreateWindowExW(0, "BUTTON", "导入配置...",
            WS_CHILD | WS_VISIBLE | WS_TABSTOP | BS_PUSHBUTTON,
            16, 220, 110, 28, hWnd, IDC_IMPORT_BTN, hInst, 0);

        // 按钮：导出配置
        CreateWindowExW(0, "BUTTON", "导出配置...",
            WS_CHILD | WS_VISIBLE | WS_TABSTOP | BS_PUSHBUTTON,
            136, 220, 110, 28, hWnd, IDC_EXPORT_BTN, hInst, 0);
    }

    [UnmanagedCallersOnly]
    private static nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        if (msg == WM_COMMAND)
        {
            int id = (int)(wParam & 0xFFFF);
            switch (id)
            {
                case IDC_COLOR_BTN:
                    ColorPicker.Pick(hWnd, ref _blendColor);
                    break;
                case IDC_EXCLUDE_BTN:
                    ExclusionList.Show(hWnd);
                    break;
                case IDC_APPLY_BTN:
                    ApplySettings(hWnd);
                    break;
                case IDC_IMPORT_BTN:
                    ImportConfig(hWnd);
                    break;
                case IDC_EXPORT_BTN:
                    ExportConfig(hWnd);
                    break;
                case IDC_CLOSE_BTN:
                    DestroyWindow(hWnd);
                    break;
            }
            return 0;
        }
        if (msg == WM_DESTROY)
        {
            _hwnd = 0;
            return 0;
        }
        return DefWindowProcW(hWnd, msg, wParam, lParam);
    }

    private static void ApplySettings(nint hWnd)
    {
        nint hCombo = GetDlgItem(hWnd, IDC_PRESET_COMBO);
        int sel = (int)SendMessageW(hCombo, CB_GETCURSEL, 0, 0);

        // 将选中的预设索引和混合色保存为待用配置。
        // 实际效果应用通过 IPC 发送到注入的 Lumina.Ext（Phase 4 实现）。
        _pendingPreset    = sel;
        _pendingBlendColor = _blendColor;
        // TODO(Phase 4): 通过命名管道/共享内存将配置推送到 Lumina.Ext
    }

    private static void ImportConfig(nint hWnd)
    {
        Span<char> buf = stackalloc char[260];
        var ofn = new OPENFILENAMEW
        {
            lStructSize = (uint)Marshal.SizeOf<OPENFILENAMEW>(),
            hwndOwner   = hWnd,
            lpstrFilter = "Lumina 配置 (*.json)\0*.json\0所有文件 (*.*)\0*.*\0",
            nFilterIndex = 1,
            Flags       = 0x00001000 /* OFN_FILEMUSTEXIST */,
        };
        unsafe { fixed (char* p = buf) ofn.lpstrFile = p; }
        ofn.nMaxFile = (uint)buf.Length;

        if (!GetOpenFileNameW(ref ofn)) return;
        string path = new string(buf[..buf.IndexOf('\0')]);
        try
        {
            var cfg = Config.AppConfig.Import(path);
            cfg.Save();
            // 更新下拉框预设
            nint hCombo = GetDlgItem(hWnd, IDC_PRESET_COMBO);
            SendMessageW(hCombo, CB_SETCURSEL, (int)cfg.ActiveEffect, 0);
            _blendColor = cfg.BlendColor;
        }
        catch { /* 静默忽略无效文件 */ }
    }

    private static void ExportConfig(nint hWnd)
    {
        Span<char> buf = stackalloc char[260];
        "lumina-config.json".AsSpan().CopyTo(buf);
        var ofn = new OPENFILENAMEW
        {
            lStructSize  = (uint)Marshal.SizeOf<OPENFILENAMEW>(),
            hwndOwner    = hWnd,
            lpstrFilter  = "Lumina 配置 (*.json)\0*.json\0所有文件 (*.*)\0*.*\0",
            nFilterIndex = 1,
            lpstrDefExt  = "json",
            Flags        = 0x00000002 /* OFN_OVERWRITEPROMPT */,
        };
        unsafe { fixed (char* p = buf) ofn.lpstrFile = p; }
        ofn.nMaxFile = (uint)buf.Length;

        if (!GetSaveFileNameW(ref ofn)) return;
        string path = new string(buf[..buf.IndexOf('\0')]);
        try { Config.AppConfig.Load().Export(path); }
        catch { /* 静默忽略 */ }
    }

    // ── P/Invoke ─────────────────────────────────────────────────
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern ushort RegisterClassExW(ref WNDCLASSEXW lpwcx);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint CreateWindowExW(uint dwExStyle, string lpClassName,
        string? lpWindowName, uint dwStyle, int X, int Y, int nWidth, int nHeight,
        nint hWndParent, int hMenu, nint hInstance, nint lpParam);

    [LibraryImport("user32.dll")]
    private static partial nint DefWindowProcW(nint hWnd, uint Msg, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(nint hWnd, int nCmdShow);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UpdateWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsWindowVisible(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetForegroundWindow(nint hWnd);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint SendMessageW(nint hWnd, uint Msg, nint wParam, string lParam);

    [LibraryImport("user32.dll")]
    private static partial nint SendMessageW(nint hWnd, uint Msg, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    private static partial nint GetDlgItem(nint hDlg, int nIDDlgItem);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandleW(string? lpModuleName);

    [DllImport("comdlg32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetOpenFileNameW(ref OPENFILENAMEW lpofn);

    [DllImport("comdlg32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSaveFileNameW(ref OPENFILENAMEW lpofn);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private unsafe struct OPENFILENAMEW
    {
        public uint   lStructSize;
        public nint   hwndOwner;
        public nint   hInstance;
        public string? lpstrFilter;
        public char*  lpstrCustomFilter;
        public uint   nMaxCustFilter;
        public uint   nFilterIndex;
        public char*  lpstrFile;
        public uint   nMaxFile;
        public char*  lpstrFileTitle;
        public uint   nMaxFileTitle;
        public string? lpstrInitialDir;
        public string? lpstrTitle;
        public uint   Flags;
        public ushort nFileOffset;
        public ushort nFileExtension;
        public string? lpstrDefExt;
        public nint   lCustData;
        public nint   lpfnHook;
        public string? lpTemplateName;
        public nint   pvReserved;
        public uint   dwReserved;
        public uint   FlagsEx;
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
}
