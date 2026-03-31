using Microsoft.Win32;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

public static partial class LuminaFormExtensions
{
    private const string PersonalizeRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private static readonly ConditionalWeakTable<Form, ThemeState> s_themeStates = new();

    /// <summary>
    /// Resets the form to the default system-following theme, including the current Windows accent color.
    /// </summary>
    public static void UseSystemTheme(this Form form)
    {
        ArgumentNullException.ThrowIfNull(form);
        ThemeState state = GetThemeState(form);
        state.ModeSelection = ThemeModeSelection.System;
        state.PaletteSelection = PaletteSelection.System;
        ApplyThemeState(form, state);
    }

    /// <summary>
    /// Applies the built-in light theme while still honoring the current Windows accent color.
    /// </summary>
    public static void UseLightTheme(this Form form)
    {
        ArgumentNullException.ThrowIfNull(form);
        ThemeState state = GetThemeState(form);
        state.ModeSelection = ThemeModeSelection.Light;
        state.PaletteSelection = PaletteSelection.System;
        ApplyThemeState(form, state);
    }

    /// <summary>
    /// Applies the built-in dark theme while still honoring the current Windows accent color.
    /// </summary>
    public static void UseDarkTheme(this Form form)
    {
        ArgumentNullException.ThrowIfNull(form);
        ThemeState state = GetThemeState(form);
        state.ModeSelection = ThemeModeSelection.Dark;
        state.PaletteSelection = PaletteSelection.System;
        ApplyThemeState(form, state);
    }

    /// <summary>
    /// Reapplies the current theme mode using the detected Windows accent color.
    /// </summary>
    public static void UseSystemColors(this Form form)
    {
        ArgumentNullException.ThrowIfNull(form);
        ThemeState state = GetThemeState(form);
        state.PaletteSelection = PaletteSelection.System;
        ApplyThemeState(form, state);
    }

    /// <summary>
    /// Applies a built-in custom accent palette while preserving the current light or dark mode.
    /// </summary>
    /// <param name="form">The target form.</param>
    /// <param name="accent">The custom accent color in ARGB format.</param>
    public static void UseCustomTheme(this Form form, uint accent = 0xFF_8B_5C_F6)
    {
        ArgumentNullException.ThrowIfNull(form);
        ThemeState state = GetThemeState(form);
        state.PaletteSelection = PaletteSelection.Custom;
        state.CustomAccent = EnsureOpaque(accent);
        ApplyThemeState(form, state);
    }

    private static ThemeState GetThemeState(Form form) => s_themeStates.GetOrCreateValue(form);

    private static void ApplyThemeState(Form form, ThemeState state)
    {
        EnsureHandleHook(form, state);
        if (!form.IsHandleCreated)
        {
            return;
        }

        ThemeModeSelection resolvedMode = ResolveThemeMode(state.ModeSelection);
        uint accent = state.PaletteSelection == PaletteSelection.Custom
            ? state.CustomAccent
            : GetSystemAccentOrDefault(resolvedMode);

        WinFormsThemePalette palette = CreatePalette(resolvedMode, accent);
        ApplyDarkMode(form, resolvedMode == ThemeModeSelection.Dark);
        ApplyPalette(form, palette);
    }

    private static void EnsureHandleHook(Form form, ThemeState state)
    {
        if (state.IsHandleHooked)
        {
            return;
        }

        form.HandleCreated += OnHandleCreated;
        state.IsHandleHooked = true;
    }

    private static void OnHandleCreated(object? sender, EventArgs e)
    {
        if (sender is Form form && s_themeStates.TryGetValue(form, out ThemeState? state))
        {
            ApplyThemeState(form, state);
        }
    }

    private static ThemeModeSelection ResolveThemeMode(ThemeModeSelection modeSelection)
    {
        if (modeSelection != ThemeModeSelection.System)
        {
            return modeSelection;
        }

        return DetectSystemDarkMode() ? ThemeModeSelection.Dark : ThemeModeSelection.Light;
    }

    private static bool DetectSystemDarkMode()
    {
        try
        {
            object? value = Registry.GetValue($@"HKEY_CURRENT_USER\{PersonalizeRegistryKey}", "AppsUseLightTheme", 1);
            return value is int intValue && intValue == 0;
        }
        catch
        {
            return false;
        }
    }

    private static uint GetSystemAccentOrDefault(ThemeModeSelection modeSelection)
    {
        if (TryGetSystemAccentColor(out uint accent))
        {
            return accent;
        }

        return modeSelection == ThemeModeSelection.Dark ? 0xFF_60_CD_FF : 0xFF_00_5F_B8;
    }

    private static bool TryGetSystemAccentColor(out uint accent)
    {
        accent = 0;

        try
        {
            if (DwmGetColorizationColor(out uint colorizationColor, out _) == 0)
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

    private static WinFormsThemePalette CreatePalette(ThemeModeSelection modeSelection, uint accent)
    {
        bool dark = modeSelection == ThemeModeSelection.Dark;
        Color windowBackground = Color.FromArgb(unchecked((int)(dark ? 0xFF_20_20_20 : 0xFF_F3_F3_F3)));
        Color windowForeground = Color.FromArgb(unchecked((int)(dark ? 0xFF_F3_F3_F3 : 0xFF_1C_1C_1C)));
        Color surfaceBackground = Color.FromArgb(unchecked((int)(dark ? 0xFF_2B_2B_2B : 0xFF_FF_FF_FF)));
        Color surfaceForeground = Color.FromArgb(unchecked((int)(dark ? 0xFF_F3_F3_F3 : 0xFF_1C_1C_1C)));
        Color controlBackground = Color.FromArgb(unchecked((int)(dark ? 0xFF_2D_2D_2D : 0xFF_FC_FC_FC)));
        Color controlForeground = Color.FromArgb(unchecked((int)(dark ? 0xFF_F3_F3_F3 : 0xFF_1C_1C_1C)));
        Color accentColor = Color.FromArgb(unchecked((int)EnsureOpaque(accent)));
        Color accentForeground = GetContrastColor(accentColor);
        Color selection = Color.FromArgb(dark ? 0x66 : 0x33, accentColor);
        Color menuSelection = Blend(surfaceBackground, accentColor, dark ? 0.24f : 0.14f);

        return new WinFormsThemePalette(
            windowBackground,
            windowForeground,
            surfaceBackground,
            surfaceForeground,
            controlBackground,
            controlForeground,
            accentColor,
            accentForeground,
            selection,
            menuSelection);
    }

    private static void ApplyDarkMode(Form form, bool darkMode)
    {
        if (!form.IsHandleCreated)
        {
            return;
        }

        int useDarkMode = darkMode ? 1 : 0;
        _ = DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
    }

    private static void ApplyPalette(Form form, WinFormsThemePalette palette)
    {
        form.SuspendLayout();
        form.BackColor = palette.WindowBackground;
        form.ForeColor = palette.WindowForeground;

        if (form.MainMenuStrip is not null)
        {
            ApplyToolStrip(form.MainMenuStrip, palette);
        }

        foreach (Control control in form.Controls)
        {
            ApplyControl(control, palette);
        }

        form.ResumeLayout(true);
        form.Invalidate(true);
        form.Update();
    }

    private static void ApplyControl(Control control, WinFormsThemePalette palette)
    {
        switch (control)
        {
            case MenuStrip menuStrip:
                ApplyToolStrip(menuStrip, palette);
                break;

            case StatusStrip statusStrip:
                ApplyToolStrip(statusStrip, palette);
                break;

            case ToolStrip toolStrip:
                ApplyToolStrip(toolStrip, palette);
                break;

            case GroupBox or Panel or FlowLayoutPanel or SplitContainer or TabPage:
                control.BackColor = palette.SurfaceBackground;
                control.ForeColor = palette.SurfaceForeground;
                break;

            case PictureBox:
                control.BackColor = palette.SurfaceBackground;
                control.ForeColor = palette.SurfaceForeground;
                break;

            case LinkLabel linkLabel:
                linkLabel.BackColor = palette.WindowBackground;
                linkLabel.ForeColor = palette.Accent;
                linkLabel.LinkColor = palette.Accent;
                linkLabel.ActiveLinkColor = palette.Accent;
                linkLabel.VisitedLinkColor = palette.Accent;
                break;

            case MonthCalendar monthCalendar:
                monthCalendar.BackColor = palette.SurfaceBackground;
                monthCalendar.ForeColor = palette.SurfaceForeground;
                monthCalendar.TitleBackColor = palette.Accent;
                monthCalendar.TitleForeColor = palette.AccentForeground;
                monthCalendar.TrailingForeColor = Blend(palette.SurfaceForeground, palette.SurfaceBackground, 0.45f);
                break;

            case TextBoxBase or ComboBox or ListBox or ListView or TreeView or NumericUpDown or DateTimePicker:
                control.BackColor = palette.ControlBackground;
                control.ForeColor = palette.ControlForeground;
                break;

            default:
                control.BackColor = palette.ControlBackground;
                control.ForeColor = palette.ControlForeground;
                break;
        }

        foreach (Control child in control.Controls)
        {
            ApplyControl(child, palette);
        }
    }

    private static void ApplyToolStrip(ToolStrip toolStrip, WinFormsThemePalette palette)
    {
        toolStrip.BackColor = palette.SurfaceBackground;
        toolStrip.ForeColor = palette.SurfaceForeground;
        toolStrip.RenderMode = ToolStripRenderMode.Professional;
        toolStrip.Renderer = new ToolStripProfessionalRenderer(new LuminaToolStripColorTable(palette));

        foreach (ToolStripItem item in toolStrip.Items)
        {
            ApplyToolStripItem(item, palette);
        }
    }

    private static void ApplyToolStripItem(ToolStripItem item, WinFormsThemePalette palette)
    {
        item.BackColor = palette.SurfaceBackground;
        item.ForeColor = palette.SurfaceForeground;

        if (item is ToolStripDropDownItem dropDownItem)
        {
            foreach (ToolStripItem child in dropDownItem.DropDownItems)
            {
                ApplyToolStripItem(child, palette);
            }
        }
    }

    private static Color GetContrastColor(Color color)
    {
        double luminance = (0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B);
        return luminance < 140d ? Color.White : Color.FromArgb(0x08, 0x08, 0x08);
    }

    private static Color Blend(Color background, Color foreground, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        int red = (int)Math.Clamp((background.R * (1f - amount)) + (foreground.R * amount), 0f, 255f);
        int green = (int)Math.Clamp((background.G * (1f - amount)) + (foreground.G * amount), 0f, 255f);
        int blue = (int)Math.Clamp((background.B * (1f - amount)) + (foreground.B * amount), 0f, 255f);
        return Color.FromArgb(red, green, blue);
    }

    private static uint EnsureOpaque(uint argb) => 0xFF_00_00_00 | (argb & 0x00_FF_FF_FF);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(nint hwnd, int attribute, ref int value, int valueSize);

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetColorizationColor(out uint colorizationColor, [MarshalAs(UnmanagedType.Bool)] out bool opaqueBlend);

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    private sealed class ThemeState
    {
        public ThemeModeSelection ModeSelection { get; set; } = ThemeModeSelection.System;

        public PaletteSelection PaletteSelection { get; set; } = PaletteSelection.System;

        public uint CustomAccent { get; set; } = 0xFF_8B_5C_F6;

        public bool IsHandleHooked { get; set; }
    }

    private sealed class WinFormsThemePalette(
        Color windowBackground,
        Color windowForeground,
        Color surfaceBackground,
        Color surfaceForeground,
        Color controlBackground,
        Color controlForeground,
        Color accent,
        Color accentForeground,
        Color selection,
        Color menuSelection)
    {
        public Color WindowBackground { get; } = windowBackground;

        public Color WindowForeground { get; } = windowForeground;

        public Color SurfaceBackground { get; } = surfaceBackground;

        public Color SurfaceForeground { get; } = surfaceForeground;

        public Color ControlBackground { get; } = controlBackground;

        public Color ControlForeground { get; } = controlForeground;

        public Color Accent { get; } = accent;

        public Color AccentForeground { get; } = accentForeground;

        public Color Selection { get; } = selection;

        public Color MenuSelection { get; } = menuSelection;
    }

    private sealed class LuminaToolStripColorTable(WinFormsThemePalette palette) : ProfessionalColorTable
    {
        public override Color ToolStripBorder => palette.Accent;

        public override Color MenuBorder => palette.Accent;

        public override Color ToolStripDropDownBackground => palette.SurfaceBackground;

        public override Color ImageMarginGradientBegin => palette.SurfaceBackground;

        public override Color ImageMarginGradientMiddle => palette.SurfaceBackground;

        public override Color ImageMarginGradientEnd => palette.SurfaceBackground;

        public override Color MenuItemSelected => palette.MenuSelection;

        public override Color MenuItemSelectedGradientBegin => palette.MenuSelection;

        public override Color MenuItemSelectedGradientEnd => palette.MenuSelection;

        public override Color MenuItemPressedGradientBegin => palette.Selection;

        public override Color MenuItemPressedGradientMiddle => palette.Selection;

        public override Color MenuItemPressedGradientEnd => palette.Selection;

        public override Color ToolStripGradientBegin => palette.SurfaceBackground;

        public override Color ToolStripGradientMiddle => palette.SurfaceBackground;

        public override Color ToolStripGradientEnd => palette.SurfaceBackground;

        public override Color StatusStripGradientBegin => palette.SurfaceBackground;

        public override Color StatusStripGradientEnd => palette.SurfaceBackground;
    }

    private enum ThemeModeSelection
    {
        System,
        Light,
        Dark,
    }

    private enum PaletteSelection
    {
        System,
        Custom,
    }
}
