namespace Lumina.Forms;

/// <summary>
/// Specifies how a LuminaForms window should choose its light or dark appearance.
/// </summary>
public enum ThemeMode
{
    /// <summary>
    /// Follow the operating system preference.
    /// </summary>
    System = 0,

    /// <summary>
    /// Force light appearance.
    /// </summary>
    Light = 1,

    /// <summary>
    /// Force dark appearance.
    /// </summary>
    Dark = 2,
}
