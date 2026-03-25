---
title: "SDK 使用指南"
description: "WinForms、WPF、Win32 与无边框窗口的完整 Lumina SDK 集成示例。"
permalink: /zh/blog/sdk-guide/
lang: "zh-CN"
nav_key: "blog"
---

# SDK 使用指南

## EffectKind — 效果类型

```csharp
public enum EffectKind
{
    None,      // 移除效果
    Acrylic,   // 亚克力模糊
    Mica,      // Mica 材质（Windows 11+）
    MicaAlt,   // Mica Alt（Windows 11 22H2+）
    Aero,      // Aero 玻璃（高级模式）
    Blur,      // 自定义模糊
}
```

## WinForms

```csharp
using Lumina.WinForms;

public class MainForm : Form
{
    public MainForm()
    {
        this.SetMica();   // 或 SetAcrylic / SetAero / SetBlur
    }
}
```

带参数：

```csharp
this.SetEffect(EffectKind.Acrylic, new EffectOptions
{
    BlurRadius = 40,
    BlendColor = 0x60_10_10_30,
    Opacity    = 0.85f
});
```

## WPF

XAML：

```xml
<Window xmlns:lumina="clr-namespace:Lumina.Wpf;assembly=Lumina.Wpf"
        lumina:LuminaWindow.Effect="Mica">
```

代码后置：

```csharp
using Lumina;
using Lumina.Wpf;

var hwnd = new WindowInteropHelper(this).Handle;
LuminaWindow.SetEffect(hwnd, EffectKind.MicaAlt);
```

## Win32 原生

```csharp
using Lumina;

nint hwnd = /* GetForegroundWindow() 或其他方式获取 */;
LuminaWindow.SetEffect(hwnd, EffectKind.Mica, EffectOptions.Default);
```

## 无边框窗口

```csharp
// 先移除标题栏
this.FormBorderStyle = FormBorderStyle.None;
// 再应用效果
this.SetBlur(new EffectOptions { BlurRadius = 20 });
```

## EffectProfile — 配置持久化

```csharp
// 保存
var profile = new EffectProfile
{
    Name      = "MyPreset",
    Kind      = EffectKind.Acrylic,
    BlurRadius = 30,
    BlendColor = 0x80_00_00_00,
    Opacity    = 0.8f
};
profile.SaveJson(@"C:\preset.json");

// 加载
var loaded = EffectProfile.LoadJson(@"C:\preset.json");
LuminaWindow.SetEffect(hwnd, loaded.Kind, loaded.ToOptions());
```

## 续读

- [效果参数详解](/zh/blog/effect-options/)
- [高级模式指南](/zh/blog/advanced-guide/)
- [兼容性说明](/zh/compatibility/)
