---
title: "NativeForms 迁移指南"
permalink: /zh/nativeforms-migration/
layout: default
---

# NativeForms 迁移指南

## 迁移目标

目标不是重新设计旧应用，而是尽量保持 WinForms 的 API 使用习惯，让迁移成本集中在工程切换上。

## 推荐迁移顺序

1. 保留原来的窗体拆分方式，先不要动业务逻辑。
2. 把项目改成双目标：`net10.0-windows` 和 `net10.0`。
3. `net10.0-windows` 继续引用 WinForms，保证设计器可用。
4. `net10.0` 改为引用 `Lumina.NativeForms`，逐步替换命名空间。
5. 先验证事件和属性行为，再处理不兼容能力。

## 项目文件示例

```xml
<PropertyGroup>
  <TargetFrameworks>net10.0-windows;net10.0</TargetFrameworks>
  <UseWinForms>false</UseWinForms>
  <UseNativeForms>false</UseNativeForms>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-windows'">
  <UseWinForms>true</UseWinForms>
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0'">
  <UseNativeForms>true</UseNativeForms>
</PropertyGroup>
```

## 命名空间切换示例

WinForms：

```csharp
using System.Windows.Forms;
```

NativeForms：

```csharp
using Lumina.NativeForms;
```

## 兼容性边界

- 当前优先覆盖的是高频标准控件与标准事件。
- NativeForms 仍然基于原生 Win32 控件，复杂 owner-draw、嵌套容器和设计期元数据还在继续补齐。
- 对于暂未兼容的能力，建议先在 `net10.0-windows` 目标保留 WinForms 路径，同时在 `net10.0` 目标逐步收缩差异面。
