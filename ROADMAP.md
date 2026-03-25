# Roadmap

This document outlines the planned development phases for Lumina.

---

## Phase 1 — Foundation (current)

- [x] Project structure: `Lumina.App` + `Lumina.Ext` as Native AOT
- [x] Solution file and project configuration
- [x] x64 inline hook engine (`InlineHook.cs`)
- [x] Win32 / DWM P/Invoke definitions (`NativeMethods.cs`)
- [x] OS build number detection (`OsVersion.cs`)
- [x] DLL entry point (`ExtMain.cs`)
- [x] Basic DLL injection into `dwm.exe`
- [x] Symbol offset table for `udwm.dll` (per Windows build)

## Phase 2 — Effects Core

- [ ] SystemBackdrop effect (Mica / Acrylic via `DwmSetWindowAttribute`)
- [ ] AccentBlur effect (Hook `CAccent::UpdateAccentPolicy`)
- [ ] CustomBlur effect (Hook `CTopLevelWindow`, `Windows.UI.Composition` graph)
- [ ] Aero reflection + parallax
- [ ] Blur radius / color customization
- [ ] Light/Dark mode auto-switching

## Phase 3 — GUI

- [ ] Main settings window (Win32 API + Direct2D)
- [ ] System tray icon with quick enable/disable toggle
- [ ] Effect preset switcher (Blur / Aero / Acrylic / Mica / MicaAlt)
- [ ] Color picker for blend color and text color
- [ ] Per-window exclusion list

## Phase 4 — Config & i18n

- [ ] XML config file read/write (`System.Xml`)
- [ ] Multi-language support (XML language files)
- [ ] Auto-start on Windows login
- [ ] Import/export settings

## Phase 5 — Polish & Release

- [ ] Installer (Inno Setup or MSIX)
- [ ] Crash dump collection
- [ ] Power saving mode (disable effects on battery)
- [ ] Third-party theme compatibility
- [ ] GitHub Actions CI build pipeline
- [ ] v1.0.0 release
