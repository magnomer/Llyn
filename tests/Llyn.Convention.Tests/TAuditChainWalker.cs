using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static class TAuditChainWalker
{
    public static IReadOnlyList<TAuditHit> TAuditRun()
    {
        Dictionary<string, HashSet<string>> inner = TAuditInnerRead();
        ConcurrentBag<List<TAuditHit>> bags = [];
        Parallel.ForEach(TAuditBinder.TAuditTrees, tree =>
        {
            string relative = TAuditBinder.TAuditRelativeRead(tree.FilePath);
            string? ring = TAuditBinder.TAuditRingRead(relative, TAuditChainSetting.TAuditChainReach.Keys);
            if (ring is not null)
            {
                bags.Add(TAuditTreeScan(TAuditBinder.TAuditModelRead(tree), relative, ring, inner));
            }
        });

        return bags.SelectMany(bag => bag)
            .OrderBy(hit => hit.TAuditHitPath, StringComparer.Ordinal)
            .ThenBy(hit => hit.TAuditHitLine)
            .ToList();
    }

    public static IReadOnlyList<TAuditHit> TAuditDeclaredRead()
    {
        List<TAuditHit> declared = [];
        foreach (SyntaxTree tree in TAuditBinder.TAuditTrees)
        {
            string relative = TAuditBinder.TAuditRelativeRead(tree.FilePath);
            string? ring = TAuditBinder.TAuditRingRead(relative, TAuditChainSetting.TAuditChainReach.Keys);
            if (ring is null)
            {
                continue;
            }

            IEnumerable<BaseTypeDeclarationSyntax> declarations = tree.GetRoot()
                .DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>();
            foreach (BaseTypeDeclarationSyntax declaration in declarations)
            {
                int line = declaration.Identifier.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                declared.Add(new TAuditHit(relative, line, ring, "declared", ring, declaration.Identifier.Text));
            }
        }

        return declared;
    }

    public static IReadOnlyDictionary<string, int> TAuditFileRead()
    {
        return TAuditBinder.TAuditTrees
            .Select(tree => TAuditBinder.TAuditRingRead(
                TAuditBinder.TAuditRelativeRead(tree.FilePath), TAuditChainSetting.TAuditChainReach.Keys))
            .Where(ring => ring is not null)
            .GroupBy(ring => ring!, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
    }

    public static IReadOnlyList<TAuditHit> TAuditSurfaceScan()
    {
        Dictionary<string, HashSet<string>> inner = TAuditInnerRead();
        List<TAuditHit> hits = [];
        HashSet<string> taken = new(StringComparer.Ordinal);
        foreach ((string pair, string[] surface) in TAuditChainSetting.TAuditChainSurface)
        {
            string ring = pair[..pair.IndexOf('>')];
            string neighbour = pair[(pair.IndexOf('>') + 1)..];
            if (!TAuditChainSetting.TAuditChainCut.Contains(ring, StringComparer.Ordinal))
            {
                continue;
            }

            IEnumerable<ISymbol> members = surface
                .SelectMany(name => TAuditBinder.TAuditCompilation.GetSymbolsWithName(name, SymbolFilter.Type))
                .OfType<INamedTypeSymbol>()
                .Where(type => TAuditBinder.TAuditSourceRead(type) is { } source
                    && TAuditBinder.TAuditRingRead(source, TAuditChainSetting.TAuditChainReach.Keys) == neighbour)
                .SelectMany(type => type.GetMembers())
                .Where(member => member.DeclaredAccessibility == Accessibility.Public && !member.IsImplicitlyDeclared);
            foreach (ISymbol member in members)
            {
                Location? location = member.Locations.FirstOrDefault(place => place.IsInSource);
                if (location is null)
                {
                    continue;
                }

                string relative = TAuditBinder.TAuditRelativeRead(location.SourceTree!.FilePath);
                int line = location.GetLineSpan().StartLinePosition.Line + 1;
                foreach (INamedTypeSymbol type in TAuditSignatureRead(member))
                {
                    string? source = TAuditBinder.TAuditSourceRead(type);
                    string? target = source is null
                        ? null
                        : TAuditBinder.TAuditRingRead(source, TAuditChainSetting.TAuditChainReach.Keys);
                    if (target is not null && inner[neighbour].Contains(target)
                        && taken.Add($"{relative}|{line}|{ring}|{target}|{type.Name}"))
                    {
                        hits.Add(new TAuditHit(relative, line, ring, "expose", target, type.Name));
                    }
                }
            }
        }

        return hits;
    }

    private static IEnumerable<INamedTypeSymbol> TAuditSignatureRead(ISymbol member)
    {
        IEnumerable<ITypeSymbol> types = member switch
        {
            IMethodSymbol { MethodKind: MethodKind.Ordinary or MethodKind.Constructor } method =>
                method.Parameters.Select(parameter => parameter.Type).Prepend(method.ReturnType),
            IPropertySymbol property => property.Parameters.Select(parameter => parameter.Type).Prepend(property.Type),
            IEventSymbol handler => [handler.Type],
            _ => [],
        };
        Stack<ITypeSymbol> pending = new(types);
        while (pending.Count > 0)
        {
            ITypeSymbol type = pending.Pop();
            if (type is IArrayTypeSymbol array)
            {
                pending.Push(array.ElementType);
            }
            else if (type is INamedTypeSymbol named)
            {
                yield return named.OriginalDefinition;
                foreach (ITypeSymbol argument in named.TypeArguments)
                {
                    pending.Push(argument);
                }
            }
        }
    }

    private static Dictionary<string, HashSet<string>> TAuditInnerRead()
    {
        Dictionary<string, HashSet<string>> inner = new(StringComparer.Ordinal);
        foreach ((string ring, string[] reach) in TAuditChainSetting.TAuditChainReach)
        {
            HashSet<string> seen = new(StringComparer.Ordinal);
            Queue<string> pending = new(reach);
            while (pending.Count > 0)
            {
                string name = pending.Dequeue();
                if (!seen.Add(name))
                {
                    continue;
                }

                foreach (string deeper in TAuditChainSetting.TAuditChainReach.GetValueOrDefault(name, []))
                {
                    pending.Enqueue(deeper);
                }
            }

            inner[ring] = seen;
        }

        return inner;
    }

    private static List<TAuditHit> TAuditTreeScan(
        SemanticModel model, string relative, string ring, Dictionary<string, HashSet<string>> inner)
    {
        List<TAuditHit> hits = [];
        HashSet<string> taken = new(StringComparer.Ordinal);
        bool isRoot = TAuditChainSetting.TAuditChainRoot.Contains(relative, StringComparer.OrdinalIgnoreCase);
        string[] reach = TAuditChainSetting.TAuditChainReach[ring];
        bool isCut = TAuditChainSetting.TAuditChainCut.Contains(ring, StringComparer.Ordinal);

        foreach (SimpleNameSyntax name in model.SyntaxTree.GetRoot().DescendantNodes().OfType<SimpleNameSyntax>())
        {
            if (TAuditBinder.TAuditHeaderCheck(name))
            {
                continue;
            }

            ISymbol? symbol = TAuditBinder.TAuditSymbolRead(model, name);
            if (symbol is null || TAuditBinder.TAuditTypeRead(symbol) is not INamedTypeSymbol type)
            {
                continue;
            }

            string? source = TAuditBinder.TAuditSourceRead(type);
            string? target = source is null
                ? null
                : TAuditBinder.TAuditRingRead(source, TAuditChainSetting.TAuditChainReach.Keys);
            if (target is null || target == ring)
            {
                continue;
            }

            string kind = isRoot ? "root"
                : reach.Contains(target, StringComparer.Ordinal) ? "neighbour"
                : !inner[ring].Contains(target) ? "outward"
                : isCut && !TAuditChainSetting.TAuditChainCut.Contains(target, StringComparer.Ordinal) ? "cross"
                : TAuditBinder.TAuditDataCheck(type) ? "carry"
                : "reach";
            int line = name.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            if (taken.Add($"{line}|{kind}|{target}|{type.Name}"))
            {
                hits.Add(new TAuditHit(relative, line, ring, kind, target, type.Name));
            }
        }

        return hits;
    }
}
