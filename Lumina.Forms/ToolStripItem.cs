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

    /// <summary>
    /// Occurs when the item is activated.
    /// </summary>
    public event EventHandler? Click;

    /// <summary>
    /// Gets or sets the design-time name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the item can be activated.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is visible.
    /// </summary>
    public bool Visible
    {
        get => _visible;
        set => _visible = value;
    }

    /// <summary>
    /// Gets or sets the displayed text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the design-time size.
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// Gets or sets the image displayed by the item.
    /// </summary>
    public Image? Image { get; set; }

    /// <summary>
    /// Gets or sets the color treated as transparent when the image is rendered.
    /// </summary>
    public Color ImageTransparentColor { get; set; }

    /// <summary>
    /// Gets or sets how the item shows its image and text.
    /// </summary>
    public ToolStripItemDisplayStyle DisplayStyle { get; set; } = ToolStripItemDisplayStyle.ImageAndText;

    internal bool SupportsMenuImage => Image is not null && DisplayStyle is not ToolStripItemDisplayStyle.Text and not ToolStripItemDisplayStyle.None;

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
}