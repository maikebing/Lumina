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
#if NET10_0_WINDOWS
            toolStripStatusLabel1.Text = "Ready";
#else
            luminaExtWinFormsToolStripMenuItem.Enabled = false;
            toolStripStatusLabel1.Text = "Lumina.Ext.WinForms 风格仅在 net10.0-windows 目标可用。";
#endif
        }

        private void clearEffectToolStripMenuItem_Click(object? sender, EventArgs e)
        {
#if NET10_0_WINDOWS
            TryApplyStyle(clearEffectToolStripMenuItem, "默认", () => this.ClearLuminaEffect());
#else
            ShowStyleUnavailable();
#endif
        }

        private void micaToolStripMenuItem_Click(object? sender, EventArgs e)
        {
#if NET10_0_WINDOWS
            TryApplyStyle(micaToolStripMenuItem, "Mica", () => this.SetMica());
#else
            ShowStyleUnavailable();
#endif
        }

        private void micaAltToolStripMenuItem_Click(object? sender, EventArgs e)
        {
#if NET10_0_WINDOWS
            TryApplyStyle(micaAltToolStripMenuItem, "Mica Alt", () => this.SetMicaAlt());
#else
            ShowStyleUnavailable();
#endif
        }

        private void acrylicToolStripMenuItem_Click(object? sender, EventArgs e)
        {
#if NET10_0_WINDOWS
            TryApplyStyle(acrylicToolStripMenuItem, "Acrylic", () => this.SetAcrylic());
#else
            ShowStyleUnavailable();
#endif
        }

        private void aeroToolStripMenuItem_Click(object? sender, EventArgs e)
        {
#if NET10_0_WINDOWS
            TryApplyStyle(aeroToolStripMenuItem, "Aero", () => this.SetAero());
#else
            ShowStyleUnavailable();
#endif
        }

        private void blurToolStripMenuItem_Click(object? sender, EventArgs e)
        {
#if NET10_0_WINDOWS
            TryApplyStyle(blurToolStripMenuItem, "Blur", () => this.SetBlur());
#else
            ShowStyleUnavailable();
#endif
        }

        private void TryApplyStyle(ToolStripMenuItem selectedItem, string styleName, Action applyStyle)
        {
            try
            {
                applyStyle();
                SetSelectedWindowStyle(selectedItem);
                toolStripStatusLabel1.Text = $"已应用 {styleName} 风格。";
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = $"应用 {styleName} 风格失败：{ex.Message}";
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

        private void ShowStyleUnavailable()
        {
            toolStripStatusLabel1.Text = "Lumina.Ext.WinForms 风格仅在 net10.0-windows 目标可用。";
        }
    }
}
