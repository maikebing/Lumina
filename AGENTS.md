# AGENTS.md

## Purpose

This repository contains the Lumina codebase and the LuminaForms compatibility effort.

LuminaForms is the WinForms replacement track in this repository. Its goal is not to invent a new desktop UI framework. Its goal is to be a practical drop-in replacement for WinForms on the `net10.0` + Native AOT path while preserving familiar APIs, control names, behavior, and migration ergonomics.

All agents working in this repository must treat the following rules as hard constraints.

## Canonical Direction

- Treat `LuminaForms` as the product direction for the WinForms-compatible UI layer in this repository.
- The current implementation may still contain historical names such as `NativeForms`, `Lumina.NativeForms`, or compatibility code inside `Lumina.WinForms` and related projects.
- When you see those historical names, interpret them as part of the same WinForms-replacement effort unless the user explicitly asks for a rename or a split.
- Treat `Lumina.Forms` as the current project, package, and namespace name for the LuminaForms replacement layer.
- Do not introduce additional naming drift.
- Do not rename existing projects, files, namespaces, MSBuild properties, or public packages unless the user explicitly requests it.

## Non-Negotiable Product Rules

- LuminaForms must be able to act as a WinForms substitute for the `net10.0` target.
- `NativeFormsDemo` is a fixed dual-target validation project.
- `net10.0-windows` in `NativeFormsDemo` must stay on real WinForms.
- The `net10.0-windows` path must remain friendly to the Visual Studio designer.
- `net10.0` in `NativeFormsDemo` must use LuminaForms.
- The `net10.0` path must remain Native AOT compatible.
- Do not collapse the Demo into a single target.
- Do not replace the dual-target Demo model with another project structure unless the user explicitly asks for it.
- Do not introduce another UI stack such as WPF, MAUI, Avalonia, Uno, or a custom abstraction layer as a substitute for this compatibility work.

## API Compatibility Rules

- Do not invent new public control concepts when WinForms already has an established concept.
- Do not invent new public method names, property names, event names, or lifecycle patterns when the WinForms surface already defines them.
- Prefer WinForms names, signatures, defaults, event semantics, and behavioral expectations whenever practical.
- Favor migration by reference switch and namespace switch, not migration by rewrite.
- If a WinForms member is not implemented yet, prefer documenting or staging the gap over introducing a repo-specific replacement API.
- Preserve WinForms expectations for `Application`, `ApplicationConfiguration`, `Form`, `Control`, container controls, item collections, layout, autoscaling, and disposal semantics.
- Preserve `IDisposable`-based lifetime semantics. Do not create a separate custom release model for forms or controls.
- Public API changes should be compatibility-first and additive when possible.

## AOT And Runtime Rules

- The `net10.0` LuminaForms path must stay publishable with Native AOT.
- Avoid designs that depend on runtime code generation, dynamic proxy generation, unsupported reflection patterns, or other AOT-hostile techniques.
- Keep interop, marshaling, and unsafe details encapsulated inside the library instead of leaking them into Demo code or consumer code.
- Prefer implementation techniques that are trimming-friendly and AOT-friendly.
- Favor modern .NET 10 primitives where they materially improve safety, clarity, or performance without harming compatibility.

## Demo Rules

- `NativeFormsDemo` is the compatibility proving ground.
- The WinForms target exists to preserve designer support and provide a behavioral baseline.
- The `net10.0` LuminaForms target exists to prove the replacement path and AOT viability.
- When adding or changing supported controls, properties, methods, events, or layout behavior, keep the Demo useful as a side-by-side migration sample.
- Prefer changes that let the same app concept work across both targets with minimal divergence.

## Agent Working Style For This Repository

- Optimize for WinForms behavioral parity, not framework novelty.
- Treat compatibility regressions as more serious than missing optional enhancements.
- When choosing between a clever new API and a boring WinForms-compatible API, choose the WinForms-compatible API.
- Do not “improve” the public surface by making it different from WinForms unless the user explicitly asks for that break from parity.
- Do not propose architecture changes that weaken designer support on `net10.0-windows`.
- Do not propose implementation changes that weaken AOT support on `net10.0`.
- Preserve the current split of responsibilities unless there is a clear user request to change it.

## Validation Expectations

- When touching LuminaForms compatibility behavior, consider both targets, not just one.
- Verify that changes do not accidentally pull WinForms-only dependencies into the `net10.0` AOT path.
- Prefer tests and Demo coverage that check API parity, behavioral parity, autoscaling, container behavior, item collection behavior, and disposal behavior.
- If a change intentionally diverges from WinForms, call it out explicitly in code review notes, task summaries, or documentation.

## Short Version

- LuminaForms is a WinForms replacement effort.
- `net10.0-windows` stays on WinForms for designer support.
- `net10.0` stays on LuminaForms and must remain AOT-capable.
- Do not invent replacement APIs when WinForms already has the answer.
- Aim for drop-in migration with the same control names, methods, properties, events, defaults, and behavior.