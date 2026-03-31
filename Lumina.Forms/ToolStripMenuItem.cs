namespace Lumina.Forms;

/// <summary>
/// Represents a selectable menu command item.
/// </summary>
public class ToolStripMenuItem : ToolStripDropDownItem
{
	/// <summary>
	/// Gets or sets the shortcut keys associated with the menu item.
	/// </summary>
	public Keys ShortcutKeys { get; set; }

	/// <summary>
	/// Gets or sets the explicit shortcut text displayed in the menu.
	/// </summary>
	public string ShortcutKeyDisplayString { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets a value indicating whether shortcut text should be displayed.
	/// </summary>
	public bool ShowShortcutKeys { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether the menu item is checked.
	/// </summary>
	public bool Checked { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the checked state toggles when the item is clicked.
	/// </summary>
	public bool CheckOnClick { get; set; }

	internal bool MatchesShortcut(Keys keyData)
	{
		return NormalizeShortcut(ShortcutKeys) != Keys.None
			&& NormalizeShortcut(ShortcutKeys) == NormalizeShortcut(keyData);
	}

	internal string GetShortcutDisplayText()
	{
		if (!ShowShortcutKeys)
		{
			return string.Empty;
		}

		if (!string.IsNullOrWhiteSpace(ShortcutKeyDisplayString))
		{
			return ShortcutKeyDisplayString;
		}

		return FormatShortcutKeys(ShortcutKeys);
	}

	/// <inheritdoc />
	protected override void OnClick(EventArgs e)
	{
		if (CheckOnClick)
		{
			Checked = !Checked;
		}

		base.OnClick(e);
	}

	private static Keys NormalizeShortcut(Keys keyData)
	{
		return keyData & (Keys.KeyCode | Keys.Modifiers);
	}

	private static string FormatShortcutKeys(Keys shortcutKeys)
	{
		Keys normalized = NormalizeShortcut(shortcutKeys);
		if (normalized == Keys.None)
		{
			return string.Empty;
		}

		List<string> parts = [];
		if (normalized.HasFlag(Keys.Control))
		{
			parts.Add("Ctrl");
		}

		if (normalized.HasFlag(Keys.Shift))
		{
			parts.Add("Shift");
		}

		if (normalized.HasFlag(Keys.Alt))
		{
			parts.Add("Alt");
		}

		Keys keyCode = normalized & Keys.KeyCode;
		string keyText = keyCode switch
		{
			Keys.D0 => "0",
			Keys.D1 => "1",
			Keys.D2 => "2",
			Keys.D3 => "3",
			Keys.D4 => "4",
			Keys.D5 => "5",
			Keys.D6 => "6",
			Keys.D7 => "7",
			Keys.D8 => "8",
			Keys.D9 => "9",
			>= Keys.A and <= Keys.Z => keyCode.ToString(),
			>= Keys.F1 and <= Keys.F12 => keyCode.ToString(),
			_ => keyCode switch
			{
				Keys.Back => "Backspace",
				Keys.Tab => "Tab",
				Keys.Enter => "Enter",
				Keys.Escape => "Esc",
				Keys.Space => "Space",
				Keys.PageUp => "PgUp",
				Keys.PageDown => "PgDn",
				Keys.End => "End",
				Keys.Home => "Home",
				Keys.Left => "Left",
				Keys.Up => "Up",
				Keys.Right => "Right",
				Keys.Down => "Down",
				Keys.Insert => "Ins",
				Keys.Delete => "Del",
				_ => string.Empty,
			},
		};

		if (!string.IsNullOrWhiteSpace(keyText))
		{
			parts.Add(keyText);
		}

		return string.Join("+", parts);
	}
}