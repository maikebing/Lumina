#nullable disable

namespace Lumina.NotepadDemo;

partial class frmNotepad
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        menuStrip1 = new MenuStrip();
        文件ToolStripMenuItem = new ToolStripMenuItem();
        新建ToolStripMenuItem = new ToolStripMenuItem();
        打开ToolStripMenuItem = new ToolStripMenuItem();
        打开文件夹ToolStripMenuItem = new ToolStripMenuItem();
        保存ToolStripMenuItem = new ToolStripMenuItem();
        另存为ToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        退出ToolStripMenuItem = new ToolStripMenuItem();
        编辑ToolStripMenuItem = new ToolStripMenuItem();
        查找ToolStripMenuItem = new ToolStripMenuItem();
        替换ToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator2 = new ToolStripSeparator();
        插入时间日期ToolStripMenuItem = new ToolStripMenuItem();
        统计ToolStripMenuItem = new ToolStripMenuItem();
        格式ToolStripMenuItem = new ToolStripMenuItem();
        自动换行ToolStripMenuItem = new ToolStripMenuItem();
        字体ToolStripMenuItem = new ToolStripMenuItem();
        颜色ToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator3 = new ToolStripSeparator();
        帮助ToolStripMenuItem = new ToolStripMenuItem();
        系统关于ToolStripMenuItem = new ToolStripMenuItem();
        关于ToolStripMenuItem = new ToolStripMenuItem();
        editorTextBox = new TextBox();
        statusStrip1 = new StatusStrip();
        primaryStatusLabel = new ToolStripStatusLabel();
        metricsStatusLabel = new ToolStripStatusLabel();
        menuStrip1.SuspendLayout();
        statusStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.Items.AddRange(new ToolStripItem[] { 文件ToolStripMenuItem, 编辑ToolStripMenuItem, 格式ToolStripMenuItem, 帮助ToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(1080, 25);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // 文件ToolStripMenuItem
        // 
        文件ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 新建ToolStripMenuItem, 打开ToolStripMenuItem, 打开文件夹ToolStripMenuItem, 保存ToolStripMenuItem, 另存为ToolStripMenuItem, toolStripSeparator1, 退出ToolStripMenuItem });
        文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
        文件ToolStripMenuItem.Size = new Size(44, 21);
        文件ToolStripMenuItem.Text = "文件";
        // 
        // 新建ToolStripMenuItem
        // 
        新建ToolStripMenuItem.Name = "新建ToolStripMenuItem";
        新建ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        新建ToolStripMenuItem.Size = new Size(180, 22);
        新建ToolStripMenuItem.Text = "新建";
        新建ToolStripMenuItem.Click += 新建ToolStripMenuItem_Click;
        // 
        // 打开ToolStripMenuItem
        // 
        打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
        打开ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        打开ToolStripMenuItem.Size = new Size(180, 22);
        打开ToolStripMenuItem.Text = "打开";
        打开ToolStripMenuItem.Click += 打开ToolStripMenuItem_Click;
        // 
        // 打开文件夹ToolStripMenuItem
        // 
        打开文件夹ToolStripMenuItem.Name = "打开文件夹ToolStripMenuItem";
        打开文件夹ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.O;
        打开文件夹ToolStripMenuItem.Size = new Size(206, 22);
        打开文件夹ToolStripMenuItem.Text = "打开文件夹";
        打开文件夹ToolStripMenuItem.Click += 打开文件夹ToolStripMenuItem_Click;
        // 
        // 保存ToolStripMenuItem
        // 
        保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
        保存ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        保存ToolStripMenuItem.Size = new Size(206, 22);
        保存ToolStripMenuItem.Text = "保存";
        保存ToolStripMenuItem.Click += 保存ToolStripMenuItem_Click;
        // 
        // 另存为ToolStripMenuItem
        // 
        另存为ToolStripMenuItem.Name = "另存为ToolStripMenuItem";
        另存为ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
        另存为ToolStripMenuItem.Size = new Size(206, 22);
        另存为ToolStripMenuItem.Text = "另存为";
        另存为ToolStripMenuItem.Click += 另存为ToolStripMenuItem_Click;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(203, 6);
        // 
        // 退出ToolStripMenuItem
        // 
        退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
        退出ToolStripMenuItem.Size = new Size(206, 22);
        退出ToolStripMenuItem.Text = "退出";
        退出ToolStripMenuItem.Click += 退出ToolStripMenuItem_Click;
        // 
        // 编辑ToolStripMenuItem
        // 
        编辑ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 查找ToolStripMenuItem, 替换ToolStripMenuItem, toolStripSeparator2, 插入时间日期ToolStripMenuItem, 统计ToolStripMenuItem });
        编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
        编辑ToolStripMenuItem.Size = new Size(44, 21);
        编辑ToolStripMenuItem.Text = "编辑";
        // 
        // 查找ToolStripMenuItem
        // 
        查找ToolStripMenuItem.Name = "查找ToolStripMenuItem";
        查找ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
        查找ToolStripMenuItem.Size = new Size(180, 22);
        查找ToolStripMenuItem.Text = "查找";
        查找ToolStripMenuItem.Click += 查找ToolStripMenuItem_Click;
        // 
        // 替换ToolStripMenuItem
        // 
        替换ToolStripMenuItem.Name = "替换ToolStripMenuItem";
        替换ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.H;
        替换ToolStripMenuItem.Size = new Size(180, 22);
        替换ToolStripMenuItem.Text = "替换";
        替换ToolStripMenuItem.Click += 替换ToolStripMenuItem_Click;
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(177, 6);
        // 
        // 插入时间日期ToolStripMenuItem
        // 
        插入时间日期ToolStripMenuItem.Name = "插入时间日期ToolStripMenuItem";
        插入时间日期ToolStripMenuItem.ShortcutKeys = Keys.F5;
        插入时间日期ToolStripMenuItem.Size = new Size(180, 22);
        插入时间日期ToolStripMenuItem.Text = "时间/日期";
        插入时间日期ToolStripMenuItem.Click += 插入时间日期ToolStripMenuItem_Click;
        // 
        // 统计ToolStripMenuItem
        // 
        统计ToolStripMenuItem.Name = "统计ToolStripMenuItem";
        统计ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.W;
        统计ToolStripMenuItem.Size = new Size(180, 22);
        统计ToolStripMenuItem.Text = "字数统计";
        统计ToolStripMenuItem.Click += 统计ToolStripMenuItem_Click;
        // 
        // 格式ToolStripMenuItem
        // 
        格式ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 自动换行ToolStripMenuItem, toolStripSeparator3, 字体ToolStripMenuItem, 颜色ToolStripMenuItem });
        格式ToolStripMenuItem.Name = "格式ToolStripMenuItem";
        格式ToolStripMenuItem.Size = new Size(44, 21);
        格式ToolStripMenuItem.Text = "格式";
        // 
        // 自动换行ToolStripMenuItem
        // 
        自动换行ToolStripMenuItem.Checked = true;
        自动换行ToolStripMenuItem.CheckOnClick = true;
        自动换行ToolStripMenuItem.Name = "自动换行ToolStripMenuItem";
        自动换行ToolStripMenuItem.Size = new Size(124, 22);
        自动换行ToolStripMenuItem.Text = "自动换行";
        自动换行ToolStripMenuItem.Click += 自动换行ToolStripMenuItem_Click;
        // 
        // 字体ToolStripMenuItem
        // 
        字体ToolStripMenuItem.Name = "字体ToolStripMenuItem";
        字体ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.F;
        字体ToolStripMenuItem.Size = new Size(180, 22);
        字体ToolStripMenuItem.Text = "字体...";
        字体ToolStripMenuItem.Click += 字体ToolStripMenuItem_Click;
        // 
        // 颜色ToolStripMenuItem
        // 
        颜色ToolStripMenuItem.Name = "颜色ToolStripMenuItem";
        颜色ToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.C;
        颜色ToolStripMenuItem.Size = new Size(180, 22);
        颜色ToolStripMenuItem.Text = "颜色...";
        颜色ToolStripMenuItem.Click += 颜色ToolStripMenuItem_Click;
        // 
        // toolStripSeparator3
        // 
        toolStripSeparator3.Name = "toolStripSeparator3";
        toolStripSeparator3.Size = new Size(177, 6);
        // 
        // 帮助ToolStripMenuItem
        // 
        帮助ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 系统关于ToolStripMenuItem, 关于ToolStripMenuItem });
        帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
        帮助ToolStripMenuItem.Size = new Size(44, 21);
        帮助ToolStripMenuItem.Text = "帮助";
        // 
        // 系统关于ToolStripMenuItem
        // 
        系统关于ToolStripMenuItem.Name = "系统关于ToolStripMenuItem";
        系统关于ToolStripMenuItem.Size = new Size(180, 22);
        系统关于ToolStripMenuItem.Text = "系统关于";
        系统关于ToolStripMenuItem.Click += 系统关于ToolStripMenuItem_Click;
        // 
        // 关于ToolStripMenuItem
        // 
        关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
        关于ToolStripMenuItem.Size = new Size(180, 22);
        关于ToolStripMenuItem.Text = "关于";
        关于ToolStripMenuItem.Click += 关于ToolStripMenuItem_Click;
        // 
        // editorTextBox
        // 
        editorTextBox.Dock = DockStyle.Fill;
        editorTextBox.Location = new Point(0, 25);
        editorTextBox.Multiline = true;
        editorTextBox.Name = "editorTextBox";
        editorTextBox.Size = new Size(1080, 589);
        editorTextBox.TabIndex = 2;
        editorTextBox.TextChanged += editorTextBox_TextChanged;
        // 
        // statusStrip1
        // 
        statusStrip1.Items.AddRange(new ToolStripItem[] { primaryStatusLabel, metricsStatusLabel });
        statusStrip1.Location = new Point(0, 614);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new Size(1080, 22);
        statusStrip1.TabIndex = 3;
        statusStrip1.Text = "statusStrip1";
        // 
        // primaryStatusLabel
        // 
        primaryStatusLabel.Name = "primaryStatusLabel";
        primaryStatusLabel.Spring = true;
        primaryStatusLabel.Size = new Size(913, 17);
        primaryStatusLabel.Text = "就绪";
        // 
        // metricsStatusLabel
        // 
        metricsStatusLabel.Name = "metricsStatusLabel";
        metricsStatusLabel.Size = new Size(152, 17);
        metricsStatusLabel.Text = "行: 1 | 字符: 0 | 词: 0";
        // 
        // frmNotepad
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1080, 636);
        Controls.Add(editorTextBox);
        Controls.Add(statusStrip1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Name = "frmNotepad";
        Text = "Lumina Notepad";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip1;
    private ToolStripMenuItem 文件ToolStripMenuItem;
    private ToolStripMenuItem 新建ToolStripMenuItem;
    private ToolStripMenuItem 打开ToolStripMenuItem;
    private ToolStripMenuItem 打开文件夹ToolStripMenuItem;
    private ToolStripMenuItem 保存ToolStripMenuItem;
    private ToolStripMenuItem 另存为ToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem 退出ToolStripMenuItem;
    private ToolStripMenuItem 编辑ToolStripMenuItem;
    private ToolStripMenuItem 查找ToolStripMenuItem;
    private ToolStripMenuItem 替换ToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem 插入时间日期ToolStripMenuItem;
    private ToolStripMenuItem 统计ToolStripMenuItem;
    private ToolStripMenuItem 格式ToolStripMenuItem;
    private ToolStripMenuItem 自动换行ToolStripMenuItem;
    private ToolStripMenuItem 字体ToolStripMenuItem;
    private ToolStripMenuItem 颜色ToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripMenuItem 帮助ToolStripMenuItem;
    private ToolStripMenuItem 系统关于ToolStripMenuItem;
    private ToolStripMenuItem 关于ToolStripMenuItem;
    private TextBox editorTextBox;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel primaryStatusLabel;
    private ToolStripStatusLabel metricsStatusLabel;
}
