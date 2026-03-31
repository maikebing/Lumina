using Lumina.Forms;
using Xunit;

namespace Lumina.Tests;

public class ControlBehaviorTests
{
    [Fact]
    public void ButtonPerformClick_RaisesClick()
    {
        var button = new Button();
        int clickCount = 0;

        button.Click += (_, _) => clickCount++;
        button.PerformClick();

        Assert.Equal(1, clickCount);
    }

    [Fact]
    public void CheckBoxChecked_RaisesCheckedChangedWhenValueChanges()
    {
        var checkBox = new CheckBox();
        int changedCount = 0;

        checkBox.CheckedChanged += (_, _) => changedCount++;
        checkBox.Checked = true;
        checkBox.Checked = true;
        checkBox.Checked = false;

        Assert.Equal(2, changedCount);
    }

    [Fact]
    public void TextBoxText_RaisesTextChangedWhenValueChanges()
    {
        var textBox = new TextBox();
        int changedCount = 0;

        textBox.TextChanged += (_, _) => changedCount++;
        textBox.Text = "Hello";
        textBox.Text = "Hello";
        textBox.AppendText(" World");

        Assert.Equal(2, changedCount);
        Assert.Equal("Hello World", textBox.Text);
    }
}
