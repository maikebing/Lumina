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
├── Lumina.Tests        ← 单元测试
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

## Phase 3 — 双线 GUI 与 SDK API ✅

### 3-A：Lumina.App — 系统美化工具

- [x] 系统托盘图标 + 快速启用/禁用切换（`GUI/TrayIcon.cs`）
- [x] 主设置窗口，纯 Win32 API，Native AOT 兼容（`GUI/SettingsWindow.cs`）
- [x] 效果预设切换（无 / Blur / Aero / Acrylic / Mica / MicaAlt）
- [x] 混合色拾色器（系统 `ChooseColorW` 对话框，`GUI/ColorPicker.cs`）
- [x] 排除窗口列表（`GUI/ExclusionList.cs`）
- [ ] 实时预览（切换预设时即时生效）

### 3-B：Lumina SDK — 开发者库

- [x] `Lumina.Core`：标准模式公开 API
  - `LuminaWindow.SetEffect(hwnd, EffectKind, EffectOptions)`
  - `EffectKind`：`Blur` · `Acrylic` · `Mica` · `MicaAlt` · `Aero`
  - `EffectOptions`：模糊半径、混合色、透明度
- [x] `Lumina.Core.Advanced`：高级模式 API（需管理员权限）
  - `LuminaAdvanced.Inject()` / `Eject()`
  - `LuminaAdvanced.IsElevated`
  - `LuminaAdvanced.Exclude(hwnd)` / `Include` / `IsExcluded`
- [x] `Lumina.WinForms`：扩展方法适配器
  - `form.SetMica()` · `form.SetAcrylic(color)` · `form.SetAero()`
- [x] `Lumina.Wpf`：附加属性适配器
  - `<Window lumina:Effect.Kind="Mica" />`
- [x] 原生 Win32 适配器：`LuminaWindow.SetEffect(nint hwnd, ...)` 直接接受 HWND，无框架依赖
- [ ] 无边框窗口支持：自动处理 `WM_NCCALCSIZE` / `WM_NCHITTEST`

## Phase 4 — 配置与文档 ✅

### Phase 4-A：Lumina.App

- [x] JSON 配置文件读写（效果预设持久化）— `Config/AppConfig.cs`
- [x] 多语言支持（XML 语言文件）— `Config/Strings.cs` + `lang/zh-CN.xml` + `lang/en-US.xml`
- [x] 开机自启（写入 `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`）— `Config/AutoStart.cs`，托盘菜单集成
- [x] 导入 / 导出配置 — 设置窗口内置文件对话框（`GetOpenFileNameW` / `GetSaveFileNameW`）

### Phase 4-B：Lumina SDK

- [x] `EffectProfile` 序列化（JSON）— `Lumina.Core/EffectProfile.cs`
- [x] XML 文档注释（IntelliSense 支持）— `Lumina.Core` / `Lumina.WinForms` / `Lumina.Wpf`
- [x] README 快速上手示例（WinForms / WPF / Win32 / 无边框各一份）— `docs/quickstart.md`
- [x] 排除窗口列表 API（`LuminaAdvanced.Exclude(hwnd)` / `Include` / `IsExcluded`）— `Lumina.Core.Advanced/LuminaAdvanced.cs`

## Phase 5 — 发布 (current)

### Phase 5-A：Lumina.App

- [x] 安装包（Inno Setup `installer/Lumina.iss` + MSIX `msix/Package.appxmanifest`）
- [x] 崩溃转储收集（`CrashHandler.cs`，写入 `%AppData%\Lumina\crashes\`）
- [x] 节能模式（`PowerMonitor.cs`，电池供电时自动降级效果）
- [x] 单元测试（`Lumina.Tests/`，覆盖 `EffectKind` / `EffectOptions` / `EffectProfile`）
- [ ] 第三方主题兼容性测试用例（`Lumina.Tests/` 已建，兼容性用例待补充）
- [ ] **MSIX 图标素材设计**（`msix/Assets/` 下需提供以下尺寸）：
  - `StoreLogo.png` 50×50
  - `Square44x44Logo.png` 44×44
  - `Square150x150Logo.png` 150×150
  - `Wide310x150Logo.png` 310×150
  - `SplashScreen.png` 620×300

### Phase 5-B：Lumina SDK

- [x] NuGet 包元数据（`Lumina` · `Lumina.Advanced` · `Lumina.WinForms` · `Lumina.Wpf`，各 csproj 已配置）
- [x] GitHub Actions CI：构建 + 单元测试 + 发布 NuGet + Inno Setup + MSIX + GitHub Release（`.github/workflows/ci.yml`）
- [ ] v1.0.0 正式发布

### Phase 5-C：文档站

- [x] 文档站结构（Jekyll 格式，`docs/` 目录，中英双语）
- [ ] 文档站部署（GitHub Pages，域名 `lumina.maikebing.com`）

## Phase 6 - NativeForms 与迁移兼容

### Phase 6-A：Lumina.NativeForms 核心

- [x] 新增 `Lumina.NativeForms` 项目并纳入解决方案
- [x] 公开 WinForms 风格命名入口：`Application` / `Form` / `Control` / `Button` / `Label` / `TextBox` / `ComboBox` / `CheckBox` / `GroupBox`
- [x] 删除旧 `Native*` 文件命名，降低旧项目迁移成本
- [x] 将窗口过程封送改为托管委托，避免 `unsafe` 向 Demo 与使用方传染
- [ ] 按 WinForms 迁移优先级继续补齐常用控件族，优先覆盖旧项目高频控件
- [ ] 对齐 WinForms 的属性名、方法名、事件名、默认值与行为语义，目标是大部分旧项目仅切换引用与命名空间即可迁移
- [ ] 为暂未兼容的能力提供最小替代层、兼容说明与迁移清单
- [ ] 补充无 WinForms 依赖的 AOT 构建样例、兼容性测试与回归用例
- [ ] 新增 `Lumina.NativeForms.Analyzers`，提供启动约束、`Application.Run` 用法、`partial Form` 约束与迁移友好规则

### Phase 6-B：系统风格与主题

- [ ] 将默认视觉初始化入口收敛到 `Application.EnableVisualStyles()`
- [ ] 根据操作系统版本自动选择最贴近系统的窗口效果与外观：
  - Windows 7：Aero / Glass
  - Windows 8 / 8.1：贴近系统的扁平风格
  - Windows 10：Fluent / Blur / Acrylic 风格回退
  - Windows 11：Mica / MicaAlt
- [ ] 保留应用级与窗口级覆盖口，允许用户显式指定效果、主题模式与调色板
- [ ] 首先支持跟随系统浅色 / 深色模式
- [ ] 设计可扩展主题模型：语义颜色 token、状态色、标题栏、边框、背景、文本、控件层级
- [ ] 支持 JSON 主题文件的加载、保存、导入、导出，为用户与 AI 生成主题预留空间
- [ ] 参考 `SkiaShell.Theme` 的 palette 组织方式，整理 Lumina 自己的主题 token / palette 架构
- [ ] 为系统主题跟随、效果回退、主题切换补充示例与验证

### Phase 6-C：Demo 与设计器友好

- [ ] `Lumina.NativeForms.Demo` 改为 `net10.0-windows` + `net10.0` 双目标
- [ ] `net10.0-windows` 目标使用 WinForms，并保持 Visual Studio 设计器友好
- [ ] `net10.0` 目标使用 `Lumina.NativeForms` 的同名控件
- [ ] 增加 `UseNativeForms` MSBuild 属性，与 `UseWinForms` 区分，便于条件编译与后续扩展
- [ ] 拆分 WinForms / NativeForms 两套入口与窗体文件，避免互相影响
- [ ] 将称台模拟器（`WdsScaleSimulator`）界面迁移为 NativeForms Demo，并保留原有交互流程作为迁移样板
- [ ] 在 Demo 中覆盖目前已实现的全部控件、事件、属性与典型布局，验证与 WinForms API / 行为的一致性
- [ ] 提供“旧 WinForms 应用切换引用与命名空间”的迁移示例
- [ ] 验证 Demo 在 VS 设计器、运行时与 AOT 场景下的可用性

### Phase 6-D：文档与开发者体验

- [ ] 为 `Lumina.NativeForms` 公开类型、属性、方法、事件补齐 XML 注释，帮助使用者理解 Win32 实现与兼容性边界
- [ ] 更新 README、quickstart 与 migration 文档，补充 NativeForms 章节
- [ ] 新增控件支持矩阵、兼容差异清单、主题配置说明
- [ ] 补充 Demo 截图 / 录屏、构建说明、回归测试与后续发布准备
