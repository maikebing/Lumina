namespace NativeFormsDemo;

public partial class frmMain : Form
{
    public frmMain()
    {
        InitializeComponent();
        InitializeDemoContent();
    }

    private void InitializeDemoContent()
    {
        Text = "LuminaForms Controls Demo";

        button1.Text = "Primary Button";
        button2.Text = "Action";
        button3.Text = "Open";
        button4.Text = "Panel Button";
        button5.Text = "Split Action";
        button6.Text = "Tab Action";
        button7.Text = "Apply";

        checkBox1.Text = "Enable option";
        checkBox2.Text = "Wrap content";
        checkBox3.Text = "Pinned";

        radioButton1.Text = "Choice A";
        radioButton2.Text = "Choice B";
        radioButton3.Text = "Table A";
        radioButton4.Text = "Table B";

        label1.Text = "LuminaForms compatibility sample";
        linkLabel1.Text = "Migration-friendly control surface";
        groupBox1.Text = "Grouped Controls";
        tabPage1.Text = "General";
        tabPage2.Text = "Input";

        textBox2.Text = "Search";
        richTextBox1.Text = "This demo only showcases common controls and layout containers across both targets. Theme implementation stays inside Lumina.Forms.";
        toolStripStatusLabel1.Text = "Ready";
        toolStripProgressBar1.Value = 40;

        comboBox2.Items.Clear();
        comboBox2.Items.AddRange(["Item 1", "Item 2", "Item 3"]);
        comboBox2.SelectedIndex = 0;

        toolStripComboBox1.Items.Clear();
        toolStripComboBox1.Items.AddRange(["Default", "Compact", "Expanded"]);
        toolStripComboBox1.SelectedIndex = 0;

        toolStripComboBox2.Items.Clear();
        toolStripComboBox2.Items.AddRange(["Status A", "Status B"]);
        toolStripComboBox2.SelectedIndex = 0;

        toolStripMenuItem1.Text = "Command";
        toolStripMenuItem3.Text = "Status";
        toolStripMenuItem4.Text = "Action";
        toolStripTextBox2.Text = "Filter";
        toolStripTextBox3.Text = "Search";
        toolStripDropDownButton1.Text = "Menu";
        toolStripSplitButton1.Text = "Options";
        toolStripSplitButton2.Text = "Help";

        if (OperatingSystem.IsWindows())
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            notifyIcon1.Text = "LuminaForms Demo";
        }
    }
}
