# Lumina — Quick Start

## 安装

```shell
dotnet add package Lumina.Core
# WinForms 项目额外安装：
dotnet add package Lumina.WinForms
# WPF 项目额外安装：
dotnet add package Lumina.Wpf
# 高级模式（需管理员权限）：
dotnet add package Lumina.Core.Advanced
```

---

## 标准模式示例

### WinForms

```csharp
using Lumina.WinForms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        // 一行代码启用 Mica 效果
        this.SetMica();
    }
}
```

### WPF

```xml
<Window xmlns:lumina="clr-namespace:Lumina.Wpf;assembly=Lumina.Wpf"
        lumina:Effect.Kind="Mica">
    ...
</Window>
```

### 原生 Win32

```csharp
using Lumina;

// 直接传入 HWND
LuminaWindow.SetEffect(hwnd, EffectKind.Mica);

// 自定义参数
LuminaWindow.SetEffect(hwnd, EffectKind.Acrylic, new EffectOptions
{
    BlendColor = 0xCC_00_00_00,
    BlurRadius = 30,
    Opacity    = 0.9f,
});

// 移除效果
LuminaWindow.Clear(hwnd);
```

### 无边框窗口

```csharp
using Lumina;

// 同原生 Win32，Lumina 自动处理 WM_NCCALCSIZE / WM_NCHITTEST
LuminaWindow.SetEffect(hwnd, EffectKind.Mica);
```

---

## 高级模式（管理员权限）

```csharp
using Lumina.Advanced;

// 检查权限
if (!LuminaAdvanced.IsElevated)
{
    Console.Error.WriteLine("需要管理员权限");
    return;
}

// 注入 dwm.exe，解锁深度模糊 / Aero 效果
LuminaAdvanced.Inject();

// 设置自定义模糊半径和混合色
LuminaAdvanced.SetBlur(radius: 30, blendColor: 0x80_00_00_00);

// 退出时卸载
LuminaAdvanced.Eject();
```

---

## 配置持久化（SDK）

```csharp
using Lumina;

var profile = new EffectProfile
{
    Name       = "我的配置",
    Kind       = EffectKind.Acrylic,
    BlendColor = 0x80_10_10_10,
    BlurRadius = 25,
};

// 保存
profile.SaveJson("lumina-profile.json");
profile.SaveXml("lumina-profile.xml");

// 加载并应用
var loaded = EffectProfile.LoadJson("lumina-profile.json");
LuminaWindow.SetEffect(hwnd, loaded.Kind, loaded.ToOptions());
```

---

## 效果类型对照

| EffectKind | 说明 | 最低系统要求 |
| --- | --- | --- |
| `None` | 移除所有效果 | 任意 |
| `Blur` | 自定义模糊 | Windows 10 |
| `Acrylic` | 亚克力模糊 | Windows 10 1803+ |
| `Aero` | 经典 Aero 玻璃 | Windows 10（高级模式）|
| `Mica` | Mica 材质 | Windows 11 |
| `MicaAlt` | Mica Alt 材质 | Windows 11 22H2+ |
