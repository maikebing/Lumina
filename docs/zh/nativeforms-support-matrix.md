---
title: "NativeForms 支持矩阵"
permalink: /zh/nativeforms-support-matrix/
layout: default
---

# NativeForms 支持矩阵

## 控件与基础 API

| 类型 | 状态 | 说明 |
| --- | --- | --- |
| `Application` | 已支持 | 消息循环、视觉初始化、主题加载、`Exit()` |
| `ApplicationConfiguration` | 已支持 | WinForms 风格启动入口 |
| `Form` | 已支持 | 窗口创建、布局、效果、主题、自动缩放 |
| `Control` | 已支持 | 文本、位置、尺寸、可见、可用、`Tag`、`TabIndex`、`IDisposable` |
| `Button` | 已支持 | `Click`、`PerformClick()` |
| `Label` | 已支持 | 静态文本 |
| `TextBox` | 已支持 | 单行、多行、只读、`AppendText()`、`TextChanged` |
| `ComboBox` | 已支持 | `Items`、`SelectedIndex`、`SelectedItem`、`AddRange(...)` |
| `CheckBox` | 已支持 | `Checked`、`CheckedChanged` |
| `RadioButton` | 已支持 | `Checked`、`CheckedChanged` |
| `ListBox` | 已支持 | `Items`、`SelectedIndex`、`SelectedItem`、`AddRange(...)` |
| `GroupBox` | 已支持 | 分组容器、支持子控件 |
| `Panel` | 已支持 | 简单容器、支持子控件 |

## 视觉与主题

| 能力 | 状态 | 说明 |
| --- | --- | --- |
| `Application.EnableVisualStyles()` | 已支持 | 根据系统版本解析默认 `VisualStyleKind` |
| 系统浅色 / 深色跟随 | 已支持 | 默认读取 Windows 主题偏好 |
| 应用级默认覆盖 | 已支持 | `ConfigureVisualStyles(...)` |
| 窗口级默认覆盖 | 已支持 | `UseTheme(...)`、`SetThemeMode(...)`、`SetPalette(...)` |
| 默认效果策略 | 已支持 | `Mica` / `Blur` / `Aero` / `None` 按系统解析 |
| JSON 主题文件 | 已支持 | `NativeTheme.LoadJson(...)`、`SaveJson(...)` |
| 语义调色板 | 已支持 | `ThemePalette` 覆盖窗口、标题栏、表面、控件、状态色 |

## 迁移友好能力

| 项目 | 状态 | 说明 |
| --- | --- | --- |
| `Application.Run(form)` 分析器 | 已支持 | `LNF001` ~ `LNF004` |
| `partial Form` 约束 | 已支持 | 分析器提示保持 WinForms 习惯 |
| `Controls.Find(...)` | 已支持 | 支持按 `Name` 递归查找 |
| 容器嵌套自动缩放 | 已支持 | `Form`、`GroupBox`、`Panel` |
| `net10.0` + Native AOT 路径 | 已支持 | NativeForms 本体保持 AOT 友好 |
| VS 设计器 | 边界明确 | 由 Demo 的 `net10.0-windows` WinForms 路径承担 |

## 当前边界

- Owner-draw、自定义绘制和复杂视觉着色仍在继续补齐。
- 更复杂的高级控件族仍会继续扩展。
- Demo 的界面与设计器文件现在由手工维护，不再自动改动。
