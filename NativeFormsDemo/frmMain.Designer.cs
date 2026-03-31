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
            TreeNode treeNode14 = new TreeNode("节点4");
            TreeNode treeNode15 = new TreeNode("节点6");
            TreeNode treeNode16 = new TreeNode("节点5", new TreeNode[] { treeNode15 });
            TreeNode treeNode17 = new TreeNode("节点0", new TreeNode[] { treeNode14, treeNode16 });
            TreeNode treeNode18 = new TreeNode("节点7");
            TreeNode treeNode19 = new TreeNode("节点1", new TreeNode[] { treeNode18 });
            TreeNode treeNode20 = new TreeNode("节点8");
            TreeNode treeNode21 = new TreeNode("节点9");
            TreeNode treeNode22 = new TreeNode("节点2", new TreeNode[] { treeNode20, treeNode21 });
            TreeNode treeNode23 = new TreeNode("节点12");
            TreeNode treeNode24 = new TreeNode("节点11", new TreeNode[] { treeNode23 });
            TreeNode treeNode25 = new TreeNode("节点10", new TreeNode[] { treeNode24 });
            TreeNode treeNode26 = new TreeNode("节点3", new TreeNode[] { treeNode25 });
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            button1 = new Button();
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
            contextMenuStrip1 = new ContextMenuStrip(components);
            菜单1ToolStripMenuItem = new ToolStripMenuItem();
            sdfToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripTextBox1 = new ToolStripTextBox();
            fsdaToolStripMenuItem = new ToolStripMenuItem();
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
            tabPage2 = new TabPage();
            button6 = new Button();
            comboBox2 = new ComboBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            radioButton3 = new RadioButton();
            radioButton4 = new RadioButton();
            textBox2 = new TextBox();
            button7 = new Button();
            richTextBox1 = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            contextMenuStrip1.SuspendLayout();
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
            button1.Location = new Point(24, 22);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(155, 27);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(89, 21);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "1", "2", "3", "4" });
            comboBox1.Location = new Point(307, 26);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 25);
            comboBox1.TabIndex = 2;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(31, 144);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(200, 23);
            dateTimePicker1.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(484, 18);
            label1.Name = "label1";
            label1.Size = new Size(43, 17);
            label1.TabIndex = 4;
            label1.Text = "label1";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(573, 24);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(66, 17);
            linkLabel1.TabIndex = 5;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "linkLabel1";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            listBox1.Location = new Point(265, 146);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(120, 89);
            listBox1.TabIndex = 6;
            // 
            // listView1
            // 
            listView1.Location = new Point(454, 153);
            listView1.Name = "listView1";
            listView1.Size = new Size(121, 97);
            listView1.TabIndex = 7;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // maskedTextBox1
            // 
            maskedTextBox1.Location = new Point(688, 24);
            maskedTextBox1.Name = "maskedTextBox1";
            maskedTextBox1.Size = new Size(100, 23);
            maskedTextBox1.TabIndex = 8;
            // 
            // monthCalendar1
            // 
            monthCalendar1.Location = new Point(24, 225);
            monthCalendar1.Name = "monthCalendar1";
            monthCalendar1.TabIndex = 9;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(51, 89);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(120, 23);
            numericUpDown1.TabIndex = 10;
            // 
            // pictureBox1
            // 
            pictureBox1.ContextMenuStrip = contextMenuStrip1;
            pictureBox1.Image = Properties.Resources.SplashScreen;
            pictureBox1.Location = new Point(265, 241);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(622, 303);
            pictureBox1.TabIndex = 11;
            pictureBox1.TabStop = false;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(247, 94);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(247, 23);
            progressBar1.TabIndex = 12;
            progressBar1.Value = 12;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(537, 94);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(102, 21);
            radioButton1.TabIndex = 13;
            radioButton1.Text = "radioButton1";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Checked = true;
            radioButton2.Location = new Point(544, 62);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(102, 21);
            radioButton2.TabIndex = 14;
            radioButton2.TabStop = true;
            radioButton2.Text = "radioButton2";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.ContextMenuStrip = contextMenuStrip1;
            textBox1.Location = new Point(667, 74);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 15;
            textBox1.Text = "ABC 我在那";
            // 
            // treeView1
            // 
            treeView1.ContextMenuStrip = contextMenuStrip1;
            treeView1.Location = new Point(610, 153);
            treeView1.Name = "treeView1";
            treeNode14.Name = "节点4";
            treeNode14.Text = "节点4";
            treeNode15.Name = "节点6";
            treeNode15.Text = "节点6";
            treeNode16.Name = "节点5";
            treeNode16.Text = "节点5";
            treeNode17.Name = "节点0";
            treeNode17.Text = "节点0";
            treeNode18.Name = "节点7";
            treeNode18.Text = "节点7";
            treeNode19.Name = "节点1";
            treeNode19.Text = "节点1";
            treeNode20.Name = "节点8";
            treeNode20.Text = "节点8";
            treeNode21.Name = "节点9";
            treeNode21.Text = "节点9";
            treeNode22.Name = "节点2";
            treeNode22.Text = "节点2";
            treeNode23.Name = "节点12";
            treeNode23.Text = "节点12";
            treeNode24.Name = "节点11";
            treeNode24.Text = "节点11";
            treeNode25.Name = "节点10";
            treeNode25.Text = "节点10";
            treeNode26.Name = "节点3";
            treeNode26.Text = "节点3";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode17, treeNode19, treeNode22, treeNode26 });
            treeView1.Size = new Size(121, 97);
            treeView1.TabIndex = 16;
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
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 文件ToolStripMenuItem, 编辑ToolStripMenuItem, 关于ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1297, 25);
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
            toolStripMenuItem1.Size = new Size(192, 22);
            toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new Size(121, 25);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(189, 6);
            // 
            // toolStripTextBox2
            // 
            toolStripTextBox2.Name = "toolStripTextBox2";
            toolStripTextBox2.Size = new Size(100, 23);
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
            statusStrip1.Location = new Point(0, 769);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1297, 22);
            statusStrip1.TabIndex = 19;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(131, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
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
            toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(192, 22);
            toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // toolStripComboBox2
            // 
            toolStripComboBox2.Name = "toolStripComboBox2";
            toolStripComboBox2.Size = new Size(121, 25);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(189, 6);
            // 
            // toolStripTextBox3
            // 
            toolStripTextBox3.Name = "toolStripTextBox3";
            toolStripTextBox3.Size = new Size(100, 23);
            // 
            // toolStripSplitButton1
            // 
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem4 });
            toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(32, 20);
            toolStripSplitButton1.Text = "toolStripSplitButton1";
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new Size(192, 22);
            toolStripMenuItem4.Text = "toolStripMenuItem4";
            // 
            // toolStripSplitButton2
            // 
            toolStripSplitButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton2.Image = (Image)resources.GetObject("toolStripSplitButton2.Image");
            toolStripSplitButton2.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton2.Name = "toolStripSplitButton2";
            toolStripSplitButton2.Size = new Size(32, 20);
            toolStripSplitButton2.Text = "toolStripSplitButton2";
            // 
            // toolStrip1
            // 
            toolStrip1.Location = new Point(0, 25);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1297, 25);
            toolStrip1.TabIndex = 20;
            toolStrip1.Text = "toolStrip1";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(button2);
            flowLayoutPanel1.Controls.Add(checkBox2);
            flowLayoutPanel1.Location = new Point(55, 576);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(200, 100);
            flowLayoutPanel1.TabIndex = 21;
            // 
            // button2
            // 
            button2.Location = new Point(3, 3);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 0;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(84, 3);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(89, 21);
            checkBox2.TabIndex = 1;
            checkBox2.Text = "checkBox2";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button3);
            groupBox1.Location = new Point(279, 586);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(200, 100);
            groupBox1.TabIndex = 22;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // button3
            // 
            button3.Location = new Point(21, 34);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 0;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.ContextMenuStrip = contextMenuStrip1;
            panel1.Controls.Add(button4);
            panel1.Location = new Point(534, 619);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 100);
            panel1.TabIndex = 23;
            // 
            // button4
            // 
            button4.Location = new Point(31, 37);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 0;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Location = new Point(769, 584);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(button5);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(checkBox3);
            splitContainer1.Size = new Size(282, 120);
            splitContainer1.SplitterDistance = 94;
            splitContainer1.TabIndex = 24;
            // 
            // button5
            // 
            button5.Location = new Point(6, 44);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 0;
            button5.Text = "button5";
            button5.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(30, 24);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(89, 21);
            checkBox3.TabIndex = 0;
            checkBox3.Text = "checkBox3";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(946, 249);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(289, 277);
            tabControl1.TabIndex = 25;
            // 
            // tabPage1
            // 
            tabPage1.ContextMenuStrip = contextMenuStrip1;
            tabPage1.Controls.Add(button6);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(281, 247);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(comboBox2);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(281, 247);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Location = new Point(67, 88);
            button6.Name = "button6";
            button6.Size = new Size(75, 23);
            button6.TabIndex = 0;
            button6.Text = "button6";
            button6.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(87, 121);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(121, 25);
            comboBox2.TabIndex = 0;
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
            tableLayoutPanel1.Location = new Point(1096, 570);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(200, 173);
            tableLayoutPanel1.TabIndex = 26;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(3, 3);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(94, 21);
            radioButton3.TabIndex = 0;
            radioButton3.TabStop = true;
            radioButton3.Text = "radioButton3";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new Point(3, 89);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(94, 21);
            radioButton4.TabIndex = 1;
            radioButton4.TabStop = true;
            radioButton4.Text = "radioButton4";
            radioButton4.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(103, 3);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(94, 23);
            textBox2.TabIndex = 2;
            // 
            // button7
            // 
            button7.Location = new Point(103, 89);
            button7.Name = "button7";
            button7.Size = new Size(75, 23);
            button7.TabIndex = 3;
            button7.Text = "button7";
            button7.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(841, 86);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(145, 122);
            richTextBox1.TabIndex = 27;
            richTextBox1.Text = "";
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1297, 791);
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
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
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
    }
}
