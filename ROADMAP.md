# Roadmap

Lumina 包含两条并行产品线，共享同一套底层 Hook 引擎与 DWM 效果实现。

## 产品线概览

| 产品 | 定位 | 类比 |
| --- | --- | --- |
| **Lumina.App** | 独立桌面工具，美化整个 Windows 系统界面 | MicaForEveryone、Windhawk |
| **Lumina SDK** | NuGet 库，开发者引用后美化自己的应用窗口 | ModernWpf、WPF-UI |

## 解决方案结构

```text
Lumina.sln
├── Lumina.Ext          ← 注入 DLL：Hook 引擎 + 效果核心（两条线共享）
├── Lumina.Core         ← SDK 公开 API 层（标准模式，普通权限）
├── Lumina.Core.Advanced← SDK 高级 API 层（注入模式，需管理员权限）
├── Lumina.WinForms     ← WinForms 适配器（扩展方法）
├── Lumina.Wpf          ← WPF 适配器（附加属性）
└── Lumina.App          ← 独立桌面美化工具
    └── GUI/            ← 托盘图标、设置窗口
```

---

## Phase 1 — 基础层 ✅

共享基础，两条线通用。

- [x] 解决方案结构与项目配置
- [x] x64 内联 Hook 引擎（`InlineHook.cs`）
- [x] Win32 / DWM P/Invoke 定义（`NativeMethods.cs`）
- [x] OS 构建号检测（`OsVersion.cs`）
- [x] `udwm.dll` 符号偏移表（按 Windows 构建号）
- [x] DLL 注入到 `dwm.exe`（`Injector`）

## Phase 2 — 效果核心 ✅

共享效果实现，两条线通用。

- [x] SystemBackdrop 效果（Mica / Acrylic / MicaAlt，via `DwmSetWindowAttribute`）
- [x] AccentBlur 效果（Hook `CAccent::UpdateAccentPolicy`）
- [x] CustomBlur 效果（Hook `CTopLevelWindow`，`Windows.UI.Composition` 图层）
- [x] Aero 反射 + 视差效果
- [x] 模糊半径 / 混合色自定义
- [x] 亮色 / 暗色模式自动切换

## Phase 3 — 双线 GUI 与 SDK API (current)

### 3-A：Lumina.App — 系统美化工具

- [x] 系统托盘图标 + 快速启用/禁用切换（`GUI/TrayIcon.cs`）
- [x] 主设置窗口，纯 Win32 API，Native AOT 兼容（`GUI/SettingsWindow.cs`）
- [x] 效果预设切换（无 / Blur / Aero / Acrylic / Mica / MicaAlt）
- [x] 混合色拾色器（系统 `ChooseColorW` 对话框）
- [ ] 排除窗口列表（指定哪些窗口不应用效果）
- [ ] 实时预览（切换预设时即时生效）

### 3-B：Lumina SDK — 开发者库

- [ ] `Lumina.Core`：标准模式公开 API
  - `LuminaWindow.SetEffect(hwnd, EffectKind, EffectOptions)`
  - `EffectKind`：`Blur` · `Acrylic` · `Mica` · `MicaAlt` · `Aero`
  - `EffectOptions`：模糊半径、混合色、透明度
- [ ] `Lumina.Core.Advanced`：高级模式 API（需管理员权限）
  - `LuminaAdvanced.Inject()` / `Eject()`
  - `LuminaAdvanced.IsElevated`
  - `LuminaAdvanced.Exclude(hwnd)`
- [ ] `Lumina.WinForms`：扩展方法适配器
  - `form.SetMica()` · `form.SetAcrylic(color)` · `form.SetAero()`
- [ ] `Lumina.Wpf`：附加属性适配器
  - `<Window lumina:Effect.Kind="Mica" />`
- [ ] 原生 Win32 适配器：直接接受 `HWND`，无框架依赖
- [ ] 无边框窗口支持：自动处理 `WM_NCCALCSIZE` / `WM_NCHITTEST`

## Phase 4 — 配置与文档

### Phase 4-A：Lumina.App

- [x] XML 配置文件读写（效果预设持久化）— `Config/AppConfig.cs`
- [x] 多语言支持（XML 语言文件）— `Config/Strings.cs` + `lang/zh-CN.xml` + `lang/en-US.xml`
- [x] 开机自启（写入 `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`）— `Config/AutoStart.cs`，托盘菜单集成
- [ ] 导入 / 导出配置

### Phase 4-B：Lumina SDK

- [x] `EffectProfile` 序列化（JSON / XML）— `Lumina.Core/EffectProfile.cs`
- [x] XML 文档注释（IntelliSense 支持）— `Lumina.Core` / `Lumina.WinForms` / `Lumina.Wpf`
- [x] README 快速上手示例（WinForms / WPF / Win32 / 无边框各一份）— `docs/quickstart.md`
- [ ] 排除窗口列表 API（`LuminaAdvanced.Exclude(hwnd)`）

## Phase 5 — 发布

### Phase 5-A：Lumina.App

- [ ] 安装包（Inno Setup 或 MSIX）
- [ ] 崩溃转储收集
- [ ] 节能模式（电池供电时自动降级效果）
- [ ] 第三方主题兼容性测试

### Phase 5-B：Lumina SDK

- [ ] NuGet 包结构（`Lumina.Core` · `Lumina.Core.Advanced` · `Lumina.WinForms` · `Lumina.Wpf`）
- [ ] GitHub Actions CI：构建 + 单元测试 + 发布 NuGet
- [ ] v1.0.0 正式发布
