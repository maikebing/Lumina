---
title: "Lumina.App 桌面工具指南"
description: "Lumina.App 的安装、托盘操作、效果预设、节能模式与崩溃报告说明。"
permalink: /zh/blog/app-guide/
lang: "zh-CN"
nav_key: "blog"
---

# Lumina.App 桌面工具指南

Lumina.App 是面向普通用户的独立桌面美化工具，无需写代码，安装即用。

## 安装

从 [GitHub Releases](https://github.com/maikebing/Lumina/releases) 下载最新版本：

- `Lumina-x64-setup.exe` — Inno Setup 安装包
- `Lumina-x64.msix` — MSIX 包（需开发者模式或企业证书）

运行安装包，按提示完成安装。安装时可选择创建桌面快捷方式和开机自启。

## 系统托盘

安装完成后，Lumina 图标出现在系统托盘区域。右键图标可：

- **启用 / 禁用**：一键切换效果开关
- **选择效果**：无 / Blur / Acrylic / Aero / Mica / MicaAlt
- **打开设置**：进入完整设置窗口
- **退出**：关闭 Lumina

## 设置窗口

设置窗口提供以下功能：

| 功能 | 说明 |
| --- | --- |
| 效果预设 | 切换 6 种视觉效果 |
| 混合色 | 通过系统拾色器选择叠加颜色 |
| 模糊半径 | 调整 Blur / Acrylic 的模糊强度 |
| 开机自启 | 写入注册表 `HKCU\...\Run` |
| 导入配置 | 从 JSON 文件加载预设 |
| 导出配置 | 将当前预设保存为 JSON 文件 |

## 节能模式

当检测到设备使用电池供电（未连接充电器）时，Lumina 自动降级效果以减少 GPU 负担。重新接入电源后自动恢复原效果。

## 崩溃报告

若 Lumina 意外崩溃，会在以下目录生成 minidump 文件：

```
%AppData%\Lumina\crashes\lumina_YYYYMMDD_HHmmss.dmp
```

如需反馈问题，请将 `.dmp` 文件附带提交至 [GitHub Issues](https://github.com/maikebing/Lumina/issues)。

## 配置文件位置

```
%AppData%\Lumina\config.json
```

## 续读

- [兼容性说明](/zh/compatibility/)
- [SDK 使用指南](/zh/blog/sdk-guide/)（开发者）
