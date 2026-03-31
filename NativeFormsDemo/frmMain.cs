namespace NativeFormsDemo
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            InitializeViewMenu();
        }

        private void InitializeViewMenu()
        {
            SetSelectedWindowStyle(clearEffectToolStripMenuItem);
            toolStripStatusLabel1.Text = "Ready";
        }

        private void clearEffectToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TryApplyStyle(clearEffectToolStripMenuItem, "Default", () => this.ClearLuminaEffect());
        }

        private void micaToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TryApplyStyle(micaToolStripMenuItem, "Mica", () => this.SetMica());
        }

        private void micaAltToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TryApplyStyle(micaAltToolStripMenuItem, "Mica Alt", () => this.SetMicaAlt());
        }

        private void acrylicToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TryApplyStyle(acrylicToolStripMenuItem, "Acrylic", () => this.SetAcrylic());
        }

        private void aeroToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TryApplyStyle(aeroToolStripMenuItem, "Aero", () => this.SetAero());
        }

        private void blurToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TryApplyStyle(blurToolStripMenuItem, "Blur", () => this.SetBlur());
        }

        private void TryApplyStyle(ToolStripMenuItem selectedItem, string styleName, Action applyStyle)
        {
            try
            {
                applyStyle();
                SetSelectedWindowStyle(selectedItem);
                toolStripStatusLabel1.Text = $"Applied {styleName} style.";
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = $"Failed to apply {styleName}: {ex.Message}";
            }
        }

        private void SetSelectedWindowStyle(ToolStripMenuItem selectedItem)
        {
            clearEffectToolStripMenuItem.Checked = selectedItem == clearEffectToolStripMenuItem;
            micaToolStripMenuItem.Checked = selectedItem == micaToolStripMenuItem;
            micaAltToolStripMenuItem.Checked = selectedItem == micaAltToolStripMenuItem;
            acrylicToolStripMenuItem.Checked = selectedItem == acrylicToolStripMenuItem;
            aeroToolStripMenuItem.Checked = selectedItem == aeroToolStripMenuItem;
            blurToolStripMenuItem.Checked = selectedItem == blurToolStripMenuItem;
        }
    }
}
