using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lumina.NativeForms.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NativeFormsFormShouldBePartialAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [NativeFormsDiagnosticDescriptors.NativeFormsFormShouldBePartial];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        if (classDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken) is not INamedTypeSymbol classSymbol)
        {
            return;
        }

        if (!NativeFormsAnalyzerHelpers.IsNativeFormsForm(classSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            NativeFormsDiagnosticDescriptors.NativeFormsFormShouldBePartial,
            classDeclaration.Identifier.GetLocation(),
            classSymbol.Name));
    }
}
