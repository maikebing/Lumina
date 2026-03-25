namespace Lumina.App.GUI;

/// <summary>
/// 排除窗口列表对话框（Phase 4 占位实现，需 IPC 通道支持）。
/// </summary>
internal static class ExclusionList
{
    /// <summary>显示排除列表窗口。</summary>
    internal static void Show(nint ownerHwnd)
    {
        // TODO(Phase 4): 实现排除列表 UI，通过 IPC 将列表推送到 Lumina.Ext
        _ = ownerHwnd;
    }
}
