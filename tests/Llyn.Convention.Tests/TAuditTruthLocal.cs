using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static partial class TAuditTruthWalker
{
    private static void TAuditToggleCheck(
        TAuditTruthField field, MemberDeclarationSyntax scope, HashSet<string> writers, List<TViolation> violations)
    {
        List<SyntaxNode> writes = scope.DescendantNodes().OfType<IdentifierNameSyntax>()
            .Where(identifier => TAuditNameCheck(identifier, field.TFieldName)
                                 || (writers.Contains(identifier.Identifier.ValueText)
                                     && identifier.Parent is InvocationExpressionSyntax))
            .Select(TAuditReferenceRead)
            .Where(reference => reference.Parent is not MemberAccessExpressionSyntax)
            .Where(reference => TAuditWriteCheck(reference, out _) || reference.Parent is InvocationExpressionSyntax)
            .ToList();
        if (writes.Count < 2)
        {
            return;
        }

        bool toggled = scope.DescendantNodes()
            .Where(node => node is InvocationExpressionSyntax or BaseObjectCreationExpressionSyntax)
            .Where(node => TAuditCallRead((ExpressionSyntax)node) is not null)
            .Any(request => writes.Any(write => write.SpanStart < request.SpanStart)
                            && writes.Any(write => write.SpanStart > request.SpanStart));
        if (toggled)
        {
            SyntaxNode last = writes[^1];
            string through = last is IdentifierNameSyntax { Parent: InvocationExpressionSyntax } relay
                ? $" through {relay.Identifier.ValueText}"
                : string.Empty;
            violations.Add(new TViolation(
                last.SyntaxTree.FilePath,
                TAuditLineRead(last),
                field.TFieldName,
                "Guard",
                $"toggled around a request{through}"));
        }
    }

    private static HashSet<string> TAuditWriterRead(TAuditTruthField field, IReadOnlyList<TypeDeclarationSyntax> type)
    {
        HashSet<string> writers = new(StringComparer.Ordinal);
        foreach (MethodDeclarationSyntax method in type.SelectMany(part =>
                     part.DescendantNodes().OfType<MethodDeclarationSyntax>()))
        {
            if (method.Body is not { Statements.Count: > 0 } body || TAuditRequestCheck(body))
            {
                continue;
            }

            bool writes = body.DescendantNodes().OfType<IdentifierNameSyntax>()
                .Where(identifier => TAuditNameCheck(identifier, field.TFieldName))
                .Select(TAuditReferenceRead)
                .Where(reference => reference.Parent is not MemberAccessExpressionSyntax)
                .Any(reference => TAuditWriteCheck(reference, out _));
            if (writes)
            {
                writers.Add(method.Identifier.ValueText);
            }
        }

        return writers;
    }

    private static void TAuditBaseCheck(IReadOnlyList<TypeDeclarationSyntax> type, List<TViolation> violations)
    {
        foreach (TypeDeclarationSyntax part in type)
        {
            foreach (BaseTypeSyntax baseType in part.BaseList?.Types ?? [])
            {
                string? name = TAuditNameRead(baseType.Type);
                if (name is null
                    || !TAuditLogicCheck(name)
                    || TAuditTruthSetting.TAuditTruthHandles.Contains(name, StringComparer.Ordinal))
                {
                    continue;
                }

                violations.Add(new TViolation(
                    part.SyntaxTree.FilePath,
                    TAuditLineRead(baseType),
                    part.Identifier.ValueText,
                    "Mirror",
                    $"derives from {name}"));
            }
        }
    }

    private static void TAuditLocalScan(IReadOnlyList<TypeDeclarationSyntax> type, List<TViolation> violations)
    {
        foreach (MemberDeclarationSyntax scope in type.SelectMany(part => part.Members))
        {
            if (scope is BaseTypeDeclarationSyntax or FieldDeclarationSyntax)
            {
                continue;
            }

            HashSet<string> answered = TAuditAnsweredRead(scope);
            if (answered.Count == 0)
            {
                continue;
            }

            string member = TAuditMemberRead(scope);

            HashSet<string> seen = new(StringComparer.Ordinal);
            foreach (IdentifierNameSyntax identifier in scope.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                string name = identifier.Identifier.ValueText;
                if (!answered.Contains(name) || TAuditWriteCheck(identifier, out _))
                {
                    continue;
                }

                SyntaxNode carried = identifier;
                while (carried.Parent is MemberAccessExpressionSyntax { Expression: var owner } access
                       && owner == carried)
                {
                    carried = access;
                }

                (string TViolationKind, string TViolationReason)? sink = TAuditSinkRead(carried);
                if (sink is null)
                {
                    continue;
                }

                int line = TAuditLineRead(identifier);
                if (seen.Add($"{line}:{name}:{sink.Value.TViolationKind}"))
                {
                    violations.Add(new TViolation(
                        identifier.SyntaxTree.FilePath,
                        line,
                        $"{member}.{name}",
                        sink.Value.TViolationKind,
                        $"engine answer {sink.Value.TViolationReason} through local '{name}'"));
                }
            }
        }
    }

    private static HashSet<string> TAuditAnsweredRead(MemberDeclarationSyntax scope)
    {
        HashSet<string> answered = new(StringComparer.Ordinal);
        foreach (SyntaxNode node in scope.DescendantNodes())
        {
            switch (node)
            {
                case VariableDeclaratorSyntax { Initializer.Value: var value } declarator
                    when TAuditAnswerCheck(value):
                    answered.Add(declarator.Identifier.ValueText);
                    break;
                case AssignmentExpressionSyntax { Left: IdentifierNameSyntax local } assignment
                    when TAuditAnswerCheck(assignment.Right):
                    answered.Add(local.Identifier.ValueText);
                    break;
                case AssignmentExpressionSyntax assignment when TAuditAnswerCheck(assignment.Right):
                    TAuditDesignationAdd(assignment.Left, answered);
                    break;
                case ForEachStatementSyntax loop when TAuditAnswerCheck(loop.Expression):
                    answered.Add(loop.Identifier.ValueText);
                    break;
                case ForEachVariableStatementSyntax loop when TAuditAnswerCheck(loop.Expression):
                    TAuditDesignationAdd(loop.Variable, answered);
                    break;
                case IsPatternExpressionSyntax pattern when TAuditAnswerCheck(pattern.Expression):
                    TAuditDesignationAdd(pattern.Pattern, answered);
                    break;
                case InvocationExpressionSyntax call when TAuditCallRead(call) is not null:
                    foreach (ArgumentSyntax argument in call.ArgumentList.Arguments)
                    {
                        if (argument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword))
                        {
                            TAuditDesignationAdd(argument.Expression, answered);
                        }

                        foreach (ParameterSyntax parameter in argument.Expression switch
                                 {
                                     SimpleLambdaExpressionSyntax simple => [simple.Parameter],
                                     ParenthesizedLambdaExpressionSyntax full => full.ParameterList.Parameters,
                                     _ => (IEnumerable<ParameterSyntax>)[]
                                 })
                        {
                            answered.Add(parameter.Identifier.ValueText);
                        }
                    }

                    break;
            }
        }

        return answered;
    }

    private static void TAuditDesignationAdd(SyntaxNode node, HashSet<string> answered)
    {
        foreach (SingleVariableDesignationSyntax designation in
                 node.DescendantNodesAndSelf().OfType<SingleVariableDesignationSyntax>())
        {
            answered.Add(designation.Identifier.ValueText);
        }

        foreach (IdentifierNameSyntax name in node.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>())
        {
            if (name.Parent is TupleExpressionSyntax or ArgumentSyntax { Parent: TupleExpressionSyntax })
            {
                answered.Add(name.Identifier.ValueText);
            }
        }
    }

    private static string TAuditMemberRead(MemberDeclarationSyntax scope)
    {
        return scope switch
        {
            MethodDeclarationSyntax method => method.Identifier.ValueText,
            PropertyDeclarationSyntax property => property.Identifier.ValueText,
            ConstructorDeclarationSyntax constructor => constructor.Identifier.ValueText,
            EventDeclarationSyntax happening => happening.Identifier.ValueText,
            _ => scope.Kind().ToString()
        };
    }

    private static bool TAuditAnswerCheck(ExpressionSyntax value)
    {
        return value.DescendantNodesAndSelf().Any(node => node switch
        {
            InvocationExpressionSyntax or BaseObjectCreationExpressionSyntax
                => TAuditCallRead((ExpressionSyntax)node) is not null,
            MemberAccessExpressionSyntax access => TAuditLogicCheck(access.Name.Identifier.ValueText),
            MemberBindingExpressionSyntax binding => TAuditLogicCheck(binding.Name.Identifier.ValueText),
            _ => false
        });
    }
}
