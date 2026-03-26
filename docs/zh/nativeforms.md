---
title: "NativeForms 概览"
permalink: /zh/nativeforms/
layout: default
---

# NativeForms 概览

`Lumina.NativeForms` 的目标是尽量贴近 WinForms 的命名、属性、方法和事件模型，让旧项目迁移时尽量只改引用和命名空间。

## 当前能力

- 顶层窗口与消息循环：`Application`、`Form`
- 基础控件：`Button`、`Label`、`TextBox`、`ComboBox`、`CheckBox`、`GroupBox`
- 新增高频控件：`RadioButton`、`ListBox`
- 应用级默认视觉入口：`Application.EnableVisualStyles()`
- 系统主题跟随：浅色 / 深色
- 应用级与窗口级效果覆盖：`ConfigureVisualStyles`、`SetEffect`、`SetThemeMode`
- JSON 主题文件：`NativeTheme`、`ThemePalette`

## Demo 结构

`Lumina.NativeForms.Demo` 现在是双目标工程：

- `net10.0-windows`：WinForms 版本，面向 Visual Studio 设计器
- `net10.0`：NativeForms 版本，面向 Native AOT

项目文件里使用 `UseWinForms` 和 `UseNativeForms` 区分两条路径，便于后续做条件编译和迁移验证。

## 默认视觉策略

调用 `Application.EnableVisualStyles()` 后，NativeForms 会根据当前系统自动选择默认效果：

- Windows 7：`Aero`
- Windows 8 / 8.1：`None`
- Windows 10：`Blur`
- Windows 11：`Mica`

如果应用提供了主题文件或明确的覆盖设置，则优先使用应用配置。
