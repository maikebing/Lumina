namespace NativeFormsDemo
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TreeNode treeNode1 = new TreeNode("节点4");
            TreeNode treeNode2 = new TreeNode("节点6");
            TreeNode treeNode3 = new TreeNode("节点5", new TreeNode[] { treeNode2 });
            TreeNode treeNode4 = new TreeNode("节点0", new TreeNode[] { treeNode1, treeNode3 });
            TreeNode treeNode5 = new TreeNode("节点7");
            TreeNode treeNode6 = new TreeNode("节点1", new TreeNode[] { treeNode5 });
            TreeNode treeNode7 = new TreeNode("节点8");
            TreeNode treeNode8 = new TreeNode("节点9");
            TreeNode treeNode9 = new TreeNode("节点2", new TreeNode[] { treeNode7, treeNode8 });
            TreeNode treeNode10 = new TreeNode("节点12");
            TreeNode treeNode11 = new TreeNode("节点11", new TreeNode[] { treeNode10 });
            TreeNode treeNode12 = new TreeNode("节点10", new TreeNode[] { treeNode11 });
            TreeNode treeNode13 = new TreeNode("节点3", new TreeNode[] { treeNode12 });
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            button1 = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            菜单1ToolStripMenuItem = new ToolStripMenuItem();
            sdfToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripTextBox1 = new ToolStripTextBox();
            fsdaToolStripMenuItem = new ToolStripMenuItem();
            checkBox1 = new CheckBox();
            comboBox1 = new ComboBox();
            dateTimePicker1 = new DateTimePicker();
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            listBox1 = new ListBox();
            listView1 = new ListView();
            maskedTextBox1 = new MaskedTextBox();
            monthCalendar1 = new MonthCalendar();
            notifyIcon1 = new NotifyIcon(components);
            numericUpDown1 = new NumericUpDown();
            pictureBox1 = new PictureBox();
            progressBar1 = new ProgressBar();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            textBox1 = new TextBox();
            toolTip1 = new ToolTip(components);
            treeView1 = new TreeView();
            menuStrip1 = new MenuStrip();
            文件ToolStripMenuItem = new ToolStripMenuItem();
            编辑ToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripComboBox1 = new ToolStripComboBox();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripTextBox2 = new ToolStripTextBox();
            关于ToolStripMenuItem = new ToolStripMenuItem();
            guanyuToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            toolStripMenuItem3 = new ToolStripMenuItem();
            toolStripComboBox2 = new ToolStripComboBox();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripTextBox3 = new ToolStripTextBox();
            toolStripSplitButton1 = new ToolStripSplitButton();
            toolStripMenuItem4 = new ToolStripMenuItem();
            toolStripSplitButton2 = new ToolStripSplitButton();
            toolStrip1 = new ToolStrip();
            flowLayoutPanel1 = new FlowLayoutPanel();
            button2 = new Button();
            checkBox2 = new CheckBox();
            groupBox1 = new GroupBox();
            button3 = new Button();
            panel1 = new Panel();
            button4 = new Button();
            splitContainer1 = new SplitContainer();
            button5 = new Button();
            checkBox3 = new CheckBox();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            button6 = new Button();
            tabPage2 = new TabPage();
            comboBox2 = new ComboBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            radioButton3 = new RadioButton();
            radioButton4 = new RadioButton();
            textBox2 = new TextBox();
            button7 = new Button();
            richTextBox1 = new RichTextBox();
            视图ToolStripMenuItem = new ToolStripMenuItem();
            luminaExtWinFormsToolStripMenuItem = new ToolStripMenuItem();
            clearEffectToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            micaToolStripMenuItem = new ToolStripMenuItem();
            micaAltToolStripMenuItem = new ToolStripMenuItem();
            acrylicToolStripMenuItem = new ToolStripMenuItem();
            aeroToolStripMenuItem = new ToolStripMenuItem();
            blurToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.ContextMenuStrip = contextMenuStrip1;
            button1.Location = new Point(24, 64);
            button1.Name = "button1";
            button1.Size = new Size(120, 32);
            button1.TabIndex = 0;
            button1.Text = "Primary Button";
            button1.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 菜单1ToolStripMenuItem, sdfToolStripMenuItem, fsdaToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(108, 70);
            // 
            // 菜单1ToolStripMenuItem
            // 
            菜单1ToolStripMenuItem.Name = "菜单1ToolStripMenuItem";
            菜单1ToolStripMenuItem.Size = new Size(107, 22);
            菜单1ToolStripMenuItem.Text = "菜单1";
            // 
            // sdfToolStripMenuItem
            // 
            sdfToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem2, toolStripTextBox1 });
            sdfToolStripMenuItem.Name = "sdfToolStripMenuItem";
            sdfToolStripMenuItem.Size = new Size(107, 22);
            sdfToolStripMenuItem.Text = "sdf";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(160, 22);
            toolStripMenuItem2.Text = "23";
            // 
            // toolStripTextBox1
            // 
            toolStripTextBox1.Name = "toolStripTextBox1";
            toolStripTextBox1.Size = new Size(100, 23);
            // 
            // fsdaToolStripMenuItem
            // 
            fsdaToolStripMenuItem.Name = "fsdaToolStripMenuItem";
            fsdaToolStripMenuItem.Size = new Size(107, 22);
            fsdaToolStripMenuItem.Text = "fsda";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(160, 70);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(108, 21);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "Enable option";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "1", "2", "3", "4" });
            comboBox1.Location = new Point(286, 67);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(140, 25);
            comboBox1.TabIndex = 2;
            comboBox1.Text = "1";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(548, 67);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(220, 23);
            dateTimePicker1.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 107);
            label1.Name = "label1";
            label1.Size = new Size(209, 17);
            label1.TabIndex = 4;
            label1.Text = "LuminaForms compatibility sample";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(24, 130);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(204, 17);
            linkLabel1.TabIndex = 5;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Migration-friendly control surface";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            listBox1.Location = new Point(260, 180);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(160, 174);
            listBox1.TabIndex = 6;
            // 
            // listView1
            // 
            listView1.Location = new Point(436, 180);
            listView1.Name = "listView1";
            listView1.Size = new Size(236, 174);
            listView1.TabIndex = 7;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // maskedTextBox1
            // 
            maskedTextBox1.Location = new Point(784, 67);
            maskedTextBox1.Name = "maskedTextBox1";
            maskedTextBox1.Size = new Size(120, 23);
            maskedTextBox1.TabIndex = 8;
            // 
            // monthCalendar1
            // 
            monthCalendar1.Location = new Point(24, 180);
            monthCalendar1.Name = "monthCalendar1";
            monthCalendar1.TabIndex = 9;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Text = "LuminaForms Demo";
            notifyIcon1.Visible = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(440, 67);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(92, 23);
            numericUpDown1.TabIndex = 10;
            // 
            // pictureBox1
            // 
            pictureBox1.ContextMenuStrip = contextMenuStrip1;
            pictureBox1.Image = Properties.Resources.SplashScreen;
            pictureBox1.Location = new Point(260, 370);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(658, 336);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 11;
            pictureBox1.TabStop = false;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(286, 107);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(482, 18);
            progressBar1.TabIndex = 12;
            progressBar1.Value = 40;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(1210, 68);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(77, 21);
            radioButton1.TabIndex = 13;
            radioButton1.Text = "Choice A";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Checked = true;
            radioButton2.Location = new Point(1120, 68);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(77, 21);
            radioButton2.TabIndex = 14;
            radioButton2.TabStop = true;
            radioButton2.Text = "Choice B";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.ContextMenuStrip = contextMenuStrip1;
            textBox1.Location = new Point(920, 67);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(180, 23);
            textBox1.TabIndex = 15;
            textBox1.Text = "ABC 我在那";
            // 
            // treeView1
            // 
            treeView1.ContextMenuStrip = contextMenuStrip1;
            treeView1.Location = new Point(688, 180);
            treeView1.Name = "treeView1";
            treeNode1.Name = "节点4";
            treeNode1.Text = "节点4";
            treeNode2.Name = "节点6";
            treeNode2.Text = "节点6";
            treeNode3.Name = "节点5";
            treeNode3.Text = "节点5";
            treeNode4.Name = "节点0";
            treeNode4.Text = "节点0";
            treeNode5.Name = "节点7";
            treeNode5.Text = "节点7";
            treeNode6.Name = "节点1";
            treeNode6.Text = "节点1";
            treeNode7.Name = "节点8";
            treeNode7.Text = "节点8";
            treeNode8.Name = "节点9";
            treeNode8.Text = "节点9";
            treeNode9.Name = "节点2";
            treeNode9.Text = "节点2";
            treeNode10.Name = "节点12";
            treeNode10.Text = "节点12";
            treeNode11.Name = "节点11";
            treeNode11.Text = "节点11";
            treeNode12.Name = "节点10";
            treeNode12.Text = "节点10";
            treeNode13.Name = "节点3";
            treeNode13.Text = "节点3";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode4, treeNode6, treeNode9, treeNode13 });
            treeView1.Size = new Size(230, 174);
            treeView1.TabIndex = 16;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 文件ToolStripMenuItem, 视图ToolStripMenuItem, 编辑ToolStripMenuItem, 关于ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1460, 25);
            menuStrip1.TabIndex = 18;
            menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            文件ToolStripMenuItem.Size = new Size(44, 21);
            文件ToolStripMenuItem.Text = "文件";
            // 
            // 编辑ToolStripMenuItem
            // 
            编辑ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripComboBox1, toolStripSeparator1, toolStripTextBox2 });
            编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
            编辑ToolStripMenuItem.Size = new Size(44, 21);
            编辑ToolStripMenuItem.Text = "编辑";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(181, 22);
            toolStripMenuItem1.Text = "Command";
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.Items.AddRange(new object[] { "Default", "Compact", "Expanded" });
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new Size(121, 25);
            toolStripComboBox1.Text = "Default";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(178, 6);
            // 
            // toolStripTextBox2
            // 
            toolStripTextBox2.Name = "toolStripTextBox2";
            toolStripTextBox2.Size = new Size(100, 23);
            toolStripTextBox2.Text = "Filter";
            // 
            // 关于ToolStripMenuItem
            // 
            关于ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { guanyuToolStripMenuItem });
            关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            关于ToolStripMenuItem.Size = new Size(44, 21);
            关于ToolStripMenuItem.Text = "关于";
            // 
            // guanyuToolStripMenuItem
            // 
            guanyuToolStripMenuItem.Name = "guanyuToolStripMenuItem";
            guanyuToolStripMenuItem.Size = new Size(122, 22);
            guanyuToolStripMenuItem.Text = "guanyu ";
            // 
            // statusStrip1
            // 
            statusStrip1.ContextMenuStrip = contextMenuStrip1;
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripProgressBar1, toolStripDropDownButton1, toolStripSplitButton1, toolStripSplitButton2 });
            statusStrip1.Location = new Point(0, 878);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1460, 22);
            statusStrip1.TabIndex = 19;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(44, 17);
            toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 16);
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem3, toolStripComboBox2, toolStripSeparator2, toolStripTextBox3 });
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(29, 20);
            toolStripDropDownButton1.Text = "Menu";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(181, 22);
            toolStripMenuItem3.Text = "Status";
            // 
            // toolStripComboBox2
            // 
            toolStripComboBox2.Items.AddRange(new object[] { "Status A", "Status B" });
            toolStripComboBox2.Name = "toolStripComboBox2";
            toolStripComboBox2.Size = new Size(121, 25);
            toolStripComboBox2.Text = "Status A";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(178, 6);
            // 
            // toolStripTextBox3
            // 
            toolStripTextBox3.Name = "toolStripTextBox3";
            toolStripTextBox3.Size = new Size(100, 23);
            toolStripTextBox3.Text = "Search";
            // 
            // toolStripSplitButton1
            // 
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem4 });
            toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(32, 20);
            toolStripSplitButton1.Text = "Options";
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new Size(112, 22);
            toolStripMenuItem4.Text = "Action";
            // 
            // toolStripSplitButton2
            // 
            toolStripSplitButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton2.Image = (Image)resources.GetObject("toolStripSplitButton2.Image");
            toolStripSplitButton2.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton2.Name = "toolStripSplitButton2";
            toolStripSplitButton2.Size = new Size(32, 20);
            toolStripSplitButton2.Text = "Help";
            // 
            // toolStrip1
            // 
            toolStrip1.Location = new Point(0, 25);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1460, 25);
            toolStrip1.TabIndex = 20;
            toolStrip1.Text = "toolStrip1";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(button2);
            flowLayoutPanel1.Controls.Add(checkBox2);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(24, 374);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(12);
            flowLayoutPanel1.Size = new Size(220, 90);
            flowLayoutPanel1.TabIndex = 21;
            flowLayoutPanel1.WrapContents = false;
            // 
            // button2
            // 
            button2.Location = new Point(15, 15);
            button2.Margin = new Padding(0, 0, 0, 8);
            button2.Name = "button2";
            button2.Size = new Size(112, 30);
            button2.TabIndex = 0;
            button2.Text = "Action";
            button2.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(15, 53);
            checkBox2.Margin = new Padding(0);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(106, 21);
            checkBox2.TabIndex = 1;
            checkBox2.Text = "Wrap content";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button3);
            groupBox1.Location = new Point(24, 480);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(220, 108);
            groupBox1.TabIndex = 22;
            groupBox1.TabStop = false;
            groupBox1.Text = "Grouped Controls";
            // 
            // button3
            // 
            button3.Location = new Point(22, 43);
            button3.Name = "button3";
            button3.Size = new Size(112, 30);
            button3.TabIndex = 0;
            button3.Text = "Open";
            button3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.ContextMenuStrip = contextMenuStrip1;
            panel1.Controls.Add(button4);
            panel1.Location = new Point(24, 604);
            panel1.Name = "panel1";
            panel1.Size = new Size(220, 102);
            panel1.TabIndex = 23;
            // 
            // button4
            // 
            button4.Location = new Point(40, 34);
            button4.Name = "button4";
            button4.Size = new Size(112, 30);
            button4.TabIndex = 0;
            button4.Text = "Panel Button";
            button4.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(260, 718);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(button5);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(checkBox3);
            splitContainer1.Size = new Size(332, 98);
            splitContainer1.SplitterDistance = 170;
            splitContainer1.TabIndex = 24;
            // 
            // button5
            // 
            button5.Location = new Point(18, 29);
            button5.Name = "button5";
            button5.Size = new Size(112, 30);
            button5.TabIndex = 0;
            button5.Text = "Split Action";
            button5.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(22, 29);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(66, 21);
            checkBox3.TabIndex = 0;
            checkBox3.Text = "Pinned";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(938, 180);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(478, 526);
            tabControl1.TabIndex = 25;
            // 
            // tabPage1
            // 
            tabPage1.ContextMenuStrip = contextMenuStrip1;
            tabPage1.Controls.Add(button6);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(470, 496);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "General";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Location = new Point(24, 22);
            button6.Name = "button6";
            button6.Size = new Size(112, 30);
            button6.TabIndex = 0;
            button6.Text = "Tab Action";
            button6.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(comboBox2);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(470, 496);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Input";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "Item 1", "Item 2", "Item 3" });
            comboBox2.Location = new Point(24, 24);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(160, 25);
            comboBox2.TabIndex = 0;
            comboBox2.Text = "Item 1";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(radioButton3, 0, 0);
            tableLayoutPanel1.Controls.Add(radioButton4, 0, 1);
            tableLayoutPanel1.Controls.Add(textBox2, 1, 0);
            tableLayoutPanel1.Controls.Add(button7, 1, 1);
            tableLayoutPanel1.Location = new Point(1040, 718);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(376, 98);
            tableLayoutPanel1.TabIndex = 26;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(3, 3);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(70, 21);
            radioButton3.TabIndex = 0;
            radioButton3.TabStop = true;
            radioButton3.Text = "Table A";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new Point(4, 52);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(70, 21);
            radioButton4.TabIndex = 1;
            radioButton4.TabStop = true;
            radioButton4.Text = "Table B";
            radioButton4.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(192, 4);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(180, 23);
            textBox2.TabIndex = 2;
            textBox2.Text = "Search";
            // 
            // button7
            // 
            button7.Location = new Point(192, 52);
            button7.Name = "button7";
            button7.Size = new Size(112, 30);
            button7.TabIndex = 3;
            button7.Text = "Apply";
            button7.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = SystemColors.Window;
            richTextBox1.Location = new Point(920, 105);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(496, 58);
            richTextBox1.TabIndex = 27;
            richTextBox1.Text = "Use the View menu to preview window styles. The WinForms target uses Lumina.Ext.WinForms, and the net10.0 Lumina.Forms target uses the built-in methods with the same names.";
            // 
            // 视图ToolStripMenuItem
            // 
            视图ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { luminaExtWinFormsToolStripMenuItem });
            视图ToolStripMenuItem.Name = "视图ToolStripMenuItem";
            视图ToolStripMenuItem.Size = new Size(44, 21);
            视图ToolStripMenuItem.Text = "视图";
            // 
            // luminaExtWinFormsToolStripMenuItem
            // 
            luminaExtWinFormsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { clearEffectToolStripMenuItem, toolStripSeparator3, micaToolStripMenuItem, micaAltToolStripMenuItem, acrylicToolStripMenuItem, aeroToolStripMenuItem, blurToolStripMenuItem });
            luminaExtWinFormsToolStripMenuItem.Name = "luminaExtWinFormsToolStripMenuItem";
            luminaExtWinFormsToolStripMenuItem.Size = new Size(194, 22);
            luminaExtWinFormsToolStripMenuItem.Text = "Lumina.Ext.WinForms";
            // 
            // clearEffectToolStripMenuItem
            // 
            clearEffectToolStripMenuItem.Name = "clearEffectToolStripMenuItem";
            clearEffectToolStripMenuItem.Size = new Size(180, 22);
            clearEffectToolStripMenuItem.Text = "默认";
            clearEffectToolStripMenuItem.Click += clearEffectToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(177, 6);
            // 
            // micaToolStripMenuItem
            // 
            micaToolStripMenuItem.Name = "micaToolStripMenuItem";
            micaToolStripMenuItem.Size = new Size(180, 22);
            micaToolStripMenuItem.Text = "Mica";
            micaToolStripMenuItem.Click += micaToolStripMenuItem_Click;
            // 
            // micaAltToolStripMenuItem
            // 
            micaAltToolStripMenuItem.Name = "micaAltToolStripMenuItem";
            micaAltToolStripMenuItem.Size = new Size(180, 22);
            micaAltToolStripMenuItem.Text = "Mica Alt";
            micaAltToolStripMenuItem.Click += micaAltToolStripMenuItem_Click;
            // 
            // acrylicToolStripMenuItem
            // 
            acrylicToolStripMenuItem.Name = "acrylicToolStripMenuItem";
            acrylicToolStripMenuItem.Size = new Size(180, 22);
            acrylicToolStripMenuItem.Text = "Acrylic";
            acrylicToolStripMenuItem.Click += acrylicToolStripMenuItem_Click;
            // 
            // aeroToolStripMenuItem
            // 
            aeroToolStripMenuItem.Name = "aeroToolStripMenuItem";
            aeroToolStripMenuItem.Size = new Size(180, 22);
            aeroToolStripMenuItem.Text = "Aero";
            aeroToolStripMenuItem.Click += aeroToolStripMenuItem_Click;
            // 
            // blurToolStripMenuItem
            // 
            blurToolStripMenuItem.Name = "blurToolStripMenuItem";
            blurToolStripMenuItem.Size = new Size(180, 22);
            blurToolStripMenuItem.Text = "Blur";
            blurToolStripMenuItem.Click += blurToolStripMenuItem_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1460, 900);
            Controls.Add(richTextBox1);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(tabControl1);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Controls.Add(groupBox1);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(toolStrip1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(treeView1);
            Controls.Add(textBox1);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(progressBar1);
            Controls.Add(pictureBox1);
            Controls.Add(numericUpDown1);
            Controls.Add(monthCalendar1);
            Controls.Add(maskedTextBox1);
            Controls.Add(listView1);
            Controls.Add(listBox1);
            Controls.Add(linkLabel1);
            Controls.Add(label1);
            Controls.Add(dateTimePicker1);
            Controls.Add(comboBox1);
            Controls.Add(checkBox1);
            Controls.Add(button1);
            MainMenuStrip = menuStrip1;
            Name = "frmMain";
            Text = "LuminaForms Controls Demo";
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private CheckBox checkBox1;
        private ComboBox comboBox1;
        private DateTimePicker dateTimePicker1;
        private Label label1;
        private LinkLabel linkLabel1;
        private ListBox listBox1;
        private ListView listView1;
        private MaskedTextBox maskedTextBox1;
        private MonthCalendar monthCalendar1;
        private NotifyIcon notifyIcon1;
        private NumericUpDown numericUpDown1;
        private PictureBox pictureBox1;
        private ProgressBar progressBar1;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private TextBox textBox1;
        private ToolTip toolTip1;
        private TreeView treeView1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 菜单1ToolStripMenuItem;
        private ToolStripMenuItem sdfToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripTextBox toolStripTextBox1;
        private ToolStripMenuItem fsdaToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 文件ToolStripMenuItem;
        private ToolStripMenuItem 编辑ToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripTextBox toolStripTextBox2;
        private ToolStripMenuItem 关于ToolStripMenuItem;
        private ToolStripMenuItem guanyuToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripComboBox toolStripComboBox2;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripTextBox toolStripTextBox3;
        private ToolStripSplitButton toolStripSplitButton1;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripSplitButton toolStripSplitButton2;
        private ToolStrip toolStrip1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button button2;
        private CheckBox checkBox2;
        private GroupBox groupBox1;
        private Button button3;
        private Panel panel1;
        private Button button4;
        private SplitContainer splitContainer1;
        private Button button5;
        private CheckBox checkBox3;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private Button button6;
        private TabPage tabPage2;
        private ComboBox comboBox2;
        private TableLayoutPanel tableLayoutPanel1;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private TextBox textBox2;
        private Button button7;
        private RichTextBox richTextBox1;
        private ToolStripMenuItem 视图ToolStripMenuItem;
        private ToolStripMenuItem luminaExtWinFormsToolStripMenuItem;
        private ToolStripMenuItem clearEffectToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem micaToolStripMenuItem;
        private ToolStripMenuItem micaAltToolStripMenuItem;
        private ToolStripMenuItem acrylicToolStripMenuItem;
        private ToolStripMenuItem aeroToolStripMenuItem;
        private ToolStripMenuItem blurToolStripMenuItem;
    }
}
