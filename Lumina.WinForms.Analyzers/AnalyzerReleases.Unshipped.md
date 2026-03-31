### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
LNF001 | NativeForms.Usage | Warning | Mark entry points that call `Application.Run(...)` with `[STAThread]`.
LNF002 | NativeForms.Usage | Warning | Call `ApplicationConfiguration.Initialize()` or `Application.EnableVisualStyles()` before `Application.Run(...)`.
LNF003 | NativeForms.Design | Warning | Declare `Lumina.NativeForms.Form` types as `partial` for migration and designer-style friendliness.
LNF004 | NativeForms.Usage | Info | Prefer `Application.Run(form)` over calling `form.Show()` from startup code.
