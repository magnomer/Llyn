using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static class TAuditBorderWalker
{
    public static IReadOnlyList<TAuditHit> TAuditRun()
    {
        Dictionary<string, HashSet<string>> inner = TAuditInnerRead();
        ConcurrentBag<List<TAuditHit>> bags = [];
        Parallel.ForEach(TAuditBinder.TAuditTrees, tree =>
        {
            string relative = TAuditBinder.TAuditRelativeRead(tree.FilePath);
            string? ring = TAuditBinder.TAuditRingRead(relative, TAuditBorderSetting.TAuditBorderNeighbour.Keys);
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

    public static IReadOnlyList<TAuditHit> TAuditOfferScan()
    {
        Dictionary<string, HashSet<string>> inner = TAuditInnerRead();
        List<TAuditHit> hits = [];
        HashSet<string> taken = new(StringComparer.Ordinal);
        foreach ((string pair, string[] offer) in TAuditEngineSetting.TAuditEngineOffer
                     .Concat(TAuditDeportmentSetting.TAuditDeportmentOffer)
                     .Concat(TAuditDemeanorSetting.TAuditDemeanorOffer))
        {
            string ring = pair[..pair.IndexOf('>')];
            string neighbour = pair[(pair.IndexOf('>') + 1)..];
            if (!TAuditBorderSetting.TAuditBorderCut.Contains(ring, StringComparer.Ordinal))
            {
                continue;
            }

            IEnumerable<ISymbol> members = offer
                .SelectMany(name => TAuditBinder.TAuditCompilation.GetSymbolsWithName(name, SymbolFilter.Type))
                .OfType<INamedTypeSymbol>()
                .Where(type => TAuditBinderSymbol.TAuditSourceRead(type) is { } source
                    && TAuditBinder.TAuditRingRead(source, TAuditBorderSetting.TAuditBorderNeighbour.Keys) == neighbour)
                .SelectMany(TAuditPublicRead)
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
                    string? source = TAuditBinderSymbol.TAuditSourceRead(type);
                    string? target = source is null
                        ? null
                        : TAuditBinder.TAuditRingRead(source, TAuditBorderSetting.TAuditBorderNeighbour.Keys);
                    if (target is not null && inner[neighbour].Contains(target)
                        && taken.Add($"{relative}|{line}|{ring}|{target}|{type.Name}"))
                    {
                        hits.Add(new TAuditHit(relative, line, ring, "Leaking", target, type.Name));
                    }
                }
            }
        }

        return hits;
    }

    public static IReadOnlyList<TAuditHit> TAuditDriftScan()
    {
        List<(string TAuditDriftName, string? TAuditDriftRing, bool TAuditDriftPublic)> types =
            TAuditBinder.TAuditCompilation.GetSymbolsWithName(_ => true, SymbolFilter.Type)
            .OfType<INamedTypeSymbol>()
            .Select(type => (type.Name,
                TAuditBinderSymbol.TAuditSourceRead(type) is { } source
                    ? TAuditBinder.TAuditRingRead(source, TAuditBorderSetting.TAuditBorderNeighbour.Keys)
                    : null,
                TAuditPublicCheck(type)))
            .ToList();
        Dictionary<string, string[]> offers = TAuditEngineSetting.TAuditEngineOffer
            .Concat(TAuditDeportmentSetting.TAuditDeportmentOffer)
            .Concat(TAuditDemeanorSetting.TAuditDemeanorOffer)
            .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
        return TAuditDriftRead(
            offers, TAuditBorderSetting.TAuditBorderCut, TAuditBorderSetting.TAuditOfferPrefix, types);
    }

    public static IReadOnlyList<TAuditHit> TAuditDriftRead(
        IReadOnlyDictionary<string, string[]> offers,
        IReadOnlyCollection<string> cut,
        IReadOnlyDictionary<string, string[]> prefixes,
        IReadOnlyList<(string TAuditDriftName, string? TAuditDriftRing, bool TAuditDriftPublic)> types)
    {
        Dictionary<string, List<(string TAuditDriftName, string? TAuditDriftRing, bool TAuditDriftPublic)>> typesOf =
            types.GroupBy(type => type.TAuditDriftName, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
        List<TAuditHit> hits = [];
        foreach ((string pair, string[] offer) in offers)
        {
            string ring = pair[..pair.IndexOf('>')];
            string neighbour = pair[(pair.IndexOf('>') + 1)..];
            string where = "src/" + neighbour;
            string[] allowed = prefixes.GetValueOrDefault(neighbour, []);
            HashSet<string> listed = new(offer, StringComparer.Ordinal);
            foreach (string entry in offer.Distinct(StringComparer.Ordinal))
            {
                List<(string TAuditDriftName, string? TAuditDriftRing, bool TAuditDriftPublic)> found =
                    typesOf.GetValueOrDefault(entry, []);
                string? clause = found.Count == 0 ? "(a) resolves to no type"
                    : found.Count > 1 ? $"(a) resolves to {found.Count} types"
                    : found[0].TAuditDriftRing != neighbour
                        ? $"(a) is declared in {found[0].TAuditDriftRing ?? "no ring"}"
                    : !found[0].TAuditDriftPublic ? "(a) is not public"
                    : !allowed.Any(prefix => entry.StartsWith(prefix, StringComparison.Ordinal))
                        ? $"(a) lacks the prefix {string.Join(", ", allowed)}"
                    : null;
                if (clause is not null)
                {
                    hits.Add(new TAuditHit(where, 0, ring, "Drifting", neighbour, $"{entry} {clause}"));
                }
            }

            if (cut.Contains(ring, StringComparer.Ordinal))
            {
                IEnumerable<string> unlisted = typesOf
                    .Where(item => item.Value.Any(type => type.TAuditDriftRing == neighbour && type.TAuditDriftPublic))
                    .Select(item => item.Key)
                    .Where(name => !listed.Contains(name))
                    .Order(StringComparer.Ordinal);
                foreach (string name in unlisted)
                {
                    hits.Add(new TAuditHit(where, 0, ring, "Drifting", neighbour,
                        $"{name} (b) is public in {neighbour} but not offered"));
                }
            }

            foreach ((string other, string[] otherOffer) in offers
                         .Where(item => item.Key != pair && item.Key[(item.Key.IndexOf('>') + 1)..] == neighbour))
            {
                foreach (string name in otherOffer.Distinct(StringComparer.Ordinal)
                             .Where(name => !listed.Contains(name)))
                {
                    hits.Add(new TAuditHit(where, 0, ring, "Drifting", neighbour,
                        $"{name} (c) is offered by {other} but not here"));
                }
            }
        }

        return hits
            .OrderBy(hit => hit.TAuditHitRing, StringComparer.Ordinal)
            .ThenBy(hit => hit.TAuditHitTarget, StringComparer.Ordinal)
            .ThenBy(hit => hit.TAuditHitName, StringComparer.Ordinal)
            .ToList();
    }

    public static IReadOnlyList<TAuditHit> TAuditSealScan()
    {
        Dictionary<string, HashSet<string>> inner = TAuditInnerRead();
        List<TAuditHit> hits = [];
        HashSet<string> taken = new(StringComparer.Ordinal);
        foreach ((string ring, string[] prefixes) in TAuditBorderSetting.TAuditUnsealingPrefix)
        {
            if (TAuditBorderSetting.TAuditBorderNeighbour.GetValueOrDefault(ring) is not [string neighbour])
            {
                continue;
            }

            IEnumerable<ISymbol> members = TAuditBinder.TAuditCompilation
                .GetSymbolsWithName(name => TAuditSealCheck(name, prefixes), SymbolFilter.Type)
                .OfType<INamedTypeSymbol>()
                .Where(type => type.ContainingType is null && type.DeclaredAccessibility == Accessibility.Public
                    && TAuditBinderSymbol.TAuditSourceRead(type) is { } source
                    && TAuditBinder.TAuditRingRead(source, TAuditBorderSetting.TAuditBorderNeighbour.Keys) == ring)
                .SelectMany(TAuditPublicRead)
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
                    string? source = TAuditBinderSymbol.TAuditSourceRead(type);
                    string? target = source is null
                        ? null
                        : TAuditBinder.TAuditRingRead(source, TAuditBorderSetting.TAuditBorderNeighbour.Keys);
                    if (target is not null && inner[neighbour].Contains(target)
                        && taken.Add($"{relative}|{line}|{ring}|{target}|{type.Name}"))
                    {
                        hits.Add(new TAuditHit(relative, line, ring, "Unsealing", target, type.Name));
                    }
                }
            }
        }

        return hits;
    }

    public static IReadOnlyList<TAuditHit> TAuditLingerScan()
    {
        const string conduct = "Llyn.Conduct";
        HashSet<string> below = TAuditInnerRead().GetValueOrDefault(conduct, []);
        HashSet<string> removed = new(StringComparer.Ordinal);
        List<(TAuditHit TAuditLingerHit, string TAuditLingerKey)> subscribed = [];
        foreach (SyntaxTree tree in TAuditBinder.TAuditTrees)
        {
            string relative = TAuditBinder.TAuditRelativeRead(tree.FilePath);
            if (TAuditBinder.TAuditRingRead(relative, TAuditBorderSetting.TAuditBorderNeighbour.Keys) != conduct)
            {
                continue;
            }

            SemanticModel model = TAuditBinder.TAuditModelRead(tree);
            foreach (AssignmentExpressionSyntax assignment in
                     tree.GetRoot().DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                bool adding = assignment.IsKind(SyntaxKind.AddAssignmentExpression);
                if ((!adding && !assignment.IsKind(SyntaxKind.SubtractAssignmentExpression))
                    || model.GetSymbolInfo(assignment.Left).Symbol is not IEventSymbol handled
                    || assignment.FirstAncestorOrSelf<TypeDeclarationSyntax>() is not { } holder
                    || model.GetDeclaredSymbol(holder) is not INamedTypeSymbol owner)
                {
                    continue;
                }

                IEventSymbol handledEvent = handled.OriginalDefinition;
                INamedTypeSymbol declaring = handledEvent.ContainingType.OriginalDefinition;
                if (SymbolEqualityComparer.Default.Equals(declaring, owner.OriginalDefinition))
                {
                    continue;
                }

                ExpressionSyntax handler = assignment.Right;
                while (handler is ParenthesizedExpressionSyntax parenthesized)
                {
                    handler = parenthesized.Expression;
                }

                bool anonymous = handler is AnonymousFunctionExpressionSyntax;
                SymbolInfo handlerInfo = model.GetSymbolInfo(handler);
                ISymbol? handlerSymbol = anonymous
                    ? null
                    : (handlerInfo.Symbol ?? handlerInfo.CandidateSymbols.FirstOrDefault())?.OriginalDefinition;
                string key = owner.OriginalDefinition.ToDisplayString() + "|" + handledEvent.ToDisplayString() + "|"
                    + (handlerSymbol?.ToDisplayString() ?? handler.ToString());
                if (!adding)
                {
                    if (!anonymous)
                    {
                        removed.Add(key);
                    }

                    continue;
                }

                string? source = TAuditBinderSymbol.TAuditSourceRead(declaring);
                string? target = source is null
                    ? null
                    : TAuditBinder.TAuditRingRead(source, TAuditBorderSetting.TAuditBorderNeighbour.Keys);
                if (target is null || !below.Contains(target))
                {
                    continue;
                }

                int line = assignment.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                string label = anonymous ? "(lambda)" : handlerSymbol?.Name ?? "(expression)";
                TAuditHit hit = new(relative, line, conduct, "Lingering", target,
                    $"{owner.Name} {declaring.Name}.{handledEvent.Name} {label}");
                subscribed.Add((hit, anonymous ? "" : key));
            }
        }

        HashSet<string> lingered = new(StringComparer.Ordinal);
        return subscribed
            .Where(item => item.TAuditLingerKey.Length == 0 || !removed.Contains(item.TAuditLingerKey))
            .Select(item => item.TAuditLingerHit)
            .Where(hit => lingered.Add($"{hit.TAuditHitPath}|{hit.TAuditHitLine}|{hit.TAuditHitName}"))
            .OrderBy(hit => hit.TAuditHitPath, StringComparer.Ordinal)
            .ThenBy(hit => hit.TAuditHitLine)
            .ToList();
    }

    private static bool TAuditSealCheck(string name, string[] prefixes)
    {
        return prefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal));
    }

    private static bool TAuditPublicCheck(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility != Accessibility.Public)
            {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<ISymbol> TAuditPublicRead(INamedTypeSymbol type)
    {
        foreach (ISymbol member in type.GetMembers())
        {
            yield return member;
            if (member is INamedTypeSymbol { DeclaredAccessibility: Accessibility.Public } nested)
            {
                foreach (ISymbol inner in TAuditPublicRead(nested))
                {
                    yield return inner;
                }
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> TAuditSignatureRead(ISymbol member)
    {
        IEnumerable<ITypeSymbol> types = member switch
        {
            IMethodSymbol { AssociatedSymbol: null } method =>
                method.Parameters.Select(parameter => parameter.Type).Prepend(method.ReturnType),
            IPropertySymbol property => property.Parameters.Select(parameter => parameter.Type).Prepend(property.Type),
            IEventSymbol handler => [handler.Type],
            IFieldSymbol field => [field.Type],
            INamedTypeSymbol nested => nested.BaseType is null
                ? nested.Interfaces
                : nested.Interfaces.Prepend(nested.BaseType),
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
        foreach ((string ring, string[] neighbour) in TAuditBorderSetting.TAuditBorderNeighbour)
        {
            HashSet<string> seen = new(StringComparer.Ordinal);
            Queue<string> pending = new(neighbour);
            while (pending.Count > 0)
            {
                string name = pending.Dequeue();
                if (!seen.Add(name))
                {
                    continue;
                }

                foreach (string deeper in TAuditBorderSetting.TAuditBorderNeighbour.GetValueOrDefault(name, []))
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
        string[] neighbour = TAuditBorderSetting.TAuditBorderNeighbour[ring];
        bool isCut = TAuditBorderSetting.TAuditBorderCut.Contains(ring, StringComparer.Ordinal);

        void TAuditHitAdd(SyntaxNode name, string target, string label, bool data)
        {
            if (target == ring)
            {
                return;
            }

            bool under = isCut && !TAuditBorderSetting.TAuditBorderCut.Contains(target, StringComparer.Ordinal);
            string kind = neighbour.Contains(target, StringComparer.Ordinal)
                || TAuditBorderSetting.TAuditBorderCapsule.GetValueOrDefault(ring) == target ? "Commuting"
                : !inner[ring].Contains(target) ? "Trespassing"
                : under ? "Undercutting"
                : data ? "Ferrying"
                : "Leapfrogging";
            int line = name.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            if (taken.Add($"{line}|{kind}|{target}|{label}"))
            {
                hits.Add(new TAuditHit(relative, line, ring, kind, target, label));
            }
        }

        SyntaxNode root = model.SyntaxTree.GetRoot();
        foreach (UsingDirectiveSyntax directive in root.DescendantNodes().OfType<UsingDirectiveSyntax>())
        {
            if (directive.NamespaceOrType is { } named
                && model.GetSymbolInfo(named).Symbol is INamespaceSymbol space
                && TAuditSpaceRead(space.ToDisplayString()) is { } target
                && !neighbour.Contains(target, StringComparer.Ordinal)
                && TAuditBorderSetting.TAuditBorderCapsule.GetValueOrDefault(ring) != target)
            {
                TAuditHitAdd(directive, target, "using " + space.ToDisplayString(), true);
            }
        }

        foreach (SimpleNameSyntax name in root.DescendantNodes().OfType<SimpleNameSyntax>())
        {
            UsingDirectiveSyntax? header = name.FirstAncestorOrSelf<UsingDirectiveSyntax>();
            if (TAuditBinderSymbol.TAuditHeaderCheck(name)
                && header?.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) != true)
            {
                continue;
            }

            ISymbol? symbol = TAuditBinderSymbol.TAuditSymbolRead(model, name);
            if (symbol is null || TAuditBinderSymbol.TAuditTypeRead(symbol) is not INamedTypeSymbol type)
            {
                continue;
            }

            string? source = TAuditBinderSymbol.TAuditSourceRead(type);
            string? target = source is null
                ? null
                : TAuditBinder.TAuditRingRead(source, TAuditBorderSetting.TAuditBorderNeighbour.Keys);
            if (target is not null)
            {
                bool behaviour = symbol is IMethodSymbol
                {
                    MethodKind: MethodKind.Ordinary or MethodKind.ReducedExtension
                };
                TAuditHitAdd(name, target, type.Name, TAuditBinderSymbol.TAuditDataCheck(type) && !behaviour);
            }
        }

        return hits;
    }

    private static string? TAuditSpaceRead(string space)
    {
        return TAuditBorderSetting.TAuditBorderNeighbour.Keys
            .Where(ring => space == ring || space.StartsWith(ring + ".", StringComparison.Ordinal))
            .MaxBy(ring => ring.Length);
    }
}
