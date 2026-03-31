### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
LNF001 | LuminaForms.Usage | Warning | Mark entry points that call `Application.Run(...)` with `[STAThread]`.
LNF002 | LuminaForms.Usage | Warning | Call `ApplicationConfiguration.Initialize()` or `Application.EnableVisualStyles()` before `Application.Run(...)`.
LNF003 | LuminaForms.Design | Warning | Declare `Lumina.Forms.Form` types as `partial` for migration and designer-style friendliness.
LNF004 | LuminaForms.Usage | Info | Prefer `Application.Run(form)` over calling `form.Show()` from startup code.
