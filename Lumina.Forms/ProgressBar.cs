namespace Lumina.Forms;

/// <summary>
/// Represents a progress bar backed by the native common control.
/// </summary>
public class ProgressBar : Control
{
    private int _value;

    /// <summary>
    /// Gets or sets the current progress value.
    /// </summary>
    public int Value
    {
        get => _value;
        set
        {
            _value = Math.Max(0, value);
            if (Handle != 0)
            {
                _ = Win32.SendMessageW(Handle, Win32.PBM_SETPOS, (nint)_value, 0);
            }
        }
    }

    /// <inheritdoc />
    protected override string ClassName => "msctls_progress32";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        Value = _value;
    }

    /// <inheritdoc />
    protected override void ApplyTheme()
    {
        ApplyExplorerTheme();
    }
}
