---
title: "效果参数详解"
description: "BlendColor、BlurRadius、Opacity 的取值范围、格式说明与使用示例。"
permalink: /zh/blog/effect-options/
lang: "zh-CN"
nav_key: "blog"
---

# 效果参数详解

`EffectOptions` 的三个参数控制视觉效果的外观，所有字段均为可选，未设置时使用默认值。

## BlendColor — 混合叠加色

格式：`0xAARRGGBB`（ARGB，32位无符号整数）

| 字段 | 含义 | 范围 |
| --- | --- | --- |
| AA | Alpha（透明度） | 0x00（全透明）～ 0xFF（不透明） |
| RR | 红色分量 | 0x00 ～ 0xFF |
| GG | 绿色分量 | 0x00 ～ 0xFF |
| BB | 蓝色分量 | 0x00 ～ 0xFF |

默认值：`0x80_00_00_00`（半透明黑）

常用示例：

```csharp
0x80_00_00_00  // 半透明黑（默认）
0x40_FF_FF_FF  // 25% 白色叠加
0x60_1E_90_FF  // 蓝色调
0x00_00_00_00  // 完全透明（无叠加）
```

## BlurRadius — 模糊半径

范围：`0`（无模糊）～ `100`（最大模糊）

默认值：`20`

仅对 `EffectKind.Blur` 和 `EffectKind.Acrylic` 有效；Mica / MicaAlt 忽略此参数。

```csharp
new EffectOptions { BlurRadius = 0 }   // 无模糊
new EffectOptions { BlurRadius = 20 }  // 默认
new EffectOptions { BlurRadius = 60 }  // 强模糊
new EffectOptions { BlurRadius = 100 } // 最大模糊
```

## Opacity — 整体不透明度

范围：`0.0f`（全透明）～ `1.0f`（完全不透明）

默认值：`0.8f`

```csharp
new EffectOptions { Opacity = 0.5f }  // 半透明
new EffectOptions { Opacity = 0.8f }  // 默认
new EffectOptions { Opacity = 1.0f }  // 完全不透明
```

## 完整示例

```csharp
var options = new EffectOptions
{
    BlendColor = 0x60_10_10_30,  // 深蓝色调，60% 透明
    BlurRadius = 40,
    Opacity    = 0.9f
};

LuminaWindow.SetEffect(hwnd, EffectKind.Acrylic, options);
```

## EffectOptions.Default

```csharp
// 等同于：
new EffectOptions
{
    BlendColor = 0x80_00_00_00,
    BlurRadius = 20,
    Opacity    = 0.8f
};
```
