using System.Collections.Generic;
using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a basic tool strip surface that can host logical items.
/// </summary>
public class ToolStrip : ContainerControlBase
{
    private const int StripHorizontalPadding = 8;
    private const int StripVerticalPadding = 6;
    private const int StripItemSpacing = 6;

    private readonly ToolStripItemCollection _items;
    private readonly Dictionary<ToolStripItem, Control> _itemHosts = [];

    internal event EventHandler? ItemsChanged;

    /// <summary>
    /// Initializes a tool strip with a typical strip height.
    /// </summary>
    public ToolStrip()
    {
        _items = new ToolStripItemCollection(OnItemsChanged);
        Height = 36;
    }

    /// <summary>
    /// Gets the items contained in the strip.
    /// </summary>
    public ToolStripItemCollection Items => _items;

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyNativeThemeState();
        PerformLayout();
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyNativeThemeState();
    }

    private protected virtual void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
    }

    /// <inheritdoc />
    protected override void OnBoundsChanged()
    {
        PerformLayout();
    }

    /// <inheritdoc />
    public override void PerformLayout()
    {
        DockToTopStripBand();
        EnsureItemHosts();
        LayoutItemHosts();
        base.PerformLayout();
    }

    private void DockToTopStripBand()
    {
        if (Owner is null || this is StatusStrip || this is MenuStrip)
        {
            return;
        }

        int clientWidth = Owner.ClientSize.Width;
        if (Owner.Handle != 0 && Win32.GetClientRect(Owner.Handle, out var clientRect))
        {
            clientWidth = clientRect.Width;
        }

        int top = 0;
        if (Owner.MainMenuStrip is { Visible: true } menuStrip)
        {
            top += Math.Max(0, menuStrip.Height);
        }

        foreach (Control control in Owner.Controls)
        {
            if (ReferenceEquals(control, this))
            {
                continue;
            }

            if (control is ToolStrip toolStrip && control is not MenuStrip && control is not StatusStrip && control.Visible)
            {
                if (toolStrip.Top <= top)
                {
                    top += Math.Max(0, toolStrip.Height);
                }
            }
        }

        int height = Math.Max(28, Height);
        int width = Math.Max(1, clientWidth);

        if (Left == 0 && Top == top && Width == width && Height == height)
        {
            return;
        }

        SetBounds(0, top, width, height);
    }

    private void OnItemsChanged()
    {
        ItemsChanged?.Invoke(this, EventArgs.Empty);

        if (Owner is not null || Handle != 0)
        {
            PerformLayout();
        }
    }

    internal bool TryActivateMnemonic(char mnemonic)
    {
        EnsureItemHosts();

        char normalizedMnemonic = char.ToUpperInvariant(mnemonic);
        foreach (ToolStripItem item in Items)
        {
            if (!item.Visible || !item.Enabled)
            {
                continue;
            }

            if (item.GetMnemonic() != normalizedMnemonic)
            {
                continue;
            }

            ActivateItem(item);
            return true;
        }

        return false;
    }

    internal bool TryActivateFirstItem()
    {
        EnsureItemHosts();

        foreach (ToolStripItem item in Items)
        {
            if (!item.Visible || !item.Enabled)
            {
                continue;
            }

            ActivateItem(item);
            return true;
        }

        return false;
    }

    private protected bool TryGetItemHost(ToolStripItem item, out Control? host)
        => _itemHosts.TryGetValue(item, out host);

    private protected virtual bool ShouldCreateHostControl(ToolStripItem item) => true;

    private protected virtual bool IsCompatibleHostControl(Control host, ToolStripItem item)
    {
        if (OperatingSystem.IsWindows()
            && item.DisplayStyle == ToolStripItemDisplayStyle.Image
            && item.Image is not null)
        {
            return host is PictureBox;
        }

        return item switch
        {
            ToolStripLabel => host is Label,
            ToolStripStatusLabel => host is Label,
            ToolStripProgressBar => host is ProgressBar,
            ToolStripComboBox => host is ComboBox,
            ToolStripTextBox => host is TextBox,
            ToolStripSeparator => host is Panel,
            ToolStripDropDownItem => host is Button,
            ToolStripButton => host is Button,
            _ => host is Button,
        };
    }

    private protected void EnsureItemHosts()
    {
        foreach (ToolStripItem item in Items)
        {
            if (!ShouldCreateHostControl(item))
            {
                if (_itemHosts.Remove(item, out Control? removedHost))
                {
                    RemoveItemHost(removedHost);
                }

                continue;
            }

            if (_itemHosts.TryGetValue(item, out Control? existingHost))
            {
                if (existingHost is not null && IsCompatibleHostControl(existingHost, item))
                {
                    continue;
                }

                if (existingHost is not null)
                {
                    RemoveItemHost(existingHost);
                }
            }

            Control host = CreateHostControl(item);
            _itemHosts[item] = host;
            Controls.Add(host);
        }
    }

    private protected virtual void LayoutItemHosts()
    {
        int x = StripHorizontalPadding;
        int availableHeight = Math.Max(1, Height - (StripVerticalPadding * 2));

        foreach (ToolStripItem item in Items)
        {
            if (!_itemHosts.TryGetValue(item, out Control? host))
            {
                continue;
            }

            ApplyItemState(host, item);
            host.Visible = item.Visible;
            if (!item.Visible)
            {
                continue;
            }

            Size hostSize = ResolveHostSize(item, availableHeight);
            int y = Math.Max(0, (Height - hostSize.Height) / 2);
            host.SetBounds(x, y, hostSize.Width, hostSize.Height);
            x += hostSize.Width + StripItemSpacing;
        }
    }

    private protected void LayoutItemHosts(int x, int y, int itemSpacing, int availableHeight, int explicitItemHeight)
    {
        foreach (ToolStripItem item in Items)
        {
            if (!_itemHosts.TryGetValue(item, out Control? host))
            {
                continue;
            }

            ApplyItemState(host, item);
            host.Visible = item.Visible;
            if (!item.Visible)
            {
                continue;
            }

            Size hostSize = ResolveHostSize(item, availableHeight);
            int height = explicitItemHeight > 0 ? explicitItemHeight : hostSize.Height;
            host.SetBounds(x, y, hostSize.Width, height);
            x += hostSize.Width + itemSpacing;
        }
    }

    /// <summary>
    /// Creates the native host control used to represent a logical strip item on the surface.
    /// </summary>
    /// <param name="item">The item that needs a host control.</param>
    /// <returns>The control used to host the item.</returns>
    protected virtual Control CreateHostControl(ToolStripItem item)
    {
        if (OperatingSystem.IsWindows()
            && item.DisplayStyle == ToolStripItemDisplayStyle.Image
            && item.Image is not null)
        {
            return item is ToolStripDropDownItem
                ? CreateImageDropDownHost(item)
                : CreateImageButtonHost(item);
        }

        return item switch
        {
            ToolStripLabel => new Label(),
            ToolStripStatusLabel => new Label(),
            ToolStripProgressBar => new ProgressBar(),
            ToolStripComboBox => new ComboBox(),
            ToolStripTextBox => new TextBox(),
            ToolStripSeparator => new Panel(),
            ToolStripDropDownItem => CreateDropDownHost(item),
            _ => CreateButtonHost(item),
        };
    }

    /// <summary>
    /// Creates the default clickable host for non-drop-down items.
    /// </summary>
    /// <param name="item">The item that needs a clickable host.</param>
    /// <returns>The control used to invoke the item.</returns>
    protected virtual Control CreateButtonHost(ToolStripItem item)
    {
        var button = new ItemButtonHost();
        button.Click += (_, _) => item.PerformClick();
        return button;
    }

    /// <summary>
    /// Creates the default clickable image host for non-drop-down items.
    /// </summary>
    /// <param name="item">The image item that needs a host.</param>
    /// <returns>The control used to invoke the item.</returns>
    protected virtual Control CreateImageButtonHost(ToolStripItem item)
    {
        if (!OperatingSystem.IsWindows())
        {
            return CreateButtonHost(item);
        }

        var pictureBox = new ItemPictureHost
        {
            SizeMode = PictureBoxSizeMode.CenterImage,
            TabStop = false,
        };
        pictureBox.Click += (_, _) => item.PerformClick();
        return pictureBox;
    }

    /// <summary>
    /// Creates the default clickable host for a top-level drop-down item.
    /// </summary>
    /// <param name="item">The drop-down item that needs a host.</param>
    /// <returns>The control used to open the drop-down.</returns>
    protected virtual Control CreateDropDownHost(ToolStripItem item)
    {
        var button = new ItemButtonHost();
        button.Click += (_, _) => ShowDropDownWithSiblingNavigation((ToolStripDropDownItem)item, button);
        return button;
    }

    /// <summary>
    /// Creates the default clickable image host for a top-level drop-down item.
    /// </summary>
    /// <param name="item">The image drop-down item that needs a host.</param>
    /// <returns>The control used to open the drop-down.</returns>
    protected virtual Control CreateImageDropDownHost(ToolStripItem item)
    {
        if (!OperatingSystem.IsWindows())
        {
            return CreateDropDownHost(item);
        }

        var pictureBox = new ItemPictureHost
        {
            SizeMode = PictureBoxSizeMode.CenterImage,
            TabStop = false,
        };
        pictureBox.Click += (_, _) => ShowDropDownWithSiblingNavigation((ToolStripDropDownItem)item, pictureBox);
        return pictureBox;
    }

    private protected virtual void ApplyItemState(Control host, ToolStripItem item)
    {
        host.Enabled = item.Enabled;

        switch (host)
        {
            case Label label:
                label.Text = item.Text;
                break;

            case ProgressBar progressBar when item is ToolStripProgressBar stripProgressBar:
                progressBar.Value = stripProgressBar.Value;
                break;

            case ComboBox comboBox when item is ToolStripComboBox stripComboBox:
                comboBox.Items.Clear();
                comboBox.Items.AddRange([.. stripComboBox.Items]);
                if (stripComboBox.SelectedIndex >= 0 && stripComboBox.SelectedIndex < stripComboBox.Items.Count)
                {
                    comboBox.SelectedIndex = stripComboBox.SelectedIndex;
                }
                break;

            case TextBox textBox:
                textBox.Text = item.Text;
                break;

            case PictureBox pictureBox when OperatingSystem.IsWindows():
                pictureBox.Image = item.Image;
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                break;

            case Button button:
                button.Text = ResolveItemText(item);
                break;
        }
    }

    private protected virtual Size ResolveHostSize(ToolStripItem item, int availableHeight)
    {
        if (item.Size.Width > 0 && item.Size.Height > 0)
        {
            return new Size(item.Size.Width, Math.Max(1, Math.Min(item.Size.Height, Math.Max(item.Size.Height, availableHeight))));
        }

        return item switch
        {
            ToolStripLabel => new Size(Math.Max(36, item.Text.Length * 8 + 6), Math.Min(availableHeight, 20)),
            ToolStripStatusLabel => new Size(Math.Max(56, item.Text.Length * 8 + 12), Math.Min(availableHeight, 20)),
            ToolStripProgressBar => new Size(112, Math.Min(availableHeight, 18)),
            ToolStripComboBox => new Size(136, Math.Min(availableHeight, 28)),
            ToolStripTextBox => new Size(120, Math.Min(availableHeight, 28)),
            ToolStripSeparator => new Size(10, availableHeight),
            _ when OperatingSystem.IsWindowsVersionAtLeast(6, 1)
                && item.DisplayStyle == ToolStripItemDisplayStyle.Image
                && item.Image is not null
                => new Size(
                    Math.Max(28, item.Image.Width + 12),
                    Math.Max(28, Math.Min(availableHeight, item.Image.Height + 12))),
            _ => new Size(Math.Max(36, ResolveItemText(item).Length * 8 + 24), Math.Max(28, Math.Min(availableHeight, 32))),
        };
    }

    /// <summary>
    /// Resolves the display text used by host controls when an item does not provide explicit content.
    /// </summary>
    /// <param name="item">The item whose text should be resolved.</param>
    /// <returns>The text that should appear on the host control.</returns>
    protected internal static string ResolveItemText(ToolStripItem item)
    {
        if (item.DisplayStyle == ToolStripItemDisplayStyle.Image && !string.IsNullOrWhiteSpace(item.Text))
        {
            return item.Text;
        }

        if (!string.IsNullOrWhiteSpace(item.Text))
        {
            return item.Text;
        }

        return string.IsNullOrWhiteSpace(item.Name)
            ? item.GetType().Name
            : item.Name;
    }

    private void ActivateItem(ToolStripItem item)
    {
        if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0)
        {
            if (_itemHosts.TryGetValue(item, out Control? host))
            {
                ShowDropDownWithSiblingNavigation(dropDownItem, host);
                return;
            }
        }

        item.PerformClick();
    }

    /// <summary>
    /// Shows the drop-down for a top-level item and handles Left/Right sibling navigation
    /// by looping until the user either selects a command or presses Escape.
    /// </summary>
    protected void ShowDropDownWithSiblingNavigation(ToolStripDropDownItem initialItem, Control initialHost)
    {
        // Build ordered list of navigable drop-down items on this strip.
        List<ToolStripDropDownItem> navigable = [];
        foreach (ToolStripItem it in Items)
        {
            if (it is ToolStripDropDownItem ddi && ddi.Visible && ddi.Enabled && ddi.DropDownItems.Count > 0)
            {
                navigable.Add(ddi);
            }
        }

        int count = navigable.Count;
        int currentIndex = navigable.IndexOf(initialItem);

        if (currentIndex < 0 || count == 0 || !OperatingSystem.IsWindows())
        {
            ShowDropDown(initialItem, initialHost);
            return;
        }

        while (true)
        {
            ToolStripDropDownItem item = navigable[currentIndex];
            Control host = _itemHosts.TryGetValue(item, out Control? h) ? h : initialHost;
            Point screenLocation = GetDropDownScreenLocation(host);
            nint ownerHandle = Owner?.Handle ?? Handle;

            int direction = ToolStripPopupMenu.ShowForMenuBar(item.DropDownItems, ownerHandle, screenLocation, Owner?.CurrentVisualStyle ?? Application.CurrentVisualStyle);

            if (direction == 0)
            {
                break;
            }

            currentIndex = ((currentIndex + direction) % count + count) % count;
        }
    }

    private void ShowDropDown(ToolStripDropDownItem item, Control host)
    {
        if (item.DropDownItems.Count == 0)
        {
            item.PerformClick();
            return;
        }

        Point screenLocation = GetDropDownScreenLocation(host);
        ToolStripPopupMenu.Show(item.DropDownItems, Owner?.Handle ?? Handle, screenLocation, Owner?.CurrentVisualStyle ?? Application.CurrentVisualStyle);
    }

    private static Point GetDropDownScreenLocation(Control host)
    {
        if (host.Handle != 0 && Win32.GetWindowRect(host.Handle, out var rect))
        {
            return new Point(rect.Left, Math.Max(rect.Top, rect.Bottom - 1));
        }

        return new Point(host.Left, host.Bottom);
    }

    private void RemoveItemHost(Control host)
    {
        _ = RemoveChild(host);
        host.Dispose();
    }

    private sealed class ItemButtonHost : Button
    {
        private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

        private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

        protected override bool UseParentBackgroundForTheming => true;

        protected override int GetNativeHeight(int requestedHeight)
            => Math.Max(28, requestedHeight);
    }

    private sealed class ItemPictureHost : PictureBox
    {
        private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

        private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

        protected override bool UseParentBackgroundForTheming => true;

        protected override int GetNativeHeight(int requestedHeight)
            => Math.Max(28, requestedHeight);
    }
}
