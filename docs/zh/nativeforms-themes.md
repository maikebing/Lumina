---
title: "NativeForms 主题与 JSON 配置"
permalink: /zh/nativeforms-themes/
layout: default
---

# NativeForms 主题与 JSON 配置

## 目标

主题系统先解决三件事：

- 跟随系统浅色 / 深色
- 为应用提供默认效果与主题覆盖口
- 为用户和 AI 生成主题预留文件格式与语义色板

## 核心类型

- `ApplicationVisualStyleSettings`
- `ResolvedVisualStyle`
- `NativeTheme`
- `ThemePalette`
- `ThemeMode`

## 代码示例

```csharp
using Lumina.NativeForms;

Application.EnableVisualStyles();
Application.ConfigureVisualStyles(settings =>
{
    settings.ThemeMode = ThemeMode.System;
    settings.ApplyBackdropEffects = true;
});

Application.LoadTheme("themes/lumina-native-dark.json");
```

## JSON 示例

```json
{
  "Name": "Lumina Native Dark",
  "Description": "Built-in dark palette for NativeForms.",
  "Author": "Lumina",
  "ThemeMode": 2,
  "PreferredEffect": 2,
  "Palette": {
    "WindowBackground": 4280295460,
    "WindowForeground": 4294179831,
    "Accent": 4283210495
  }
}
```

## 示例主题文件

仓库内置了两个示例主题文件：

- `themes/nativeforms/lumina-native-light.json`
- `themes/nativeforms/lumina-native-dark.json`

## 现阶段说明

当前主题文件已经可以驱动：

- 应用级主题模式
- 应用级默认效果
- 语义色板持久化

原生公共控件的深度自定义着色仍会继续演进，因此目前色板更多承担“统一主题描述”和“未来 owner-draw/custom control 扩展”的角色。
