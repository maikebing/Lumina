using System;

namespace Lumina.Forms;

[Flags]
public enum AnchorStyles
{
    None = 0,
    Top = 1,
    Bottom = 2,
    Left = 4,
    Right = 8
}

public enum AutoSizeMode
{
    GrowOnly = 0,
    GrowAndShrink = 1
}

public enum CheckState
{
    Unchecked = 0,
    Checked = 1,
    Indeterminate = 2
}

public enum CloseReason
{
    None = 0,
    UserClosing = 1,
    ApplicationExitCall = 2,
    FormOwnerClosing = 3
}

public enum FormStartPosition
{
    Manual = 0,
    CenterScreen = 1
}

public enum FormWindowState
{
    Normal = 0,
    Minimized = 1,
    Maximized = 2
}

public enum HighDpiMode
{
    SystemAware = 0,
    PerMonitor = 1,
    PerMonitorV2 = 2,
    DpiUnaware = 3
}

public enum HorizontalAlignment
{
    Left = 0,
    Center = 1,
    Right = 2
}

public enum ScrollBars
{
    None = 0,
    Horizontal = 1,
    Vertical = 2,
    Both = 3
}
