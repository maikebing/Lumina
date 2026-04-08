[English](https://github.com/maikebing/Lumina/blob/main/README.md) | [简体中文](https://github.com/maikebing/Lumina/blob/main/README.zh-CN.md)

# Lumina

> 一个原生 Windows 视觉效果工具集，同时也是 LuminaForms 的主仓库。LuminaForms 是面向 net10.0 与 Native AOT 桌面应用的 WinForms 兼容 UI 层。

[![License](https://img.shields.io/github/license/maikebing/Lumina)](https://github.com/maikebing/Lumina/blob/main/LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-blue)](https://github.com/maikebing/Lumina)
[![Native AOT](https://img.shields.io/badge/runtime-Native%20AOT-0a7b83)](https://github.com/maikebing/Lumina)

## Lumina 是什么？

Lumina 目前包含两个紧密相关的方向：

- **Lumina**：为 Win32、WinForms 和 WPF 应用提供原生 Windows Blur、Aero、Acrylic、Mica 等窗口背景与标题栏视觉效果。
- **LuminaForms**：面向 net10.0 的 WinForms 兼容层，在保持 Native AOT 友好的前提下，尽量保留熟悉的控件命名、启动方式、菜单模型、对话框、自动缩放行为和迁移体验。

与许多使用 C++ 实现的同类项目不同，Lumina 整个代码库都以 **C# 与 Native AOT** 为核心方向。目标是在不引入全新桌面 UI 框架的前提下，继续保留底层 Windows 能力、裁剪友好性和简洁部署体验。

## LuminaForms

LuminaForms 通过 **Lumina.Forms** 包和命名空间提供，是 WinForms 风格桌面应用的 AOT 友好替代层。
它保留了 **Application**、**ApplicationConfiguration**、**Form**、**Button**、**TextBox**、**ComboBox**、**CheckBox**、**RadioButton**、**ListBox**、**GroupBox**、**Panel**、**MenuStrip**、**ContextMenuStrip** 等熟悉名称，让已有应用在命名空间和项目文件改动尽量少的情况下完成迁移。

### 为什么是 LuminaForms？

- 优先保持 WinForms 兼容 API 方向，而不是重新发明一套控件模型
- **net10.0** 路径面向 Native AOT 与 Win32 原生控件
- 通过 **NativeFormsDemo** 双目标验证，同一个应用概念可同时跑在 WinForms 与 LuminaForms 上
- 重点投入迁移体验、菜单兼容性、自动缩放、主题系统，以及 WinForms 一侧的设计器友好验证

### NuGet 包清单

下表列出了仓库当前已经配置的包 ID。版本徽章反映的是仓库中的包元数据版本；下载量徽章会在首次公开发布到 NuGet 后显示真实数据。

| 图标 | 包名 | 版本 | 下载量 | 用途 |
| --- | --- | --- | --- | --- |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet 包图标" /> | **Lumina.Forms** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | 面向 net10.0 与 Native AOT 桌面应用的 WinForms 兼容 UI 层。 |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet 包图标" /> | **Lumina.Forms.Analyzers** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | LuminaForms 启动方式、使用方式和迁移规则相关的 Roslyn 分析器。 |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet 包图标" /> | **Lumina.WinForms** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | 为现有 WinForms 窗体应用 Lumina 视觉效果的扩展包。 |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet 包图标" /> | **Lumina.Wpf** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | 通过附加属性在 WPF 中声明式启用 Mica、Acrylic、Aero 和模糊效果。 |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet 包图标" /> | **Lumina.Advanced** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | 通过注入 dwm.exe 提供更深层的 Aero 与模糊自定义能力。 |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet 包图标" /> | **Lumina.Core** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | 面向 Win32、WinForms、WPF 与无边框窗口场景的核心视觉效果运行时包。 |

### 命令行安装

包发布后，可以先用下面这条命令把主 LuminaForms 包安装到项目中：

```bash
dotnet add package Lumina.Forms
```

可选配套包：

```bash
dotnet add package Lumina.Forms.Analyzers
dotnet add package Lumina.Core
dotnet add package Lumina.WinForms
dotnet add package Lumina.Wpf
dotnet add package Lumina.Advanced
```

### Native AOT 体积

以下数据来自当前 **Lumina.NotepadDemo** 在 **win-x64**、单文件 Native AOT、Release 发布配置下的实际输出：

- 可执行文件大小：**3.09 MB**
- 整个发布目录大小：**3.09 MB**
- 发布后的应用不依赖额外安装的 .NET 运行时

### Demo 目标

**NativeFormsDemo** 目前使用两个目标框架：

- **net10.0-windows**：WinForms 路径，继续保持对 Visual Studio 设计器友好
- **net10.0**：LuminaForms 路径，面向 Native AOT 与 Win32 原生控件

该 demo 同时使用 **UseWindowsForms** 和 **UseLuminaForms**，因此同一套解决方案可以同时验证 WinForms 与 LuminaForms 两侧的迁移行为。

### 主题与兼容能力

LuminaForms 当前支持：

- 通过 **Application.EnableVisualStyles()** 启用感知操作系统的默认背景效果
- 通过 **VisualStyleKind** 提供感知操作系统的视觉风格族：**Classic**、**AeroGlass**、**Modern**、**Fluent**、**Mica**
- 通过 **ApplicationConfiguration.Initialize()** 保持 WinForms 风格的启动兼容性
- 跟踪系统浅色与深色模式，并对已打开窗口实时刷新
- 通过 **Application.ConfigureVisualStyles(...)** 做应用级视觉覆盖
- 通过 **UseTheme(...)**、**SetThemeMode(...)**、**SetPalette(...)** 做窗口级覆盖
- 通过 **NativeTheme** 和语义化 **ThemePalette** token 支持 JSON 主题文件
- 在活动主题之上叠加 WinForms 风格的 **BackColor** 和 **ForeColor** 覆盖
- 支持 **Controls.AddRange(...)**、**Controls.Find(...)**、**Items.AddRange(...)**、**SelectedItem** 等 WinForms 风格集合辅助能力
- 通过 **Lumina.Forms.Analyzers** 提供兼容性分析器
- 通过 **AutoScaleMode**、**AutoScaleDimensions**、**PerformAutoScale()** 提供 WinForms 风格自动缩放

示例主题文件位于 [themes/nativeforms](https://github.com/maikebing/Lumina/tree/main/themes/nativeforms)。

### 文档

- [快速开始](https://github.com/maikebing/Lumina/blob/main/docs/quickstart.md)
- [LuminaForms 概览](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms.md)
- [LuminaForms 迁移指南](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms-migration.md)
- [LuminaForms 兼容性说明](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms-compatibility.md)
- [LuminaForms 主题指南](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms-themes.md)
- [LuminaForms 支持矩阵](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms-support-matrix.md)

## Lumina 运行时特性

- **Blur**：经典高斯模糊背景
- **Aero**：Windows 7 风格玻璃反射与视差效果
- **Acrylic**：Windows 10 风格噪声加模糊材质
- **Mica**：Windows 11 风格桌面着色材质
- **MicaAlt**：适用于标签页风格的 Mica 变体
- 支持自定义模糊半径、混合颜色和标题栏文字颜色
- 支持浅色与深色模式自动切换
- 支持托盘图标快速切换
- 通过 XML 语言文件支持多语言界面
- 主 Lumina 应用使用原生 Win32 GUI，不依赖 WPF、WinForms 或外部 UI 框架

## 微信交流群

扫描下方二维码即可加入 Lumina 微信交流群。
该二维码用于长期使用。

<img src="https://raw.githubusercontent.com/maikebing/Lumina/main/docs/assets/wechat-discussion-group-qr.png" width="180" alt="Lumina 微信交流群二维码" />

## 架构

```text
Lumina.App.exe          Native AOT 可执行文件
  └── Win32 GUI         纯 Win32 API + Direct2D
  └── Config            XML 配置（System.Xml）
  └── Injector          将 Lumina.Ext.dll 注入到 dwm.exe

Lumina.Ext.dll          Native AOT 共享库
  └── ExtMain           DllMain 入口
  └── Hooks/            x64 内联 Hook 引擎（不依赖 minhook）
  └── DWM/              udwm.dll 结构定义与符号偏移
  └── Backdrops/        Blur / Aero / Acrylic / Mica 效果实现
  └── Effects/          Windows.UI.Composition 效果图节点
```

## 运行要求

- Windows 10 2004（build 19041）或更高版本
- Windows 11（所有受支持版本）
- 仅支持 x64
- 高级注入路径需要管理员权限，才能注入到 dwm.exe

## 构建

```bash
dotnet publish Lumina.App -r win-x64 -c Release
```

需要安装带 Native AOT workload 的 .NET 10 SDK。

## 致谢

以下项目为 Lumina 提供了研究方向与技术参考：

- [DWMBlurGlass](https://github.com/Maplespe/DWMBlurGlass) by Maplespe
- [OpenGlass](https://github.com/ALTaleX531/OpenGlass) by ALTaleX531
- [AcrylicEverywhere](https://github.com/ALTaleX531/AcrylicEverywhere) by ALTaleX531

Lumina 是独立实现。这些项目仅作为研究背景和行为参考，仓库中不包含它们的源代码。

更多仓库策略和上游许可证参考，请见 [THIRD_PARTY_NOTICES.md](https://github.com/maikebing/Lumina/blob/main/THIRD_PARTY_NOTICES.md)。

## 许可证

[MIT](https://github.com/maikebing/Lumina/blob/main/LICENSE)