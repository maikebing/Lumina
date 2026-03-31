using System.Buffers;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

internal static class Win32
{
    public const int CW_USEDEFAULT = unchecked((int)0x80000000);

    public const uint CS_VREDRAW = 0x0001;
    public const uint CS_HREDRAW = 0x0002;

    public const uint WS_OVERLAPPED = 0x00000000;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_CLIPCHILDREN = 0x02000000;
    public const uint WS_CLIPSIBLINGS = 0x04000000;
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
    public const uint BS_AUTORADIOBUTTON = 0x00000009;
    public const uint BS_GROUPBOX = 0x00000007;
    public const uint LBS_NOTIFY = 0x0001;
    public const uint SS_NOTIFY = 0x00000100;
    public const uint SS_BITMAP = 0x0000000E;

    public const int SW_SHOW = 5;
    public const int SW_HIDE = 0;

    public const int WM_SIZE = 0x0005;
    public const int WM_NOTIFY = 0x004E;
    public const int WM_KEYDOWN = 0x0100;
    public const int WM_SETTINGCHANGE = 0x001A;
    public const int WM_COMMAND = 0x0111;
    public const int WM_CONTEXTMENU = 0x007B;
    public const int WM_SYSCHAR = 0x0106;
    public const int WM_SYSKEYDOWN = 0x0104;
    public const int WM_DESTROY = 0x0002;
    public const int WM_NCCREATE = 0x0081;
    public const int WM_NCDESTROY = 0x0082;
    public const int WM_SETFONT = 0x0030;
    public const int WM_THEMECHANGED = 0x031A;

    public const int BN_CLICKED = 0;
    public const int STN_CLICKED = 0;
    public const int CBN_SELCHANGE = 1;
    public const int LBN_SELCHANGE = 1;
    public const int EN_CHANGE = 0x0300;
    public const int BM_GETCHECK = 0x00F0;
    public const int BM_SETCHECK = 0x00F1;
    public const int BST_UNCHECKED = 0;
    public const int BST_CHECKED = 1;

    public const int CB_ADDSTRING = 0x0143;
    public const int CB_RESETCONTENT = 0x014B;
    public const int CB_GETCURSEL = 0x0147;
    public const int CB_SETCURSEL = 0x014E;
    public const int EM_SETSEL = 0x00B1;
    public const int EM_SETREADONLY = 0x00CF;
    public const int EM_REPLACESEL = 0x00C2;
    public const int LB_ADDSTRING = 0x0180;
    public const int LB_RESETCONTENT = 0x0184;
    public const int LB_GETCURSEL = 0x0188;
    public const int LB_SETCURSEL = 0x0186;
    public const int PBM_SETPOS = 0x0402;
    public const int STM_SETIMAGE = 0x0172;

    public const int TCM_FIRST = 0x1300;
    public const int TCM_GETITEMCOUNT = TCM_FIRST + 4;
    public const int TCM_DELETEALLITEMS = TCM_FIRST + 9;
    public const int TCM_GETCURSEL = TCM_FIRST + 11;
    public const int TCM_SETCURSEL = TCM_FIRST + 12;
    public const int TCM_ADJUSTRECT = TCM_FIRST + 40;
    public const int TCM_SETITEMW = TCM_FIRST + 61;
    public const int TCM_INSERTITEMW = TCM_FIRST + 62;

    public const int TCN_FIRST = -550;
    public const int TCN_SELCHANGE = TCN_FIRST - 1;

    public const uint TCIF_TEXT = 0x0001;

    public const uint IMAGE_BITMAP = 0;
    public const uint IMAGE_ICON = 1;
    public const uint LR_DEFAULTSIZE = 0x00000040;
    public const uint LR_LOADFROMFILE = 0x00000010;

    public const int COLOR_WINDOW = 5;
    public const int COLOR_BTNFACE = 15;
    public const int DEFAULT_GUI_FONT = 17;
    public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    public const int IDC_ARROW = 32512;
    public const int VK_F10 = 0x79;
    public const int VK_SHIFT = 0x10;
    public const int VK_CONTROL = 0x11;
    public const int VK_MENU = 0x12;
    public const int VK_LEFT = 0x25;
    public const int VK_RIGHT = 0x27;
    public const int VK_ESCAPE = 0x1B;
    public const int GWLP_WNDPROC = -4;
    public const int GWLP_USERDATA = -21;
    public const int LOGPIXELSX = 88;
    public const int LOGPIXELSY = 90;

    public const uint MF_STRING = 0x00000000;
    public const uint MF_GRAYED = 0x00000001;
    public const uint MF_CHECKED = 0x00000008;
    public const uint MF_SEPARATOR = 0x00000800;
    public const uint MF_POPUP = 0x00000010;
    public const uint TPM_RIGHTBUTTON = 0x0002;
    public const uint TPM_RETURNCMD = 0x0100;

    public const uint MIIM_STATE = 0x00000001;
    public const uint MIIM_ID = 0x00000002;
    public const uint MIIM_SUBMENU = 0x00000004;
    public const uint MIIM_STRING = 0x00000040;
    public const uint MIIM_BITMAP = 0x00000080;
    public const uint MIIM_FTYPE = 0x00000100;

    public const uint MFT_STRING = 0x00000000;
    public const uint MFT_SEPARATOR = 0x00000800;
    public const uint MFT_RADIOCHECK = 0x00000200;

    public const uint MFS_DISABLED = 0x00000003;
    public const uint MFS_CHECKED = 0x00000008;

    public const int WH_MSGFILTER = -1;
    public const int MSGF_MENU = 2;

    public const int WM_INITMENUPOPUP = 0x0117;
    public const int WM_UNINITMENUPOPUP = 0x0125;
    public const int WM_MENUSELECT = 0x011F;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct MENUITEMINFOW
    {
        public uint cbSize;
        public uint fMask;
        public uint fType;
        public uint fState;
        public uint wID;
        public nint hSubMenu;
        public nint hbmpChecked;
        public nint hbmpUnchecked;
        public nuint dwItemData;
        public string? dwTypeData;
        public uint cch;
        public nint hbmpItem;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NMHDR
    {
        public nint hwndFrom;
        public nuint idFrom;
        public uint code;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct TCITEMW
    {
        public uint mask;
        public uint dwState;
        public uint dwStateMask;
        public nint pszText;
        public int cchTextMax;
        public int iImage;
        public nint lParam;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct TEXTMETRICW
    {
        public int tmHeight;
        public int tmAscent;
        public int tmDescent;
        public int tmInternalLeading;
        public int tmExternalLeading;
        public int tmAveCharWidth;
        public int tmMaxCharWidth;
        public int tmWeight;
        public int tmOverhang;
        public int tmDigitizedAspectX;
        public int tmDigitizedAspectY;
        public char tmFirstChar;
        public char tmLastChar;
        public char tmDefaultChar;
        public char tmBreakChar;
        public byte tmItalic;
        public byte tmUnderlined;
        public byte tmStruckOut;
        public byte tmPitchAndFamily;
        public byte tmCharSet;
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

    [DllImport("user32.dll", EntryPoint = "CallWindowProcW")]
    internal static extern nint CallWindowProcW(nint lpPrevWndFunc, nint hWnd, uint msg, nint wParam, nint lParam);

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

    [DllImport("user32.dll", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern nint LoadImageW(nint hInst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyIcon(nint hIcon);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    internal static extern nint GetWindowLongPtrW(nint hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    internal static extern nint SetWindowLongPtrW(nint hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
    internal static extern nint SendMessageW(nint hWnd, int msg, nint wParam, string lParam);

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    internal static extern nint SendMessageW(nint hWnd, int msg, nint wParam, nint lParam);

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    internal static extern nint SendMessageW(nint hWnd, int msg, nint wParam, ref RECT lParam);

    [DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
    internal static extern nint SendMessageW(nint hWnd, int msg, nint wParam, ref TCITEMW lParam);

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
    private static extern unsafe int GetWindowTextW(nint hWnd, char* lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetClientRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern nint GetDC(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern int ReleaseDC(nint hWnd, nint hDc);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern nint GetSysColorBrush(int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool InvalidateRect(nint hWnd, nint lpRect, bool bErase);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll")]
    internal static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll")]
    internal static extern nint CreatePopupMenu();

    [DllImport("user32.dll")]
    internal static extern nint CreateMenu();

    [DllImport("user32.dll", EntryPoint = "AppendMenuW", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AppendMenuW(nint hMenu, uint uFlags, nuint uIDNewItem, string? lpNewItem);

    [DllImport("user32.dll", EntryPoint = "InsertMenuItemW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool InsertMenuItemW(nint hMenu, uint item, bool fByPosition, ref MENUITEMINFOW menuItemInfo);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyMenu(nint hMenu);

    [DllImport("user32.dll")]
    internal static extern uint TrackPopupMenu(nint hMenu, uint uFlags, int x, int y, int nReserved, nint hWnd, nint prcRect);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetMenu(nint hWnd, nint hMenu);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DrawMenuBar(nint hWnd);

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern nint GetModuleHandleW(string? lpModuleName);

    [DllImport("gdi32.dll", SetLastError = true)]
    internal static extern nint GetStockObject(int i);

    [DllImport("gdi32.dll", SetLastError = true)]
    internal static extern nint SelectObject(nint hdc, nint h);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetTextMetricsW(nint hdc, out TEXTMETRICW lptm);

    [DllImport("gdi32.dll", SetLastError = true)]
    internal static extern int GetDeviceCaps(nint hdc, int index);

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject(nint hObject);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SetWindowTheme(nint hwnd, string? pszSubAppName, string? pszSubIdList);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern nint SetWindowsHookExW(int idHook, nint lpfn, nint hmod, uint dwThreadId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll")]
    internal static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll")]
    internal static extern uint GetCurrentThreadId();

    [DllImport("dwmapi.dll")]
    internal static extern int DwmSetWindowAttribute(nint hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("user32.dll")]
    internal static extern uint GetDpiForSystem();

    internal static int LowWord(nint value) => unchecked((ushort)(nuint)value);

    internal static int HighWord(nint value) => unchecked((ushort)(((nuint)value >> 16) & 0xFFFF));

    internal static unsafe string GetText(nint hwnd)
    {
        int length = GetWindowTextLengthW(hwnd);
        if (length <= 0)
        {
            return string.Empty;
        }

        int bufferLength = length + 1;
        if (bufferLength <= InlineCharBuffer.Capacity)
        {
            InlineCharBuffer inlineBuffer = default;
            Span<char> buffer = inlineBuffer;
            int written = FillWindowText(hwnd, buffer[..bufferLength]);
            return written <= 0 ? string.Empty : new string(buffer[..written]);
        }

        char[] rented = ArrayPool<char>.Shared.Rent(bufferLength);
        try
        {
            Span<char> buffer = rented.AsSpan(0, bufferLength);
            int written = FillWindowText(hwnd, buffer);
            return written <= 0 ? string.Empty : new string(buffer[..written]);
        }
        finally
        {
            rented.AsSpan(0, bufferLength).Clear();
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    private static unsafe int FillWindowText(nint hwnd, Span<char> buffer)
    {
        ref char firstCharacter = ref MemoryMarshal.GetReference(buffer);
        fixed (char* bufferPointer = &firstCharacter)
        {
            return GetWindowTextW(hwnd, bufferPointer, buffer.Length);
        }
    }

    internal static SizeF GetSystemDpiScaleDimensions()
    {
        try
        {
            uint dpi = GetDpiForSystem();
            if (dpi > 0)
            {
                return new SizeF(dpi, dpi);
            }
        }
        catch (EntryPointNotFoundException)
        {
        }

        nint screenDc = GetDC(0);
        if (screenDc == 0)
        {
            return new SizeF(96f, 96f);
        }

        try
        {
            int dpiX = GetDeviceCaps(screenDc, LOGPIXELSX);
            int dpiY = GetDeviceCaps(screenDc, LOGPIXELSY);
            return new SizeF(
                dpiX > 0 ? dpiX : 96f,
                dpiY > 0 ? dpiY : 96f);
        }
        finally
        {
            _ = ReleaseDC(0, screenDc);
        }
    }

    internal static SizeF GetDefaultFontScaleDimensions()
    {
        nint screenDc = GetDC(0);
        if (screenDc == 0)
        {
            return new SizeF(8f, 20f);
        }

        nint fontHandle = GetStockObject(DEFAULT_GUI_FONT);
        nint previousObject = 0;

        try
        {
            if (fontHandle != 0)
            {
                previousObject = SelectObject(screenDc, fontHandle);
            }

            if (GetTextMetricsW(screenDc, out TEXTMETRICW metrics))
            {
                float width = metrics.tmAveCharWidth > 0 ? metrics.tmAveCharWidth : 8f;
                float height = metrics.tmHeight > 0 ? metrics.tmHeight : 20f;
                return new SizeF(width, height);
            }
        }
        finally
        {
            if (previousObject != 0)
            {
                _ = SelectObject(screenDc, previousObject);
            }

            _ = ReleaseDC(0, screenDc);
        }

        return new SizeF(8f, 20f);
    }

    [InlineArray(Capacity)]
    private struct InlineCharBuffer
    {
        public const int Capacity = 260;
        private char _character;
    }
}
