---
title: "LuminaForms 概览"
permalink: /zh/nativeforms/
layout: default
---

# LuminaForms 概览

LuminaForms 通过 `Lumina.Forms` 包和命名空间提供 WinForms 平替路径，目标不是重新发明一套桌面 UI，而是尽量贴近 WinForms 的命名、属性、方法、事件和启动方式，让旧项目可以把迁移成本集中在引用切换和少量兼容差异上。

## 当前能力

- 顶层窗口与消息循环：`Application`、`ApplicationConfiguration`、`Form`
- 基础控件：`Button`、`Label`、`TextBox`、`ComboBox`、`CheckBox`、`RadioButton`、`ListBox`
- 基础容器：`GroupBox`、`Panel`
- WinForms 风格自动缩放：`AutoScaleMode`、`AutoScaleDimensions`、`CurrentAutoScaleDimensions`、`PerformAutoScale()`
- WinForms 风格集合操作：`Controls.AddRange(...)`、`Controls.Find(...)`、`Items.AddRange(...)`、`SelectedItem`
- WinForms 风格启动入口：`ApplicationConfiguration.Initialize()`
- 应用级视觉入口：`Application.EnableVisualStyles()`
- 应用级与窗口级视觉覆盖：`ConfigureVisualStyles(...)`、`UseTheme(...)`、`SetThemeMode(...)`、`SetEffect(...)`、`SetPalette(...)`
- JSON 主题文件：`NativeTheme`、`ThemePalette`
- 分析器规则：`Lumina.Forms.Analyzers`

## 默认视觉策略

调用 `Application.EnableVisualStyles()` 后，LuminaForms 会根据当前操作系统选择最接近系统的默认风格：

- Windows 7：`VisualStyleKind.AeroGlass`
- Windows 8 / 8.1：`VisualStyleKind.Modern`
- Windows 10：`VisualStyleKind.Fluent`
- Windows 11：`VisualStyleKind.Mica`

`ThemeMode.System` 会优先跟随系统浅色 / 深色偏好。应用级设置更新后，已经打开的 LuminaForms 窗口也会刷新默认视觉样式。

## Demo 约束

`NativeFormsDemo` 当前保持固定双目标结构：

- `net10.0-windows`：WinForms 路径，保持 Visual Studio 设计器友好
- `net10.0`：LuminaForms 路径，用于 Native AOT 发布

这个 Demo 的项目结构现在视为迁移验证边界，不再自动改成其他形态。
