namespace Lumina.Forms;

/// <summary>
/// Represents a WinForms-compatible list view backed by the native common control.
/// </summary>
public class ListView : Control
{
    private readonly ListViewItemCollection _items = new();
    private readonly ColumnHeaderCollection _columns = new();
    private static readonly Win32.SubclassProc s_listViewSubclassProc = ListViewSubclassProc;

    [ThreadStatic]
    private static bool s_applyingTheme;

    private const uint ModernExtendedStyles =
        Win32.LVS_EX_DOUBLEBUFFER |
        Win32.LVS_EX_LABELTIP |
        Win32.LVS_EX_FULLROWSELECT |
        Win32.LVS_EX_HEADERDRAGDROP;

    /// <summary>
    /// Initializes a list view with more spacious default layout metrics.
    /// </summary>
    public ListView()
    {
        Margin = new Padding(6);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control should preserve legacy image behavior.
    /// </summary>
    public bool UseCompatibleStateImageBehavior { get; set; } = true;

    public ListViewItemCollection Items => _items;

    public ColumnHeaderCollection Columns => _columns;

    public bool FullRowSelect { get; set; }

    public bool GridLines { get; set; }

    public View View { get; set; } = View.Details;

    /// <inheritdoc />
    protected override string ClassName => "SysListView32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_VSCROLL;

    /// <inheritdoc />
    protected override uint ExStyle => Win32.WS_EX_CLIENTEDGE;

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyExplorerTheme();
        ApplyNativeColors();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyExtendedStyles();
        InstallDarkModeSubclass();
        ApplyNativeThemeState();
        ApplyNativeColors();
    }

    /// <inheritdoc />
    protected override string GetPreferredThemeClass(ResolvedVisualStyle visualStyle)
        => visualStyle.IsDarkMode && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763)
            ? "DarkMode_Explorer"
            : "ItemsView";

    /// <inheritdoc />
    protected override string GetFallbackThemeClass(ResolvedVisualStyle visualStyle)
        => "Explorer";

    private void ApplyExtendedStyles()
    {
        if (Handle == 0)
        {
            return;
        }

        _ = Win32.SendMessageW(
            Handle,
            Win32.LVM_SETEXTENDEDLISTVIEWSTYLE,
            (nint)ModernExtendedStyles,
            (nint)ModernExtendedStyles);
    }

    private void InstallDarkModeSubclass()
    {
        if (Handle == 0 || !DarkModeNative.IsSupported)
        {
            return;
        }

        _ = Win32.SetWindowSubclass(Handle, s_listViewSubclassProc, 0, 0);
        _ = Win32.SendMessageW(Handle, Win32.WM_CHANGEUISTATE, Win32.MakeLParam(Win32.UIS_SET, Win32.UISF_HIDEFOCUS), 0);
    }

    private void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        if (s_applyingTheme)
        {
            return;
        }

        nint headerHandle = Win32.SendMessageW(Handle, Win32.LVM_GETHEADER, 0, 0);
        bool useDarkMode = CurrentVisualStyle.IsDarkMode;

        s_applyingTheme = true;
        try
        {
            _ = DarkModeNative.AllowWindowDarkMode(Handle, useDarkMode);
            if (headerHandle != 0)
            {
                _ = DarkModeNative.AllowWindowDarkMode(headerHandle, useDarkMode);
                _ = Win32.SetWindowTheme(headerHandle, "ItemsView", null);
            }

            ApplyExplorerTheme();
        }
        finally
        {
            s_applyingTheme = false;
        }
    }

    private void ApplyNativeColors()
    {
        if (Handle == 0)
        {
            return;
        }

        ThemePalette palette = CurrentVisualStyle.Palette;
        uint background = Win32.ToColorRef(palette.ControlBackground);
        uint foreground = Win32.ToColorRef(palette.ControlForeground);

        _ = Win32.SendMessageW(Handle, Win32.LVM_SETBKCOLOR, 0, unchecked((nint)background));
        _ = Win32.SendMessageW(Handle, Win32.LVM_SETTEXTBKCOLOR, 0, unchecked((nint)background));
        _ = Win32.SendMessageW(Handle, Win32.LVM_SETTEXTCOLOR, 0, unchecked((nint)foreground));
    }

    private static nint ListViewSubclassProc(nint hWnd, uint uMsg, nint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case Win32.WM_NOTIFY:
            {
                unsafe
                {
                    Win32.NMHDR* header = (Win32.NMHDR*)lParam;
                    if (header->code == unchecked((uint)Win32.NM_CUSTOMDRAW))
                    {
                        Win32.NMCUSTOMDRAW* customDraw = (Win32.NMCUSTOMDRAW*)lParam;
                        if (customDraw->dwDrawStage == Win32.CDDS_PREPAINT)
                        {
                            return Win32.CDRF_NOTIFYITEMDRAW;
                        }

                        if (customDraw->dwDrawStage == Win32.CDDS_ITEMPREPAINT)
                        {
                            ApplyHeaderTextColor(hWnd, customDraw->hdc);
                            return Win32.CDRF_DODEFAULT;
                        }
                    }
                }

                break;
            }

            case Win32.WM_THEMECHANGED:
                ApplyThemeToHandle(hWnd);
                break;

            case Win32.WM_DESTROY:
                break;
        }

        return Win32.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    private static void ApplyThemeToHandle(nint hWnd)
    {
        if (hWnd == 0)
        {
            return;
        }

        if (s_applyingTheme)
        {
            return;
        }

        bool useDarkMode = DarkModeNative.IsDarkModeEnabled;
        nint headerHandle = Win32.SendMessageW(hWnd, Win32.LVM_GETHEADER, 0, 0);

        s_applyingTheme = true;
        try
        {
            _ = DarkModeNative.AllowWindowDarkMode(hWnd, useDarkMode);
            if (headerHandle != 0)
            {
                _ = DarkModeNative.AllowWindowDarkMode(headerHandle, useDarkMode);
                _ = Win32.SetWindowTheme(headerHandle, "ItemsView", null);
            }

            _ = Win32.SetWindowTheme(hWnd, useDarkMode ? "DarkMode_Explorer" : "ItemsView", null);

            nint itemsViewTheme = Win32.OpenThemeData(0, "ItemsView");
            if (itemsViewTheme != 0)
            {
                try
                {
                    if (Win32.GetThemeColor(itemsViewTheme, 0, 0, Win32.TMT_TEXTCOLOR, out uint textColor) == 0)
                    {
                        _ = Win32.SendMessageW(hWnd, Win32.LVM_SETTEXTCOLOR, 0, unchecked((nint)textColor));
                    }

                    if (Win32.GetThemeColor(itemsViewTheme, 0, 0, Win32.TMT_FILLCOLOR, out uint fillColor) == 0)
                    {
                        _ = Win32.SendMessageW(hWnd, Win32.LVM_SETTEXTBKCOLOR, 0, unchecked((nint)fillColor));
                        _ = Win32.SendMessageW(hWnd, Win32.LVM_SETBKCOLOR, 0, unchecked((nint)fillColor));
                    }
                }
                finally
                {
                    _ = Win32.CloseThemeData(itemsViewTheme);
                }
            }
        }
        finally
        {
            s_applyingTheme = false;
        }

        _ = Win32.InvalidateRect(hWnd, 0, true);
    }

    private static void ApplyHeaderTextColor(nint listViewHandle, nint hdc)
    {
        nint headerHandle = Win32.SendMessageW(listViewHandle, Win32.LVM_GETHEADER, 0, 0);
        if (headerHandle == 0 || hdc == 0)
        {
            return;
        }

        nint headerTheme = Win32.OpenThemeData(headerHandle, "Header");
        if (headerTheme == 0)
        {
            return;
        }

        try
        {
            if (Win32.GetThemeColor(headerTheme, Win32.HP_HEADERITEM, 0, Win32.TMT_TEXTCOLOR, out uint textColor) == 0)
            {
                _ = Win32.SetTextColor(hdc, textColor);
            }
        }
        finally
        {
            _ = Win32.CloseThemeData(headerTheme);
        }
    }
}
