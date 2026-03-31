namespace Lumina.Forms;

/// <summary>
/// Defines a semantic palette for LuminaForms themes.
/// </summary>
public sealed class ThemePalette
{
    /// <summary>
    /// Gets or sets the primary form background color in ARGB format.
    /// </summary>
    public uint WindowBackground { get; set; } = 0xFF_F7_F7_F9;

    /// <summary>
    /// Gets or sets the default foreground text color in ARGB format.
    /// </summary>
    public uint WindowForeground { get; set; } = 0xFF_1C_1C_1C;

    /// <summary>
    /// Gets or sets the default top-level window border color in ARGB format.
    /// </summary>
    public uint WindowBorder { get; set; } = 0xFF_D7_D7_DC;

    /// <summary>
    /// Gets or sets the title bar background color in ARGB format.
    /// </summary>
    public uint TitleBarBackground { get; set; } = 0xF2_FF_FF_FF;

    /// <summary>
    /// Gets or sets the title bar foreground color in ARGB format.
    /// </summary>
    public uint TitleBarForeground { get; set; } = 0xFF_1C_1C_1C;

    /// <summary>
    /// Gets or sets the title bar border color in ARGB format.
    /// </summary>
    public uint TitleBarBorder { get; set; } = 0xFF_D7_D7_DC;

    /// <summary>
    /// Gets or sets the secondary surface background color in ARGB format.
    /// </summary>
    public uint SurfaceBackground { get; set; } = 0xFF_FF_FF_FF;

    /// <summary>
    /// Gets or sets the secondary surface foreground color in ARGB format.
    /// </summary>
    public uint SurfaceForeground { get; set; } = 0xFF_1C_1C_1C;

    /// <summary>
    /// Gets or sets the secondary surface border color in ARGB format.
    /// </summary>
    public uint SurfaceBorder { get; set; } = 0xFF_E1_E1_E6;

    /// <summary>
    /// Gets or sets the default control background color in ARGB format.
    /// </summary>
    public uint ControlBackground { get; set; } = 0xFF_FF_FF_FF;

    /// <summary>
    /// Gets or sets the default control foreground color in ARGB format.
    /// </summary>
    public uint ControlForeground { get; set; } = 0xFF_1C_1C_1C;

    /// <summary>
    /// Gets or sets the default control border color in ARGB format.
    /// </summary>
    public uint ControlBorder { get; set; } = 0xFF_C8_C8_CC;

    /// <summary>
    /// Gets or sets the default control background color used when a pointer hovers the control.
    /// </summary>
    public uint ControlHoverBackground { get; set; } = 0xFF_F2_F6_FE;

    /// <summary>
    /// Gets or sets the default control foreground color used when a pointer hovers the control.
    /// </summary>
    public uint ControlHoverForeground { get; set; } = 0xFF_1C_1C_1C;

    /// <summary>
    /// Gets or sets the default control background color used when the control is pressed or active.
    /// </summary>
    public uint ControlPressedBackground { get; set; } = 0xFF_E6_F0_FE;

    /// <summary>
    /// Gets or sets the default control foreground color used when the control is pressed or active.
    /// </summary>
    public uint ControlPressedForeground { get; set; } = 0xFF_0F_17_2A;

    /// <summary>
    /// Gets or sets the default background color used for disabled controls.
    /// </summary>
    public uint ControlDisabledBackground { get; set; } = 0xFF_F2_F2_F5;

    /// <summary>
    /// Gets or sets the accent color in ARGB format.
    /// </summary>
    public uint Accent { get; set; } = 0xFF_0A_64_F0;

    /// <summary>
    /// Gets or sets the foreground color used on top of the accent color.
    /// </summary>
    public uint AccentForeground { get; set; } = 0xFF_FF_FF_FF;

    /// <summary>
    /// Gets or sets the selection background color in ARGB format.
    /// </summary>
    public uint Selection { get; set; } = 0x33_0A_64_F0;

    /// <summary>
    /// Gets or sets the selection foreground color in ARGB format.
    /// </summary>
    public uint SelectionForeground { get; set; } = 0xFF_0F_17_2A;

    /// <summary>
    /// Gets or sets the focus border color in ARGB format.
    /// </summary>
    public uint FocusBorder { get; set; } = 0xFF_0A_64_F0;

    /// <summary>
    /// Gets or sets the foreground color for disabled content.
    /// </summary>
    public uint DisabledForeground { get; set; } = 0xFF_7C_7C_80;

    /// <summary>
    /// Gets or sets the border color for disabled controls.
    /// </summary>
    public uint DisabledBorder { get; set; } = 0xFF_D8_D8_DC;

    /// <summary>
    /// Gets or sets the foreground color for muted or helper text.
    /// </summary>
    public uint MutedForeground { get; set; } = 0xFF_5E_5E_66;

    /// <summary>
    /// Gets or sets the color used for success states.
    /// </summary>
    public uint Success { get; set; } = 0xFF_16_A3_4A;

    /// <summary>
    /// Gets or sets the color used for warning states.
    /// </summary>
    public uint Warning { get; set; } = 0xFF_D9_77_06;

    /// <summary>
    /// Gets or sets the color used for error or destructive states.
    /// </summary>
    public uint Danger { get; set; } = 0xFF_DC_26_26;

    /// <summary>
    /// Creates a copy of the current palette.
    /// </summary>
    /// <returns>A new palette containing the same semantic colors.</returns>
    public ThemePalette Clone() => new()
    {
        WindowBackground = WindowBackground,
        WindowForeground = WindowForeground,
        WindowBorder = WindowBorder,
        TitleBarBackground = TitleBarBackground,
        TitleBarForeground = TitleBarForeground,
        TitleBarBorder = TitleBarBorder,
        SurfaceBackground = SurfaceBackground,
        SurfaceForeground = SurfaceForeground,
        SurfaceBorder = SurfaceBorder,
        ControlBackground = ControlBackground,
        ControlForeground = ControlForeground,
        ControlBorder = ControlBorder,
        ControlHoverBackground = ControlHoverBackground,
        ControlHoverForeground = ControlHoverForeground,
        ControlPressedBackground = ControlPressedBackground,
        ControlPressedForeground = ControlPressedForeground,
        ControlDisabledBackground = ControlDisabledBackground,
        Accent = Accent,
        AccentForeground = AccentForeground,
        Selection = Selection,
        SelectionForeground = SelectionForeground,
        FocusBorder = FocusBorder,
        DisabledForeground = DisabledForeground,
        DisabledBorder = DisabledBorder,
        MutedForeground = MutedForeground,
        Success = Success,
        Warning = Warning,
        Danger = Danger,
    };

    /// <summary>
    /// Creates a palette that follows the current operating system theme and accent color.
    /// </summary>
    /// <returns>A new palette instance aligned to the current system defaults.</returns>
    public static ThemePalette CreateSystem() => CreateSystem(ThemeMode.System, VisualStyleKind.System);

    /// <summary>
    /// Creates a palette that follows the requested theme mode while still honoring the current system accent color.
    /// </summary>
    /// <param name="themeMode">The preferred theme mode.</param>
    /// <returns>A new palette instance aligned to the requested mode and current OS accent color.</returns>
    public static ThemePalette CreateSystem(ThemeMode themeMode) => CreateSystem(themeMode, VisualStyleKind.System);

    /// <summary>
    /// Creates a palette that follows the requested theme mode and visual style while honoring the current system accent color.
    /// </summary>
    /// <param name="themeMode">The preferred theme mode.</param>
    /// <param name="visualStyleKind">The preferred system-aligned visual style.</param>
    /// <returns>A new palette instance aligned to the requested mode, visual style, and OS accent color.</returns>
    public static ThemePalette CreateSystem(ThemeMode themeMode, VisualStyleKind visualStyleKind)
    {
        ThemeMode resolvedThemeMode = Application.ResolveThemeMode(themeMode);
        VisualStyleKind resolvedVisualStyleKind = Application.ResolveVisualStyleKind(visualStyleKind);
        ThemePalette palette = resolvedThemeMode == ThemeMode.Dark
            ? CreateDark(resolvedVisualStyleKind)
            : CreateLight(resolvedVisualStyleKind);

        if (TryGetSystemAccentColor(out uint accent))
        {
            palette.ApplyAccent(accent, resolvedThemeMode);
        }

        return palette;
    }

    /// <summary>
    /// Creates a palette from the built-in light or dark defaults and applies a custom accent color.
    /// </summary>
    /// <param name="themeMode">The preferred light or dark mode.</param>
    /// <param name="accent">The accent color in ARGB format.</param>
    /// <param name="visualStyleKind">The preferred system-aligned visual style.</param>
    /// <returns>A new palette instance with the requested accent.</returns>
    public static ThemePalette CreateCustom(ThemeMode themeMode, uint accent, VisualStyleKind visualStyleKind = VisualStyleKind.System)
    {
        ThemeMode resolvedThemeMode = Application.ResolveThemeMode(themeMode);
        VisualStyleKind resolvedVisualStyleKind = Application.ResolveVisualStyleKind(visualStyleKind);
        ThemePalette palette = resolvedThemeMode == ThemeMode.Dark
            ? CreateDark(resolvedVisualStyleKind)
            : CreateLight(resolvedVisualStyleKind);

        palette.ApplyAccent(accent, resolvedThemeMode);
        return palette;
    }

    /// <summary>
    /// Applies an accent color to the palette and updates dependent semantic tokens such as focus and selection colors.
    /// </summary>
    /// <param name="accent">The accent color in ARGB format.</param>
    /// <param name="themeMode">The light or dark theme mode that should influence derived values.</param>
    public void ApplyAccent(uint accent, ThemeMode themeMode)
    {
        uint resolvedAccent = EnsureOpaque(accent);
        Accent = resolvedAccent;
        AccentForeground = IsDarkColor(resolvedAccent) ? 0xFF_FF_FF_FF : 0xFF_08_08_08;
        FocusBorder = resolvedAccent;
        Selection = WithAlpha(themeMode == ThemeMode.Dark ? 0x66u : 0x33u, resolvedAccent);
        SelectionForeground = themeMode == ThemeMode.Dark ? 0xFF_FF_FF_FF : 0xFF_0F_17_2A;
        ControlHoverBackground = Blend(ControlBackground, resolvedAccent, themeMode == ThemeMode.Dark ? 0.18f : 0.10f);
        ControlPressedBackground = Blend(ControlBackground, resolvedAccent, themeMode == ThemeMode.Dark ? 0.28f : 0.18f);
        ControlPressedForeground = themeMode == ThemeMode.Dark ? 0xFF_FF_FF_FF : 0xFF_0F_17_2A;
    }

    /// <summary>
    /// Attempts to read the current Windows accent color.
    /// </summary>
    /// <param name="accent">When this method returns, contains the ARGB accent color if available.</param>
    /// <returns><see langword="true"/> when a system accent color was detected; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetSystemAccentColor(out uint accent)
    {
        accent = 0;
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        try
        {
            if (Win32.DwmGetColorizationColor(out uint colorizationColor, out _) == 0)
            {
                accent = EnsureOpaque(colorizationColor);
                return true;
            }
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }

        return false;
    }

    /// <summary>
    /// Creates the default light palette.
    /// </summary>
    /// <returns>A new light palette instance.</returns>
    public static ThemePalette CreateLight() => CreateLight(VisualStyleKind.Mica);

    /// <summary>
    /// Creates the default light palette for a specific system-aligned visual style.
    /// </summary>
    /// <param name="visualStyleKind">The style family that should influence default palette choices.</param>
    /// <returns>A new light palette instance.</returns>
    public static ThemePalette CreateLight(VisualStyleKind visualStyleKind)
    {
        ThemePalette palette = new();

        switch (visualStyleKind)
        {
            case VisualStyleKind.Mica:
                palette.WindowBackground = 0xFF_F3_F3_F3;
                palette.WindowBorder = 0xFF_E6_E6_E6;
                palette.TitleBarBackground = 0xD9_F8_F8_F8;
                palette.TitleBarBorder = 0xFF_E8_E8_E8;
                palette.SurfaceBackground = 0xFF_FF_FF_FF;
                palette.SurfaceBorder = 0xFF_E5_E5_E5;
                palette.ControlBackground = 0xFF_FC_FC_FC;
                palette.ControlBorder = 0xFF_DA_DA_DA;
                palette.ControlHoverBackground = 0xFF_F5_F5_F5;
                palette.ControlPressedBackground = 0xFF_EE_EE_EE;
                palette.ControlDisabledBackground = 0xFF_F5_F5_F5;
                palette.Accent = 0xFF_00_5F_B8;
                palette.FocusBorder = 0xFF_00_5F_B8;
                palette.Selection = 0x33_00_5F_B8;
                palette.SelectionForeground = 0xFF_11_1A_24;
                palette.MutedForeground = 0xFF_5F_5F_5F;
                break;

            case VisualStyleKind.AeroGlass:
                palette.WindowBackground = 0xFF_EF_F5_FB;
                palette.WindowBorder = 0xFF_9B_B6_D4;
                palette.TitleBarBackground = 0xCC_EC_F4_FD;
                palette.TitleBarBorder = 0xFF_9B_B6_D4;
                palette.SurfaceBorder = 0xFF_C5_D7_EA;
                palette.ControlBorder = 0xFF_A9_BD_D2;
                palette.Accent = 0xFF_4D_79_A6;
                palette.FocusBorder = 0xFF_4D_79_A6;
                palette.Selection = 0x33_4D_79_A6;
                palette.SelectionForeground = 0xFF_1E_2C_3A;
                break;

            case VisualStyleKind.Modern:
                palette.WindowBackground = 0xFF_F3_F3_F3;
                palette.WindowBorder = 0xFF_D0_D0_D0;
                palette.TitleBarBackground = 0xFF_F7_F7_F7;
                palette.TitleBarBorder = 0xFF_D0_D0_D0;
                palette.SurfaceBorder = 0xFF_D9_D9_D9;
                palette.ControlBorder = 0xFF_BB_BB_BB;
                palette.Accent = 0xFF_00_78_D7;
                palette.FocusBorder = 0xFF_00_78_D7;
                palette.Selection = 0x33_00_78_D7;
                break;

            case VisualStyleKind.Fluent:
                palette.WindowBackground = 0xFF_F6_F7_FA;
                palette.WindowBorder = 0xFF_D7_D9_E0;
                palette.TitleBarBackground = 0xE6_FF_FF_FF;
                palette.TitleBarBorder = 0xFF_D7_D9_E0;
                palette.SurfaceBorder = 0xFF_E2_E4_EA;
                palette.ControlBorder = 0xFF_C9_CF_DB;
                palette.Accent = 0xFF_00_7A_CC;
                palette.FocusBorder = 0xFF_00_7A_CC;
                palette.Selection = 0x33_00_7A_CC;
                break;

            case VisualStyleKind.Classic:
                palette.WindowBackground = 0xFF_EC_E9_D8;
                palette.WindowForeground = 0xFF_00_00_00;
                palette.WindowBorder = 0xFF_80_80_80;
                palette.TitleBarBackground = 0xFF_0A_24_6A;
                palette.TitleBarForeground = 0xFF_FF_FF_FF;
                palette.TitleBarBorder = 0xFF_80_80_80;
                palette.SurfaceBackground = 0xFF_F0_F0_F0;
                palette.SurfaceBorder = 0xFF_A0_A0_A0;
                palette.ControlBorder = 0xFF_80_80_80;
                palette.Accent = 0xFF_0A_24_6A;
                palette.FocusBorder = 0xFF_0A_24_6A;
                palette.Selection = 0x33_0A_24_6A;
                palette.SelectionForeground = 0xFF_00_00_00;
                break;

            default:
                break;
        }

        return palette;
    }

    /// <summary>
    /// Creates the default dark palette.
    /// </summary>
    /// <returns>A new dark palette instance.</returns>
    public static ThemePalette CreateDark() => CreateDark(VisualStyleKind.Mica);

    /// <summary>
    /// Creates the default dark palette for a specific system-aligned visual style.
    /// </summary>
    /// <param name="visualStyleKind">The style family that should influence default palette choices.</param>
    /// <returns>A new dark palette instance.</returns>
    public static ThemePalette CreateDark(VisualStyleKind visualStyleKind)
    {
        ThemePalette palette = new()
        {
            WindowBackground = 0xFF_20_20_24,
            WindowForeground = 0xFF_F3_F3_F3,
            WindowBorder = 0xFF_39_39_40,
            TitleBarBackground = 0xE6_20_20_24,
            TitleBarForeground = 0xFF_F3_F3_F3,
            TitleBarBorder = 0xFF_39_39_40,
            SurfaceBackground = 0xFF_2A_2A_30,
            SurfaceForeground = 0xFF_F3_F3_F3,
            SurfaceBorder = 0xFF_3C_3C_44,
            ControlBackground = 0xFF_2F_2F_35,
            ControlForeground = 0xFF_F3_F3_F3,
            ControlBorder = 0xFF_4A_4A_50,
            ControlHoverBackground = 0xFF_37_37_3D,
            ControlHoverForeground = 0xFF_F7_F7_FA,
            ControlPressedBackground = 0xFF_43_43_4C,
            ControlPressedForeground = 0xFF_FF_FF_FF,
            ControlDisabledBackground = 0xFF_27_27_2C,
            Accent = 0xFF_4C_A2_FF,
            AccentForeground = 0xFF_0C_0C_0D,
            Selection = 0x66_4C_A2_FF,
            SelectionForeground = 0xFF_FF_FF_FF,
            FocusBorder = 0xFF_76_B6_FF,
            DisabledForeground = 0xFF_8A_8A_90,
            DisabledBorder = 0xFF_3C_3C_42,
            MutedForeground = 0xFF_B6_B6_BE,
            Success = 0xFF_4A_D0_7D,
            Warning = 0xFF_F3_B2_4F,
            Danger = 0xFF_F0_71_78,
        };

        switch (visualStyleKind)
        {
            case VisualStyleKind.Mica:
                palette.WindowBackground = 0xFF_20_20_20;
                palette.WindowBorder = 0xFF_32_32_32;
                palette.TitleBarBackground = 0xD9_20_20_20;
                palette.TitleBarBorder = 0xFF_36_36_36;
                palette.SurfaceBackground = 0xFF_2B_2B_2B;
                palette.SurfaceBorder = 0xFF_3A_3A_3A;
                palette.ControlBackground = 0xFF_2D_2D_2D;
                palette.ControlBorder = 0xFF_47_47_47;
                palette.ControlHoverBackground = 0xFF_35_35_35;
                palette.ControlPressedBackground = 0xFF_3E_3E_3E;
                palette.ControlDisabledBackground = 0xFF_26_26_26;
                palette.Accent = 0xFF_60_CD_FF;
                palette.AccentForeground = 0xFF_08_08_08;
                palette.Selection = 0x44_60_CD_FF;
                palette.SelectionForeground = 0xFF_FF_FF_FF;
                palette.DisabledForeground = 0xFF_9A_9A_9A;
                palette.MutedForeground = 0xFF_C6_C6_C6;
                break;

            case VisualStyleKind.AeroGlass:
                palette.WindowBackground = 0xFF_1D_26_31;
                palette.WindowBorder = 0xFF_4F_67_7E;
                palette.TitleBarBackground = 0xCC_26_31_40;
                palette.TitleBarBorder = 0xFF_4F_67_7E;
                palette.SurfaceBorder = 0xFF_4A_5C_6F;
                palette.ControlBorder = 0xFF_5A_70_88;
                palette.Accent = 0xFF_7B_AE_E8;
                palette.FocusBorder = 0xFF_7B_AE_E8;
                palette.Selection = 0x44_7B_AE_E8;
                break;

            case VisualStyleKind.Modern:
                palette.WindowBackground = 0xFF_1F_1F_1F;
                palette.WindowBorder = 0xFF_3A_3A_3A;
                palette.TitleBarBackground = 0xFF_27_27_27;
                palette.TitleBarBorder = 0xFF_3A_3A_3A;
                palette.SurfaceBorder = 0xFF_3D_3D_3D;
                palette.ControlBorder = 0xFF_57_57_57;
                palette.Accent = 0xFF_00_99_BC;
                palette.FocusBorder = 0xFF_00_99_BC;
                palette.Selection = 0x44_00_99_BC;
                break;

            case VisualStyleKind.Fluent:
                palette.WindowBackground = 0xFF_1E_1E_24;
                palette.WindowBorder = 0xFF_38_38_40;
                palette.TitleBarBackground = 0xD9_1E_1E_24;
                palette.TitleBarBorder = 0xFF_38_38_40;
                palette.SurfaceBorder = 0xFF_40_40_49;
                palette.ControlBorder = 0xFF_52_52_5C;
                palette.Accent = 0xFF_4C_C2_F1;
                palette.FocusBorder = 0xFF_4C_C2_F1;
                palette.Selection = 0x55_4C_C2_F1;
                break;

            case VisualStyleKind.Classic:
                palette.WindowBackground = 0xFF_40_40_40;
                palette.WindowBorder = 0xFF_70_70_70;
                palette.TitleBarBackground = 0xFF_0A_24_6A;
                palette.TitleBarForeground = 0xFF_FF_FF_FF;
                palette.TitleBarBorder = 0xFF_70_70_70;
                palette.SurfaceBackground = 0xFF_34_34_34;
                palette.SurfaceForeground = 0xFF_F0_F0_F0;
                palette.SurfaceBorder = 0xFF_70_70_70;
                palette.ControlBackground = 0xFF_4A_4A_4A;
                palette.ControlForeground = 0xFF_F0_F0_F0;
                palette.ControlBorder = 0xFF_70_70_70;
                palette.Accent = 0xFF_86_AE_FF;
                palette.FocusBorder = 0xFF_86_AE_FF;
                palette.Selection = 0x44_86_AE_FF;
                break;
        }

        return palette;
    }

    internal uint GetBackground(ThemeColorSlot slot)
    {
        return slot switch
        {
            ThemeColorSlot.Window => WindowBackground,
            ThemeColorSlot.Surface => SurfaceBackground,
            _ => ControlBackground,
        };
    }

    internal uint GetForeground(ThemeColorSlot slot)
    {
        return slot switch
        {
            ThemeColorSlot.Window => WindowForeground,
            ThemeColorSlot.Surface => SurfaceForeground,
            _ => ControlForeground,
        };
    }

    private static uint EnsureOpaque(uint argb) => 0xFF_00_00_00 | (argb & 0x00_FF_FF_FF);

    private static uint WithAlpha(uint alpha, uint argb) => (alpha << 24) | (argb & 0x00_FF_FF_FF);

    private static bool IsDarkColor(uint argb)
    {
        float red = ((argb >> 16) & 0xFF) / 255f;
        float green = ((argb >> 8) & 0xFF) / 255f;
        float blue = (argb & 0xFF) / 255f;
        float luminance = (0.2126f * red) + (0.7152f * green) + (0.0722f * blue);
        return luminance < 0.5f;
    }

    private static uint Blend(uint background, uint foreground, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);

        float backgroundRed = (background >> 16) & 0xFF;
        float backgroundGreen = (background >> 8) & 0xFF;
        float backgroundBlue = background & 0xFF;

        float foregroundRed = (foreground >> 16) & 0xFF;
        float foregroundGreen = (foreground >> 8) & 0xFF;
        float foregroundBlue = foreground & 0xFF;

        byte red = (byte)Math.Clamp((backgroundRed * (1f - amount)) + (foregroundRed * amount), 0f, 255f);
        byte green = (byte)Math.Clamp((backgroundGreen * (1f - amount)) + (foregroundGreen * amount), 0f, 255f);
        byte blue = (byte)Math.Clamp((backgroundBlue * (1f - amount)) + (foregroundBlue * amount), 0f, 255f);

        return 0xFF_00_00_00
            | ((uint)red << 16)
            | ((uint)green << 8)
            | blue;
    }
}
