---
title: "NativeForms 支持矩阵"
permalink: /zh/nativeforms-support-matrix/
layout: default
---

# NativeForms 支持矩阵

## 已支持控件

| 类型 | 状态 | 说明 |
| --- | --- | --- |
| `Application` | 已支持 | 提供消息循环、视觉样式初始化、主题加载 |
| `Form` | 已支持 | 提供窗口创建、布局、效果、主题覆盖 |
| `Control` | 已支持 | 提供基础属性、句柄、位置、尺寸、可见性 |
| `Button` | 已支持 | `Click` |
| `Label` | 已支持 | 静态文本 |
| `TextBox` | 已支持 | 单行、多行、只读、`AppendText` |
| `ComboBox` | 已支持 | `Items`、`SelectedIndex`、`SelectedIndexChanged` |
| `CheckBox` | 已支持 | `Checked`、`CheckedChanged` |
| `RadioButton` | 已支持 | `Checked`、`CheckedChanged` |
| `ListBox` | 已支持 | `Items`、`SelectedIndex`、`SelectedIndexChanged` |
| `GroupBox` | 已支持 | 分组外观 |

## 已支持视觉能力

| 能力 | 状态 | 说明 |
| --- | --- | --- |
| `Application.EnableVisualStyles()` | 已支持 | 按系统版本选择默认效果 |
| 系统浅色 / 深色跟随 | 已支持 | 自动读取 Windows 主题偏好 |
| 应用级默认覆盖 | 已支持 | `ConfigureVisualStyles` |
| 窗口级覆盖 | 已支持 | `SetEffect`、`SetThemeMode` |
| JSON 主题文件 | 已支持 | `NativeTheme`、`ThemePalette` |

## 当前差异说明

| 项目 | 当前状态 |
| --- | --- |
| Visual Studio 设计器 | WinForms 目标友好，NativeForms 目标本身不提供设计器 |
| 复杂容器嵌套 | 仍在继续补齐 |
| Owner-draw / 深度自绘 | 仍在继续补齐 |
| 原生公共控件全量着色 | 目前以系统主题 + 语义色板为主 |
