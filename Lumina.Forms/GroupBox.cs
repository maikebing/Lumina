using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a group box used to visually group related controls.
/// </summary>
public class GroupBox : ContainerControlBase
{
    /// <summary>
    /// Initializes a group box with WinForms-compatible default spacing.
    /// </summary>
    public GroupBox()
    {
        Padding = new Padding(3);
        Size = new Size(200, 100);
    }

    /// <inheritdoc />
    protected override string ClassName => "BUTTON";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.BS_GROUPBOX;

    /// <inheritdoc />
    protected override uint ExStyle => base.ExStyle | Win32.WS_EX_CONTROLPARENT;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultForegroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    protected override bool UseParentBackgroundForTheming => true;

    /// <inheritdoc />
    public override Rectangle DisplayRectangle
    {
        get
        {
            int fontHeight = GetCaptionFontHeight();
            return new Rectangle(
                Padding.Left,
                fontHeight + Padding.Top,
                Math.Max(0, Width - Padding.Horizontal),
                Math.Max(0, Height - fontHeight - Padding.Vertical));
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyNativeThemeState();
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyNativeThemeState();
        ApplyExplorerTheme();
    }

    private void ApplyNativeThemeState()
    {
        if (Handle == 0)
        {
            return;
        }

        DarkModeNative.ApplyThemeToWindow(Handle, CurrentVisualStyle.IsDarkMode);
    }

    private int GetCaptionFontHeight()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            try
            {
                Font? messageBoxFont = SystemFonts.MessageBoxFont;
                if (messageBoxFont is not null && messageBoxFont.Height > 0)
                {
                    return messageBoxFont.Height;
                }
            }
            catch (ArgumentException)
            {
            }
            catch (PlatformNotSupportedException)
            {
            }
        }

        nint fontHandle = Owner is not null && Owner.UiFontHandle != 0
            ? Owner.UiFontHandle
            : Win32.GetStockObject(Win32.DEFAULT_GUI_FONT);

        return Win32.GetFontHeight(fontHandle);
    }
}
