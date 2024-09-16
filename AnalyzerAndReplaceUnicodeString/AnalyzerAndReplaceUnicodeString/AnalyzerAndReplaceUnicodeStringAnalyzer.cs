using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace AnalyzerAndReplaceUnicodeString
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnalyzerAndReplaceUnicodeStringAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AnalyzerAndReplaceUnicodeString";
       
        static string UNICODE_REGEX = @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ]+";


        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = "Unicode string should represent by key of Resource";
        private static readonly LocalizableString MessageFormat = "Unicode string should represent by key of Resource";
        private static readonly LocalizableString Description = "Unicode string should represent by key of Resource";
        private const string Category = "Value";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeLiteralUnicode, SyntaxKind.StringLiteralExpression);

            context.RegisterSyntaxNodeAction(AnalyzeInterpolatedStringUnicode, SyntaxKind.InterpolatedStringExpression);
        }


        private static void AnalyzeLiteralUnicode(SyntaxNodeAnalysisContext context)
        {
            var literaUnicodeExpr = (LiteralExpressionSyntax)context.Node;
            var literaValue = literaUnicodeExpr.Token.ValueText;
            if (IsUnicode(literaValue))
            {
                var diagnostic = Diagnostic.Create(Rule, literaUnicodeExpr.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
        private static void AnalyzeInterpolatedStringUnicode(SyntaxNodeAnalysisContext context)
        {
            var interpoStringExpr = (InterpolatedStringExpressionSyntax)context.Node;
            foreach (var content in interpoStringExpr.Contents)
            {

                if (content is InterpolatedStringTextSyntax text)
                {
                    var textValue = text.TextToken.ValueText;
                    if (IsUnicode(textValue))
                    {
                        var diagnostic = Diagnostic.Create(Rule, interpoStringExpr.GetLocation());
                        context.ReportDiagnostic(diagnostic);
                        return;
                    }
                }
            }
        }


        private static bool IsUnicode(string value)
        {
            var vietnameseRegex = new Regex(UNICODE_REGEX);
            return vietnameseRegex.IsMatch(value);
        }
    }
}
