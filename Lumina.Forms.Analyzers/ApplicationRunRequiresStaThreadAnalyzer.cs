using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lumina.Forms.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApplicationRunRequiresStaThreadAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [LuminaFormsDiagnosticDescriptors.ApplicationRunRequiresStaThread];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, Microsoft.CodeAnalysis.CSharp.SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        IMethodSymbol? invokedMethod = LuminaFormsAnalyzerHelpers.GetInvokedMethod(invocation, context.SemanticModel, context.CancellationToken);
        if (invokedMethod is null || !LuminaFormsAnalyzerHelpers.IsLuminaFormsApplicationMethod(invokedMethod, "Run"))
        {
            return;
        }

        MethodDeclarationSyntax? containingMethodSyntax = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (containingMethodSyntax is null)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(containingMethodSyntax, context.CancellationToken) is not IMethodSymbol containingMethodSymbol)
        {
            return;
        }

        if (LuminaFormsAnalyzerHelpers.HasStaThreadAttribute(containingMethodSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            LuminaFormsDiagnosticDescriptors.ApplicationRunRequiresStaThread,
            invocation.GetLocation(),
            containingMethodSymbol.Name));
    }
}
