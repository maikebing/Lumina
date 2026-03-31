---
title: "LuminaForms 兼容差异清单"
permalink: /zh/nativeforms-compatibility/
layout: default
---

# LuminaForms 兼容差异清单

## 当前已经对齐的方向

- WinForms 风格启动入口：`ApplicationConfiguration.Initialize()`
- WinForms 风格顶层窗口：`Form`
- 常用标准控件：`Button`、`Label`、`TextBox`、`ComboBox`、`CheckBox`、`RadioButton`、`ListBox`
- 基础容器层级：`Form`、`GroupBox`、`Panel`
- 常见集合操作：`Controls.AddRange(...)`、`Controls.Find(...)`、`Items.AddRange(...)`
- 常见选择 API：`SelectedIndex`、`SelectedItem`
- 常见布局属性：`Left`、`Top`、`Width`、`Height`、`Location`、`Size`、`Bounds`
- WinForms 风格自动缩放：`AutoScaleMode`、`AutoScaleDimensions`、`CurrentAutoScaleDimensions`、`PerformAutoScale()`
- WinForms 风格启动约束分析器：`LNF001` ~ `LNF004`

## 当前仍在继续补齐的方向

- 更复杂的公共控件族
- 更完整的设计期元数据
- Owner-draw、自定义绘制和更深入的主题着色
- 更复杂的高级容器、数据绑定和消息路由

## 迁移建议

1. 先保留 `net10.0-windows` 的 WinForms 路径，保证设计器和旧行为可以回退。
2. 再让 `net10.0` 切到 `Lumina.Forms`，优先验证高频窗体和高频控件。
3. 对尚未覆盖的高级能力，先收敛差异面，再决定是继续补齐 LuminaForms，还是暂时保留 WinForms 路径。
