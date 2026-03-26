using Microsoft.CodeAnalysis;

namespace Lumina.NativeForms.Analyzers;

internal static class NativeFormsDiagnosticDescriptors
{
    public const string UsageCategory = "NativeForms.Usage";
    public const string DesignCategory = "NativeForms.Design";

    public static readonly DiagnosticDescriptor ApplicationRunRequiresStaThread = new(
        id: "LNF001",
        title: "Mark NativeForms startup entry points with STAThread",
        messageFormat: "Method '{0}' calls Lumina.NativeForms.Application.Run and should be marked with [STAThread]",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "NativeForms startup methods should be marked with STAThread so the UI thread matches WinForms-style desktop expectations.");

    public static readonly DiagnosticDescriptor EnableVisualStylesBeforeRun = new(
        id: "LNF002",
        title: "Call EnableVisualStyles before Application.Run",
        messageFormat: "Call Lumina.NativeForms.ApplicationConfiguration.Initialize() or Lumina.NativeForms.Application.EnableVisualStyles() before Lumina.NativeForms.Application.Run(...)",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "NativeForms resolves system-aligned effects and theme defaults through ApplicationConfiguration.Initialize() or Application.EnableVisualStyles().");

    public static readonly DiagnosticDescriptor NativeFormsFormShouldBePartial = new(
        id: "LNF003",
        title: "Declare NativeForms forms as partial",
        messageFormat: "NativeForms form '{0}' should be declared partial to stay migration-friendly and designer-style friendly",
        category: DesignCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Using partial form declarations keeps NativeForms closer to WinForms conventions and simplifies generated-code or designer-style splits.");

    public static readonly DiagnosticDescriptor PreferApplicationRunOverShow = new(
        id: "LNF004",
        title: "Prefer Application.Run over calling Show from startup",
        messageFormat: "Prefer Lumina.NativeForms.Application.Run(form) over calling '{0}.Show()' from startup code",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Application.Run(form) keeps the main form and message loop startup consistent and is the preferred NativeForms bootstrap pattern.");
}
