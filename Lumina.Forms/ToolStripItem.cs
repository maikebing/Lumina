using System.ComponentModel;
using System.Drawing;

namespace Lumina.Forms;

/// <summary>
/// Represents a logical item hosted by a tool strip or menu.
/// </summary>
public class ToolStripItem : Component
{
    private bool _enabled = true;
    private bool _visible = true;
    private string _name = string.Empty;
    private string _text = string.Empty;
    private Size _size;
    private Image? _image;
    private Color _imageTransparentColor;
    private ToolStripItemDisplayStyle _displayStyle = ToolStripItemDisplayStyle.ImageAndText;

    /// <summary>
    /// Occurs when the item is activated.
    /// </summary>
    public event EventHandler? Click;

    internal event EventHandler? Changed;

    /// <summary>
    /// Gets or sets the design-time name.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetValue(ref _name, value ?? string.Empty);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item can be activated.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set => SetValue(ref _enabled, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is visible.
    /// </summary>
    public bool Visible
    {
        get => _visible;
        set => SetValue(ref _visible, value);
    }

    /// <summary>
    /// Gets or sets the displayed text.
    /// </summary>
    public string Text
    {
        get => _text;
        set => SetValue(ref _text, value ?? string.Empty);
    }

    /// <summary>
    /// Gets or sets the design-time size.
    /// </summary>
    public Size Size
    {
        get => _size;
        set => SetValue(ref _size, value);
    }

    /// <summary>
    /// Gets or sets the image displayed by the item.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public Image? Image
    {
        get => _image;
        set => SetValue(ref _image, value);
    }

    /// <summary>
    /// Gets or sets the color treated as transparent when the image is rendered.
    /// </summary>
    public Color ImageTransparentColor
    {
        get => _imageTransparentColor;
        set => SetValue(ref _imageTransparentColor, value);
    }

    /// <summary>
    /// Gets or sets how the item shows its image and text.
    /// </summary>
    public ToolStripItemDisplayStyle DisplayStyle
    {
        get => _displayStyle;
        set => SetValue(ref _displayStyle, value);
    }

    internal bool SupportsMenuImage => _image is not null && DisplayStyle is not ToolStripItemDisplayStyle.Text and not ToolStripItemDisplayStyle.None;

    internal char? GetMnemonic()
    {
        if (string.IsNullOrEmpty(Text))
        {
            return null;
        }

        for (int index = 0; index < Text.Length - 1; index++)
        {
            if (Text[index] != '&')
            {
                continue;
            }

            char nextCharacter = Text[index + 1];
            if (nextCharacter == '&')
            {
                index++;
                continue;
            }

            return char.ToUpperInvariant(nextCharacter);
        }

        return null;
    }

    /// <summary>
    /// Programmatically invokes the item.
    /// </summary>
    public void PerformClick()
    {
        if (!Enabled)
        {
            return;
        }

        OnClick(EventArgs.Empty);
    }

    /// <summary>
    /// Raises the <see cref="Click"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnClick(EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the internal change notification used to refresh menu and tool strip hosts.
    /// </summary>
    protected void NotifyChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void SetValue<T>(ref T field, T value)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        NotifyChanged();
    }
}
