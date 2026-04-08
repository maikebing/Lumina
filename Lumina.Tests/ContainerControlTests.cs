using System.Drawing;
using System.Reflection;
using Lumina.Forms;
using Xunit;

namespace Lumina.Tests;

public class ContainerControlTests
{
    [Fact]
    public void GroupBox_CanOwnChildControlsBeforeBeingAddedToForm()
    {
        using var form = new Form();
        var groupBox = new GroupBox();
        var button = new Button();

        groupBox.Controls.Add(button);
        form.Controls.Add(groupBox);

        Assert.Same(groupBox, button.Parent);
    }

    [Fact]
    public void GroupBox_Defaults_MatchWinFormsBaseline()
    {
        var groupBox = new GroupBox();
        Font? messageBoxFont = SystemFonts.MessageBoxFont;
        Assert.NotNull(messageBoxFont);
        int fontHeight = messageBoxFont!.Height;

        Assert.Equal(new Size(200, 100), groupBox.Size);
        Assert.Equal(new Padding(3), groupBox.Padding);
        Assert.Equal(
            new Rectangle(3, fontHeight + 3, 194, Math.Max(0, 100 - fontHeight - 6)),
            groupBox.DisplayRectangle);

        PropertyInfo? exStyleProperty = typeof(Control).GetProperty("ExStyle", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(exStyleProperty);

        uint exStyle = Assert.IsType<uint>(exStyleProperty!.GetValue(groupBox));
        Assert.NotEqual(0u, exStyle & 0x00010000u);
    }

    [Fact]
    public void PerformAutoScale_ScalesNestedContainerChildren()
    {
        using var form = new Form
        {
            AutoScaleMode = AutoScaleMode.Dpi,
            AutoScaleDimensions = new SizeF(48f, 48f),
            ClientSize = new Size(100, 80),
        };

        var panel = new Panel();
        panel.SetBounds(8, 10, 50, 30);

        var label = new Label();
        label.SetBounds(5, 6, 20, 10);

        panel.Controls.Add(label);
        form.Controls.Add(panel);

        form.PerformAutoScale();

        Assert.True(panel.Left >= 16);
        Assert.True(panel.Top >= 20);
        Assert.True(panel.Width >= 100);
        Assert.True(panel.Height >= 60);
        Assert.True(label.Left >= 10);
        Assert.True(label.Top >= 12);
        Assert.True(label.Width >= 40);
        Assert.True(label.Height >= 20);
    }

    [Fact]
    public void ControlCollections_AddRangeAndFind_WorkForNestedControls()
    {
        using var form = new Form();

        var panel = new Panel { Name = "panelRoot" };
        var firstButton = new Button { Name = "actionButton" };
        var secondButton = new Button { Name = "confirmButton" };
        var nestedLabel = new Label { Name = "statusText" };

        panel.Controls.AddRange(firstButton, nestedLabel);
        form.Controls.AddRange(panel, secondButton);

        Assert.Equal(2, form.Controls.Count);
        Assert.Equal(2, panel.Controls.Count);
        Assert.True(form.Controls.Contains(secondButton));
        Assert.Single(form.Controls.Find("confirmButton", searchAllChildren: false));
        Assert.Single(form.Controls.Find("statusText", searchAllChildren: true));
        Assert.Single(panel.Controls.Find("statusText", searchAllChildren: false));
    }

    [Fact]
    public void SplitContainer_PerformLayout_SizesPanelsFromSplitterDistance()
    {
        var splitContainer = new SplitContainer();
        splitContainer.SetBounds(0, 0, 300, 120);
        splitContainer.SplitterDistance = 90;

        splitContainer.PerformLayout();

        Assert.Equal(90, splitContainer.Panel1.Width);
        Assert.Equal(94, splitContainer.Panel2.Left);
        Assert.Equal(206, splitContainer.Panel2.Width);
        Assert.Equal(new Rectangle(90, 0, 4, 120), splitContainer.SplitterRectangle);
    }

    [Fact]
    public void SplitContainer_PerformLayout_ClampsHorizontalPanelsAgainstMinimumSizes()
    {
        var splitContainer = new SplitContainer
        {
            Orientation = Orientation.Horizontal,
            Panel1MinSize = 30,
            Panel2MinSize = 40,
        };

        splitContainer.SetBounds(0, 0, 140, 120);
        splitContainer.SplitterDistance = 100;

        splitContainer.PerformLayout();

        Assert.Equal(76, splitContainer.Panel1.Height);
        Assert.Equal(80, splitContainer.Panel2.Top);
        Assert.Equal(40, splitContainer.Panel2.Height);
    }

    [Fact]
    public void SplitContainer_FixedPanel_Panel2_PreservesTrailingPanelSizeOnResize()
    {
        var splitContainer = new SplitContainer
        {
            FixedPanel = FixedPanel.Panel2,
        };

        splitContainer.SetBounds(0, 0, 300, 120);
        splitContainer.SplitterDistance = 90;
        splitContainer.PerformLayout();

        Assert.Equal(206, splitContainer.Panel2.Width);

        splitContainer.SetBounds(0, 0, 360, 120);
        splitContainer.PerformLayout();

        Assert.Equal(150, splitContainer.Panel1.Width);
        Assert.Equal(206, splitContainer.Panel2.Width);
    }

    [Fact]
    public void SplitContainer_CollapsingPanel_HidesCollapsedPanelAndExpandsVisiblePanel()
    {
        var splitContainer = new SplitContainer();
        splitContainer.SetBounds(0, 0, 200, 100);
        splitContainer.Panel1Collapsed = true;

        splitContainer.PerformLayout();

        Assert.False(splitContainer.Panel1.Visible);
        Assert.True(splitContainer.Panel2.Visible);
        Assert.Equal(new Rectangle(0, 0, 200, 100), splitContainer.Panel2.Bounds);
        Assert.Equal(Rectangle.Empty, splitContainer.SplitterRectangle);
    }

    [Fact]
    public void Splitter_SplitPosition_ResizesAdjacentControls()
    {
        using var form = new Form
        {
            ClientSize = new Size(300, 100),
        };

        var leftPanel = new Panel();
        leftPanel.SetBounds(0, 0, 100, 100);

        var splitter = new Splitter();
        splitter.SetBounds(100, 0, 3, 100);

        var fillPanel = new Panel();
        fillPanel.SetBounds(103, 0, 197, 100);

        form.Controls.AddRange(leftPanel, splitter, fillPanel);

        splitter.SplitPosition = 120;

        Assert.Equal(120, leftPanel.Width);
        Assert.Equal(120, splitter.Left);
        Assert.Equal(123, fillPanel.Left);
        Assert.Equal(177, fillPanel.Width);
    }

    [Fact]
    public void TabControl_PerformLayout_SetsPageBoundsAndVisibility()
    {
        var tabControl = new TabControl();
        var firstPage = new TabPage();
        var secondPage = new TabPage();

        tabControl.Controls.AddRange(firstPage, secondPage);
        tabControl.SetBounds(0, 0, 200, 100);
        tabControl.SelectedIndex = 1;

        tabControl.PerformLayout();

        Assert.False(firstPage.Visible);
        Assert.True(secondPage.Visible);
        Assert.Equal(new Rectangle(4, 21, 192, 75), secondPage.Bounds);
    }

    [Fact]
    public void TabControl_SelectedIndex_ChangesVisiblePage()
    {
        var tabControl = new TabControl();
        var firstPage = new TabPage { Text = "First" };
        var secondPage = new TabPage { Text = "Second" };

        tabControl.Controls.AddRange(firstPage, secondPage);
        tabControl.SetBounds(0, 0, 240, 100);
        tabControl.PerformLayout();

        tabControl.SelectedIndex = 1;

        Assert.Equal(1, tabControl.SelectedIndex);
        Assert.False(firstPage.Visible);
        Assert.True(secondPage.Visible);
    }

    [Fact]
    public void TabControl_Defaults_MatchWinFormsBaseline()
    {
        var tabControl = new TabControl();

        Assert.Equal(new Size(200, 100), tabControl.Size);
        Assert.Equal(-1, tabControl.SelectedIndex);
        Assert.Equal(new Rectangle(4, 4, 192, 92), tabControl.DisplayRectangle);
    }

    [Fact]
    public void TabControl_PerformLayout_SelectsFirstPage_WhenPagesArePresent()
    {
        var tabControl = new TabControl();
        var firstPage = new TabPage { Text = "First" };
        var secondPage = new TabPage { Text = "Second" };

        tabControl.Controls.AddRange(firstPage, secondPage);
        tabControl.PerformLayout();

        Assert.Equal(0, tabControl.SelectedIndex);
        Assert.Same(firstPage, tabControl.SelectedTab);
        Assert.True(firstPage.Visible);
        Assert.False(secondPage.Visible);
    }

    [Fact]
    public void TableLayoutPanel_PerformLayout_AssignsControlsToCells()
    {
        var tableLayoutPanel = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 2,
        };

        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        tableLayoutPanel.SetBounds(0, 0, 200, 100);

        var firstControl = new Button();
        var secondControl = new Button();
        firstControl.Size = new Size(100, 50);
        secondControl.Size = new Size(100, 50);
        firstControl.Margin = Padding.Empty;
        secondControl.Margin = Padding.Empty;
        tableLayoutPanel.Controls.Add(firstControl, 0, 0);
        tableLayoutPanel.Controls.Add(secondControl, 1, 1);

        tableLayoutPanel.PerformLayout();

        Assert.Equal(new Rectangle(0, 0, 100, 50), firstControl.Bounds);
        Assert.Equal(new Rectangle(100, 50, 100, 50), secondControl.Bounds);
    }

    [Fact]
    public void TableLayoutPanel_PerformLayout_UsesDefaultMarginsInsideCells()
    {
        var tableLayoutPanel = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 2,
        };

        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        tableLayoutPanel.SetBounds(0, 0, 200, 100);

        var firstControl = new Button { Size = new Size(75, 23) };
        var secondControl = new TextBox { Size = new Size(80, 23) };
        tableLayoutPanel.Controls.Add(firstControl, 0, 0);
        tableLayoutPanel.Controls.Add(secondControl, 1, 1);

        tableLayoutPanel.PerformLayout();

        Assert.Equal(new Rectangle(3, 3, 75, 23), firstControl.Bounds);
        Assert.Equal(new Rectangle(103, 53, 80, 23), secondControl.Bounds);
    }

    [Fact]
    public void FlowLayoutPanel_PerformLayout_UsesPaddingMarginsAndWrap()
    {
        var flowLayoutPanel = new FlowLayoutPanel
        {
            Padding = new Padding(4),
        };
        flowLayoutPanel.SetBounds(0, 0, 70, 80);

        var firstControl = new Button { Size = new Size(20, 10) };
        var secondControl = new Button { Size = new Size(20, 10) };
        var thirdControl = new Button { Size = new Size(20, 10) };

        flowLayoutPanel.Controls.AddRange(firstControl, secondControl, thirdControl);
        flowLayoutPanel.PerformLayout();

        Assert.Equal(new Point(7, 7), firstControl.Location);
        Assert.Equal(new Point(33, 7), secondControl.Location);
        Assert.Equal(new Point(7, 23), thirdControl.Location);
    }
}
