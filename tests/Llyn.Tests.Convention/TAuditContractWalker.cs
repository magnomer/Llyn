using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static class TAuditContractWalker
{
    public static IReadOnlyList<TViolation> TAuditRun(
        IReadOnlyList<string> driverPaths, IEnumerable<string> markupPaths)
    {
        List<TViolation> violations = [];
        IReadOnlySet<string> ids = TAuditIdRead(markupPaths);
        foreach (string path in driverPaths)
        {
            TAuditHardwiringScan(path, violations);
        }

        foreach (SyntaxNode root in TAuditBinder.TAuditWalkRead(driverPaths))
        {
            TAuditMasqueradingScan(root, violations);
            TAuditDanglingScan(root, ids, violations);
        }

        return violations;
    }

    private static void TAuditHardwiringScan(string path, List<TViolation> violations)
    {
        string[] lines = File.ReadAllLines(path);
        for (int index = 0; index < lines.Length; index++)
        {
            string? marker = TAuditStrictSetting.TAuditHardwiringMarkers
                .FirstOrDefault(item => lines[index].Contains(item, StringComparison.Ordinal));
            if (marker is not null)
            {
                violations.Add(new TViolation(path, index + 1, marker, "Hardwiring", "driver line names the surface"));
            }
        }
    }

    private static void TAuditMasqueradingScan(SyntaxNode root, List<TViolation> violations)
    {
        foreach (TypeDeclarationSyntax type in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
        {
            if (TAuditBinder.TAuditSymbolRead(type) is not INamedTypeSymbol symbol)
            {
                continue;
            }

            for (INamedTypeSymbol? shape = symbol.BaseType; shape is not null; shape = shape.BaseType)
            {
                string name = shape.OriginalDefinition.ToDisplayString();
                if (TAuditStrictSetting.TAuditMasqueradingTypes.Contains(name, StringComparer.Ordinal))
                {
                    violations.Add(new TViolation(
                        type.SyntaxTree.FilePath,
                        type.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        type.Identifier.ValueText,
                        "Masquerading",
                        $"driver type derives from {name}"));
                    break;
                }
            }
        }
    }

    private static void TAuditDanglingScan(SyntaxNode root, IReadOnlySet<string> ids, List<TViolation> violations)
    {
        SemanticModel model = TAuditBinder.TAuditModelRead(root);
        foreach (InvocationExpressionSyntax call in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (model.GetSymbolInfo(call).Symbol is not IMethodSymbol method
                || method.ContainingType?.Name != TAuditStrictSetting.TAuditContractType)
            {
                continue;
            }

            foreach (ArgumentSyntax argument in call.ArgumentList.Arguments)
            {
                if (model.GetTypeInfo(argument.Expression).ConvertedType?.SpecialType != SpecialType.System_String)
                {
                    continue;
                }

                int line = argument.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                Optional<object?> constant = model.GetConstantValue(argument.Expression);
                if (constant is not { HasValue: true, Value: string id })
                {
                    violations.Add(new TViolation(
                        call.SyntaxTree.FilePath, line, argument.Expression.ToString(), "Dangling",
                        "contract ID is not a constant"));
                }
                else if (!ids.Contains(id))
                {
                    violations.Add(new TViolation(
                        call.SyntaxTree.FilePath, line, id, "Dangling",
                        "contract ID has no element or resource in the surface"));
                }
            }
        }
    }

    private static IReadOnlySet<string> TAuditIdRead(IEnumerable<string> markupPaths)
    {
        HashSet<string> ids = new(StringComparer.Ordinal);
        foreach (string path in markupPaths)
        {
            XDocument document;
            try
            {
                document = XDocument.Load(path);
            }
            catch (XmlException)
            {
                continue;
            }

            ids.UnionWith(document.Descendants()
                .SelectMany(element => element.Attributes())
                .Where(attribute => !attribute.IsNamespaceDeclaration
                    && TAuditStrictSetting.TAuditContractIds.Contains(attribute.Name.LocalName, StringComparer.Ordinal))
                .Select(attribute => attribute.Value));
        }

        return ids;
    }
}
