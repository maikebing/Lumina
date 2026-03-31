using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lumina.Forms.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EnableVisualStylesBeforeRunAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [LuminaFormsDiagnosticDescriptors.EnableVisualStylesBeforeRun];

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
                IMethodSymbol? priorMethod = LuminaFormsAnalyzerHelpers.GetInvokedMethod(priorInvocation, context.SemanticModel, context.CancellationToken);
                if (priorMethod is not null
                    && (LuminaFormsAnalyzerHelpers.IsLuminaFormsApplicationMethod(priorMethod, "EnableVisualStyles")
                        || LuminaFormsAnalyzerHelpers.IsLuminaFormsApplicationConfigurationMethod(priorMethod, "Initialize")))
                {
                    return;
                }
            }
        }

        context.ReportDiagnostic(Diagnostic.Create(
            LuminaFormsDiagnosticDescriptors.EnableVisualStylesBeforeRun,
            invocation.GetLocation()));
    }
}
