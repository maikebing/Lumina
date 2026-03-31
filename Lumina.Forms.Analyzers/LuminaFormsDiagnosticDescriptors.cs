using Microsoft.CodeAnalysis;

namespace Lumina.Forms.Analyzers;

internal static class LuminaFormsDiagnosticDescriptors
{
    public const string UsageCategory = "LuminaForms.Usage";
    public const string DesignCategory = "LuminaForms.Design";

    public static readonly DiagnosticDescriptor ApplicationRunRequiresStaThread = new(
        id: "LNF001",
        title: "Mark LuminaForms startup entry points with STAThread",
        messageFormat: "Method '{0}' calls Lumina.Forms.Application.Run and should be marked with [STAThread]",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "LuminaForms startup methods should be marked with STAThread so the UI thread matches WinForms-style desktop expectations.");

    public static readonly DiagnosticDescriptor EnableVisualStylesBeforeRun = new(
        id: "LNF002",
        title: "Call EnableVisualStyles before Application.Run",
        messageFormat: "Call Lumina.Forms.ApplicationConfiguration.Initialize() or Lumina.Forms.Application.EnableVisualStyles() before Lumina.Forms.Application.Run(...)",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "LuminaForms resolves system-aligned effects and theme defaults through ApplicationConfiguration.Initialize() or Application.EnableVisualStyles().");

    public static readonly DiagnosticDescriptor LuminaFormsFormShouldBePartial = new(
        id: "LNF003",
        title: "Declare LuminaForms forms as partial",
        messageFormat: "LuminaForms form '{0}' should be declared partial to stay migration-friendly and designer-style friendly",
        category: DesignCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Using partial form declarations keeps LuminaForms closer to WinForms conventions and simplifies generated-code or designer-style splits.");

    public static readonly DiagnosticDescriptor PreferApplicationRunOverShow = new(
        id: "LNF004",
        title: "Prefer Application.Run over calling Show from startup",
        messageFormat: "Prefer Lumina.Forms.Application.Run(form) over calling '{0}.Show()' from startup code",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Application.Run(form) keeps the main form and message loop startup consistent and is the preferred LuminaForms bootstrap pattern.");
}
