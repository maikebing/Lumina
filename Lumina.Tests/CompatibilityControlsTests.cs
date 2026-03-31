using System.ComponentModel;
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
}