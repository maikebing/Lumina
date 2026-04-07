namespace Lumina.Forms;

internal static class MenuRenderingModeResolver
{
    internal static MenuRenderingMode ResolveForPopupMenus()
    {
        DarkModeCapabilities.Snapshot capabilities = DarkModeCapabilities.Current;
        if (!capabilities.IsWindows || capabilities.IsLegacyWindows)
        {
            return MenuRenderingMode.Classic;
        }

        if (capabilities.SupportsImmersivePopupMenus)
        {
            return Win32DarkModeApi.IsSupported
                ? MenuRenderingMode.ImmersivePopup
                : MenuRenderingMode.Classic;
        }

        return MenuRenderingMode.Classic;
    }
}
