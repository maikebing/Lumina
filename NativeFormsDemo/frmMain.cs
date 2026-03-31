using System.Runtime.InteropServices;

namespace NativeFormsDemo
{
    public partial class frmMain : Form
    {
        private const uint MfString = 0x00000000;
        private const uint MfSeparator = 0x00000800;
        private const uint TpmRightButton = 0x0002;
        private const uint TpmReturnCmd = 0x0100;
        private const uint ShowCommandId = 1001;
        private const uint ExitCommandId = 1002;

        public frmMain()
        {
            InitializeComponent();
#if NET10_0 && !WINDOWS
            InitializeLuminaThemeDemo();
#endif
        }

#if NET10_0 && !WINDOWS
        private static readonly NativeTheme SunsetTheme = new()
        {
            Name = "Lumina Sunset Fluent",
            Description = "Fluent-inspired warm dark theme used by the compatibility demo.",
            Author = "NativeFormsDemo",
            ThemeMode = ThemeMode.Dark,
            PreferredVisualStyle = VisualStyleKind.Fluent,
            PreferredEffect = Lumina.EffectKind.Acrylic,
            PreferredEffectOptions = new Lumina.EffectOptions
            {
                BlendColor = 0xCC_22_16_1Cu,
                BlurRadius = 18,
                Opacity = 0.92f,
            },
            Palette = new ThemePalette
            {
                WindowBackground = 0xFF_1D_18_1Fu,
                WindowForeground = 0xFF_F8_F1_EB,
                WindowBorder = 0xFF_4D_3A_44,
                TitleBarBackground = 0xE6_24_1C_24,
                TitleBarForeground = 0xFF_F8_F1_EB,
                TitleBarBorder = 0xFF_4D_3A_44,
                SurfaceBackground = 0xFF_28_20_28,
                SurfaceForeground = 0xFF_F8_F1_EB,
                SurfaceBorder = 0xFF_54_43_4F,
                ControlBackground = 0xFF_34_2A_34,
                ControlForeground = 0xFF_F8_F1_EB,
                ControlBorder = 0xFF_66_52_61,
                ControlHoverBackground = 0xFF_40_32_40,
                ControlPressedBackground = 0xFF_4A_39_48,
                ControlDisabledBackground = 0xFF_27_21_27,
                Accent = 0xFF_F2_8C_28,
                AccentForeground = 0xFF_1A_13_10,
                Selection = 0x55_F2_8C_28,
                SelectionForeground = 0xFF_FF_F8_F1,
                FocusBorder = 0xFF_F2_8C_28,
                DisabledForeground = 0xFF_B6_AA_B1,
                DisabledBorder = 0xFF_4B_40_48,
                MutedForeground = 0xFF_C8_BC_C3,
                Success = 0xFF_6F_D2_8A,
                Warning = 0xFF_F2_B5_4D,
                Danger = 0xFF_F2_76_6D,
            },
        };

        private void InitializeLuminaThemeDemo()
        {
            Text = "LuminaForms Phase 6 Theme Demo";

            button1.Text = "跟随系统";
            button2.Text = "浅色 Mica";
            button3.Text = "深色 Mica";
            button4.Text = "Sunset Fluent";
            button5.Text = "琥珀强调色";
            button6.Text = "清除覆盖";
            button7.Text = "Mica Alt";

            label1.Text = "Phase 6-B: 系统风格与主题";
            richTextBox1.Text = "这个页面在 net10.0 路径下演示 Lumina.Forms 的窗口级主题覆盖、语义调色板覆盖和运行时主题切换。";
            toolStripStatusLabel1.Text = "LuminaForms 主题已启用";

            button1.Click += (_, _) => ResetWindowThemeOverrides();
            button2.Click += (_, _) => ApplyWindowTheme(NativeTheme.CreateLightTheme());
            button3.Click += (_, _) => ApplyWindowTheme(NativeTheme.CreateDarkTheme());
            button4.Click += (_, _) => ApplyWindowTheme(SunsetTheme);
            button5.Click += (_, _) => ApplyAmberPaletteOverride();
            button6.Click += (_, _) => ResetWindowThemeOverrides();
            button7.Click += (_, _) => SetMicaAlt();

            UpdateThemeStatus();
        }

        private void ApplyWindowTheme(NativeTheme theme)
        {
            UseTheme(theme);
            ResetPalette();
            UpdateThemeStatus();
        }

        private void ApplyAmberPaletteOverride()
        {
            ThemePalette palette = CurrentVisualStyle.Palette;
            SetPalette(new ThemePalette
            {
                WindowBackground = palette.WindowBackground,
                WindowForeground = palette.WindowForeground,
                WindowBorder = palette.WindowBorder,
                TitleBarBackground = palette.TitleBarBackground,
                TitleBarForeground = palette.TitleBarForeground,
                TitleBarBorder = palette.TitleBarBorder,
                SurfaceBackground = palette.SurfaceBackground,
                SurfaceForeground = palette.SurfaceForeground,
                SurfaceBorder = palette.SurfaceBorder,
                ControlBackground = palette.ControlBackground,
                ControlForeground = palette.ControlForeground,
                ControlBorder = palette.ControlBorder,
                ControlHoverBackground = palette.ControlHoverBackground,
                ControlHoverForeground = palette.ControlHoverForeground,
                ControlPressedBackground = palette.ControlPressedBackground,
                ControlPressedForeground = palette.ControlPressedForeground,
                ControlDisabledBackground = palette.ControlDisabledBackground,
                Accent = 0xFF_FF_9F_1C,
                AccentForeground = 0xFF_1D_15_08,
                Selection = 0x44_FF_9F_1C,
                SelectionForeground = palette.SelectionForeground,
                FocusBorder = 0xFF_FF_9F_1C,
                DisabledForeground = palette.DisabledForeground,
                DisabledBorder = palette.DisabledBorder,
                MutedForeground = palette.MutedForeground,
                Success = palette.Success,
                Warning = palette.Warning,
                Danger = palette.Danger,
            });
            UpdateThemeStatus();
        }

        private void ResetWindowThemeOverrides()
        {
            ResetTheme();
            ResetPalette();
            ResetThemeMode();
            UpdateThemeStatus();
        }

        private void UpdateThemeStatus()
        {
            toolStripStatusLabel1.Text = $"LuminaForms | {CurrentVisualStyle.ThemeMode} | {CurrentVisualStyle.VisualStyleKind} | {CurrentVisualStyle.EffectKind}";
        }
#endif
    }
}
