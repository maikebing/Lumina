using System.Collections.Generic;

namespace Lumina.Forms;

/// <summary>
/// Represents a lightweight tab control.
/// </summary>
public class TabControl : ContainerControlBase
{
    private readonly Dictionary<TabPage, Button> _headerButtons = [];
    private readonly Dictionary<TabPage, EventHandler> _textChangedHandlers = [];
    private int _selectedIndex;

    /// <summary>
    /// Gets or sets the selected page index.
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            int pageCount = GetTabPages().Count;
            _selectedIndex = pageCount == 0
                ? 0
                : Math.Clamp(value, 0, pageCount - 1);
            UpdateTabPages();
            UpdateHeaderStates();
        }
    }

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        EnsureHeaderButtons();
        UpdateTabPages();
    }

    /// <inheritdoc />
    protected override void OnBoundsChanged()
    {
        PerformLayout();
    }

    /// <inheritdoc />
    public override void PerformLayout()
    {
        EnsureHeaderButtons();
        LayoutHeaderButtons();
        UpdateTabPages();
        base.PerformLayout();
    }

    /// <inheritdoc />
    protected override void OnDisposing()
    {
        foreach ((TabPage page, EventHandler handler) in _textChangedHandlers)
        {
            page.TextChanged -= handler;
        }

        foreach (Button button in _headerButtons.Values)
        {
            button.Dispose();
        }

        _textChangedHandlers.Clear();
        _headerButtons.Clear();

        base.OnDisposing();
    }

    private void EnsureHeaderButtons()
    {
        List<TabPage> pages = GetTabPages();
        foreach (TabPage page in pages)
        {
            if (!_headerButtons.TryGetValue(page, out Button? button))
            {
                button = CreateHeaderButton(page);
                _headerButtons[page] = button;
            }

            if (button.Parent is null)
            {
                button.SetParent(this);
            }

            if (Owner is not null && button.Owner is null)
            {
                Owner.AttachControl(button, this);
            }

            if (Handle != 0 && button.Owner is not null && button.Handle == 0)
            {
                button.CreateHandleRecursive();
            }

            button.Visible = true;
        }

        foreach ((TabPage page, Button button) in _headerButtons)
        {
            if (!pages.Contains(page))
            {
                button.Visible = false;
            }
        }

        UpdateHeaderStates();
    }

    private Button CreateHeaderButton(TabPage page)
    {
        var button = new TabHeaderButton
        {
            TabStop = false,
            UseVisualStyleBackColor = true,
            Text = ResolvePageText(page),
        };

        button.Click += (_, _) => SelectedIndex = GetTabPages().IndexOf(page);

        EventHandler textChangedHandler = (_, _) =>
        {
            button.Text = ResolvePageText(page);
            LayoutHeaderButtons();
        };

        page.TextChanged += textChangedHandler;
        _textChangedHandlers[page] = textChangedHandler;
        return button;
    }

    private void LayoutHeaderButtons()
    {
        int x = 0;
        foreach (TabPage page in GetTabPages())
        {
            if (!_headerButtons.TryGetValue(page, out Button? button) || !button.Visible)
            {
                continue;
            }

            string text = ResolvePageText(page);
            button.Text = text;
            int width = Math.Max(64, text.Length * 8 + 18);
            button.SetBounds(x, 0, width, 24);
            x += width + 2;
        }
    }

    private void UpdateHeaderStates()
    {
        List<TabPage> pages = GetTabPages();
        for (int index = 0; index < pages.Count; index++)
        {
            if (_headerButtons.TryGetValue(pages[index], out Button? button))
            {
                button.Enabled = index != _selectedIndex;
            }
        }
    }

    private void UpdateTabPages()
    {
        int headerHeight = 26;
        int contentHeight = Math.Max(1, Height - headerHeight);
        List<TabPage> pages = GetTabPages();
        if (pages.Count == 0)
        {
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, pages.Count - 1);
        for (int tabPageIndex = 0; tabPageIndex < pages.Count; tabPageIndex++)
        {
            TabPage tabPage = pages[tabPageIndex];
            tabPage.SetBounds(0, headerHeight, Math.Max(1, Width), contentHeight);
            tabPage.Visible = tabPageIndex == _selectedIndex;
        }
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

    private sealed class TabHeaderButton : Button
    {
        protected override int GetNativeHeight(int requestedHeight)
            => Math.Max(24, requestedHeight);
    }
}