---
title: "高级模式指南"
description: "通过注入 dwm.exe 解锁 Aero 玻璃与深层模糊效果，需要管理员权限。"
permalink: /zh/blog/advanced-guide/
lang: "zh-CN"
nav_key: "blog"
---

# 高级模式指南

高级模式通过向 `dwm.exe` 注入 `Lumina.Ext.dll` 来 Hook DWM 内部函数，解锁标准模式无法实现的效果，如 Aero 玻璃反射与自定义深层模糊。

## 安装

```shell
dotnet add package Lumina.Advanced
```

## 权限检查

```csharp
using Lumina.Advanced;

if (!LuminaAdvanced.IsElevated)
{
    // 提示用户以管理员身份重启
    MessageBox.Show("高级模式需要管理员权限。");
    return;
}
```

## 注入与卸载

```csharp
// 注入 Lumina.Ext.dll 到 dwm.exe
LuminaAdvanced.Inject();

// 应用效果（注入后生效）
LuminaWindow.SetEffect(hwnd, EffectKind.Aero);

// 卸载注入（退出时调用）
LuminaAdvanced.Eject();
```

## 排除窗口

指定某些窗口不应用效果：

```csharp
// 排除
LuminaAdvanced.Exclude(hwnd);

// 检查是否已排除
bool excluded = LuminaAdvanced.IsExcluded(hwnd);

// 重新加入
LuminaAdvanced.Include(hwnd);
```

## 注意事项

- 必须以管理员身份运行宿主进程
- `Inject()` 只需调用一次，进程生命周期内有效
- 退出前务必调用 `Eject()`，避免 dwm.exe 状态异常
- Aero 玻璃效果仅在高级模式下可用
- 与标准模式 API 可以混用，但同一窗口同时只应有一个效果

## 续读

- [SDK 使用指南](/zh/blog/sdk-guide/)
- [兼容性说明](/zh/compatibility/)
