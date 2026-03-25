# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Planned
- DLL injection into `dwm.exe`
- SystemBackdrop / AccentBlur / CustomBlur effect implementations
- Win32 native GUI with Direct2D
- XML config system
- Multi-language support

---

## [0.1.0] — 2026-03-25

### Added
- Initial project structure: `Lumina.App` (AOT WinExe) + `Lumina.Ext` (AOT Shared DLL)
- Solution file (`Lumina.slnx`)
- Native AOT configuration for both projects (net10.0-windows, x64)
- `InlineHook.cs` — x64 inline hook engine using VirtualProtect trampoline, no minhook dependency
- `NativeMethods.cs` — Win32 and DWM P/Invoke definitions (`dwmapi.dll`, `kernel32.dll`, `user32.dll`)
- `OsVersion.cs` — Windows build number detection helper
- `ExtMain.cs` — Native DLL entry point with `[UnmanagedCallersOnly]` DllMain export
- `.gitignore` based on Visual Studio template with AOT `native/` entry
- `README.md`, `ROADMAP.md`, `CHANGELOG.md`
