using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static partial class TAuditTruthWalker
{
    private static readonly CSharpParseOptions TAuditSyntaxOptions = new(
        languageVersion: LanguageVersion.Preview,
        documentationMode: DocumentationMode.None,
        kind: SourceCodeKind.Regular);

    private static readonly object TAuditGate = new();

    private static List<SyntaxNode> TAuditRoots = [];

    private static readonly char[] TAuditTypeBreaks = ['<', '>', ',', '.', '[', ']', ' ', '(', ')'];

    private sealed record TAuditTruthField(
        string TFieldName,
        string TFieldType,
        string TFieldPath,
        int TFieldLine,
        bool TFieldShared);

    public static IReadOnlyList<TViolation> TAuditRun(IEnumerable<string> sourcePaths, IEnumerable<string> controls)
    {
        lock (TAuditGate)
        {
            List<TViolation> violations = [];
            TAuditControlNames = new HashSet<string>(controls, StringComparer.Ordinal);
            Dictionary<string, List<TypeDeclarationSyntax>> parts = TAuditPartRead(sourcePaths);
            foreach (SyntaxNode root in TAuditRoots)
            {
                TAuditMutationScan(root, violations);
                TAuditShapeScan(root, violations);
            }

            foreach (List<TypeDeclarationSyntax> type in parts.Values)
            {
                TAuditBaseCheck(type, violations);
                TAuditLocalScan(type, violations);
                TAuditSequenceScan(type, violations);
                foreach (TAuditTruthField field in TAuditFieldRead(type))
                {
                    TAuditFieldCheck(field, type, violations);
                }
            }

            return violations;
        }
    }

    public static IReadOnlySet<string> TAuditReaderRead(IEnumerable<string> sourcePaths)
    {
        lock (TAuditGate)
        {
            TAuditPartRead(sourcePaths);
            return new HashSet<string>(TAuditReaderNames.Concat(TAuditRelayNames), StringComparer.Ordinal);
        }
    }

    private static Dictionary<string, List<TypeDeclarationSyntax>> TAuditPartRead(IEnumerable<string> sourcePaths)
    {
        Dictionary<string, List<TypeDeclarationSyntax>> parts = new(StringComparer.Ordinal);
        TAuditRoots = [];
        foreach (string path in sourcePaths)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path), TAuditSyntaxOptions, path);
            SyntaxNode root = tree.GetRoot();
            TAuditRoots.Add(root);
            foreach (TypeDeclarationSyntax type in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (type.Ancestors().OfType<TypeDeclarationSyntax>().Any())
                {
                    continue;
                }

                string key = type.Identifier.ValueText;
                if (!parts.TryGetValue(key, out List<TypeDeclarationSyntax>? list))
                {
                    list = [];
                    parts[key] = list;
                }

                list.Add(type);
            }
        }

        TAuditRelayNames = TAuditRelayRead(parts.Values.SelectMany(type => type).ToList());
        TAuditSendNames = TAuditSendResolve(parts.Values.SelectMany(type => type).ToList());
        return parts;
    }

    private static IEnumerable<TAuditTruthField> TAuditFieldRead(IReadOnlyList<TypeDeclarationSyntax> type)
    {
        foreach (TypeDeclarationSyntax part in type.SelectMany(part =>
                     part.DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>()))
        {
            foreach (FieldDeclarationSyntax field in part.Members.OfType<FieldDeclarationSyntax>())
            {
                bool fixture = field.Modifiers.Any(modifier =>
                    modifier.IsKind(SyntaxKind.ReadOnlyKeyword)
                    || modifier.IsKind(SyntaxKind.ConstKeyword));
                string typeName = TAuditTypeRead(field.Declaration.Type);
                foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    string name = variable.Identifier.ValueText;
                    bool wired = TAuditWiredCheck(variable.Initializer?.Value);
                    if (!wired && (!fixture || TAuditFillCheck(name, type)))
                    {
                        yield return new TAuditTruthField(
                            name, typeName, field.SyntaxTree.FilePath, TAuditLineRead(variable), false);
                    }
                }
            }

            foreach (ParameterSyntax parameter in part.ParameterList?.Parameters ?? [])
            {
                if (parameter.Type is not null)
                {
                    yield return new TAuditTruthField(
                        parameter.Identifier.ValueText,
                        TAuditTypeRead(parameter.Type),
                        parameter.SyntaxTree.FilePath,
                        TAuditLineRead(parameter),
                        true);
                }
            }

            foreach (PropertyDeclarationSyntax property in part.Members.OfType<PropertyDeclarationSyntax>())
            {
                bool settable = property.AccessorList?.Accessors.Any(accessor =>
                    accessor.IsKind(SyntaxKind.SetAccessorDeclaration)
                    || accessor.IsKind(SyntaxKind.InitAccessorDeclaration)) == true;
                string name = property.Identifier.ValueText;
                if (settable && !TAuditWiredCheck(property.Initializer?.Value))
                {
                    yield return new TAuditTruthField(
                        name,
                        TAuditTypeRead(property.Type),
                        property.SyntaxTree.FilePath,
                        TAuditLineRead(property),
                        true);
                }
            }
        }
    }

    private static string TAuditTypeRead(TypeSyntax type)
    {
        return type is NullableTypeSyntax nullable ? nullable.ElementType.ToString() : type.ToString();
    }

    private static bool TAuditFillCheck(string name, IReadOnlyList<TypeDeclarationSyntax> type)
    {
        return type.SelectMany(part => part.DescendantNodes().OfType<IdentifierNameSyntax>())
            .Where(identifier => string.Equals(identifier.Identifier.ValueText, name, StringComparison.Ordinal))
            .Select(TAuditReferenceRead)
            .Any(reference => TAuditWriteCheck(reference, out _));
    }

    private static bool TAuditWiredCheck(ExpressionSyntax? value)
    {
        return value is PostfixUnaryExpressionSyntax
        {
            RawKind: (int)SyntaxKind.SuppressNullableWarningExpression,
            Operand: LiteralExpressionSyntax { RawKind: (int)SyntaxKind.NullLiteralExpression }
        };
    }

    private static void TAuditFieldCheck(
        TAuditTruthField field, IReadOnlyList<TypeDeclarationSyntax> type, List<TViolation> violations)
    {
        bool handle = TAuditTruthSetting.TAuditTruthHandles.Contains(field.TFieldType, StringComparer.Ordinal);
        if (!handle && field.TFieldType.Split(TAuditTypeBreaks, StringSplitOptions.RemoveEmptyEntries)
                .Any(TAuditLogicCheck))
        {
            violations.Add(new TViolation(
                field.TFieldPath, field.TFieldLine, field.TFieldName, "Mirror", $"holds a {field.TFieldType}"));
        }
        else if (field.TFieldName.EndsWith(TAuditTruthSetting.TAuditStateSuffix, StringComparison.Ordinal))
        {
            violations.Add(new TViolation(
                field.TFieldPath, field.TFieldLine, field.TFieldName, "Mirror", "names a state the engine owns"));
        }
        else if (string.Equals(field.TFieldType, "object", StringComparison.Ordinal))
        {
            violations.Add(new TViolation(
                field.TFieldPath, field.TFieldLine, field.TFieldName, "Mirror", "holds an untyped value"));
        }

        HashSet<MemberDeclarationSyntax> scopes = [];
        HashSet<MemberDeclarationSyntax> toggles = [];
        HashSet<string> writers = TAuditWriterRead(field, type);
        SyntaxNode? engineWrite = null;
        SyntaxNode? plainWrite = null;

        IEnumerable<SyntaxNode> homes = field.TFieldShared ? type.Concat<SyntaxNode>(TAuditRoots) : type;
        foreach (SyntaxNode part in homes)
        {
            foreach (IdentifierNameSyntax identifier in part.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                string name = identifier.Identifier.ValueText;
                if (!string.Equals(name, field.TFieldName, StringComparison.Ordinal))
                {
                    if (!handle && part is not CompilationUnitSyntax && writers.Contains(name)
                        && identifier.Parent is InvocationExpressionSyntax
                        && identifier.FirstAncestorOrSelf<MemberDeclarationSyntax>() is { } caller
                        && toggles.Add(caller))
                    {
                        TAuditToggleCheck(field, caller, writers, violations);
                    }

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
                    if (!handle && part is not CompilationUnitSyntax && toggles.Add(scope))
                    {
                        TAuditToggleCheck(field, scope, writers, violations);
                    }

                    if (reference.Parent is AssignmentExpressionSyntax
                        {
                            RawKind: (int)SyntaxKind.CoalesceAssignmentExpression
                        } cache
                        && TAuditRequestCheck(cache.Right))
                    {
                        violations.Add(new TViolation(
                            reference.SyntaxTree.FilePath,
                            TAuditLineRead(reference),
                            field.TFieldName,
                            "Mirror",
                            "caches a request"));
                    }

                    bool emptied = value is null && reference.Parent is MemberAccessExpressionSyntax;
                    switch (emptied ? "clear" : TAuditWriterResolve(value, scope))
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

                if (handle || part is CompilationUnitSyntax || !scopes.Add(scope))
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

        bool asked = value.DescendantNodesAndSelf().Any(node => node switch
        {
            MemberAccessExpressionSyntax access => TAuditLogicCheck(access.Name.Identifier.ValueText),
            MemberBindingExpressionSyntax binding => TAuditLogicCheck(binding.Name.Identifier.ValueText),
            InvocationExpressionSyntax call
                => TAuditCallRead(call) is not null
                   || (TAuditNameRead(call.Expression) is string callee && TAuditReaderNames.Contains(callee)),
            BaseObjectCreationExpressionSyntax creation => TAuditCallRead(creation) is not null,
            _ => false
        });
        if (asked)
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

    private static SyntaxNode TAuditReferenceRead(IdentifierNameSyntax identifier)
    {
        return identifier.Parent is MemberAccessExpressionSyntax
               {
                   Expression: ThisExpressionSyntax or IdentifierNameSyntax
               } access
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
            case ElementAccessExpressionSyntax { Parent: AssignmentExpressionSyntax slot } element
                when element.Expression == reference && slot.Left == element:
                value = slot.IsKind(SyntaxKind.SimpleAssignmentExpression) ? slot.Right : null;
                return true;
            case MemberAccessExpressionSyntax { Parent: InvocationExpressionSyntax fill } access
                when access.Expression == reference
                     && TAuditTruthSetting.TAuditFillVerbs.Contains(
                         access.Name.Identifier.ValueText, StringComparer.Ordinal):
                value = fill.ArgumentList.Arguments.LastOrDefault()?.Expression;
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
