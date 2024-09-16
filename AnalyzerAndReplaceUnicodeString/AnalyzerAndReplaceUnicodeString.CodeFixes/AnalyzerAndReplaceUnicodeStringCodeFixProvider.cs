    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Rename;
    using Microsoft.CodeAnalysis.Text;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    namespace AnalyzerAndReplaceUnicodeString
    {
        [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnalyzerAndReplaceUnicodeStringCodeFixProvider)), Shared]
        public class AnalyzerAndReplaceUnicodeStringCodeFixProvider : CodeFixProvider
        {
            public sealed override ImmutableArray<string> FixableDiagnosticIds
            {
                get { return ImmutableArray.Create(AnalyzerAndReplaceUnicodeStringAnalyzer.DiagnosticId); }
            }

            public sealed override FixAllProvider GetFixAllProvider()
            {
                // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
                return WellKnownFixAllProviders.BatchFixer;
            }

            public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
            {

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);


                var diagnostic = context.Diagnostics.First();
                var diagnosticSpan = diagnostic.Location.SourceSpan;



                var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().FirstOrDefault();
                var declarationIterpolate = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InterpolatedStringExpressionSyntax>().FirstOrDefault();
                if (declaration != null)
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Replace unicode string by key in Resource",
                            createChangedDocument: c => ReplaceUnicodeString(context.Document, declaration, c),
                            equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                        diagnostic);
                else
                {
                    context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Replace unicode string by key in Resource",
                        createChangedDocument: c => ReplaceIterpolateUnicodeString(context.Document, declarationIterpolate, c),
                        equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                    diagnostic);
                }
            }


            private async Task<Document> ReplaceUnicodeString(Document document, LiteralExpressionSyntax literalExpression, CancellationToken cancellationToken)
            {
                var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                var newString = StringHelper.ConvertToSlug(literalExpression.Token.ValueText, await GetDocumentNameAsync(literalExpression, document));
                var newStringLiteral = SyntaxFactory.ParseExpression($@"Labels.mhql.{newString}");
                var newRoot = root.ReplaceNode(literalExpression, newStringLiteral);


                return document.WithSyntaxRoot(newRoot);
            }
            private async Task<Document> ReplaceIterpolateUnicodeString(Document document, InterpolatedStringExpressionSyntax interpolatedExpression, CancellationToken cancellationToken)
            {
                var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                var newContents = new List<InterpolatedStringContentSyntax>();

                foreach (var content in interpolatedExpression.Contents)
                {
                    if (content is InterpolatedStringTextSyntax textSyntax)
                    {

                        var newString = StringHelper.ConvertToSlug(textSyntax.TextToken.ValueText, await GetDocumentNameAsync(interpolatedExpression, document));


                        var newTextToken = SyntaxFactory.Token(
                            textSyntax.TextToken.LeadingTrivia,
                            SyntaxKind.InterpolatedStringTextToken,
                            $"{{Labels.mhql.{newString}}}",
                            $"{{Labels.mhql.{newString}}}",
                            textSyntax.TextToken.TrailingTrivia);

                        var newTextSyntax = SyntaxFactory.InterpolatedStringText(newTextToken);
                        newContents.Add(newTextSyntax);
                    }
                    else
                    {
                        newContents.Add(content);
                    }
                }


                var newInterpolatedString = SyntaxFactory.InterpolatedStringExpression(
                    interpolatedExpression.StringStartToken,
                    SyntaxFactory.List(newContents),
                    interpolatedExpression.StringEndToken);

                var newRoot = root.ReplaceNode(interpolatedExpression, newInterpolatedString);

                return document.WithSyntaxRoot(newRoot);
            }
            private static string GetMethodParentName(SyntaxNode node)
            {
                var methodDeclaration = node.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                return methodDeclaration?.Identifier.ValueText ?? string.Empty; // Trả về chuỗi rỗng nếu method là null
            }


            private static string GetNameSpace(SyntaxNode node)
            {
                var namespaceDeclaration = node.AncestorsAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                return namespaceDeclaration?.Name.ToString() ?? string.Empty; 
            }
            public static async Task<string> GetDocumentNameAsync(SyntaxNode node, Document document)
            {
               
                var syntaxRoot = await document.GetSyntaxRootAsync();
                if (syntaxRoot == null)
                {
                    return string.Empty;
                }

                var documentPath = document.FilePath;
                return documentPath ?? string.Empty;
            }

    }
}
