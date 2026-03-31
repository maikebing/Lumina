using System.Windows;
using System.Windows.Interop;

namespace Lumina.Wpf;

/// <summary>
/// WPF 附加属性，在 XAML 中声明式启用 Lumina 效果。
/// <code>
/// &lt;Window lumina:Effect.Kind="Mica" /&gt;
/// </code>
/// </summary>
public static class Effect
{
    /// <summary>要应用的效果类型附加属性。</summary>
    public static readonly DependencyProperty KindProperty =
        DependencyProperty.RegisterAttached(
            "Kind",
            typeof(EffectKind),
            typeof(Effect),
            new PropertyMetadata(EffectKind.None, OnKindChanged));

    /// <summary>混合色附加属性，格式 0xAARRGGBB。</summary>
    public static readonly DependencyProperty BlendColorProperty =
        DependencyProperty.RegisterAttached(
            "BlendColor",
            typeof(uint),
            typeof(Effect),
            new PropertyMetadata(0x80_00_00_00u, OnKindChanged));

    /// <summary>获取 <see cref="KindProperty"/> 附加属性值。</summary>
    public static EffectKind GetKind(DependencyObject obj)
        => (EffectKind)obj.GetValue(KindProperty);

    /// <summary>设置 <see cref="KindProperty"/> 附加属性值。</summary>
    public static void SetKind(DependencyObject obj, EffectKind value)
        => obj.SetValue(KindProperty, value);

    /// <summary>获取 <see cref="BlendColorProperty"/> 附加属性值。</summary>
    public static uint GetBlendColor(DependencyObject obj)
        => (uint)obj.GetValue(BlendColorProperty);

    /// <summary>设置 <see cref="BlendColorProperty"/> 附加属性值。</summary>
    public static void SetBlendColor(DependencyObject obj, uint value)
        => obj.SetValue(BlendColorProperty, value);

    private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Window window) return;

        // 窗口可能尚未加载，延迟到 Loaded 事件
        if (!window.IsLoaded)
        {
            window.Loaded -= OnWindowLoaded;
            window.Loaded += OnWindowLoaded;
            return;
        }

        ApplyEffect(window);
    }

    private static void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Window window)
        {
            window.Loaded -= OnWindowLoaded;
            ApplyEffect(window);
        }
    }

    private static void ApplyEffect(Window window)
    {
        nint hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == 0) return;

        var kind  = GetKind(window);
        var color = GetBlendColor(window);
        LuminaWindow.SetEffect(hwnd, kind, new EffectOptions { BlendColor = color });
    }
}
