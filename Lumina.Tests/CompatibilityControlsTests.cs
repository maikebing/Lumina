using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Lumina.Forms;
using Xunit;

namespace Lumina.Tests;

public sealed class CompatibilityControlsTests
{
    [Fact]
    public void TreeViewNodes_AddRange_PreservesHierarchy()
    {
        var child = new TreeNode("child") { Name = "child" };
        var parent = new TreeNode("parent", [child]) { Name = "parent" };
        var treeView = new TreeView();

        treeView.Nodes.AddRange(parent);

        Assert.Single(treeView.Nodes);
        Assert.Equal("parent", treeView.Nodes[0].Text);
        Assert.Single(treeView.Nodes[0].Nodes);
        Assert.Equal("child", treeView.Nodes[0].Nodes[0].Name);
    }

    [Fact]
    public void DesignTimeComponents_SupportContainerAndInitializationPatterns()
    {
        using var container = new Container();
        using var notifyIcon = new NotifyIcon(container) { Text = "notifyIcon1", Visible = true };
        using var toolTip = new ToolTip(container);
        var numericUpDown = new NumericUpDown();
        var pictureBox = new PictureBox();

        ((ISupportInitialize)numericUpDown).BeginInit();
        numericUpDown.Value = 12;
        ((ISupportInitialize)numericUpDown).EndInit();
        ((ISupportInitialize)pictureBox).BeginInit();
        toolTip.SetToolTip(pictureBox, "preview");
        ((ISupportInitialize)pictureBox).EndInit();

        Assert.Equal(12m, numericUpDown.Value);
        Assert.True(notifyIcon.Visible);
        Assert.Equal(2, container.Components.Count);
    }

    [Fact]
    public void PictureBox_AutoSize_UsesSourceImageDimensions()
    {
        string imagePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.png");
        File.WriteAllBytes(imagePath, Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVQImWNgYGD4DwABBAEAAPp7WQAAAABJRU5ErkJggg=="));

        try
        {
            var pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                ImageLocation = imagePath,
            };

            pictureBox.Load();

            Assert.Equal(new Size(1, 1), pictureBox.Size);
        }
        finally
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
    }

    [Fact]
    public void ToolStripItem_PerformClick_RaisesClick()
    {
        var item = new ToolStripMenuItem();
        int clickCount = 0;
        item.Click += (_, _) => clickCount++;

        item.PerformClick();

        Assert.Equal(1, clickCount);
    }

    [Fact]
    public void MenuStrip_CanBeConstructed_WithoutThrowing()
    {
        var menuStrip = new MenuStrip();
        var fileMenu = new ToolStripMenuItem { Text = "File" };

        menuStrip.Items.Add(fileMenu);
        menuStrip.PerformLayout();

        Assert.Single(menuStrip.Items);
        Assert.Same(fileMenu, menuStrip.Items[0]);
    }

    [Fact]
    public void ToolStripMenuItem_CheckOnClick_TogglesCheckedState()
    {
        var item = new ToolStripMenuItem
        {
            CheckOnClick = true,
        };

        item.PerformClick();
        Assert.True(item.Checked);

        item.PerformClick();
        Assert.False(item.Checked);
    }

    [Fact]
    public void ToolStripMenuItem_GetShortcutDisplayText_UsesFormattedShortcutKeys()
    {
        var item = new ToolStripMenuItem
        {
            ShortcutKeys = Keys.Control | Keys.Shift | Keys.S,
        };

        MethodInfo? method = typeof(ToolStripMenuItem).GetMethod("GetShortcutDisplayText", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        string shortcutText = Assert.IsType<string>(method!.Invoke(item, null));
        Assert.Equal("Ctrl+Shift+S", shortcutText);
    }

    [Fact]
    public void Form_MenuShortcut_InvokesMatchingMenuItem()
    {
        var form = new Form();
        var menuStrip = new MenuStrip();
        var fileMenu = new ToolStripMenuItem { Text = "File" };
        var saveItem = new ToolStripMenuItem { ShortcutKeys = Keys.Control | Keys.S };
        int clickCount = 0;
        saveItem.Click += (_, _) => clickCount++;

        fileMenu.DropDownItems.Add(saveItem);
        menuStrip.Items.Add(fileMenu);
        form.MainMenuStrip = menuStrip;

        MethodInfo? method = typeof(Form).GetMethod("TryHandleMenuShortcut", BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(Keys)], null);
        Assert.NotNull(method);

        bool handled = Assert.IsType<bool>(method!.Invoke(form, [Keys.Control | Keys.S]));

        Assert.True(handled);
        Assert.Equal(1, clickCount);
    }
}