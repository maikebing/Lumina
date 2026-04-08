using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
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
    public void Form_UsesEmbeddedDefaultIcon_WhenUnset()
    {
        Type providerType = typeof(Form).Assembly.GetType("Lumina.Forms.DefaultAppIconProvider", throwOnError: true)!;
        MethodInfo? providerMethod = providerType.GetMethod("GetIconHandle", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo? resolveMethod = typeof(Form).GetMethod("ResolveIconHandle", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(providerMethod);
        Assert.NotNull(resolveMethod);

        using var form = new Form();
        nint defaultHandle = Assert.IsType<nint>(providerMethod!.Invoke(null, null));
        nint resolvedHandle = Assert.IsType<nint>(resolveMethod!.Invoke(form, null));

        Assert.NotEqual(0, defaultHandle);
        Assert.Equal(defaultHandle, resolvedHandle);
        Assert.NotNull(form.Icon);
    }

    [Fact]
    public void NotifyIcon_UsesEmbeddedDefaultIcon_WhenUnset()
    {
        Type providerType = typeof(Form).Assembly.GetType("Lumina.Forms.DefaultAppIconProvider", throwOnError: true)!;
        MethodInfo? providerMethod = providerType.GetMethod("GetIconHandle", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo? resolveMethod = typeof(NotifyIcon).GetMethod("ResolveIconHandle", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(providerMethod);
        Assert.NotNull(resolveMethod);

        using var notifyIcon = new NotifyIcon();
        nint defaultHandle = Assert.IsType<nint>(providerMethod!.Invoke(null, null));
        nint resolvedHandle = Assert.IsType<nint>(resolveMethod!.Invoke(notifyIcon, null));

        Assert.NotEqual(0, defaultHandle);
        Assert.Equal(defaultHandle, resolvedHandle);
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
    public void StripDefaults_MatchWinFormsBaselineMetrics()
    {
        Assert.Equal(new Size(100, 25), new ToolStrip().Size);
        Assert.Equal(new Padding(0, 0, 1, 0), new ToolStrip().Padding);

        Assert.Equal(new Size(200, 24), new MenuStrip().Size);
        Assert.Equal(new Padding(6, 2, 0, 2), new MenuStrip().Padding);

        Assert.Equal(new Size(200, 22), new StatusStrip().Size);
        Assert.Equal(new Padding(1, 0, 14, 0), new StatusStrip().Padding);
    }

    [Fact]
    public void StripTextHosts_UseWinFormsLikePaddingOverMeasuredTextWidth()
    {
        var toolStrip = new ToolStrip();
        var label = new ToolStripLabel { Text = "主题" };
        var button = new ToolStripButton { Text = "主题" };
        toolStrip.Items.Add(label);
        toolStrip.Items.Add(button);
        toolStrip.PerformLayout();

        var menuStrip = new MenuStrip();
        var menuItem = new ToolStripMenuItem { Text = "主题" };
        menuStrip.Items.Add(menuItem);
        menuStrip.PerformLayout();

        var statusStrip = new StatusStrip();
        var statusLabel = new ToolStripStatusLabel { Text = "主题" };
        statusStrip.Items.Add(statusLabel);
        statusStrip.PerformLayout();

        FieldInfo? itemHostsField = typeof(ToolStrip).GetField("_itemHosts", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(itemHostsField);

        var toolStripHosts = Assert.IsType<Dictionary<ToolStripItem, Control>>(itemHostsField!.GetValue(toolStrip));
        var menuStripHosts = Assert.IsType<Dictionary<ToolStripItem, Control>>(itemHostsField.GetValue(menuStrip));
        var statusStripHosts = Assert.IsType<Dictionary<ToolStripItem, Control>>(itemHostsField.GetValue(statusStrip));

        Control labelHost = toolStripHosts[label];
        Control buttonHost = toolStripHosts[button];
        Control menuHost = menuStripHosts[menuItem];
        Control statusHost = statusStripHosts[statusLabel];

        Assert.Equal(labelHost.Width, statusHost.Width);
        Assert.Equal(labelHost.Width + 4, buttonHost.Width);
        Assert.Equal(labelHost.Width + 12, menuHost.Width);
        Assert.Equal(20, menuHost.Height);
    }

    [Fact]
    public void MenuStrip_MaterializesHosts_WhenActingAsMainMenuLikeWinForms()
    {
        var form = new Form();
        var menuStrip = new MenuStrip();
        form.Controls.Add(menuStrip);
        form.MainMenuStrip = menuStrip;

        var fileMenu = new ToolStripMenuItem { Text = "File" };
        menuStrip.Items.Add(fileMenu);
        menuStrip.PerformLayout();

        FieldInfo? itemHostsField = typeof(ToolStrip).GetField("_itemHosts", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(itemHostsField);

        var itemHosts = Assert.IsType<Dictionary<ToolStripItem, Control>>(itemHostsField!.GetValue(menuStrip));
        Assert.Single(itemHosts);
        Assert.IsAssignableFrom<Label>(itemHosts[fileMenu]);
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
    public void MonthCalendar_DefaultSize_MatchesSingleMonthBaseline()
    {
        var monthCalendar = new MonthCalendar();

        Assert.Equal(new Size(178, 155), monthCalendar.Size);
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

    [Fact]
    public void ToolStrip_TryActivateMnemonic_InvokesMatchingItem()
    {
        var menuStrip = new MenuStrip();
        var fileMenu = new ToolStripMenuItem { Text = "&File" };
        int clickCount = 0;
        fileMenu.Click += (_, _) => clickCount++;
        menuStrip.Items.Add(fileMenu);

        MethodInfo? method = typeof(ToolStrip).GetMethod("TryActivateMnemonic", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        bool handled = Assert.IsType<bool>(method!.Invoke(menuStrip, ['f']));

        Assert.True(handled);
        Assert.Equal(1, clickCount);
    }

    [Fact]
    public void Form_MenuMnemonic_InvokesMatchingTopLevelItem()
    {
        var form = new Form();
        var menuStrip = new MenuStrip();
        var helpItem = new ToolStripMenuItem { Text = "&Help" };
        int clickCount = 0;
        helpItem.Click += (_, _) => clickCount++;
        menuStrip.Items.Add(helpItem);
        form.MainMenuStrip = menuStrip;

        MethodInfo? method = typeof(Form).GetMethod("TryHandleMenuMnemonic", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        bool handled = Assert.IsType<bool>(method!.Invoke(form, ['h']));

        Assert.True(handled);
        Assert.Equal(1, clickCount);
    }

    [Fact]
    public void ToolStripPopupMenu_CreateMenuBitmap_ReturnsBitmapHandleForImageItems()
    {
        using var bitmap = new Bitmap(12, 12);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Red);
        }

        var item = new ToolStripMenuItem
        {
            Image = bitmap,
        };

        Type popupMenuType = typeof(MenuStrip).Assembly.GetType("Lumina.Forms.ToolStripPopupMenu", throwOnError: true)!;
        MethodInfo? method = popupMenuType.GetMethod("CreateMenuBitmap", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(method);

        nint bitmapHandle = Assert.IsType<nint>(method!.Invoke(null, [item]));
        try
        {
            Assert.NotEqual(0, bitmapHandle);
        }
        finally
        {
            if (bitmapHandle != 0)
            {
                _ = DeleteObject(bitmapHandle);
            }
        }
    }

    [Fact]
    public void NativeMenuRenderer_RegisterItem_AllowsOwnerDrawMeasurement()
    {
        var item = new ToolStripMenuItem
        {
            Text = "&Theme",
            ShortcutKeys = Keys.Control | Keys.T,
            Checked = true,
        };

        Type assemblyTypeAnchor = typeof(MenuStrip);
        Type rendererType = assemblyTypeAnchor.Assembly.GetType("Lumina.Forms.NativeMenuRenderer", throwOnError: true)!;
        Type measureItemType = assemblyTypeAnchor.Assembly.GetType("Lumina.Forms.Win32+MEASUREITEMSTRUCT", throwOnError: true)!;

        MethodInfo? registerMethod = rendererType.GetMethod("Register", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo? unregisterMethod = rendererType.GetMethod("Unregister", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo? measureMethod = rendererType.GetMethod("TryHandleMeasureItem", BindingFlags.Static | BindingFlags.NonPublic);

        Assert.NotNull(registerMethod);
        Assert.NotNull(unregisterMethod);
        Assert.NotNull(measureMethod);

        nuint ownerDrawKey = (nuint)registerMethod!.Invoke(null, [item, new Form().CurrentVisualStyle])!;
        object measureItem = Activator.CreateInstance(measureItemType)!;
        measureItemType.GetField("CtlType")!.SetValue(measureItem, 1u);
        measureItemType.GetField("itemData")!.SetValue(measureItem, ownerDrawKey);

        nint buffer = Marshal.AllocHGlobal(Marshal.SizeOf(measureItemType));
        try
        {
            Marshal.StructureToPtr(measureItem, buffer, false);

            bool handled = Assert.IsType<bool>(measureMethod!.Invoke(null, [buffer]));
            Assert.True(handled);

            object updated = Marshal.PtrToStructure(buffer, measureItemType)!;
            uint width = Assert.IsType<uint>(measureItemType.GetField("itemWidth")!.GetValue(updated));
            uint height = Assert.IsType<uint>(measureItemType.GetField("itemHeight")!.GetValue(updated));

            Assert.True(width >= 120);
            Assert.True(height >= 28);
        }
        finally
        {
            unregisterMethod!.Invoke(null, [ownerDrawKey]);
            Marshal.FreeHGlobal(buffer);
        }
    }

    [DllImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(nint hObject);

    [Fact]
    public void ToolStripMenuItem_RadioCheck_PropertyRoundTrips()
    {
        var item = new ToolStripMenuItem { Text = "Option A", Checked = true, RadioCheck = true };

        Assert.True(item.RadioCheck);
        Assert.True(item.Checked);

        item.RadioCheck = false;
        Assert.False(item.RadioCheck);
    }

    [Fact]
    public void ToolStripMenuItem_RadioCheck_DefaultIsFalse()
    {
        var item = new ToolStripMenuItem { Text = "Option B" };

        Assert.False(item.RadioCheck);
    }

    [Fact]
    public void ToolStripPopupMenu_NotifyMenuDepthChange_DoesNotThrow()
    {
        // Depth tracking calls are safe to invoke directly (they just update ThreadStatic state).
        Type type = typeof(MenuStrip).Assembly.GetType("Lumina.Forms.ToolStripPopupMenu", throwOnError: true)!;
        MethodInfo? incMethod = type.GetMethod("NotifyMenuDepthChange", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        MethodInfo? selMethod = type.GetMethod("NotifyMenuSelectionChanged", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        Assert.NotNull(incMethod);
        Assert.NotNull(selMethod);

        // Increment, then decrement — should not throw.
        incMethod!.Invoke(null, [1]);
        selMethod!.Invoke(null, [true]);
        incMethod!.Invoke(null, [-1]);
    }

    [Fact]
    public void StatusStrip_ReplacingItemHost_DoesNotAccumulateObsoleteControls()
    {
        var statusStrip = new StatusStrip();
        var item = new ToolStripDropDownButton();

        statusStrip.Items.Add(item);
        statusStrip.PerformLayout();

        Assert.Single(statusStrip.Controls);
        Assert.IsAssignableFrom<Button>(statusStrip.Controls[0]);

        using var bitmap = new Bitmap(16, 16);
        item.DisplayStyle = ToolStripItemDisplayStyle.Image;
        item.Image = bitmap;

        statusStrip.PerformLayout();

        Assert.Single(statusStrip.Controls);
        Assert.IsAssignableFrom<PictureBox>(statusStrip.Controls[0]);
    }

    [Fact]
    public void StatusStrip_MaterializesStatusLabelHosts()
    {
        var statusStrip = new StatusStrip();
        var item = new ToolStripStatusLabel { Text = "Ready" };

        statusStrip.Items.Add(item);
        statusStrip.PerformLayout();

        Assert.Single(statusStrip.Controls);
        Assert.IsAssignableFrom<Label>(statusStrip.Controls[0]);
    }
}
