---
title: "LuminaForms 主题与 JSON 配置"
permalink: /zh/nativeforms-themes/
layout: default
---

# LuminaForms 主题与 JSON 配置

## 目标

LuminaForms 的主题系统优先解决三件事：

- 跟随系统浅色 / 深色
- 根据操作系统版本选择最接近系统的默认视觉风格
- 为用户主题和 AI 生成主题预留稳定的 JSON 文件格式

## 核心类型

- `ApplicationVisualStyleSettings`
- `ResolvedVisualStyle`
- `NativeTheme`
- `ThemePalette`
- `ThemeMode`
- `VisualStyleKind`

## 调色板组织方式

`ThemePalette` 采用“窗口 / 标题栏 / 表面 / 控件 / 状态色”的语义分层，而不是只暴露零散颜色值。这个组织思路参考了 `SkiaShell.Theme` 里按区域和状态分组 palette 的方式，方便后续继续扩展主题文件、Owner-draw 控件和 AI 生成模板。

当前主要 token 包括：

- 窗口层：`WindowBackground`、`WindowForeground`、`WindowBorder`
- 标题栏：`TitleBarBackground`、`TitleBarForeground`、`TitleBarBorder`
- 表面层：`SurfaceBackground`、`SurfaceForeground`、`SurfaceBorder`
- 控件层：`ControlBackground`、`ControlForeground`、`ControlBorder`
- 状态层：`ControlHoverBackground`、`ControlPressedBackground`、`Selection`、`FocusBorder`
- 语义色：`Accent`、`Success`、`Warning`、`Danger`

## 代码示例

```csharp
using Lumina.Forms;

ApplicationConfiguration.Initialize();
Application.ConfigureVisualStyles(settings =>
{
    settings.ThemeMode = ThemeMode.System;
    settings.PreferredVisualStyle = VisualStyleKind.System;
    settings.ApplyBackdropEffects = true;
});

using var form = new MainForm();
form.UseTheme(NativeTheme.CreateDarkTheme());
form.SetPalette(new ThemePalette
{
    Accent = 0xFFFF7A00,
    FocusBorder = 0xFFFF7A00,
});

Application.Run(form);
```

## JSON 示例

```json
{
  "Name": "Lumina Native Dark",
  "Description": "Built-in dark palette for LuminaForms.",
  "Author": "Lumina",
  "ThemeMode": 2,
  "PreferredVisualStyle": 5,
  "PreferredEffect": 2,
  "Palette": {
    "WindowBackground": 4280295460,
    "WindowForeground": 4294179831,
    "WindowBorder": 4281940288,
    "TitleBarBackground": 3860865060,
    "TitleBarForeground": 4294179831,
    "ControlBackground": 4281284405,
    "ControlBorder": 4283050576,
    "Accent": 4283210495,
    "FocusBorder": 4285970175
  }
}
```

未在 JSON 中显式写出的 token 会回落到类型初始化时的默认值，因此主题文件既可以写成完整调色板，也可以只覆盖少量关键颜色。

## 示例主题文件

仓库内置了两份示例主题文件：

- `themes/nativeforms/lumina-native-light.json`
- `themes/nativeforms/lumina-native-dark.json`
