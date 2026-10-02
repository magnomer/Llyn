using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static partial class TAuditTruthWalker
{
    private static readonly object TAuditGate = new();

    private static IReadOnlyList<SyntaxNode> TAuditRoots = [];

    private static Dictionary<ISymbol, List<IdentifierNameSyntax>> TAuditIndex = new(SymbolEqualityComparer.Default);

    private sealed record TAuditTruthField(
        HashSet<ISymbol> TFieldSymbols,
        string TFieldName,
        ITypeSymbol TFieldType,
        string TFieldPath,
        int TFieldLine,
        bool TFieldShared);

    public static IReadOnlyList<TViolation> TAuditRun(IReadOnlyList<string> sourcePaths)
    {
        lock (TAuditGate)
        {
            List<TViolation> violations = [];
            Dictionary<INamedTypeSymbol, List<TypeDeclarationSyntax>> parts = TAuditPartRead(sourcePaths);
            foreach (SyntaxNode root in TAuditRoots)
            {
                TAuditTamperingScan(root, violations);
                TAuditMisfiringScan(root, violations);
            }

            TAuditMismatchingScan(violations);
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

    public static IReadOnlySet<ISymbol> TAuditReaderRead(IReadOnlyList<string> sourcePaths)
    {
        lock (TAuditGate)
        {
            TAuditPartRead(sourcePaths);
            return new HashSet<ISymbol>(TAuditReaderNames.Concat(TAuditRelayNames), SymbolEqualityComparer.Default);
        }
    }

    private static Dictionary<INamedTypeSymbol, List<TypeDeclarationSyntax>> TAuditPartRead(
        IReadOnlyList<string> sourcePaths)
    {
        Dictionary<INamedTypeSymbol, List<TypeDeclarationSyntax>> parts = new(SymbolEqualityComparer.Default);
        TAuditRoots = TAuditBinder.TAuditWalkRead(sourcePaths).Where(TAuditBinder.TAuditWalkCheck).ToList();
        foreach (SyntaxNode root in TAuditRoots)
        {
            foreach (TypeDeclarationSyntax type in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (type.Ancestors().OfType<TypeDeclarationSyntax>().Any()
                    || TAuditBinder.TAuditSymbolRead(type) is not INamedTypeSymbol key)
                {
                    continue;
                }

                if (!parts.TryGetValue(key, out List<TypeDeclarationSyntax>? list))
                {
                    list = [];
                    parts[key] = list;
                }

                list.Add(type);
            }
        }

        TAuditIndex = new Dictionary<ISymbol, List<IdentifierNameSyntax>>(SymbolEqualityComparer.Default);
        foreach (IdentifierNameSyntax identifier in TAuditRoots.SelectMany(root =>
                     root.DescendantNodes(node => !TAuditNameofCheck(node)).OfType<IdentifierNameSyntax>()))
        {
            if (TAuditBinder.TAuditSymbolRead(identifier) is not { } symbol)
            {
                continue;
            }

            if (!TAuditIndex.TryGetValue(symbol, out List<IdentifierNameSyntax>? uses))
            {
                uses = [];
                TAuditIndex[symbol] = uses;
            }

            uses.Add(identifier);
        }

        List<TypeDeclarationSyntax> every = parts.Values.SelectMany(type => type).ToList();
        TAuditRelayRead(every);
        TAuditSendResolve(every);
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
                foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    if (TAuditBinder.TAuditSymbolRead(variable) is not IFieldSymbol symbol
                        || TAuditHandleCheck(symbol.Type))
                    {
                        continue;
                    }

                    HashSet<ISymbol> symbols = TAuditAliasRead(symbol, type);
                    if (!fixture || TAuditBinder.TAuditEngineCheck(symbol.Type) || TAuditFillCheck(symbols, type))
                    {
                        yield return new TAuditTruthField(
                            symbols,
                            TAuditBinder.TAuditLabelRead(symbol),
                            symbol.Type,
                            field.SyntaxTree.FilePath,
                            TAuditLineRead(variable),
                            symbol.IsStatic);
                    }
                }
            }

            foreach (ParameterSyntax parameter in part.ParameterList?.Parameters ?? [])
            {
                if (TAuditBinder.TAuditSymbolRead(parameter) is not IParameterSymbol symbol
                    || TAuditHandleCheck(symbol.Type))
                {
                    continue;
                }

                HashSet<ISymbol> symbols = new([symbol], SymbolEqualityComparer.Default);
                foreach (IPropertySymbol property in symbol.ContainingType?.GetMembers(symbol.Name)
                             .OfType<IPropertySymbol>() ?? [])
                {
                    symbols.Add(property.OriginalDefinition);
                }

                yield return new TAuditTruthField(
                    symbols,
                    TAuditBinder.TAuditLabelRead(symbol),
                    symbol.Type,
                    parameter.SyntaxTree.FilePath,
                    TAuditLineRead(parameter),
                    true);
            }

            foreach (PropertyDeclarationSyntax property in part.Members.OfType<PropertyDeclarationSyntax>())
            {
                bool settable = property.AccessorList?.Accessors.Any(accessor =>
                    accessor.IsKind(SyntaxKind.SetAccessorDeclaration)
                    || accessor.IsKind(SyntaxKind.InitAccessorDeclaration)) == true;
                if (!settable
                    || TAuditBinder.TAuditSymbolRead(property) is not IPropertySymbol symbol
                    || TAuditHandleCheck(symbol.Type))
                {
                    continue;
                }

                yield return new TAuditTruthField(
                    new HashSet<ISymbol>([symbol], SymbolEqualityComparer.Default),
                    TAuditBinder.TAuditLabelRead(symbol),
                    symbol.Type,
                    property.SyntaxTree.FilePath,
                    TAuditLineRead(property),
                    true);
            }
        }
    }

    private static bool TAuditFillCheck(HashSet<ISymbol> symbols, IReadOnlyList<TypeDeclarationSyntax> type)
    {
        return TAuditUseRead(symbols)
            .Where(identifier => TAuditInsideCheck(identifier, type))
            .Select(TAuditReferenceRead)
            .Any(reference => TAuditWriteCheck(reference, out _));
    }

    private static IEnumerable<IdentifierNameSyntax> TAuditUseRead(IEnumerable<ISymbol> symbols)
    {
        return symbols.SelectMany(symbol => TAuditIndex.GetValueOrDefault(symbol) ?? []);
    }

    private static bool TAuditInsideCheck(SyntaxNode node, IReadOnlyList<TypeDeclarationSyntax> type)
    {
        return type.Any(part => part.SyntaxTree == node.SyntaxTree && part.Span.Contains(node.Span));
    }

    private static bool TAuditFieldCheck(IdentifierNameSyntax identifier, HashSet<ISymbol> symbols)
    {
        ISymbol? symbol = TAuditBinder.TAuditSymbolRead(identifier);
        return symbol is not null && symbols.Contains(symbol);
    }

    private static void TAuditFieldCheck(
        TAuditTruthField field, IReadOnlyList<TypeDeclarationSyntax> type, List<TViolation> violations)
    {
        if (TAuditBinder.TAuditEngineCheck(field.TFieldType))
        {
            violations.Add(new TViolation(
                field.TFieldPath,
                field.TFieldLine,
                field.TFieldName,
                "Duplicating",
                $"holds a {field.TFieldType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}"));
        }
        else if (field.TFieldName.EndsWith(TAuditTruthSetting.TAuditStateSuffix, StringComparison.Ordinal))
        {
            violations.Add(new TViolation(
                field.TFieldPath, field.TFieldLine, field.TFieldName, "Duplicating", "names a state the engine owns"));
        }
        else if (field.TFieldType.SpecialType == SpecialType.System_Object)
        {
            violations.Add(new TViolation(
                field.TFieldPath, field.TFieldLine, field.TFieldName, "Duplicating", "holds an untyped value"));
        }

        HashSet<MemberDeclarationSyntax> scopes = [];
        HashSet<MemberDeclarationSyntax> toggles = [];
        HashSet<ISymbol> writers = TAuditWriterRead(field, type);
        SyntaxNode? engineWrite = null;
        List<SyntaxNode> plainWrites = [];
        List<(SyntaxNode, ExpressionSyntax?)> written = [];

        foreach (IdentifierNameSyntax identifier in TAuditUseRead(writers))
        {
            if (identifier.Parent is InvocationExpressionSyntax
                && TAuditInsideCheck(identifier, type)
                && identifier.FirstAncestorOrSelf<MemberDeclarationSyntax>() is { } caller
                && toggles.Add(caller))
            {
                TAuditToggleCheck(field, caller, writers, violations);
            }
        }

        foreach (IdentifierNameSyntax identifier in TAuditUseRead(field.TFieldSymbols)
                     .OrderBy(identifier => identifier.SyntaxTree.FilePath, StringComparer.Ordinal)
                     .ThenBy(identifier => identifier.SpanStart))
        {
            bool inside = TAuditInsideCheck(identifier, type)
                || TAuditBinder.TAuditSymbolRead(identifier) is IParameterSymbol { RefKind: not RefKind.None };
            if (!inside && !field.TFieldShared)
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
                if (inside && toggles.Add(scope))
                {
                    TAuditToggleCheck(field, scope, writers, violations);
                }

                if (inside
                    && reference.Parent is AssignmentExpressionSyntax
                    {
                        RawKind: (int)SyntaxKind.CoalesceAssignmentExpression
                    } cache
                    && TAuditRequestCheck(cache.Right))
                {
                    violations.Add(new TViolation(
                        reference.SyntaxTree.FilePath,
                        TAuditLineRead(reference),
                        field.TFieldName,
                        "Duplicating",
                        "caches a request"));
                }

                bool emptied = value is null && reference.Parent is MemberAccessExpressionSyntax;
                foreach (ExpressionSyntax? given in
                         TAuditOriginWalker.TAuditHelperRead(identifier, reference) ?? [value])
                {
                    string verdict = emptied ? "clear" : TAuditOriginWalker.TAuditWriterResolve(given);
                    if (verdict == "engine")
                    {
                        engineWrite ??= reference;
                    }
                    else if (verdict == "plain")
                    {
                        plainWrites.Add(reference);
                    }

                    if (verdict != "clear")
                    {
                        written.Add((reference, given));
                    }
                }

                continue;
            }

            if (!inside || !scopes.Add(scope))
            {
                continue;
            }

            TAuditScopeCheck(field, scope, violations);
        }

        if (engineWrite is not null && plainWrites.Count > 0
            && TAuditLookupWalker.TAuditLookupRead(written) is { } looked)
        {
            (engineWrite, plainWrites) = looked;
        }

        if (engineWrite is not null && plainWrites.FirstOrDefault() is { } plainWrite)
        {
            string where = engineWrite.SyntaxTree == plainWrite.SyntaxTree
                ? $"line {TAuditLineRead(engineWrite)}"
                : $"{Path.GetFileName(engineWrite.SyntaxTree.FilePath)}:{TAuditLineRead(engineWrite)}";
            string others = string.Join(", ", plainWrites.Skip(1).Select(write =>
                $"{Path.GetFileName(write.SyntaxTree.FilePath)}:{TAuditLineRead(write)}"));
            string also = others.Length > 0 ? $", also at {others}" : string.Empty;
            violations.Add(new TViolation(
                plainWrite.SyntaxTree.FilePath,
                TAuditLineRead(plainWrite),
                field.TFieldName,
                "Contesting",
                $"written by the engine at {where} and by the shell here{also}"));
        }
    }

    private static void TAuditScopeCheck(
        TAuditTruthField field, MemberDeclarationSyntax scope, List<TViolation> violations)
    {
        HashSet<ISymbol> tainted = TAuditTaintRead(field, scope);
        HashSet<string> seen = new(StringComparer.Ordinal);
        foreach (IdentifierNameSyntax identifier in scope.DescendantNodes(node => !TAuditNameofCheck(node))
                     .OfType<IdentifierNameSyntax>())
        {
            bool direct = TAuditFieldCheck(identifier, field.TFieldSymbols);
            if (!direct && !TAuditFieldCheck(identifier, tainted))
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

            string? via = TAuditBinder.TAuditSymbolRead(identifier) is IParameterSymbol { RefKind: not RefKind.None }
                ? "ref"
                : direct ? null : "local";
            string reason = via is null
                ? sink.Value.TViolationReason
                : $"{sink.Value.TViolationReason} through {via} '{identifier.Identifier.ValueText}'";
            int line = TAuditLineRead(reference);
            if (seen.Add($"{line}:{sink.Value.TViolationKind}"))
            {
                violations.Add(new TViolation(
                    reference.SyntaxTree.FilePath, line, field.TFieldName, sink.Value.TViolationKind, reason));
            }
        }
    }

    private static HashSet<ISymbol> TAuditTaintRead(TAuditTruthField field, MemberDeclarationSyntax scope)
    {
        HashSet<ISymbol> tainted = new(SymbolEqualityComparer.Default);
        foreach (SyntaxNode node in scope.DescendantNodes())
        {
            switch (node)
            {
                case VariableDeclaratorSyntax { Initializer: not null } declarator
                    when TAuditNameCheck(declarator.Initializer.Value, field.TFieldSymbols):
                    TAuditSymbolAdd(declarator, tainted);
                    break;
                case AssignmentExpressionSyntax { Left: IdentifierNameSyntax local } assignment
                    when TAuditBinder.TAuditSymbolRead(local) is ILocalSymbol
                         && TAuditNameCheck(assignment.Right, field.TFieldSymbols):
                    TAuditSymbolAdd(local, tainted);
                    break;
                case IsPatternExpressionSyntax pattern when TAuditNameCheck(pattern.Expression, field.TFieldSymbols):
                    foreach (SingleVariableDesignationSyntax designation in
                             pattern.Pattern.DescendantNodesAndSelf().OfType<SingleVariableDesignationSyntax>())
                    {
                        TAuditSymbolAdd(designation, tainted);
                    }

                    break;
            }
        }

        return tainted;
    }

    private static void TAuditSymbolAdd(SyntaxNode node, HashSet<ISymbol> symbols)
    {
        if (TAuditBinder.TAuditSymbolRead(node) is { } symbol)
        {
            symbols.Add(symbol);
        }
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

    internal static bool TAuditWriteCheck(SyntaxNode reference, out ExpressionSyntax? value)
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

    private static bool TAuditNameCheck(SyntaxNode node, HashSet<ISymbol> symbols)
    {
        return node.DescendantNodesAndSelf(parent => !TAuditNameofCheck(parent)).OfType<IdentifierNameSyntax>()
            .Any(identifier => TAuditFieldCheck(identifier, symbols));
    }

    private static int TAuditLineRead(SyntaxNode node)
    {
        return node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
    }
}
