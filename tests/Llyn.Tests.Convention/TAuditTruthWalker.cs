using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static partial class TAuditTruthWalker
{
    private static readonly object TAuditGate = new();

    private static IReadOnlyList<SyntaxNode> TAuditRoots = [];

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
                foreach (TAuditTruthField field in TAuditTruthField.TAuditFieldRead(type))
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
                    || TAuditBinderSymbol.TAuditSymbolRead(type) is not INamedTypeSymbol key)
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

        TAuditTruthReference.TAuditIndexResolve(TAuditRoots);
        List<TypeDeclarationSyntax> every = parts.Values.SelectMany(type => type).ToList();
        TAuditRelayRead(every);
        TAuditSendResolve(every);
        return parts;
    }

    private static void TAuditFieldCheck(
        TAuditTruthField field, IReadOnlyList<TypeDeclarationSyntax> type, List<TViolation> violations)
    {
        if (TAuditBinderSide.TAuditEngineCheck(field.TFieldType))
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
        List<(SyntaxNode, ExpressionSyntax?, string)> sorted = [];

        foreach (IdentifierNameSyntax identifier in TAuditTruthReference.TAuditUseRead(writers))
        {
            if (identifier.Parent is InvocationExpressionSyntax
                && TAuditTruthReference.TAuditInsideCheck(identifier, type)
                && identifier.FirstAncestorOrSelf<MemberDeclarationSyntax>() is { } caller
                && toggles.Add(caller))
            {
                TAuditToggleCheck(field, caller, writers, violations);
            }
        }

        foreach (IdentifierNameSyntax identifier in TAuditTruthReference.TAuditUseRead(field.TFieldSymbols)
                     .OrderBy(identifier => identifier.SyntaxTree.FilePath, StringComparer.Ordinal)
                     .ThenBy(identifier => identifier.SpanStart))
        {
            bool inside = TAuditTruthReference.TAuditInsideCheck(identifier, type)
                || TAuditBinderSymbol.TAuditSymbolRead(identifier) is IParameterSymbol { RefKind: not RefKind.None };
            if (!inside && !field.TFieldShared)
            {
                continue;
            }

            SyntaxNode reference = TAuditTruthReference.TAuditReferenceRead(identifier);
            MemberDeclarationSyntax? scope = reference.FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (scope is null || scope is FieldDeclarationSyntax)
            {
                continue;
            }

            if (TAuditTruthReference.TAuditWriteCheck(reference, out ExpressionSyntax? value))
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
                        TAuditTruthReference.TAuditLineRead(reference),
                        field.TFieldName,
                        "Duplicating",
                        "caches a request"));
                }

                bool emptied = value is null && reference.Parent is MemberAccessExpressionSyntax;
                foreach (ExpressionSyntax? given in
                         TAuditOriginHolder.TAuditHelperRead(identifier, reference) ?? [value])
                {
                    sorted.Add((reference, given, emptied ? "clear" : TAuditOriginWalker.TAuditWriterResolve(given)));
                }

                continue;
            }

            if (!inside || !scopes.Add(scope))
            {
                continue;
            }

            TAuditScopeCheck(field, scope, violations);
        }

        TAuditTruthContest.TAuditContestCheck(field, sorted, violations);
    }

    private static void TAuditScopeCheck(
        TAuditTruthField field, MemberDeclarationSyntax scope, List<TViolation> violations)
    {
        HashSet<ISymbol> tainted = TAuditTruthReference.TAuditTaintRead(field, scope);
        HashSet<string> seen = new(StringComparer.Ordinal);
        foreach (IdentifierNameSyntax identifier in scope.DescendantNodes(node => !TAuditNameofCheck(node))
                     .OfType<IdentifierNameSyntax>())
        {
            bool direct = TAuditTruthReference.TAuditFieldCheck(identifier, field.TFieldSymbols);
            if (!direct && !TAuditTruthReference.TAuditFieldCheck(identifier, tainted))
            {
                continue;
            }

            SyntaxNode reference = TAuditTruthReference.TAuditReferenceRead(identifier);
            if (TAuditTruthReference.TAuditWriteCheck(reference, out _))
            {
                continue;
            }

            (string TViolationKind, string TViolationReason)? sink = TAuditSinkRead(reference);
            if (sink is null)
            {
                continue;
            }

            string? via = TAuditBinderSymbol.TAuditSymbolRead(identifier)
                is IParameterSymbol { RefKind: not RefKind.None } ? "ref"
                : direct ? null : "local";
            string reason = via is null
                ? sink.Value.TViolationReason
                : $"{sink.Value.TViolationReason} through {via} '{identifier.Identifier.ValueText}'";
            int line = TAuditTruthReference.TAuditLineRead(reference);
            if (seen.Add($"{line}:{sink.Value.TViolationKind}"))
            {
                violations.Add(new TViolation(
                    reference.SyntaxTree.FilePath, line, field.TFieldName, sink.Value.TViolationKind, reason));
            }
        }
    }
}
