using System.Drawing;
using Lumina.NativeForms;
using Xunit;

namespace Lumina.Tests;

public class AutoScaleTests
{
    [Fact]
    public void PerformAutoScale_ScalesFormAndAttachedControls()
    {
        using var form = new Form
        {
            AutoScaleMode = AutoScaleMode.Dpi,
            AutoScaleDimensions = new SizeF(48f, 48f),
            ClientSize = new Size(100, 80),
        };

        var button = new Button();
        button.SetBounds(10, 12, 30, 20);
        form.Controls.Add(button);

        form.PerformAutoScale();

        Assert.True(form.Width >= 200);
        Assert.True(form.Height >= 160);
        Assert.True(button.Left >= 20);
        Assert.True(button.Top >= 24);
        Assert.True(button.Width >= 60);
        Assert.True(button.Height >= 40);
        Assert.Equal(form.CurrentAutoScaleDimensions, form.AutoScaleDimensions);
    }

    [Fact]
    public void PerformAutoScale_WithNoneMode_DoesNotChangeBounds()
    {
        using var form = new Form
        {
            AutoScaleMode = AutoScaleMode.None,
            ClientSize = new Size(100, 80),
        };

        var label = new Label();
        label.SetBounds(10, 12, 30, 20);
        form.Controls.Add(label);

        form.PerformAutoScale();

        Assert.Equal(100, form.Width);
        Assert.Equal(80, form.Height);
        Assert.Equal(new Rectangle(10, 12, 30, 20), label.Bounds);
    }
}
