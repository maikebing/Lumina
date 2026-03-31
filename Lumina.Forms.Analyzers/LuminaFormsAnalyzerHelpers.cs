using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lumina.Forms.Analyzers;

internal static class LuminaFormsAnalyzerHelpers
{
    private const string ApplicationTypeName = "Lumina.Forms.Application";
    private const string ApplicationConfigurationTypeName = "Lumina.Forms.ApplicationConfiguration";
    private const string FormTypeName = "Lumina.Forms.Form";
    private const string StaThreadAttributeTypeName = "System.STAThreadAttribute";

    public static IMethodSymbol? GetInvokedMethod(
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(invocation, cancellationToken);
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
        {
            return methodSymbol;
        }

        return symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();
    }

    public static bool IsLuminaFormsApplicationMethod(IMethodSymbol methodSymbol, string methodName)
    {
        return methodSymbol.Name == methodName
            && methodSymbol.ContainingType.ToDisplayString() == ApplicationTypeName;
    }

    public static bool IsLuminaFormsApplicationConfigurationMethod(IMethodSymbol methodSymbol, string methodName)
    {
        return methodSymbol.Name == methodName
            && methodSymbol.ContainingType.ToDisplayString() == ApplicationConfigurationTypeName;
    }

    public static bool IsLuminaFormsForm(ITypeSymbol? typeSymbol)
    {
        for (ITypeSymbol? current = typeSymbol; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString() == FormTypeName)
            {
                return true;
            }
        }

        return false;
    }

    public static bool HasStaThreadAttribute(IMethodSymbol methodSymbol)
    {
        return methodSymbol
            .GetAttributes()
            .Any(attribute => attribute.AttributeClass?.ToDisplayString() == StaThreadAttributeTypeName);
    }

    public static bool IsStartupMethod(IMethodSymbol methodSymbol)
    {
        return methodSymbol.Name == "Main" || HasStaThreadAttribute(methodSymbol);
    }
}
