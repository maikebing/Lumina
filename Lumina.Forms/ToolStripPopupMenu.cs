using System.Drawing;
using System.Threading;

namespace Lumina.Forms;

internal static class ToolStripPopupMenu
{
    private static int s_nextCommandId = 30000;

    internal static void Show(ToolStripItemCollection items, nint ownerHandle, Point screenLocation)
    {
        if (ownerHandle == 0)
        {
            return;
        }

        nint menuHandle = Win32.CreatePopupMenu();
        if (menuHandle == 0)
        {
            return;
        }

        var commands = new Dictionary<uint, ToolStripItem>();
        try
        {
            PopulateMenu(menuHandle, items, commands);
            _ = Win32.SetForegroundWindow(ownerHandle);

            uint command = Win32.TrackPopupMenu(
                menuHandle,
                Win32.TPM_RIGHTBUTTON | Win32.TPM_RETURNCMD,
                screenLocation.X,
                screenLocation.Y,
                0,
                ownerHandle,
                0);

            if (command != 0 && commands.TryGetValue(command, out ToolStripItem? item))
            {
                item.PerformClick();
            }
        }
        finally
        {
            _ = Win32.DestroyMenu(menuHandle);
        }
    }

    private static void PopulateMenu(nint menuHandle, IEnumerable<ToolStripItem> items, Dictionary<uint, ToolStripItem> commands)
    {
        foreach (ToolStripItem item in items)
        {
            if (!item.Visible)
            {
                continue;
            }

            if (item is ToolStripSeparator)
            {
                _ = Win32.AppendMenuW(menuHandle, Win32.MF_SEPARATOR, 0, null);
                continue;
            }

            if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0)
            {
                nint subMenuHandle = Win32.CreatePopupMenu();
                if (subMenuHandle == 0)
                {
                    continue;
                }

                PopulateMenu(subMenuHandle, dropDownItem.DropDownItems, commands);
                uint flags = Win32.MF_POPUP | Win32.MF_STRING;
                if (!item.Enabled)
                {
                    flags |= Win32.MF_GRAYED;
                }

                if (item is ToolStripMenuItem { Checked: true })
                {
                    flags |= Win32.MF_CHECKED;
                }

                _ = Win32.AppendMenuW(menuHandle, flags, unchecked((nuint)subMenuHandle), GetDisplayText(item));
                continue;
            }

            uint commandId = unchecked((uint)Interlocked.Increment(ref s_nextCommandId));
            commands[commandId] = item;
            uint itemFlags = Win32.MF_STRING;
            if (!item.Enabled || item is ToolStripComboBox || item is ToolStripTextBox || item is ToolStripProgressBar)
            {
                itemFlags |= Win32.MF_GRAYED;
            }

            if (item is ToolStripMenuItem { Checked: true })
            {
                itemFlags |= Win32.MF_CHECKED;
            }

            _ = Win32.AppendMenuW(menuHandle, itemFlags, commandId, GetDisplayText(item));
        }
    }

    private static string GetDisplayText(ToolStripItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Text))
        {
            return item.Text;
        }

        if (item is ToolStripComboBox comboBox && comboBox.SelectedIndex >= 0 && comboBox.SelectedIndex < comboBox.Items.Count)
        {
            return comboBox.Items[comboBox.SelectedIndex]?.ToString() ?? comboBox.Name;
        }

        if (item is ToolStripProgressBar progressBar)
        {
            return $"{progressBar.Name} {progressBar.Value}".Trim();
        }

        return string.IsNullOrWhiteSpace(item.Name)
            ? item.GetType().Name
            : item.Name;
    }
}