using System.Runtime.InteropServices;

namespace Lumina.App.GUI;

/// <summary>
/// 系统颜色选择对话框（ChooseColorW）。
/// </summary>
internal static class ColorPicker
{
    private const uint CC_RGBINIT     = 0x01;
    private const uint CC_FULLOPEN    = 0x02;
    private const uint CC_ANYCOLOR    = 0x100;

    private static readonly uint[] s_customColors = new uint[16];

    /// <summary>
    /// 弹出系统拾色器。用户确认后将所选颜色写入 <paramref name="color"/>（ARGB 0xAARRGGBB）。
    /// </summary>
    internal static void Pick(nint ownerHwnd, ref uint color)
    {
        // ChooseColorW 使用 COLORREF（0x00BBGGRR），需要转换
        uint colorref = ToColorRef(color);

        var cc = new CHOOSECOLORW
        {
            lStructSize  = (uint)Marshal.SizeOf<CHOOSECOLORW>(),
            hwndOwner    = ownerHwnd,
            rgbResult    = colorref,
            lpCustColors = s_customColors,
            Flags        = CC_RGBINIT | CC_FULLOPEN | CC_ANYCOLOR,
        };

        if (ChooseColorW(ref cc))
            color = FromColorRef(cc.rgbResult, alpha: (byte)(color >> 24));
    }

    // COLORREF = 0x00BBGGRR,  ARGB = 0xAARRGGBB
    private static uint ToColorRef(uint argb)
    {
        byte r = (byte)(argb >> 16);
        byte g = (byte)(argb >>  8);
        byte b = (byte) argb;
        return (uint)(b << 16 | g << 8 | r);
    }

    private static uint FromColorRef(uint colorref, byte alpha)
    {
        byte r = (byte) colorref;
        byte g = (byte)(colorref >>  8);
        byte b = (byte)(colorref >> 16);
        return (uint)(alpha << 24 | r << 16 | g << 8 | b);
    }

    [DllImport("comdlg32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ChooseColorW(ref CHOOSECOLORW lpcc);

    [StructLayout(LayoutKind.Sequential)]
    private struct CHOOSECOLORW
    {
        public uint   lStructSize;
        public nint   hwndOwner;
        public nint   hInstance;
        public uint   rgbResult;
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)]
        public uint[] lpCustColors;
        public uint   Flags;
        public nint   lCustData;
        public nint   lpfnHook;
        public nint   lpTemplateName;
    }
}
