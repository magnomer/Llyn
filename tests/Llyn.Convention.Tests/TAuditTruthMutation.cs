using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static partial class TAuditTruthWalker
{
    private static void TAuditMutationScan(SyntaxNode root, List<TViolation> violations)
    {
        foreach (AssignmentExpressionSyntax assignment in root.DescendantNodes().OfType<AssignmentExpressionSyntax>())
        {
            if (!assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                continue;
            }

            if (assignment.Left is TupleExpressionSyntax tuple
                && tuple.Arguments.Any(argument => argument.Expression is ElementAccessExpressionSyntax))
            {
                ElementAccessExpressionSyntax slot = tuple.Arguments
                    .Select(argument => argument.Expression)
                    .OfType<ElementAccessExpressionSyntax>()
                    .First();
                violations.Add(new TViolation(
                    root.SyntaxTree.FilePath,
                    TAuditLineRead(assignment),
                    slot.Expression.ToString(),
                    "Mutation",
                    "reorders a collection with a swap"));
                continue;
            }

            if (assignment.Parent is InitializerExpressionSyntax { Parent: WithExpressionSyntax }
                && assignment.Left is IdentifierNameSyntax field
                && TAuditLogicCheck(field.Identifier.ValueText))
            {
                violations.Add(new TViolation(
                    root.SyntaxTree.FilePath,
                    TAuditLineRead(assignment),
                    field.Identifier.ValueText,
                    "Mutation",
                    "overrides a logic member in a record copy"));
                continue;
            }

            if (assignment.Left is ElementAccessExpressionSyntax { Expression: var rows }
                && TAuditNameRead(rows) is string held
                && TAuditStoreCheck(held, assignment))
            {
                violations.Add(new TViolation(
                    root.SyntaxTree.FilePath,
                    TAuditLineRead(assignment),
                    rows.ToString(),
                    "Mutation",
                    "overwrites a slot of a row store"));
                continue;
            }

            if (assignment.Parent is InitializerExpressionSyntax
                || assignment.Left is not MemberAccessExpressionSyntax target
                || !TAuditLogicCheck(target.Name.Identifier.ValueText))
            {
                continue;
            }

            violations.Add(new TViolation(
                root.SyntaxTree.FilePath,
                TAuditLineRead(assignment),
                target.ToString(),
                "Mutation",
                "assigns a logic member from the shell"));
        }

        foreach (InvocationExpressionSyntax call in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (call.Expression is not MemberAccessExpressionSyntax access
                || !TAuditTruthSetting.TAuditOrderVerbs.Contains(
                    access.Name.Identifier.ValueText, StringComparer.Ordinal)
                || TAuditNameRead(access.Expression) is not string rows
                || !TAuditStoreCheck(rows, call))
            {
                continue;
            }

            violations.Add(new TViolation(
                root.SyntaxTree.FilePath,
                TAuditLineRead(call),
                access.Expression.ToString(),
                "Mutation",
                $"reorders a collection with {access.Name.Identifier.ValueText}"));
        }
    }

    private static bool TAuditStoreCheck(string name, SyntaxNode site)
    {
        foreach (SyntaxNode ancestor in site.Ancestors())
        {
            TypeSyntax? declared = ancestor.DescendantNodes().Select(node => node switch
            {
                VariableDeclarationSyntax declaration
                    when declaration.Variables.Any(variable =>
                        string.Equals(variable.Identifier.ValueText, name, StringComparison.Ordinal))
                    => declaration.Type,
                ParameterSyntax parameter
                    when string.Equals(parameter.Identifier.ValueText, name, StringComparison.Ordinal)
                    => parameter.Type,
                PropertyDeclarationSyntax property
                    when string.Equals(property.Identifier.ValueText, name, StringComparison.Ordinal)
                    => property.Type,
                _ => null
            }).FirstOrDefault(type => type is not null);
            if (declared is null)
            {
                continue;
            }

            string[] names = TAuditTypeRead(declared).Split(TAuditTypeBreaks, StringSplitOptions.RemoveEmptyEntries);
            return names.Skip(1).Any(part => TAuditLogicCheck(part) || TAuditShellCheck(part) || part == "object");
        }

        return true;
    }

    private static bool TAuditShellCheck(string name)
    {
        return name.Length >= 2 && name[0] == 'P' && char.IsUpper(name[1]);
    }
}
