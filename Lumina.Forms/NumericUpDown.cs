using System.ComponentModel;
using System.Globalization;

namespace Lumina.Forms;

/// <summary>
/// Represents a numeric input control.
/// </summary>
public class NumericUpDown : TextBox, ISupportInitialize
{
    private decimal _value;

    /// <summary>
    /// Gets or sets the numeric value.
    /// </summary>
    public decimal Value
    {
        get => _value;
        set
        {
            _value = value;
            Text = value.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public decimal Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public decimal Maximum { get; set; } = 100;

    /// <inheritdoc />
    public void BeginInit()
    {
    }

    /// <inheritdoc />
    public void EndInit()
    {
        Value = Math.Clamp(Value, Minimum, Maximum);
    }
}