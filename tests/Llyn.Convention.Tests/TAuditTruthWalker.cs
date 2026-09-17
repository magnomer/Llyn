using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static class TAuditTruthWalker
{
    private static readonly CSharpParseOptions TAuditSyntaxOptions = new(
        languageVersion: LanguageVersion.Preview,
        documentationMode: DocumentationMode.None,
        kind: SourceCodeKind.Regular);

    private sealed record TAuditTruthField(
        string TFieldName,
        string TFieldType,
        string TFieldPath,
        int TFieldLine);

    public static IReadOnlyList<TViolation> TAuditRun(IEnumerable<string> sourcePaths)
    {
        List<TViolation> violations = [];
        Dictionary<string, List<ClassDeclarationSyntax>> parts = new(StringComparer.Ordinal);
        foreach (string path in sourcePaths)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path), TAuditSyntaxOptions, path);
            SyntaxNode root = tree.GetRoot();
            TAuditMutationScan(root, violations);
            foreach (ClassDeclarationSyntax type in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                string key = type.Identifier.ValueText;
                if (!parts.TryGetValue(key, out List<ClassDeclarationSyntax>? list))
                {
                    list = [];
                    parts[key] = list;
                }

                list.Add(type);
            }
        }

        foreach (List<ClassDeclarationSyntax> type in parts.Values)
        {
            foreach (TAuditTruthField field in TAuditFieldRead(type))
            {
                TAuditFieldCheck(field, type, violations);
            }
        }

        return violations;
    }

    private static IEnumerable<TAuditTruthField> TAuditFieldRead(IReadOnlyList<ClassDeclarationSyntax> type)
    {
        foreach (ClassDeclarationSyntax part in type)
        {
            foreach (FieldDeclarationSyntax field in part.Members.OfType<FieldDeclarationSyntax>())
            {
                bool fixture = field.Modifiers.Any(modifier =>
                    modifier.IsKind(SyntaxKind.ReadOnlyKeyword)
                    || modifier.IsKind(SyntaxKind.ConstKeyword)
                    || modifier.IsKind(SyntaxKind.StaticKeyword));
                if (fixture)
                {
                    continue;
                }

                string typeName = field.Declaration.Type.ToString().TrimEnd('?');
                foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    string name = variable.Identifier.ValueText;
                    bool wired = variable.Initializer?.Value is PostfixUnaryExpressionSyntax
                    {
                        RawKind: (int)SyntaxKind.SuppressNullableWarningExpression,
                        Operand: LiteralExpressionSyntax { RawKind: (int)SyntaxKind.NullLiteralExpression }
                    };
                    if (!wired && name.StartsWith(TAuditTruthSetting.TAuditFieldPrefix, StringComparison.Ordinal))
                    {
                        yield return new TAuditTruthField(
                            name, typeName, field.SyntaxTree.FilePath, TAuditLineRead(variable));
                    }
                }
            }
        }
    }

    private static void TAuditFieldCheck(
        TAuditTruthField field, IReadOnlyList<ClassDeclarationSyntax> type, List<TViolation> violations)
    {
        bool handle = TAuditTruthSetting.TAuditTruthHandles.Contains(field.TFieldType, StringComparer.Ordinal);
        HashSet<MemberDeclarationSyntax> scopes = [];
        SyntaxNode? engineWrite = null;
        SyntaxNode? plainWrite = null;

        foreach (ClassDeclarationSyntax part in type)
        {
            foreach (IdentifierNameSyntax identifier in part.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                if (!string.Equals(identifier.Identifier.ValueText, field.TFieldName, StringComparison.Ordinal))
                {
                    continue;
                }

                SyntaxNode reference = TAuditReferenceRead(identifier);
                MemberDeclarationSyntax? scope = reference.FirstAncestorOrSelf<MemberDeclarationSyntax>();
                if (scope is null || scope is FieldDeclarationSyntax)
                {
                    continue;
                }

                if (TAuditWriteCheck(reference, out ExpressionSyntax? value))
                {
                    switch (TAuditWriterResolve(value, scope))
                    {
                        case "engine":
                            engineWrite ??= reference;
                            break;
                        case "plain":
                            plainWrite ??= reference;
                            break;
                    }

                    continue;
                }

                if (handle || !scopes.Add(scope))
                {
                    continue;
                }

                TAuditScopeCheck(field, scope, violations);
            }
        }

        if (engineWrite is not null && plainWrite is not null)
        {
            violations.Add(new TViolation(
                plainWrite.SyntaxTree.FilePath,
                TAuditLineRead(plainWrite),
                field.TFieldName,
                "Fork",
                $"written by the engine at line {TAuditLineRead(engineWrite)} and by the shell here"));
        }
    }

    private static void TAuditScopeCheck(
        TAuditTruthField field, MemberDeclarationSyntax scope, List<TViolation> violations)
    {
        HashSet<string> tainted = TAuditTaintRead(field.TFieldName, scope);
        HashSet<string> seen = new(StringComparer.Ordinal);
        foreach (IdentifierNameSyntax identifier in scope.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            string name = identifier.Identifier.ValueText;
            bool direct = string.Equals(name, field.TFieldName, StringComparison.Ordinal);
            if (!direct && !tainted.Contains(name))
            {
                continue;
            }

            SyntaxNode reference = TAuditReferenceRead(identifier);
            if (TAuditWriteCheck(reference, out _))
            {
                continue;
            }

            (string TViolationKind, string TViolationReason)? sink = TAuditSinkRead(reference);
            if (sink is null)
            {
                continue;
            }

            string reason = direct
                ? sink.Value.TViolationReason
                : $"{sink.Value.TViolationReason} through local '{name}'";
            int line = TAuditLineRead(reference);
            if (seen.Add($"{line}:{sink.Value.TViolationKind}"))
            {
                violations.Add(new TViolation(
                    reference.SyntaxTree.FilePath, line, field.TFieldName, sink.Value.TViolationKind, reason));
            }
        }
    }

    private static HashSet<string> TAuditTaintRead(string fieldName, MemberDeclarationSyntax scope)
    {
        HashSet<string> tainted = new(StringComparer.Ordinal);
        foreach (SyntaxNode node in scope.DescendantNodes())
        {
            switch (node)
            {
                case VariableDeclaratorSyntax { Initializer: not null } declarator
                    when TAuditNameCheck(declarator.Initializer.Value, fieldName):
                    tainted.Add(declarator.Identifier.ValueText);
                    break;
                case AssignmentExpressionSyntax { Left: IdentifierNameSyntax local } assignment
                    when !local.Identifier.ValueText.StartsWith('_')
                         && TAuditNameCheck(assignment.Right, fieldName):
                    tainted.Add(local.Identifier.ValueText);
                    break;
                case IsPatternExpressionSyntax pattern when TAuditNameCheck(pattern.Expression, fieldName):
                    foreach (SingleVariableDesignationSyntax designation in
                             pattern.Pattern.DescendantNodesAndSelf().OfType<SingleVariableDesignationSyntax>())
                    {
                        tainted.Add(designation.Identifier.ValueText);
                    }

                    break;
            }
        }

        return tainted;
    }

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
                case ArgumentSyntax { Parent.Parent: ExpressionSyntax call }
                    when TAuditCallRead(call) is string callee:
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

    private static string? TAuditCallRead(ExpressionSyntax call)
    {
        string? name = call switch
        {
            InvocationExpressionSyntax invocation => TAuditNameRead(invocation.Expression),
            ObjectCreationExpressionSyntax creation => TAuditNameRead(creation.Type),
            _ => null
        };
        return name is not null && TAuditLogicCheck(name) ? name : null;
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

    private static string TAuditWriterResolve(ExpressionSyntax? value, MemberDeclarationSyntax scope)
    {
        if (value is null)
        {
            return "plain";
        }

        if (value.IsKind(SyntaxKind.NullLiteralExpression)
            || value.IsKind(SyntaxKind.DefaultLiteralExpression)
            || value is DefaultExpressionSyntax)
        {
            return "clear";
        }

        if (value.DescendantNodesAndSelf().OfType<SimpleNameSyntax>()
            .Any(name => TAuditLogicCheck(name.Identifier.ValueText)))
        {
            return "engine";
        }

        HashSet<string> names = new(
            value.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Select(name => name.Identifier.ValueText),
            StringComparer.Ordinal);
        return TAuditSourceCheck(names, scope) ? "engine" : "plain";
    }

    private static bool TAuditSourceCheck(HashSet<string> names, MemberDeclarationSyntax scope)
    {
        foreach (SyntaxNode node in scope.DescendantNodes())
        {
            bool logic = node switch
            {
                ParameterSyntax { Type: not null } parameter
                    when names.Contains(parameter.Identifier.ValueText)
                    => TAuditNameRead(parameter.Type) is string typeName && TAuditLogicCheck(typeName),
                VariableDeclaratorSyntax { Initializer: not null } declarator
                    when names.Contains(declarator.Identifier.ValueText)
                    => declarator.Initializer.Value.DescendantNodesAndSelf().OfType<SimpleNameSyntax>()
                        .Any(name => TAuditLogicCheck(name.Identifier.ValueText)),
                _ => false
            };
            if (logic)
            {
                return true;
            }
        }

        return false;
    }

    private static void TAuditMutationScan(SyntaxNode root, List<TViolation> violations)
    {
        foreach (AssignmentExpressionSyntax assignment in root.DescendantNodes().OfType<AssignmentExpressionSyntax>())
        {
            if (!assignment.IsKind(SyntaxKind.SimpleAssignmentExpression)
                || assignment.Parent is InitializerExpressionSyntax
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
    }

    private static SyntaxNode TAuditReferenceRead(IdentifierNameSyntax identifier)
    {
        return identifier.Parent is MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } access
               && access.Name == identifier
            ? access
            : identifier;
    }

    private static bool TAuditWriteCheck(SyntaxNode reference, out ExpressionSyntax? value)
    {
        value = null;
        switch (reference.Parent)
        {
            case AssignmentExpressionSyntax assignment when assignment.Left == reference:
                value = assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) ? assignment.Right : null;
                return true;
            case PrefixUnaryExpressionSyntax or PostfixUnaryExpressionSyntax:
                return reference.Parent.IsKind(SyntaxKind.PreIncrementExpression)
                       || reference.Parent.IsKind(SyntaxKind.PreDecrementExpression)
                       || reference.Parent.IsKind(SyntaxKind.PostIncrementExpression)
                       || reference.Parent.IsKind(SyntaxKind.PostDecrementExpression);
            case ArgumentSyntax argument:
                return argument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword)
                       || argument.RefKindKeyword.IsKind(SyntaxKind.RefKeyword);
            default:
                return false;
        }
    }

    private static bool TAuditNameCheck(SyntaxNode node, string name)
    {
        return node.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
            .Any(identifier => string.Equals(identifier.Identifier.ValueText, name, StringComparison.Ordinal));
    }

    private static bool TAuditLogicCheck(string name)
    {
        return name.Length >= 2 && name[0] == 'L' && char.IsUpper(name[1]);
    }

    private static int TAuditLineRead(SyntaxNode node)
    {
        return node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
    }
}
