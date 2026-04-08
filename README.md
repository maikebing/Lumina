[English](https://github.com/maikebing/Lumina/blob/main/README.md) | [简体中文](https://github.com/maikebing/Lumina/blob/main/README.zh-CN.md)

# Lumina

> A native Windows visual-effects toolkit and the home of LuminaForms, a WinForms-compatible UI layer for net10.0 plus Native AOT desktop applications.

[![License](https://img.shields.io/github/license/maikebing/Lumina)](https://github.com/maikebing/Lumina/blob/main/LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-blue)](https://github.com/maikebing/Lumina)
[![Native AOT](https://img.shields.io/badge/runtime-Native%20AOT-0a7b83)](https://github.com/maikebing/Lumina)

## What is Lumina?

Lumina currently covers two closely related directions:

- **Lumina**: native Windows blur, Aero, Acrylic, Mica, and window-backdrop effects for Win32, WinForms, and WPF applications.
- **LuminaForms**: a WinForms-compatible layer for net10.0 that keeps familiar control names, startup patterns, menu models, dialogs, autoscaling behavior, and migration ergonomics while staying friendly to Native AOT.

Unlike similar projects written in C++, the Lumina codebase is implemented in **C# with Native AOT in mind**. The goal is to keep low-level Windows access, trim-friendliness, and deployment simplicity without introducing a new desktop UI stack.

## LuminaForms

LuminaForms ships through the **Lumina.Forms** package and namespace as the AOT-friendly companion layer for WinForms-style desktop apps.
It keeps familiar names such as **Application**, **ApplicationConfiguration**, **Form**, **Button**, **TextBox**, **ComboBox**, **CheckBox**, **RadioButton**, **ListBox**, **GroupBox**, **Panel**, **MenuStrip**, and **ContextMenuStrip** so existing apps can move with minimal namespace and project-file changes.

### Why LuminaForms?

- WinForms-compatible API direction instead of a brand-new control model
- net10.0 path intended for Native AOT and Win32-native controls
- dual-target validation through **NativeFormsDemo** so the same app concept can run on WinForms and LuminaForms
- focus on migration ergonomics, menu compatibility, autoscaling, theming, and designer-friendly validation on the WinForms side

### NuGet Packages

The following package IDs are configured in the repository today. Version badges reflect the current repository package metadata. Download badges will start showing real numbers after the first public NuGet release.

| Icon | Package | Version | Downloads | Purpose |
| --- | --- | --- | --- | --- |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet package icon" /> | **Lumina.Forms** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | WinForms-compatible UI layer for net10.0 and Native AOT desktop apps. |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet package icon" /> | **Lumina.Forms.Analyzers** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | Roslyn analyzers for LuminaForms startup, usage, and migration rules. |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet package icon" /> | **Lumina.WinForms** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | WinForms extension package for applying Lumina visual effects to existing forms. |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet package icon" /> | **Lumina.Wpf** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | WPF attached-property adapter for Mica, Acrylic, Aero, and blur effects. |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet package icon" /> | **Lumina.Advanced** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | Advanced effect path through dwm.exe injection for deeper Aero and blur customization. |
| <img src="https://raw.githubusercontent.com/maikebing/Lumina/main/assets/nuget-icon.png" width="28" alt="NuGet package icon" /> | **Lumina.Core** | ![Version](https://img.shields.io/badge/version-0.1.0-0a7b83?logo=nuget) | ![Downloads](https://img.shields.io/badge/downloads-pending-lightgrey?logo=nuget) | Core visual-effects runtime package for Win32, WinForms, WPF, and borderless window scenarios. |

### Install from the Command Line

Once packages are published, install the main LuminaForms package into a project with:

```bash
dotnet add package Lumina.Forms
```

Optional companion packages:

```bash
dotnet add package Lumina.Forms.Analyzers
dotnet add package Lumina.Core
dotnet add package Lumina.WinForms
dotnet add package Lumina.Wpf
dotnet add package Lumina.Advanced
```

### Native AOT Footprint

Measured from the current **Lumina.NotepadDemo** Release publish output for **win-x64** single-file Native AOT:

- executable size: **3.09 MB**
- full publish directory size: **3.09 MB**
- no external .NET runtime dependency required for the published app

### Demo Targets

**NativeFormsDemo** uses two targets:

- **net10.0-windows**: WinForms path, kept friendly to the Visual Studio designer
- **net10.0**: LuminaForms path, intended for Native AOT and Win32-native controls

The demo project uses both **UseWindowsForms** and **UseLuminaForms** so the same solution can validate migration behavior on both sides.

### Themes and Compatibility

LuminaForms supports:

- OS-aware default backdrops through **Application.EnableVisualStyles()**
- OS-aware visual style families through **VisualStyleKind** (**Classic**, **AeroGlass**, **Modern**, **Fluent**, **Mica**)
- WinForms-style startup compatibility through **ApplicationConfiguration.Initialize()**
- system light and dark tracking with live refresh for open windows
- app-level visual overrides through **Application.ConfigureVisualStyles(...)**
- window-level overrides through **UseTheme(...)**, **SetThemeMode(...)**, and **SetPalette(...)**
- JSON theme files through **NativeTheme** and semantic **ThemePalette** tokens
- WinForms-style **BackColor** and **ForeColor** overrides on forms and controls, layered on top of the active theme
- WinForms-like collection helpers such as **Controls.AddRange(...)**, **Controls.Find(...)**, **Items.AddRange(...)**, and **SelectedItem**
- compatibility analyzers through **Lumina.Forms.Analyzers**
- WinForms-style autoscaling through **AutoScaleMode**, **AutoScaleDimensions**, and **PerformAutoScale()**

Sample theme files live under [themes/nativeforms](https://github.com/maikebing/Lumina/tree/main/themes/nativeforms).

### Docs

- [Quick Start](https://github.com/maikebing/Lumina/blob/main/docs/quickstart.md)
- [LuminaForms Overview](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms.md)
- [LuminaForms Migration Guide](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms-migration.md)
- [LuminaForms Compatibility Notes](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms-compatibility.md)
- [LuminaForms Theme Guide](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms-themes.md)
- [LuminaForms Support Matrix](https://github.com/maikebing/Lumina/blob/main/docs/zh/nativeforms-support-matrix.md)

## Lumina Runtime Features

- **Blur**: classic Gaussian blur background
- **Aero**: Windows 7-style glass with reflection and parallax
- **Acrylic**: Windows 10 acrylic noise plus blur material
- **Mica**: Windows 11 desktop-tinted material
- **MicaAlt**: Mica with tab-style tint variant
- custom blur radius, blend color, and title-bar text color
- light and dark mode auto-switching
- system tray icon with quick toggle
- multi-language UI through XML language files
- native Win32 GUI with no WPF, no WinForms, and no external UI framework in the main Lumina app

## WeChat Group

Scan the QR code below to join the Lumina discussion group on WeChat.
This QR code is intended for long-term use.

<img src="https://raw.githubusercontent.com/maikebing/Lumina/main/docs/assets/wechat-discussion-group-qr.png" width="180" alt="Lumina WeChat discussion group QR code" />

## Architecture

```text
Lumina.App.exe          Native AOT executable
  └── Win32 GUI         Pure Win32 API + Direct2D
  └── Config            XML config (System.Xml)
  └── Injector          Injects Lumina.Ext.dll into dwm.exe

Lumina.Ext.dll          Native AOT shared library
  └── ExtMain           DllMain entry point
  └── Hooks/            x64 inline hook engine (no minhook)
  └── DWM/              udwm.dll struct definitions + symbol offsets
  └── Backdrops/        Blur / Aero / Acrylic / Mica effect implementations
  └── Effects/          Windows.UI.Composition effect graph nodes
```

## Requirements

- Windows 10 2004 (build 19041) or later
- Windows 11 (all supported builds)
- x64 only
- administrator privileges are required for the advanced injection path into dwm.exe

## Building

```bash
dotnet publish Lumina.App -r win-x64 -c Release
```

Requires the .NET 10 SDK with the Native AOT workload.

## Acknowledgements

Inspired by and technically informed by:

- [DWMBlurGlass](https://github.com/Maplespe/DWMBlurGlass) by Maplespe
- [OpenGlass](https://github.com/ALTaleX531/OpenGlass) by ALTaleX531
- [AcrylicEverywhere](https://github.com/ALTaleX531/AcrylicEverywhere) by ALTaleX531

Lumina is an independent implementation. These projects are credited for research context and behavioral reference only. No source code from them is included in this repository.

See [THIRD_PARTY_NOTICES.md](https://github.com/maikebing/Lumina/blob/main/THIRD_PARTY_NOTICES.md) for repository policy and upstream license references.

## License

[MIT](https://github.com/maikebing/Lumina/blob/main/LICENSE)
