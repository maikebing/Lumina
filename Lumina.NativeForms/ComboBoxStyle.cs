namespace Lumina.NativeForms;

/// <summary>
/// Specifies the behavior of a NativeForms combo box.
/// </summary>
public enum ComboBoxStyle
{
    /// <summary>
    /// The list is always visible and the edit portion is editable.
    /// </summary>
    Simple = 1,

    /// <summary>
    /// The list is shown on demand and the edit portion is editable.
    /// </summary>
    DropDown = 2,

    /// <summary>
    /// The list is shown on demand and selection is limited to list items.
    /// </summary>
    DropDownList = 3,
}
