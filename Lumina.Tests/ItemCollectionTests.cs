using Lumina.Forms;
using Xunit;

namespace Lumina.Tests;

public class ItemCollectionTests
{
    [Fact]
    public void ComboBoxItemsAddRange_PreservesSelectedIndexValue()
    {
        var comboBox = new ComboBox();
        comboBox.Items.AddRange(["COM1", "COM2", "COM3"]);
        comboBox.SelectedIndex = 1;

        Assert.Equal(1, comboBox.SelectedIndex);
    }

    [Fact]
    public void ListBoxItemsAddRange_PreservesSelectedIndexValue()
    {
        var listBox = new ListBox();
        listBox.Items.AddRange(["One", "Two", "Three"]);
        listBox.SelectedIndex = 2;

        Assert.Equal(2, listBox.SelectedIndex);
    }

    [Fact]
    public void ComboBoxItems_SupportObjectValuesAndSelectedItem()
    {
        var comboBox = new ComboBox();
        comboBox.Items.AddRange("COM1", 2, "COM3");

        comboBox.SelectedItem = 2;

        Assert.Equal(3, comboBox.Items.Count);
        Assert.Equal(1, comboBox.SelectedIndex);
        Assert.Equal(2, comboBox.SelectedItem);
        Assert.Equal("2", comboBox.Items[1]!.ToString());
    }

    [Fact]
    public void ListBoxItems_SupportObjectValuesAndSelectedItem()
    {
        var listBox = new ListBox();
        listBox.Items.AddRange("One", 2, "Three");

        listBox.SelectedItem = "Three";

        Assert.Equal(3, listBox.Items.Count);
        Assert.Equal(2, listBox.SelectedIndex);
        Assert.Equal("Three", listBox.SelectedItem);
        Assert.Equal("2", listBox.Items[1]!.ToString());
    }
}
