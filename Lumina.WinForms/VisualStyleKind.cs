namespace Lumina.NativeForms;

/// <summary>
/// Describes the system-aligned visual style family that NativeForms should emulate by default.
/// </summary>
public enum VisualStyleKind
{
    /// <summary>
    /// Let NativeForms choose the most appropriate style for the current operating system.
    /// </summary>
    System = 0,

    /// <summary>
    /// Use a classic non-material desktop appearance with no Lumina backdrop effect.
    /// </summary>
    Classic = 1,

    /// <summary>
    /// Use an Aero Glass inspired style suitable for Windows 7-era desktop chrome.
    /// </summary>
    AeroGlass = 2,

    /// <summary>
    /// Use a flatter Windows 8 / 8.1 style with minimal backdrop material.
    /// </summary>
    Modern = 3,

    /// <summary>
    /// Use a Windows 10 style centered around Fluent-era blur materials.
    /// </summary>
    Fluent = 4,

    /// <summary>
    /// Use a Windows 11 style centered around Mica materials.
    /// </summary>
    Mica = 5,
}
