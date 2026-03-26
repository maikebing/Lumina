using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lumina.NativeForms.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferApplicationRunOverShowAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [NativeFormsDiagnosticDescriptors.PreferApplicationRunOverShow];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, Microsoft.CodeAnalysis.CSharp.SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        IMethodSymbol? invokedMethod = NativeFormsAnalyzerHelpers.GetInvokedMethod(invocation, context.SemanticModel, context.CancellationToken);
        if (invokedMethod is null || invokedMethod.Name != "Show")
        {
            return;
        }

        if (!NativeFormsAnalyzerHelpers.IsNativeFormsForm(invokedMethod.ContainingType))
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

        if (!NativeFormsAnalyzerHelpers.IsStartupMethod(containingMethodSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            NativeFormsDiagnosticDescriptors.PreferApplicationRunOverShow,
            invocation.GetLocation(),
            invokedMethod.ContainingType.Name));
    }
}
