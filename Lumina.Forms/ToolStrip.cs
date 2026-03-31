using System.Collections.Generic;
using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a basic tool strip surface that can host logical items.
/// </summary>
public class ToolStrip : ContainerControlBase
{
    private readonly ToolStripItemCollection _items;
    private readonly Dictionary<ToolStripItem, Control> _itemHosts = [];

    /// <summary>
    /// Initializes a tool strip with a typical strip height.
    /// </summary>
    public ToolStrip()
    {
        _items = new ToolStripItemCollection(OnItemsChanged);
        Height = 25;
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
        PerformLayout();
    }

    /// <inheritdoc />
    protected override void OnBoundsChanged()
    {
        PerformLayout();
    }

    /// <inheritdoc />
    public override void PerformLayout()
    {
        EnsureItemHosts();
        LayoutItemHosts();
        base.PerformLayout();
    }

    private void OnItemsChanged()
    {
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

    private void EnsureItemHosts()
    {
        foreach (ToolStripItem item in Items)
        {
            if (_itemHosts.ContainsKey(item))
            {
                continue;
            }

            Control host = CreateHostControl(item);
            _itemHosts[item] = host;
            Controls.Add(host);
        }
    }

    private void LayoutItemHosts()
    {
        int x = 3;
        int availableHeight = Math.Max(1, Height - 6);

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
            x += hostSize.Width + 4;
        }
    }

    private Control CreateHostControl(ToolStripItem item)
    {
        return item switch
        {
            ToolStripStatusLabel => new Label(),
            ToolStripProgressBar => new ProgressBar(),
            ToolStripComboBox => new ComboBox(),
            ToolStripTextBox => new TextBox(),
            ToolStripSeparator => new Panel(),
            ToolStripDropDownItem => CreateDropDownHost(item),
            _ => CreateButtonHost(item),
        };
    }

    private Control CreateButtonHost(ToolStripItem item)
    {
        var button = new ItemButtonHost();
        button.Click += (_, _) => item.PerformClick();
        return button;
    }

    private Control CreateDropDownHost(ToolStripItem item)
    {
        var button = new ItemButtonHost();
        button.Click += (_, _) => ShowDropDown((ToolStripDropDownItem)item, button);
        return button;
    }

    private void ApplyItemState(Control host, ToolStripItem item)
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

            case Button button:
                button.Text = ResolveButtonText(item);
                break;
        }
    }

    private Size ResolveHostSize(ToolStripItem item, int availableHeight)
    {
        if (item.Size.Width > 0 && item.Size.Height > 0)
        {
            return new Size(item.Size.Width, Math.Max(1, Math.Min(item.Size.Height, Math.Max(item.Size.Height, availableHeight))));
        }

        return item switch
        {
            ToolStripStatusLabel => new Size(Math.Max(48, item.Text.Length * 8 + 8), Math.Min(availableHeight, 17)),
            ToolStripProgressBar => new Size(100, Math.Min(availableHeight, 16)),
            ToolStripComboBox => new Size(121, Math.Min(availableHeight, 25)),
            ToolStripTextBox => new Size(100, Math.Min(availableHeight, 23)),
            ToolStripSeparator => new Size(6, availableHeight),
            _ => new Size(Math.Max(24, ResolveButtonText(item).Length * 8 + 16), Math.Min(Math.Max(20, availableHeight), Math.Max(20, availableHeight))),
        };
    }

    private static string ResolveButtonText(ToolStripItem item)
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
                ShowDropDown(dropDownItem, host);
                return;
            }
        }

        item.PerformClick();
    }

    private void ShowDropDown(ToolStripDropDownItem item, Control host)
    {
        if (item.DropDownItems.Count == 0)
        {
            item.PerformClick();
            return;
        }

        Point screenLocation = new(host.Left, host.Bottom);
        if (host.Handle != 0 && Win32.GetWindowRect(host.Handle, out var rect))
        {
            screenLocation = new Point(rect.Left, rect.Bottom);
        }

        ToolStripPopupMenu.Show(item.DropDownItems, Owner?.Handle ?? Handle, screenLocation);
    }

    private sealed class ItemButtonHost : Button
    {
        protected override int GetNativeHeight(int requestedHeight)
            => Math.Max(20, requestedHeight);
    }
}