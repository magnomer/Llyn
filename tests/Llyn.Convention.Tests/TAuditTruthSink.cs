using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static partial class TAuditTruthWalker
{
    private static (string TViolationKind, string TViolationReason)? TAuditSinkRead(SyntaxNode reference)
    {
        if (reference.Parent is MemberAccessExpressionSyntax { Expression: var owner } && owner == reference
            || reference.Parent is ConditionalAccessExpressionSyntax { Expression: var target } && target == reference)
        {
            return null;
        }

        foreach (SyntaxNode ancestor in reference.Ancestors())
        {
            switch (ancestor)
            {
                case MemberDeclarationSyntax:
                    return null;
                case ArgumentSyntax { Parent.Parent: ExpressionSyntax call } argument
                    when TAuditCallRead(call) is string callee && TAuditHotCheck(callee, argument):
                    return ("Argument", $"passed to {callee}");
                case InitializerExpressionSyntax { Parent: WithExpressionSyntax }:
                    return ("Argument", "written into a record copy");
                case InitializerExpressionSyntax { Parent: BaseObjectCreationExpressionSyntax creation }
                    when TAuditCallRead(creation) is string built:
                    return ("Argument", $"written into new {built}");
                case IfStatementSyntax branch
                    when branch.Condition.Span.Contains(reference.Span) && TAuditGuardCheck(branch):
                    return ("Guard", "decides a request in an if");
                case ConditionalExpressionSyntax choice
                    when choice.Condition.Span.Contains(reference.Span)
                         && (TAuditRequestCheck(choice.WhenTrue) || TAuditRequestCheck(choice.WhenFalse)):
                    return ("Guard", "decides a request in a ternary");
                case SwitchStatementSyntax select
                    when select.Expression.Span.Contains(reference.Span) && TAuditRequestCheck(select):
                    return ("Guard", "decides a request in a switch");
                case SwitchExpressionSyntax arms
                    when arms.GoverningExpression.Span.Contains(reference.Span) && TAuditRequestCheck(arms):
                    return ("Guard", "decides a request in a switch expression");
            }
        }

        return null;
    }

    private static bool TAuditGuardCheck(IfStatementSyntax branch)
    {
        if (TAuditRequestCheck(branch.Statement)
            || (branch.Else is not null && TAuditRequestCheck(branch.Else)))
        {
            return true;
        }

        bool jump = branch.Statement.DescendantNodesAndSelf().Any(node =>
            node is ReturnStatementSyntax or ThrowStatementSyntax or ContinueStatementSyntax or BreakStatementSyntax);
        MemberDeclarationSyntax? scope = branch.FirstAncestorOrSelf<MemberDeclarationSyntax>();
        return jump && scope is not null && TAuditRequestCheck(scope);
    }

    private static bool TAuditRequestCheck(SyntaxNode node)
    {
        return node.DescendantNodesAndSelf().Any(child =>
            child is ExpressionSyntax call
            && (call is InvocationExpressionSyntax || call is BaseObjectCreationExpressionSyntax)
            && TAuditCallRead(call) is not null);
    }

    private static bool TAuditHotCheck(string callee, ArgumentSyntax argument)
    {
        if (TAuditLogicCheck(callee))
        {
            return true;
        }

        if (argument.NameColon is not null || argument.Parent is not BaseArgumentListSyntax list)
        {
            return false;
        }

        return TAuditHotNames.TryGetValue(callee, out HashSet<int>? hot)
               && hot.Contains(list.Arguments.IndexOf(argument));
    }

    private static string? TAuditCallRead(ExpressionSyntax call)
    {
        string? name = call switch
        {
            InvocationExpressionSyntax invocation => TAuditNameRead(invocation.Expression),
            ObjectCreationExpressionSyntax creation => TAuditNameRead(creation.Type),
            _ => null
        };
        return name is not null && (TAuditLogicCheck(name) || TAuditRelayNames.Contains(name)) ? name : null;
    }

    private static string? TAuditNameRead(ExpressionSyntax expression)
    {
        return expression switch
        {
            SimpleNameSyntax simple => simple.Identifier.ValueText,
            MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText,
            MemberBindingExpressionSyntax binding => binding.Name.Identifier.ValueText,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            NullableTypeSyntax nullable => TAuditNameRead(nullable.ElementType),
            _ => null
        };
    }
}
