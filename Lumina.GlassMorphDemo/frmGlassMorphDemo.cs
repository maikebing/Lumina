using System.Runtime.Versioning;

namespace Lumina.GlassMorphDemo;

[SupportedOSPlatform("windows6.1")]
public sealed class frmGlassMorphDemo : Form
{
    private readonly Label _eyebrowLabel;
    private readonly Label _titleLabel;
    private readonly Label _subtitleLabel;
    private readonly Label _specLabel;
    private readonly Panel _stagePanel;
    private readonly Label _stageTitleLabel;
    private readonly Label _stageHintLabel;
    private readonly Label _statusLabel;
    private readonly Label _controlsTitleLabel;
    private readonly Label _controlsHintLabel;
    private readonly GlassMorphButton _launchButton;
    private readonly GlassMorphButton _previewButton;
    private readonly GlassMorphButton _aotButton;
    private readonly GlassMorphTextBox _glassTextBox;
    private readonly GlassMorphComboBox _glassComboBox;
    private readonly GlassMorphCheckBox _glassCheckBox;
    private readonly GlassMorphRadioButton _glassRadioBlue;
    private readonly GlassMorphRadioButton _glassRadioGreen;
    private readonly GlassMorphProgressBar _glassProgressBar;
    private int _backdropIndex;

    public frmGlassMorphDemo()
    {
        Name = nameof(frmGlassMorphDemo);
        Text = "Lumina Glass Morph Demo";
        ClientSize = new Size(1200, 760);
        BackColor = Color.FromArgb(66, 18, 92);
        ForeColor = Color.FromArgb(241, 239, 249);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96f, 96f);

        UseDarkTheme();
        SetAcrylic(0xA8_45_18_60);

        _eyebrowLabel = new Label
        {
            Name = "eyebrowLabel",
            Text = "Lumina.Forms + Native AOT + CSS glass morph reference palette",
            ForeColor = Color.FromArgb(200, 214, 255),
        };

        _titleLabel = new Label
        {
            Name = "titleLabel",
            Text = "Lumina.GlassMorphDemo",
            ForeColor = Color.FromArgb(246, 248, 255),
        };

        _subtitleLabel = new Label
        {
            Name = "subtitleLabel",
            Text = "A standalone net10.0 demo project using Lumina.Forms to recreate the purple glassmorphism button palette and neon hover glow from the reference.",
            ForeColor = Color.FromArgb(221, 208, 236),
        };

        _specLabel = new Label
        {
            Name = "specLabel",
            Text = "Hover keeps the button mostly in place and drives a tighter neon glow.\r\nThe color language now follows the purple base plus blue, pink, and green accents.\r\nEverything on this page still runs on the Lumina.Forms AOT path.",
            ForeColor = Color.FromArgb(198, 184, 221),
        };

        _stagePanel = new Panel
        {
            Name = "stagePanel",
            BackColor = Color.FromArgb(76, 24, 104),
        };

        _stageTitleLabel = new Label
        {
            Name = "stageTitleLabel",
            Text = "Hover Showcase",
            ForeColor = Color.FromArgb(244, 247, 255),
        };

        _stageHintLabel = new Label
        {
            Name = "stageHintLabel",
            Text = "The buttons below are self-painted Lumina.Forms controls. The motion is now restrained, while the glow and glass tint do most of the work like the reference page.",
            ForeColor = Color.FromArgb(213, 194, 230),
        };

        _statusLabel = new Label
        {
            Name = "statusLabel",
            Text = "Status: ready. Hover any button to see the center glow and bottom neon band respond.",
            ForeColor = Color.FromArgb(232, 222, 246),
        };

        _controlsTitleLabel = new Label
        {
            Name = "controlsTitleLabel",
            Text = "Glass Field Set",
            ForeColor = Color.FromArgb(244, 247, 255),
        };

        _controlsHintLabel = new Label
        {
            Name = "controlsHintLabel",
            Text = "TextBox, CheckBox, ComboBox, RadioButton, and ProgressBar now follow the same purple glass shell with accent-driven glow.",
            ForeColor = Color.FromArgb(213, 194, 230),
        };

        _launchButton = CreateButton(
            "launchButton",
            "Read more 1",
            Color.FromArgb(255, 64, 148));
        _previewButton = CreateButton(
            "previewButton",
            "Read more 2",
            Color.FromArgb(88, 206, 255));
        _aotButton = CreateButton(
            "aotButton",
            "Read more 3",
            Color.FromArgb(74, 255, 112));

        _glassTextBox = new GlassMorphTextBox
        {
            Name = "glassTextBox",
            AccentColor = Color.FromArgb(255, 64, 148),
        };

        _glassComboBox = new GlassMorphComboBox
        {
            Name = "glassComboBox",
            AccentColor = Color.FromArgb(88, 206, 255),
        };
        _glassComboBox.ComboBox.Items.AddRange("Glass Blue", "Neon Pink", "Acid Green");
        _glassComboBox.ComboBox.SelectedIndex = 0;

        _glassCheckBox = new GlassMorphCheckBox
        {
            Name = "glassCheckBox",
            Text = "Enable glow pulse",
            AccentColor = Color.FromArgb(255, 64, 148),
            Size = new Size(220, 26),
            Checked = true,
        };

        _glassRadioBlue = new GlassMorphRadioButton
        {
            Name = "glassRadioBlue",
            Text = "Blue scene",
            AccentColor = Color.FromArgb(88, 206, 255),
            Size = new Size(140, 26),
            Checked = true,
        };

        _glassRadioGreen = new GlassMorphRadioButton
        {
            Name = "glassRadioGreen",
            Text = "Green scene",
            AccentColor = Color.FromArgb(74, 255, 112),
            Size = new Size(140, 26),
        };

        _glassProgressBar = new GlassMorphProgressBar
        {
            Name = "glassProgressBar",
            AccentColor = Color.FromArgb(88, 206, 255),
            Size = new Size(540, 24),
            Value = 68,
        };

        _launchButton.Click += LaunchButton_Click;
        _previewButton.Click += PreviewButton_Click;
        _aotButton.Click += AotButton_Click;
        _glassCheckBox.CheckedChanged += GlassCheckBox_CheckedChanged;
        _glassRadioBlue.CheckedChanged += GlassRadioBlue_CheckedChanged;
        _glassRadioGreen.CheckedChanged += GlassRadioGreen_CheckedChanged;
        _glassComboBox.ComboBox.SelectedIndexChanged += GlassComboBox_SelectedIndexChanged;

        _stagePanel.Controls.AddRange(
            _stageTitleLabel,
            _stageHintLabel,
            _controlsTitleLabel,
            _controlsHintLabel,
            _launchButton,
            _previewButton,
            _aotButton,
            _glassTextBox,
            _glassComboBox,
            _glassCheckBox,
            _glassRadioBlue,
            _glassRadioGreen,
            _glassProgressBar,
            _statusLabel);

        Controls.AddRange(
            _eyebrowLabel,
            _titleLabel,
            _subtitleLabel,
            _specLabel,
            _stagePanel);

        Layout += (_, _) => LayoutScene();
        SizeChanged += (_, _) => LayoutScene();
        Shown += (_, _) => LayoutScene();

        LayoutScene();
    }

    private static GlassMorphButton CreateButton(string name, string text, Color accentColor)
    {
        return new GlassMorphButton
        {
            Name = name,
            Text = text,
            AccentColor = accentColor,
            Size = new Size(260, 120),
        };
    }

    private void LayoutScene()
    {
        int clientWidth = ClientSize.Width;
        int clientHeight = ClientSize.Height;
        int pageMargin = clientWidth >= 1080 ? 56 : 32;
        int top = 28;

        _eyebrowLabel.SetBounds(pageMargin, top, Math.Max(280, clientWidth - (pageMargin * 2)), 22);
        top += 30;

        _titleLabel.SetBounds(pageMargin, top, Math.Max(320, clientWidth - (pageMargin * 2)), 30);
        top += 38;

        _subtitleLabel.SetBounds(pageMargin, top, Math.Max(340, clientWidth - (pageMargin * 2)), 42);
        top += 54;

        _specLabel.SetBounds(pageMargin, top, Math.Max(360, clientWidth - (pageMargin * 2)), 64);
        top += 88;

        int stageWidth = Math.Max(620, clientWidth - (pageMargin * 2));
        if (clientWidth < 760)
        {
            stageWidth = clientWidth - (pageMargin * 2);
        }

        int remainingHeight = Math.Max(360, clientHeight - top - 42);
        _stagePanel.SetBounds(pageMargin, top, Math.Max(320, stageWidth), remainingHeight);

        LayoutStageContents();
    }

    private void LayoutStageContents()
    {
        int panelWidth = _stagePanel.Width;
        int panelHeight = _stagePanel.Height;
        int innerMargin = panelWidth >= 840 ? 28 : 18;

        _stageTitleLabel.SetBounds(innerMargin, 18, Math.Max(200, panelWidth - (innerMargin * 2)), 24);
        _stageHintLabel.SetBounds(innerMargin, 48, Math.Max(240, panelWidth - (innerMargin * 2)), 48);

        int buttonWidth = panelWidth >= 980
            ? 272
            : panelWidth >= 840
                ? 238
                : Math.Max(160, (panelWidth - (innerMargin * 2) - 24) / 3);
        int buttonHeight = panelHeight >= 420 ? 126 : 112;
        int spacing = panelWidth >= 980 ? 22 : 12;
        int groupWidth = (buttonWidth * 3) + (spacing * 2);
        int left = Math.Max(innerMargin, (panelWidth - groupWidth) / 2);
        int top = Math.Max(108, panelHeight / 4 - (buttonHeight / 2));

        _launchButton.SetBounds(left, top, buttonWidth, buttonHeight);
        _previewButton.SetBounds(left + buttonWidth + spacing, top, buttonWidth, buttonHeight);
        _aotButton.SetBounds(left + ((buttonWidth + spacing) * 2), top, buttonWidth, buttonHeight);

        int controlsTop = top + buttonHeight + 42;
        _controlsTitleLabel.SetBounds(innerMargin, controlsTop, Math.Max(220, panelWidth - (innerMargin * 2)), 24);
        _controlsHintLabel.SetBounds(innerMargin, controlsTop + 26, Math.Max(260, panelWidth - (innerMargin * 2)), 34);

        int fieldTop = controlsTop + 72;
        int leftColumnWidth = Math.Max(240, (panelWidth - (innerMargin * 2) - 18) / 2);
        int rightColumnLeft = innerMargin + leftColumnWidth + 18;
        int rightColumnWidth = Math.Max(220, panelWidth - rightColumnLeft - innerMargin);

        _glassTextBox.SetBounds(innerMargin, fieldTop, leftColumnWidth, 42);
        _glassComboBox.SetBounds(innerMargin, fieldTop + 56, leftColumnWidth, 42);

        _glassCheckBox.SetBounds(rightColumnLeft, fieldTop + 8, Math.Min(220, rightColumnWidth), 26);
        _glassRadioBlue.SetBounds(rightColumnLeft, fieldTop + 44, Math.Min(140, rightColumnWidth), 26);
        _glassRadioGreen.SetBounds(rightColumnLeft + Math.Min(152, rightColumnWidth / 2), fieldTop + 44, Math.Min(140, Math.Max(120, rightColumnWidth - 152)), 26);
        _glassProgressBar.SetBounds(innerMargin, fieldTop + 118, Math.Max(240, panelWidth - (innerMargin * 2)), 24);

        _statusLabel.SetBounds(innerMargin, Math.Max(fieldTop + 154, panelHeight - 72), Math.Max(220, panelWidth - (innerMargin * 2)), 28);
    }

    private void LaunchButton_Click(object? sender, EventArgs e)
    {
        SetAcrylic(0xA8_45_18_60);
        _statusLabel.Text = "Status: pink button clicked. Kept the interaction non-blocking to avoid the previous modal freeze path.";
    }

    private void PreviewButton_Click(object? sender, EventArgs e)
    {
        _backdropIndex = (_backdropIndex + 1) % 3;
        switch (_backdropIndex)
        {
            case 0:
                SetAcrylic(0xB4_11_18_2A);
                _statusLabel.Text = "Status: acrylic backdrop active. The purple glass base now reads closest to the reference.";
                break;

            case 1:
                SetMicaAlt();
                _statusLabel.Text = "Status: mica alt backdrop active. The scene gets denser while the neon hover stays intact.";
                break;

            default:
                SetBlur(22);
                _statusLabel.Text = "Status: blur backdrop active. The purple field softens while the button glows remain concentrated.";
                break;
        }
    }

    private void AotButton_Click(object? sender, EventArgs e)
    {
        UseCustomTheme(0xFF_47_BD_FF);
        _statusLabel.Text = "Status: accent refreshed for the AOT-ready scene. Publish this project directly on the Lumina.Forms net10.0 path.";
    }

    private void GlassCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        _glassProgressBar.Value = _glassCheckBox.Checked ? 82 : 54;
        _statusLabel.Text = _glassCheckBox.Checked
            ? "Status: checkbox enabled the stronger glow state."
            : "Status: checkbox relaxed the glow state.";
    }

    private void GlassRadioBlue_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_glassRadioBlue.Checked)
        {
            return;
        }

        _glassRadioGreen.Checked = false;
        _glassProgressBar.AccentColor = Color.FromArgb(88, 206, 255);
        _statusLabel.Text = "Status: radio button switched the progress accent to blue.";
        _glassProgressBar.Invalidate();
    }

    private void GlassRadioGreen_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_glassRadioGreen.Checked)
        {
            return;
        }

        _glassRadioBlue.Checked = false;
        _glassProgressBar.AccentColor = Color.FromArgb(74, 255, 112);
        _statusLabel.Text = "Status: radio button switched the progress accent to green.";
        _glassProgressBar.Invalidate();
    }

    private void GlassComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _glassProgressBar.Value = _glassComboBox.ComboBox.SelectedIndex switch
        {
            0 => 68,
            1 => 84,
            2 => 92,
            _ => 68,
        };

        _statusLabel.Text = $"Status: combo box changed to {_glassComboBox.ComboBox.SelectedItem}.";
    }
}
