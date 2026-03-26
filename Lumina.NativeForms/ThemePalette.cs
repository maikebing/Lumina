namespace Lumina.NativeForms;

/// <summary>
/// Defines a semantic palette for NativeForms themes.
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
    /// Gets or sets the secondary surface background color in ARGB format.
    /// </summary>
    public uint SurfaceBackground { get; set; } = 0xFF_FF_FF_FF;

    /// <summary>
    /// Gets or sets the secondary surface foreground color in ARGB format.
    /// </summary>
    public uint SurfaceForeground { get; set; } = 0xFF_1C_1C_1C;

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
    /// Gets or sets the foreground color for disabled content.
    /// </summary>
    public uint DisabledForeground { get; set; } = 0xFF_7C_7C_80;

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
    /// Creates the default light palette.
    /// </summary>
    /// <returns>A new light palette instance.</returns>
    public static ThemePalette CreateLight() => new();

    /// <summary>
    /// Creates the default dark palette.
    /// </summary>
    /// <returns>A new dark palette instance.</returns>
    public static ThemePalette CreateDark() => new()
    {
        WindowBackground = 0xFF_20_20_24,
        WindowForeground = 0xFF_F3_F3_F3,
        SurfaceBackground = 0xFF_2A_2A_30,
        SurfaceForeground = 0xFF_F3_F3_F3,
        ControlBackground = 0xFF_2F_2F_35,
        ControlForeground = 0xFF_F3_F3_F3,
        ControlBorder = 0xFF_4A_4A_50,
        Accent = 0xFF_4C_A2_FF,
        AccentForeground = 0xFF_0C_0C_0D,
        Selection = 0x66_4C_A2_FF,
        SelectionForeground = 0xFF_FF_FF_FF,
        DisabledForeground = 0xFF_8A_8A_90,
        MutedForeground = 0xFF_B6_B6_BE,
        Success = 0xFF_4A_D0_7D,
        Warning = 0xFF_F3_B2_4F,
        Danger = 0xFF_F0_71_78,
    };
}
