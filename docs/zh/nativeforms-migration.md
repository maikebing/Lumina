---
title: "LuminaForms 迁移指南"
permalink: /zh/nativeforms-migration/
layout: default
---

# LuminaForms 迁移指南

## 迁移目标

迁移目标不是重写旧应用，而是尽量保持 WinForms 的写法，把改动集中在目标框架、引用、命名空间和少量兼容差异上。

## 推荐迁移顺序

1. 保留原有窗体拆分方式，先不要动业务逻辑。
2. 把项目改成双目标：`net10.0-windows` 和 `net10.0`。
3. 在 `net10.0-windows` 下继续使用 WinForms，保留 Visual Studio 设计器。
4. 在 `net10.0` 下切到 `Lumina.Forms`，用于 Native AOT。
5. 先验证高频窗口、常用控件、事件和布局，再收敛剩余兼容差异。

## 项目文件示例

```xml
<PropertyGroup>
  <TargetFrameworks>net10.0-windows;net10.0</TargetFrameworks>
  <UseWindowsForms>false</UseWindowsForms>
  <UseLuminaForms>false</UseLuminaForms>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-windows'">
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0'">
  <UseLuminaForms>true</UseLuminaForms>
</PropertyGroup>
```

## 命名空间切换

WinForms:

```csharp
using System.Windows.Forms;
```

LuminaForms:

```csharp
using Lumina.Forms;
```

## 启动入口兼容

在 `net10.0` 目标下，可以继续保留 WinForms 风格的启动写法：

```csharp
ApplicationConfiguration.Initialize();
Application.Run(new MainForm());
```

## 迁移时优先验证的 API

- 自动缩放：`AutoScaleMode`、`AutoScaleDimensions`、`PerformAutoScale()`
- 容器集合：`Controls.Add(...)`、`Controls.AddRange(...)`、`Controls.Find(...)`
- 列表集合：`Items.Add(...)`、`Items.AddRange(...)`、`SelectedIndex`、`SelectedItem`
- 窗口主题：`Application.EnableVisualStyles()`、`ConfigureVisualStyles(...)`、`UseTheme(...)`
- 常见事件：`Click`、`CheckedChanged`、`SelectedIndexChanged`、`TextChanged`

## 当前迁移边界

- 当前优先覆盖的是高频标准控件和常见窗体 API。
- LuminaForms 仍然基于原生 Win32 控件，复杂自绘、Owner-draw 和更多高级控件仍在继续补齐。
- 如果某个高级能力尚未对齐，建议继续保留 `net10.0-windows` 的 WinForms 路径，同时在 `net10.0` 路径逐步收敛差异。
