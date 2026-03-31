namespace Lumina.Forms;

#pragma warning disable CS1591

/// <summary>
/// Specifies keyboard keys and modifier combinations used by menu shortcuts.
/// </summary>
[Flags]
public enum Keys
{
    /// <summary>
    /// No key is specified.
    /// </summary>
    None = 0x00000000,

    /// <summary>
    /// The SHIFT modifier key.
    /// </summary>
    Shift = 0x00010000,

    /// <summary>
    /// The CTRL modifier key.
    /// </summary>
    Control = 0x00020000,

    /// <summary>
    /// The ALT modifier key.
    /// </summary>
    Alt = 0x00040000,

    /// <summary>
    /// The key-code mask.
    /// </summary>
    KeyCode = 0x0000FFFF,

    /// <summary>
    /// The modifier mask.
    /// </summary>
    Modifiers = unchecked((int)0xFFFF0000),

    Back = 0x0008,
    Tab = 0x0009,
    Enter = 0x000D,
    Escape = 0x001B,
    Space = 0x0020,
    PageUp = 0x0021,
    PageDown = 0x0022,
    End = 0x0023,
    Home = 0x0024,
    Left = 0x0025,
    Up = 0x0026,
    Right = 0x0027,
    Down = 0x0028,
    Insert = 0x002D,
    Delete = 0x002E,
    D0 = 0x0030,
    D1 = 0x0031,
    D2 = 0x0032,
    D3 = 0x0033,
    D4 = 0x0034,
    D5 = 0x0035,
    D6 = 0x0036,
    D7 = 0x0037,
    D8 = 0x0038,
    D9 = 0x0039,
    A = 0x0041,
    B = 0x0042,
    C = 0x0043,
    D = 0x0044,
    E = 0x0045,
    F = 0x0046,
    G = 0x0047,
    H = 0x0048,
    I = 0x0049,
    J = 0x004A,
    K = 0x004B,
    L = 0x004C,
    M = 0x004D,
    N = 0x004E,
    O = 0x004F,
    P = 0x0050,
    Q = 0x0051,
    R = 0x0052,
    S = 0x0053,
    T = 0x0054,
    U = 0x0055,
    V = 0x0056,
    W = 0x0057,
    X = 0x0058,
    Y = 0x0059,
    Z = 0x005A,
    F1 = 0x0070,
    F2 = 0x0071,
    F3 = 0x0072,
    F4 = 0x0073,
    F5 = 0x0074,
    F6 = 0x0075,
    F7 = 0x0076,
    F8 = 0x0077,
    F9 = 0x0078,
    F10 = 0x0079,
    F11 = 0x007A,
    F12 = 0x007B,
}

#pragma warning restore CS1591