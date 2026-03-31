using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lumina.Forms.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LuminaFormsFormShouldBePartialAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [LuminaFormsDiagnosticDescriptors.LuminaFormsFormShouldBePartial];

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

        if (!LuminaFormsAnalyzerHelpers.IsLuminaFormsForm(classSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            LuminaFormsDiagnosticDescriptors.LuminaFormsFormShouldBePartial,
            classDeclaration.Identifier.GetLocation(),
            classSymbol.Name));
    }
}
