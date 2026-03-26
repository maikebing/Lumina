using System.Runtime.InteropServices;
using System.Text;

namespace Lumina.NativeForms;

internal static class Win32
{
    public const int CW_USEDEFAULT = unchecked((int)0x80000000);

    public const uint CS_VREDRAW = 0x0001;
    public const uint CS_HREDRAW = 0x0002;

    public const uint WS_OVERLAPPED = 0x00000000;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_CLIPCHILDREN = 0x02000000;
    public const uint WS_CAPTION = 0x00C00000;
    public const uint WS_SYSMENU = 0x00080000;
    public const uint WS_THICKFRAME = 0x00040000;
    public const uint WS_MINIMIZEBOX = 0x00020000;
    public const uint WS_MAXIMIZEBOX = 0x00010000;
    public const uint WS_VSCROLL = 0x00200000;
    public const uint WS_TABSTOP = 0x00010000;

    public const uint WS_OVERLAPPEDWINDOW =
        WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;

    public const uint WS_EX_APPWINDOW = 0x00040000;
    public const uint WS_EX_CLIENTEDGE = 0x00000200;

    public const uint ES_LEFT = 0x0000;
    public const uint ES_MULTILINE = 0x0004;
    public const uint ES_AUTOVSCROLL = 0x0040;
    public const uint ES_AUTOHSCROLL = 0x0080;
    public const uint ES_READONLY = 0x0800;
    public const uint ES_WANTRETURN = 0x1000;

    public const uint CBS_DROPDOWNLIST = 0x0003;
    public const uint CBS_DROPDOWN = 0x0002;
    public const uint CBS_SIMPLE = 0x0001;

    public const uint BS_PUSHBUTTON = 0x00000000;
    public const uint BS_AUTOCHECKBOX = 0x00000003;
    public const uint BS_GROUPBOX = 0x00000007;

    public const int SW_SHOW = 5;
    public const int SW_HIDE = 0;

    public const int WM_SIZE = 0x0005;
    public const int WM_COMMAND = 0x0111;
    public const int WM_DESTROY = 0x0002;
    public const int WM_NCCREATE = 0x0081;
    public const int WM_NCDESTROY = 0x0082;
    public const int WM_SETFONT = 0x0030;

    public const int BN_CLICKED = 0;
    public const int CBN_SELCHANGE = 1;
    public const int BM_GETCHECK = 0x00F0;
    public const int BM_SETCHECK = 0x00F1;
    public const int BST_UNCHECKED = 0;
    public const int BST_CHECKED = 1;

    public const int CB_ADDSTRING = 0x0143;
    public const int CB_RESETCONTENT = 0x014B;
    public const int CB_GETCURSEL = 0x0147;
    public const int CB_SETCURSEL = 0x014E;

    public const int COLOR_WINDOW = 5;
    public const int DEFAULT_GUI_FONT = 17;
    public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    public const int IDC_ARROW = 32512;
    public const int GWLP_USERDATA = -21;

    [StructLayout(LayoutKind.Sequential)]
    internal struct WNDCLASSEXW
    {
        public uint cbSize;
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string? lpszMenuName;
        public string? lpszClassName;
        public nint hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CREATESTRUCTW
    {
        public nint lpCreateParams;
        public nint hInstance;
        public nint hMenu;
        public nint hwndParent;
        public int cy;
        public int cx;
        public int y;
        public int x;
        public uint style;
        public nint lpszName;
        public nint lpszClass;
        public uint dwExStyle;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public nint hwnd;
        public uint message;
        public nuint wParam;
        public nint lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width => Right - Left;

        public int Height => Bottom - Top;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate nint WindowProc(nint hwnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern ushort RegisterClassExW(ref WNDCLASSEXW lpwcx);

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern nint CreateWindowExW(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nint lpParam);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
    internal static extern nint DefWindowProcW(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyWindow(nint hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UpdateWindow(nint hWnd);

    [DllImport("user32.dll", EntryPoint = "GetMessageW", SetLastError = true)]
    internal static extern int GetMessage(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool TranslateMessage(in MSG lpMsg);

    [DllImport("user32.dll", EntryPoint = "DispatchMessageW")]
    internal static extern nint DispatchMessage(in MSG lpMsg);

    [DllImport("user32.dll")]
    internal static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll", EntryPoint = "LoadCursorW", SetLastError = true)]
    internal static extern nint LoadCursorW(nint hInstance, nint lpCursorName);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    internal static extern nint GetWindowLongPtrW(nint hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    internal static extern nint SetWindowLongPtrW(nint hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
    internal static extern nint SendMessageW(nint hWnd, int msg, nint wParam, string lParam);

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    internal static extern nint SendMessageW(nint hWnd, int msg, nint wParam, nint lParam);

    [DllImport("user32.dll", EntryPoint = "MoveWindow", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool MoveWindow(nint hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnableWindow(nint hWnd, bool bEnable);

    [DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowTextW(nint hWnd, string lpString);

    [DllImport("user32.dll", EntryPoint = "GetWindowTextLengthW", SetLastError = true)]
    internal static extern int GetWindowTextLengthW(nint hWnd);

    [DllImport("user32.dll", EntryPoint = "GetWindowTextW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int GetWindowTextW(nint hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetClientRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern nint GetSysColorBrush(int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool InvalidateRect(nint hWnd, nint lpRect, bool bErase);

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern nint GetModuleHandleW(string? lpModuleName);

    [DllImport("gdi32.dll", SetLastError = true)]
    internal static extern nint GetStockObject(int i);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SetWindowTheme(nint hwnd, string? pszSubAppName, string? pszSubIdList);

    [DllImport("dwmapi.dll")]
    internal static extern int DwmSetWindowAttribute(nint hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    internal static int LowWord(nint value) => unchecked((ushort)(nuint)value);

    internal static int HighWord(nint value) => unchecked((ushort)(((nuint)value >> 16) & 0xFFFF));

    internal static string GetText(nint hwnd)
    {
        int length = GetWindowTextLengthW(hwnd);
        if (length <= 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder(length + 1);
        _ = GetWindowTextW(hwnd, builder, builder.Capacity);
        return builder.ToString();
    }
}
