namespace Lumina.NativeForms;

/// <summary>
/// Specifies how a NativeForms window should choose its light or dark appearance.
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
    Light,

    /// <summary>
    /// Force dark appearance.
    /// </summary>
    Dark,
}
