using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static partial class TAuditTruthWalker
{
    private static HashSet<string> TAuditControlNames = [];

    private static void TAuditShapeScan(SyntaxNode root, List<TViolation> violations)
    {
        HashSet<string> seen = new(StringComparer.Ordinal);
        foreach (SyntaxNode node in root.DescendantNodes())
        {
            (string TViolationName, string TViolationReason)? hit = node switch
            {
                IfStatementSyntax branch when TAuditControlRead(branch.Condition) is string control
                                              && TAuditGuardCheck(branch)
                    => (control, "control decides a request in an if"),
                ConditionalExpressionSyntax choice when TAuditControlRead(choice.Condition) is string control
                                                        && (TAuditRequestCheck(choice.WhenTrue)
                                                            || TAuditRequestCheck(choice.WhenFalse))
                    => (control, "control decides a request in a ternary"),
                AssignmentExpressionSyntax
                    {
                        RawKind: (int)SyntaxKind.AddAssignmentExpression,
                        Left: MemberAccessExpressionSyntax clock
                    } wired when TAuditClockCheck(clock.Expression, root) && TAuditDriveCheck(wired.Right)
                    => (clock.Expression.ToString(), "a clock drives a request"),
                MethodDeclarationSyntax handler when TAuditDeafRead(handler) is string bulletin
                    => (handler.Identifier.ValueText, $"handles the bulletin '{bulletin}' without reading it"),
                LambdaExpressionSyntax deaf when TAuditLambdaCheck(deaf)
                    => (TAuditTruthSetting.TAuditObserverType, "handles a bulletin without reading it"),
                _ => null
            };
            if (hit is null)
            {
                continue;
            }

            int line = TAuditLineRead(node);
            if (seen.Add($"{line}:{hit.Value.TViolationName}"))
            {
                violations.Add(new TViolation(
                    root.SyntaxTree.FilePath, line, hit.Value.TViolationName, "Shape", hit.Value.TViolationReason));
            }
        }
    }

    private static string? TAuditControlRead(ExpressionSyntax condition)
    {
        foreach (MemberAccessExpressionSyntax access in condition.DescendantNodesAndSelf()
                     .OfType<MemberAccessExpressionSyntax>())
        {
            string? owner = access.Expression switch
            {
                IdentifierNameSyntax name => name.Identifier.ValueText,
                MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } own => own.Name.Identifier.ValueText,
                _ => null
            };
            if (owner is not null && TAuditControlNames.Contains(owner))
            {
                return owner;
            }
        }

        return null;
    }

    private static bool TAuditClockCheck(ExpressionSyntax clock, SyntaxNode root)
    {
        string name = clock switch
        {
            IdentifierNameSyntax bare => bare.Identifier.ValueText,
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } own => own.Name.Identifier.ValueText,
            _ => string.Empty
        };
        if (name.Length == 0)
        {
            return false;
        }

        return root.DescendantNodes().OfType<VariableDeclarationSyntax>()
            .Where(declaration => declaration.Variables.Any(variable =>
                string.Equals(variable.Identifier.ValueText, name, StringComparison.Ordinal)))
            .Select(declaration => TAuditNameRead(declaration.Type))
            .Any(typeName => typeName is not null
                             && TAuditTruthSetting.TAuditClockTypes.Contains(typeName, StringComparer.Ordinal));
    }

    private static bool TAuditLambdaCheck(LambdaExpressionSyntax lambda)
    {
        if (lambda.Parent is not ArgumentSyntax { Parent.Parent: ObjectCreationExpressionSyntax creation }
            || TAuditNameRead(creation.Type) != TAuditTruthSetting.TAuditObserverType)
        {
            return false;
        }

        ParameterSyntax? parameter = lambda switch
        {
            SimpleLambdaExpressionSyntax simple => simple.Parameter,
            ParenthesizedLambdaExpressionSyntax full => full.ParameterList.Parameters.FirstOrDefault(),
            _ => null
        };
        if (parameter is null)
        {
            return false;
        }

        string name = parameter.Identifier.ValueText;
        return name == "_" || !TAuditNameCheck(lambda.Body, name);
    }

    private static bool TAuditDriveCheck(ExpressionSyntax handler)
    {
        return TAuditRequestCheck(handler)
               || handler.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                   .Any(name => TAuditRelayNames.Contains(name.Identifier.ValueText));
    }

    private static string? TAuditDeafRead(MethodDeclarationSyntax handler)
    {
        foreach (ParameterSyntax parameter in handler.ParameterList.Parameters)
        {
            if (parameter.Type is null || TAuditNameRead(parameter.Type) != TAuditTruthSetting.TAuditBulletinType)
            {
                continue;
            }

            string name = parameter.Identifier.ValueText;
            SyntaxNode? body = (SyntaxNode?)handler.Body ?? handler.ExpressionBody;
            if (body is not null && !TAuditNameCheck(body, name))
            {
                return name;
            }
        }

        return null;
    }
}
