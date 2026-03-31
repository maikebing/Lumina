using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lumina.NativeForms.Analyzers;

internal static class NativeFormsAnalyzerHelpers
{
    private const string ApplicationTypeName = "Lumina.NativeForms.Application";
    private const string ApplicationConfigurationTypeName = "Lumina.NativeForms.ApplicationConfiguration";
    private const string FormTypeName = "Lumina.NativeForms.Form";
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

    public static bool IsNativeFormsApplicationMethod(IMethodSymbol methodSymbol, string methodName)
    {
        return methodSymbol.Name == methodName
            && methodSymbol.ContainingType.ToDisplayString() == ApplicationTypeName;
    }

    public static bool IsNativeFormsApplicationConfigurationMethod(IMethodSymbol methodSymbol, string methodName)
    {
        return methodSymbol.Name == methodName
            && methodSymbol.ContainingType.ToDisplayString() == ApplicationConfigurationTypeName;
    }

    public static bool IsNativeFormsForm(ITypeSymbol? typeSymbol)
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
