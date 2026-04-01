namespace Lumina.Forms;

/// <summary>
/// Represents a standard push button control.
/// </summary>
public class Button : Control
{
    /// <summary>
    /// Gets or sets a value indicating whether the control should use system visual styles for its background.
    /// </summary>
    public bool UseVisualStyleBackColor { get; set; } = true;

    /// <summary>
    /// Occurs when the user activates the button.
    /// </summary>
    public event EventHandler? Click;

    /// <summary>
    /// Programmatically raises the button click action.
    /// </summary>
    public void PerformClick()
    {
        OnClick(EventArgs.Empty);
    }

    /// <inheritdoc />
    protected override string ClassName => "BUTTON";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.WS_TABSTOP | Win32.BS_PUSHBUTTON;

    /// <inheritdoc />
    protected override bool UseParentBackgroundForTheming => UseVisualStyleBackColor;

    /// <inheritdoc />
    protected override int GetNativeHeight(int requestedHeight)
        => Math.Max(30, requestedHeight);

    /// <inheritdoc />
    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.BN_CLICKED)
        {
            return false;
        }

        OnClick(EventArgs.Empty);
        return true;
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyExplorerTheme();
    }

    /// <summary>
    /// Raises the <see cref="Click"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnClick(EventArgs e)
    {
        Click?.Invoke(this, e);
    }
}
