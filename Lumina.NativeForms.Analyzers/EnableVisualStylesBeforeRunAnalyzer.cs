using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lumina.NativeForms.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EnableVisualStylesBeforeRunAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [NativeFormsDiagnosticDescriptors.EnableVisualStylesBeforeRun];

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
        if (invokedMethod is null || !NativeFormsAnalyzerHelpers.IsNativeFormsApplicationMethod(invokedMethod, "Run"))
        {
            return;
        }

        StatementSyntax? currentStatement = invocation.FirstAncestorOrSelf<StatementSyntax>();
        if (currentStatement?.Parent is not BlockSyntax containingBlock)
        {
            return;
        }

        foreach (StatementSyntax statement in containingBlock.Statements)
        {
            if (statement == currentStatement)
            {
                break;
            }

            foreach (InvocationExpressionSyntax priorInvocation in statement.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
            {
                IMethodSymbol? priorMethod = NativeFormsAnalyzerHelpers.GetInvokedMethod(priorInvocation, context.SemanticModel, context.CancellationToken);
                if (priorMethod is not null
                    && (NativeFormsAnalyzerHelpers.IsNativeFormsApplicationMethod(priorMethod, "EnableVisualStyles")
                        || NativeFormsAnalyzerHelpers.IsNativeFormsApplicationConfigurationMethod(priorMethod, "Initialize")))
                {
                    return;
                }
            }
        }

        context.ReportDiagnostic(Diagnostic.Create(
            NativeFormsDiagnosticDescriptors.EnableVisualStylesBeforeRun,
            invocation.GetLocation()));
    }
}
