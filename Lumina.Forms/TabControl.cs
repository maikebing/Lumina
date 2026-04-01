using System.Drawing;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents a WinForms-compatible tab control backed by the native common control.
/// </summary>
public class TabControl : ContainerControlBase
{
    private readonly Dictionary<TabPage, EventHandler> _textChangedHandlers = [];
    private int _selectedIndex;

    /// <summary>
    /// Initializes a tab control with more spacious default layout metrics.
    /// </summary>
    public TabControl()
    {
        Margin = new Padding(6);
        Padding = new Padding(8);
    }

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
                if (nativeIndex >= 0)
                {
                    _selectedIndex = nativeIndex;
                }
            }

            return _selectedIndex;
        }
        set
        {
            int pageCount = GetTabPages().Count;
            _selectedIndex = pageCount == 0
                ? 0
                : Math.Clamp(value, 0, pageCount - 1);

            ApplySelectedIndex();
        }
    }

    /// <inheritdoc />
    protected override string ClassName => "SysTabControl32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.WS_CLIPSIBLINGS;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyTabHeaderMetrics();
        SynchronizeTextHandlers();
        SynchronizeNativeTabs();
        ApplySelectedIndex();
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
        ApplySelectedIndex();
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
        ApplyExplorerTheme();
    }

    /// <inheritdoc />
    protected override bool OnNotify(int notificationCode, nint lParam)
    {
        if (notificationCode != Win32.TCN_SELCHANGE)
        {
            return false;
        }

        int nativeIndex = (int)Win32.SendMessageW(Handle, Win32.TCM_GETCURSEL, 0, 0);
        if (nativeIndex >= 0)
        {
            _selectedIndex = nativeIndex;
        }

        UpdateTabPages();
        return true;
    }

    private void ApplySelectedIndex()
    {
        if (Handle != 0 && GetTabPages().Count > 0)
        {
            _ = Win32.SendMessageW(Handle, Win32.TCM_SETCURSEL, (nint)_selectedIndex, 0);
        }

        UpdateTabPages();
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
                ApplySelectedIndex();
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
            _selectedIndex = 0;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, pages.Count - 1);
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

        return new Rectangle(
            8,
            34,
            Math.Max(1, Width - 16),
            Math.Max(1, Height - 42));
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

        _ = Win32.SendMessageW(Handle, Win32.TCM_SETPADDING, 0, Win32.MakeLParam(18, 6));
    }
}
