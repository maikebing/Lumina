using System.Drawing;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents a WinForms-compatible tab control backed by the native common control.
/// </summary>
public class TabControl : ContainerControlBase
{
    private readonly Dictionary<TabPage, EventHandler> _textChangedHandlers = [];
    private int _selectedIndex = -1;

    [ThreadStatic]
    private static bool s_applyingTabTheme;

    /// <summary>
    /// Initializes a tab control with WinForms-compatible default layout metrics.
    /// </summary>
    public TabControl()
    {
        Size = new Size(200, 100);
        Margin = new Padding(3);
    }

    /// <summary>
    /// Occurs when the selected page index changes.
    /// </summary>
    public event EventHandler? SelectedIndexChanged;

    /// <summary>
    /// Gets or sets the selected page index.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (Handle != 0)
            {
                int nativeIndex = (int)Win32.SendMessageW(Handle, Win32.TCM_GETCURSEL, 0, 0);
                if (nativeIndex >= -1)
                {
                    _selectedIndex = nativeIndex;
                }
            }

            return _selectedIndex;
        }
        set
        {
            int pageCount = GetTabPages().Count;
            int resolvedIndex = pageCount == 0
                ? -1
                : Math.Clamp(value, 0, pageCount - 1);

            if (_selectedIndex == resolvedIndex)
            {
                ApplySelectedIndex(raiseChangedEvent: false);
                return;
            }

            _selectedIndex = resolvedIndex;
            ApplySelectedIndex(raiseChangedEvent: true);
        }
    }

    /// <summary>
    /// Gets the number of hosted pages.
    /// </summary>
    public int TabCount => GetTabPages().Count;

    /// <summary>
    /// Gets the currently selected page, if any.
    /// </summary>
    public TabPage? SelectedTab
    {
        get
        {
            List<TabPage> pages = GetTabPages();
            return _selectedIndex >= 0 && _selectedIndex < pages.Count
                ? pages[_selectedIndex]
                : null;
        }
    }

    /// <inheritdoc />
    public override Rectangle DisplayRectangle => GetPageBounds();

    /// <inheritdoc />
    protected override string ClassName => "SysTabControl32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_CLIPSIBLINGS;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyNativeThemeState();
        ApplyTabHeaderMetrics();
        ApplyNativeColors();
        SynchronizeTextHandlers();
        SynchronizeNativeTabs();
        EnsureSelection();
        ApplySelectedIndex(raiseChangedEvent: false);
    }

    /// <inheritdoc />
    protected override void OnBoundsChanged()
    {
        PerformLayout();
    }

    /// <inheritdoc />
    public override void PerformLayout()
    {
        SynchronizeTextHandlers();
        SynchronizeNativeTabs();
        EnsureSelection();
        ApplySelectedIndex(raiseChangedEvent: false);
        base.PerformLayout();
    }

    /// <inheritdoc />
    protected override void OnDisposing()
    {
        foreach ((TabPage page, EventHandler handler) in _textChangedHandlers)
        {
            page.TextChanged -= handler;
        }

        _textChangedHandlers.Clear();
        base.OnDisposing();
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyNativeThemeState();
        ApplyExplorerTheme();
        ApplyNativeColors();
    }

    private void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        if (s_applyingTabTheme)
        {
            return;
        }

        s_applyingTabTheme = true;
        try
        {
            DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
            _ = Win32.SetWindowTheme(Handle, CurrentVisualStyle.IsDarkMode ? "Explorer" : "Tab", null);
        }
        finally
        {
            s_applyingTabTheme = false;
        }
    }

    /// <inheritdoc />
    protected override bool OnNotify(int notificationCode, nint lParam)
    {
        if (notificationCode != Win32.TCN_SELCHANGE)
        {
            return false;
        }

        int previousIndex = _selectedIndex;
        int nativeIndex = (int)Win32.SendMessageW(Handle, Win32.TCM_GETCURSEL, 0, 0);
        if (nativeIndex >= -1)
        {
            _selectedIndex = nativeIndex;
        }

        UpdateTabPages();
        if (previousIndex != _selectedIndex)
        {
            OnSelectedIndexChanged(EventArgs.Empty);
        }

        return true;
    }

    internal override void OnChildAdded(Control control)
    {
        if (control is TabPage)
        {
            EnsureSelection();
        }
    }

    internal override void OnChildRemoved(Control control)
    {
        if (control is TabPage)
        {
            EnsureSelection();
            ApplySelectedIndex(raiseChangedEvent: false);
        }
    }

    private void ApplySelectedIndex(bool raiseChangedEvent)
    {
        if (Handle != 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.TCM_SETCURSEL, (nint)_selectedIndex, 0);
        }

        UpdateTabPages();
        if (raiseChangedEvent)
        {
            OnSelectedIndexChanged(EventArgs.Empty);
        }
    }

    private void SynchronizeTextHandlers()
    {
        List<TabPage> pages = GetTabPages();
        foreach (TabPage page in pages)
        {
            if (_textChangedHandlers.ContainsKey(page))
            {
                continue;
            }

            EventHandler handler = (_, _) =>
            {
                SynchronizeNativeTabs();
                ApplySelectedIndex(raiseChangedEvent: false);
            };

            page.TextChanged += handler;
            _textChangedHandlers[page] = handler;
        }

        foreach ((TabPage page, EventHandler handler) in _textChangedHandlers.ToArray())
        {
            if (pages.Contains(page))
            {
                continue;
            }

            page.TextChanged -= handler;
            _ = _textChangedHandlers.Remove(page);
        }
    }

    private void SynchronizeNativeTabs()
    {
        if (Handle == 0)
        {
            return;
        }

        _ = Win32.SendMessageW(Handle, Win32.TCM_DELETEALLITEMS, 0, 0);

        List<TabPage> pages = GetTabPages();
        for (int index = 0; index < pages.Count; index++)
        {
            InsertNativeTab(index, ResolvePageText(pages[index]));
        }
    }

    private void InsertNativeTab(int index, string text)
    {
        nint textPointer = Marshal.StringToHGlobalUni(text);
        try
        {
            var item = new Win32.TCITEMW
            {
                mask = Win32.TCIF_TEXT,
                pszText = textPointer,
                cchTextMax = text.Length,
            };

            _ = Win32.SendMessageW(Handle, Win32.TCM_INSERTITEMW, (nint)index, ref item);
        }
        finally
        {
            Marshal.FreeHGlobal(textPointer);
        }
    }

    private void UpdateTabPages()
    {
        List<TabPage> pages = GetTabPages();
        if (pages.Count == 0)
        {
            _selectedIndex = -1;
            return;
        }

        EnsureSelection();
        Rectangle pageBounds = GetPageBounds();
        for (int tabPageIndex = 0; tabPageIndex < pages.Count; tabPageIndex++)
        {
            TabPage tabPage = pages[tabPageIndex];
            tabPage.SetBounds(pageBounds.X, pageBounds.Y, pageBounds.Width, pageBounds.Height);
            tabPage.Visible = tabPageIndex == _selectedIndex;
        }
    }

    private Rectangle GetPageBounds()
    {
        if (Handle != 0 && Win32.GetClientRect(Handle, out var rect))
        {
            _ = Win32.SendMessageW(Handle, Win32.TCM_ADJUSTRECT, 0, ref rect);
            return new Rectangle(
                rect.Left,
                rect.Top,
                Math.Max(1, rect.Width),
                Math.Max(1, rect.Height));
        }

        if (GetTabPages().Count == 0)
        {
            return new Rectangle(
                4,
                4,
                Math.Max(1, Width - 8),
                Math.Max(1, Height - 8));
        }

        int fontHeight = GetFallbackHeaderHeight();
        return new Rectangle(
            4,
            fontHeight + 4,
            Math.Max(1, Width - 8),
            Math.Max(1, Height - fontHeight - 8));
    }

    private List<TabPage> GetTabPages()
    {
        List<TabPage> pages = [];
        foreach (Control control in Controls)
        {
            if (control is TabPage tabPage)
            {
                pages.Add(tabPage);
            }
        }

        return pages;
    }

    private static string ResolvePageText(TabPage page)
    {
        if (!string.IsNullOrWhiteSpace(page.Text))
        {
            return page.Text;
        }

        return string.IsNullOrWhiteSpace(page.Name)
            ? nameof(TabPage)
            : page.Name;
    }

    private void ApplyTabHeaderMetrics()
    {
        if (Handle == 0)
        {
            return;
        }

        _ = Win32.SendMessageW(Handle, Win32.TCM_SETPADDING, 0, Win32.MakeLParam(6, 3));
    }

    private void EnsureSelection()
    {
        int pageCount = GetTabPages().Count;
        if (pageCount == 0)
        {
            _selectedIndex = -1;
            return;
        }

        if (_selectedIndex < 0 || _selectedIndex >= pageCount)
        {
            _selectedIndex = 0;
        }
    }

    private static int GetFallbackHeaderHeight() => 17;

    /// <summary>
    /// Raises the <see cref="SelectedIndexChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnSelectedIndexChanged(EventArgs e)
    {
        SelectedIndexChanged?.Invoke(this, e);
    }

    private void ApplyNativeColors()
    {
        if (Handle == 0)
        {
            return;
        }

        ThemePalette palette = CurrentVisualStyle.Palette;
        BackColor = Color.FromArgb(unchecked((int)palette.SurfaceBackground));
        ForeColor = Color.FromArgb(unchecked((int)palette.SurfaceForeground));

        foreach (TabPage page in GetTabPages())
        {
            if (page.UseVisualStyleBackColor)
            {
                page.BackColor = Color.Empty;
            }

            if (page.ForeColor.IsEmpty)
            {
                page.ForeColor = Color.Empty;
            }
        }
    }
}
