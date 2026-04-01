using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Lumina.Forms.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnableVisualStylesBeforeRunCodeFixProvider)), Shared]
public sealed class EnableVisualStylesBeforeRunCodeFixProvider : CodeFixProvider
{
    private const string Title = "Insert ApplicationConfiguration.Initialize()";

    public override ImmutableArray<string> FixableDiagnosticIds
        => [LuminaFormsDiagnosticDescriptors.EnableVisualStylesBeforeRun.Id];

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Unable to load syntax root.");

        Diagnostic diagnostic = context.Diagnostics[0];
        SyntaxNode? diagnosticNode = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
        InvocationExpressionSyntax? runInvocation = diagnosticNode.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (runInvocation is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: cancellationToken => InsertInitializeCallAsync(context.Document, root, runInvocation, cancellationToken),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> InsertInitializeCallAsync(
        Document document,
        SyntaxNode root,
        InvocationExpressionSyntax runInvocation,
        CancellationToken cancellationToken)
    {
        StatementSyntax? currentStatement = runInvocation.FirstAncestorOrSelf<StatementSyntax>();
        if (currentStatement?.Parent is not BlockSyntax containingBlock)
        {
            return document;
        }

        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Unable to load semantic model.");

        string initializeCall = CanUseSimpleApplicationConfigurationName(semanticModel, currentStatement.SpanStart)
            ? "ApplicationConfiguration.Initialize();"
            : "global::Lumina.Forms.ApplicationConfiguration.Initialize();";

        StatementSyntax initializeStatement = SyntaxFactory.ParseStatement(initializeCall)
            .WithAdditionalAnnotations(Formatter.Annotation)
            .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);

        StatementSyntax formattedStatement = initializeStatement
            .WithLeadingTrivia(currentStatement.GetLeadingTrivia());

        SyntaxList<StatementSyntax> updatedStatements = containingBlock.Statements.Insert(
            containingBlock.Statements.IndexOf(currentStatement),
            formattedStatement);

        BlockSyntax updatedBlock = containingBlock.WithStatements(updatedStatements);
        SyntaxNode updatedRoot = root.ReplaceNode(containingBlock, updatedBlock);

        return document.WithSyntaxRoot(updatedRoot);
    }

    private static bool CanUseSimpleApplicationConfigurationName(SemanticModel semanticModel, int position)
    {
        return semanticModel.LookupSymbols(position, name: "ApplicationConfiguration")
            .OfType<INamedTypeSymbol>()
            .Any(typeSymbol => typeSymbol.ToDisplayString() == "Lumina.Forms.ApplicationConfiguration");
    }
}
