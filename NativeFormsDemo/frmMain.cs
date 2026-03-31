namespace NativeFormsDemo
{
    public partial class frmMain : Form
    {
        private const uint ShowcaseAccent = 0xFF_8B_5C_F6;
        private bool _themeInitialized;

        public frmMain()
        {
            InitializeComponent();
            InitializeViewMenu();
            Shown += frmMain_Shown;
        }

        private void InitializeViewMenu()
        {
            SetSelectedWindowStyle(clearEffectToolStripMenuItem);
            SetSelectedThemeMode(followSystemThemeToolStripMenuItem);
            SetSelectedPalette(systemColorsToolStripMenuItem);
            toolStripStatusLabel1.Text = "Ready";
        }

        private void frmMain_Shown(object? sender, EventArgs e)
        {
            if (_themeInitialized)
            {
                return;
            }

            _themeInitialized = true;
            ApplyThemeSelection(followSystemThemeToolStripMenuItem, systemColorsToolStripMenuItem);
        }

        private void followSystemThemeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ApplyThemeSelection(followSystemThemeToolStripMenuItem, GetSelectedPaletteMenuItem());
        }

        private void lightThemeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ApplyThemeSelection(lightThemeToolStripMenuItem, GetSelectedPaletteMenuItem());
        }

        private void darkThemeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ApplyThemeSelection(darkThemeToolStripMenuItem, GetSelectedPaletteMenuItem());
        }

        private void systemColorsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ApplyThemeSelection(GetSelectedThemeMenuItem(), systemColorsToolStripMenuItem);
        }

        private void customThemeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ApplyThemeSelection(GetSelectedThemeMenuItem(), customThemeToolStripMenuItem);
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

        private void ApplyThemeSelection(ToolStripMenuItem selectedThemeItem, ToolStripMenuItem selectedPaletteItem)
        {
            try
            {
                switch (selectedThemeItem.Name)
                {
                    case nameof(lightThemeToolStripMenuItem):
                        this.UseLightTheme();
                        break;

                    case nameof(darkThemeToolStripMenuItem):
                        this.UseDarkTheme();
                        break;

                    default:
                        this.UseSystemTheme();
                        break;
                }

                if (selectedPaletteItem == customThemeToolStripMenuItem)
                {
                    this.UseCustomTheme(ShowcaseAccent);
                }
                else
                {
                    this.UseSystemColors();
                }

                SetSelectedThemeMode(selectedThemeItem);
                SetSelectedPalette(selectedPaletteItem);
                toolStripStatusLabel1.Text = BuildThemeStatusText(selectedThemeItem, selectedPaletteItem);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = $"Failed to apply theme: {ex.Message}";
            }
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

        private static string BuildThemeStatusText(ToolStripMenuItem themeItem, ToolStripMenuItem paletteItem)
        {
            string themeName = themeItem.Text ?? string.Empty;
            string paletteName = paletteItem.Text ?? string.Empty;
            return $"Applied {themeName} / {paletteName}.";
        }

        private ToolStripMenuItem GetSelectedThemeMenuItem()
        {
            if (lightThemeToolStripMenuItem.Checked)
            {
                return lightThemeToolStripMenuItem;
            }

            if (darkThemeToolStripMenuItem.Checked)
            {
                return darkThemeToolStripMenuItem;
            }

            return followSystemThemeToolStripMenuItem;
        }

        private ToolStripMenuItem GetSelectedPaletteMenuItem()
        {
            return customThemeToolStripMenuItem.Checked
                ? customThemeToolStripMenuItem
                : systemColorsToolStripMenuItem;
        }

        private void SetSelectedThemeMode(ToolStripMenuItem selectedItem)
        {
            followSystemThemeToolStripMenuItem.Checked = selectedItem == followSystemThemeToolStripMenuItem;
            lightThemeToolStripMenuItem.Checked = selectedItem == lightThemeToolStripMenuItem;
            darkThemeToolStripMenuItem.Checked = selectedItem == darkThemeToolStripMenuItem;
        }

        private void SetSelectedPalette(ToolStripMenuItem selectedItem)
        {
            systemColorsToolStripMenuItem.Checked = selectedItem == systemColorsToolStripMenuItem;
            customThemeToolStripMenuItem.Checked = selectedItem == customThemeToolStripMenuItem;
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
