---
title: "兼容性说明"
description: "各效果类型的 Windows 版本要求、框架支持与模式对比。"
permalink: /zh/compatibility/
lang: "zh-CN"
nav_key: "docs"
---

# 兼容性说明

## 效果 × Windows 版本

| 效果 | Windows 10 | Windows 11 | Windows 11 22H2+ |
| --- | --- | --- | --- |
| Blur（自定义模糊） | ✅ | ✅ | ✅ |
| Acrylic | ✅ | ✅ | ✅ |
| Aero 玻璃 | ✅（高级模式） | ✅（高级模式） | ✅（高级模式） |
| Mica | ❌ | ✅ | ✅ |
| MicaAlt | ❌ | ❌ | ✅ |

## 标准模式 vs 高级模式

| 能力 | 标准模式（`Lumina`） | 高级模式（`Lumina.Advanced`） |
| --- | --- | --- |
| 管理员权限 | 不需要 | 需要 |
| Mica / MicaAlt | ✅ | ✅ |
| Acrylic | ✅ | ✅ |
| Blur | ✅ | ✅ |
| Aero 玻璃 | ❌ | ✅ |
| 自定义模糊半径 | ✅ | ✅ |
| 注入 dwm.exe | ❌ | ✅ |
| 排除窗口列表 | ❌ | ✅ |

## 框架支持

| 框架 | NuGet 包 | 说明 |
| --- | --- | --- |
| WinForms | `Lumina.WinForms` | 扩展方法：`form.SetMica()` 等 |
| WPF | `Lumina.Wpf` | 附加属性 + 代码后置 |
| Win32 原生 | `Lumina` | `LuminaWindow.SetEffect(hwnd, ...)` |
| 无边框窗口 | `Lumina` | 同 Win32，需手动传入 hwnd |

## 运行时要求

- .NET 10 或更高版本
- 仅支持 x64 架构
- 目标平台：Windows（`net10.0-windows`）

## 续读

- [快速开始](/zh/getting-started/)
- [SDK 使用指南](/zh/blog/sdk-guide/)
- [高级模式指南](/zh/blog/advanced-guide/)
