# Lumina

> A lightweight, native Windows DWM blur/glass effects manager — rewritten in C# Native AOT, zero runtime dependencies.

[![License](https://img.shields.io/github/license/maikebing/Lumina)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-blue)](#)
[![Language](https://img.shields.io/badge/language-C%23%20AOT-purple)](#)

## What is Lumina?

Lumina brings back and extends the classic Windows Aero Glass / Blur / Mica / Acrylic effects to the system title bar and window frames on Windows 10 and Windows 11.

Unlike similar tools built in C++, Lumina is written entirely in **C# with Native AOT** — producing a self-contained native binary with no .NET runtime dependency, while retaining full access to low-level Windows DWM internals via P/Invoke and unsafe code.

## Features

- **Blur** — Classic Gaussian blur background
- **Aero** — Windows 7-style glass with reflection and parallax
- **Acrylic** — Windows 10 acrylic noise+blur material
- **Mica** — Windows 11 desktop-tinted material
- **MicaAlt** — Mica with tab-style tint variant
- Custom blur radius, blend color, title bar text color
- Light/Dark mode auto-switching
- System tray icon with quick toggle
- Multi-language UI (XML-based language files)
- Native Win32 GUI — no WPF, no WinForms, no external UI framework

## Architecture

```
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
- Administrator privileges (required to inject into dwm.exe)

## Building

```bash
dotnet publish Lumina.App -r win-x64 -c Release
```

Requires .NET 10 SDK with Native AOT workload.

## Acknowledgements

Inspired by and technical references:
- [DWMBlurGlass](https://github.com/Maplespe/DWMBlurGlass) by Maplespe
- [OpenGlass](https://github.com/ALTaleX531/OpenGlass) by ALTaleX531
- [AcrylicEverywhere](https://github.com/ALTaleX531/AcrylicEverywhere) by ALTaleX531

Lumina is an independent implementation. These projects are credited for research
context and behavioral reference only; no source code from them is included in
this repository.

See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) for the repository policy
and upstream license references.

## License

[MIT](LICENSE)
