using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Convention.Tests;

internal static class TAuditOriginWalker
{
    private static readonly Dictionary<ISymbol, bool> TAuditOriginNames = new(SymbolEqualityComparer.Default);

    private static Dictionary<string, List<SyntaxNode>> TAuditSiteNames = new(StringComparer.Ordinal);

    private static Compilation? TAuditOriginCompilation;

    internal static string TAuditWriterResolve(ExpressionSyntax? value)
    {
        if (!ReferenceEquals(TAuditOriginCompilation, TAuditBinder.TAuditCompilation))
        {
            TAuditOriginCompilation = TAuditBinder.TAuditCompilation;
            TAuditOriginNames.Clear();
            IReadOnlyList<string> shell = TAuditBinder.TAuditRootRead(
                [.. TAuditTruthSetting.TAuditShellInclude, .. TAuditTruthSetting.TAuditCapsuleInclude]);
            TAuditSiteNames = TAuditBinder.TAuditTrees
                .Where(tree => shell.Any(folder => TAuditBinder.TAuditRelativeRead(tree.FilePath)
                    .StartsWith(folder + "/", StringComparison.OrdinalIgnoreCase)))
                .SelectMany(tree => tree.GetRoot().DescendantNodes()
                    .Where(node => node is IdentifierNameSyntax or ImplicitObjectCreationExpressionSyntax
                    or ConstructorInitializerSyntax))
                .GroupBy(node => node is IdentifierNameSyntax name ? name.Identifier.ValueText : "new",
                    StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);
        }

        return TAuditWriterResolve(value, new HashSet<ISymbol>(SymbolEqualityComparer.Default));
    }

    private static string TAuditWriterResolve(ExpressionSyntax? value, HashSet<ISymbol> visiting)
    {
        if (value is null)
        {
            return "plain";
        }

        if (TAuditBlankCheck(value))
        {
            return "clear";
        }

        bool asked = value.DescendantNodesAndSelf(node => !TAuditTruthWalker.TAuditNameofCheck(node)
                                                          && !TAuditCapsuleCheck(node)).Any(node =>
            node switch
            {
                MemberAccessExpressionSyntax or MemberBindingExpressionSyntax
                    => TAuditBinder.TAuditLogicCheck(TAuditBinder.TAuditSymbolRead(node)),
                InvocationExpressionSyntax call
                    => TAuditTruthWalker.TAuditCallRead(call) is not null
                       || (TAuditBinder.TAuditSymbolRead(call) is { } callee
                           && TAuditTruthWalker.TAuditReaderNames.Contains(callee)),
                BaseObjectCreationExpressionSyntax creation => TAuditTruthWalker.TAuditCallRead(creation) is not null,
                IdentifierNameSyntax name => TAuditBinder.TAuditSymbolRead(name) switch
                {
                    ILocalSymbol or IParameterSymbol when TAuditBinder.TAuditLogicCheck(name) => true,
                    { } held => TAuditOriginCheck(held, visiting),
                    _ => false
                },
                _ => false
            });
        return asked ? "engine" : "plain";
    }

    private static bool TAuditOriginCheck(ISymbol held, HashSet<ISymbol> visiting)
    {
        bool top = visiting.Count == 0;
        if (top && TAuditOriginNames.TryGetValue(held, out bool known))
        {
            return known;
        }

        if (!visiting.Add(held))
        {
            return false;
        }

        bool engine = false;
        foreach (ExpressionSyntax? write in TAuditOriginRead(held) ?? [])
        {
            if (write is IdentifierNameSyntax or MemberAccessExpressionSyntax
                && TAuditBinder.TAuditSymbolRead(write) is { } copied && visiting.Contains(copied))
            {
                continue;
            }

            string verdict = TAuditWriterResolve(write, visiting);
            if (verdict == "clear")
            {
                continue;
            }

            engine = verdict == "engine";
            if (!engine)
            {
                break;
            }
        }

        visiting.Remove(held);
        if (top)
        {
            TAuditOriginNames[held] = engine;
        }

        return engine;
    }

    internal static List<ExpressionSyntax?>? TAuditOriginRead(ISymbol held)
    {
        ISymbol owner = held is IParameterSymbol
        {
            ContainingSymbol: IMethodSymbol
            {
                MethodKind: MethodKind.PropertySet, AssociatedSymbol: IPropertySymbol property
            }
        }
            ? property
            : held;
        if (owner is not (IFieldSymbol or IPropertySymbol { IsIndexer: false } or ILocalSymbol
                or IParameterSymbol { ContainingSymbol: IMethodSymbol { MethodKind: MethodKind.Constructor } }
                or IParameterSymbol
                {
                    RefKind: RefKind.None, ContainingSymbol: IMethodSymbol { MethodKind: MethodKind.Ordinary }
                })
            || owner.DeclaringSyntaxReferences.IsEmpty
            || !TAuditBinder.TAuditShellCheck(owner.ContainingType))
        {
            return null;
        }

        if (owner is IFieldSymbol or IPropertySymbol
            && owner.DeclaredAccessibility != Accessibility.Private
            && owner.ContainingType.AllInterfaces.Any(face =>
                face.ToDisplayString() == "System.Windows.Markup.IComponentConnector"))
        {
            return [null];
        }

        SyntaxNode declaration = owner.DeclaringSyntaxReferences[0].GetSyntax();
        List<ExpressionSyntax?> writes = [];
        if (ReferenceEquals(owner, held))
        {
            switch (owner)
            {
                case IFieldSymbol { Type.Name: "DependencyProperty" or "DependencyPropertyKey" }:
                    return TAuditPropertyRead(owner, declaration);
                case IPropertySymbol when declaration is PropertyDeclarationSyntax body
                                          && TAuditGetterRead(body) is { } getter:
                    return getter;
                case IPropertySymbol positional when declaration is ParameterSyntax:
                    writes.AddRange(TAuditArgumentRead(
                        positional.ContainingType.InstanceConstructors.FirstOrDefault(constructor =>
                            constructor.DeclaringSyntaxReferences.Any(source =>
                                source.GetSyntax() is TypeDeclarationSyntax)),
                        positional.Name));
                    break;
                case IParameterSymbol { ContainingSymbol: IMethodSymbol method } parameter:
                    writes.AddRange(TAuditArgumentRead(method, parameter.Name));
                    break;
                case ILocalSymbol:
                    writes.AddRange(TAuditLocalRead(declaration));
                    break;
                default:
                    if (declaration is VariableDeclaratorSyntax { Initializer.Value: { } start })
                    {
                        writes.Add(start);
                    }
                    else if (declaration is PropertyDeclarationSyntax { Initializer.Value: { } first })
                    {
                        writes.Add(first);
                    }

                    break;
            }
        }

        foreach (IdentifierNameSyntax identifier in TAuditSiteRead(owner))
        {
            SyntaxNode reference = identifier.Parent is MemberAccessExpressionSyntax access && access.Name == identifier
                ? access
                : identifier;
            if (TAuditTruthWalker.TAuditWriteCheck(reference, out ExpressionSyntax? value)
                && (value is not null || reference.Parent is not MemberAccessExpressionSyntax))
            {
                writes.AddRange(TAuditHelperRead(identifier, reference) ?? [value]);
            }
        }

        return writes;
    }

    internal static List<ExpressionSyntax?>? TAuditHelperRead(IdentifierNameSyntax identifier, SyntaxNode reference)
    {
        if (TAuditBinder.TAuditSymbolRead(identifier) is IParameterSymbol
            {
                RefKind: RefKind.Ref, ContainingSymbol: IMethodSymbol inner
            } held
            && reference.Parent is AssignmentExpressionSyntax
            && TAuditHelperScan(inner, held) is not null)
        {
            return [];
        }

        if (reference.Parent is not ArgumentSyntax
            {
                Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax call }
            } argument
            || !argument.RefKindKeyword.IsKind(SyntaxKind.RefKeyword)
            || TAuditBinder.TAuditModelRead(call).GetOperation(call) is not IInvocationOperation invoked)
        {
            return null;
        }

        IParameterSymbol? target = invoked.Arguments.FirstOrDefault(given => given.Syntax == argument)?.Parameter;
        List<IParameterSymbol>? sources = target is null
            ? null
            : TAuditHelperScan(invoked.TargetMethod.OriginalDefinition, target.OriginalDefinition);
        return sources?.Select(source => (invoked.Arguments.FirstOrDefault(given =>
                    SymbolEqualityComparer.Default.Equals(given.Parameter?.OriginalDefinition, source))
                ?.Syntax as ArgumentSyntax)?.Expression)
            .ToList() ?? [null];
    }

    private static List<IParameterSymbol>? TAuditHelperScan(IMethodSymbol method, IParameterSymbol held)
    {
        List<IParameterSymbol> sources = [];
        foreach (IdentifierNameSyntax use in method.DeclaringSyntaxReferences
                     .SelectMany(source => source.GetSyntax().DescendantNodes().OfType<IdentifierNameSyntax>()))
        {
            if (!SymbolEqualityComparer.Default.Equals(TAuditBinder.TAuditSymbolRead(use), held)
                || !TAuditTruthWalker.TAuditWriteCheck(use, out _))
            {
                continue;
            }

            if (use.Parent is not AssignmentExpressionSyntax
                {
                    RawKind: (int)SyntaxKind.SimpleAssignmentExpression
                } set
                || set.Left != use
                || TAuditBinder.TAuditSymbolRead(set.Right) is not IParameterSymbol { RefKind: RefKind.None } given
                || !SymbolEqualityComparer.Default.Equals(given.ContainingSymbol, method))
            {
                return null;
            }

            sources.Add(given);
        }

        return sources.Count > 0 ? sources : null;
    }

    private static IEnumerable<IdentifierNameSyntax> TAuditSiteRead(ISymbol held)
    {
        return (TAuditSiteNames.GetValueOrDefault(held.Name) ?? [])
            .OfType<IdentifierNameSyntax>()
            .Where(identifier => !identifier.Ancestors().Any(TAuditTruthWalker.TAuditNameofCheck)
                                 && SymbolEqualityComparer.Default.Equals(
                                     TAuditBinder.TAuditSymbolRead(identifier), held));
    }

    private static List<ExpressionSyntax?> TAuditPropertyRead(ISymbol held, SyntaxNode declaration)
    {
        List<ExpressionSyntax?> writes = [];
        foreach (BaseObjectCreationExpressionSyntax creation in declaration.DescendantNodes()
                     .OfType<BaseObjectCreationExpressionSyntax>())
        {
            if (TAuditBinder.TAuditModelRead(creation).GetOperation(creation) is IObjectCreationOperation built
                && built.Arguments.FirstOrDefault(argument => argument.Parameter?.Name == "defaultValue")
                    ?.Value.Syntax is ExpressionSyntax fallback
                && !TAuditNeutralCheck(fallback))
            {
                writes.Add(fallback);
            }
        }

        foreach (IdentifierNameSyntax identifier in TAuditSiteRead(held))
        {
            if (identifier.Parent is not ArgumentSyntax
                {
                    Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax call } list
                } argument)
            {
                continue;
            }

            switch ((TAuditBinder.TAuditSymbolRead(call) as IMethodSymbol)?.Name)
            {
                case "SetValue" or "SetCurrentValue" when list.Arguments.Count == 2 && list.Arguments[0] == argument:
                    writes.Add(list.Arguments[1].Expression);
                    break;
                case "GetValue" or "ReadLocalValue" or "ClearValue" or "GetValueSource" or "InvalidateProperty"
                    or "CoerceValue":
                    break;
                default:
                    writes.Add(null);
                    break;
            }
        }

        return writes;
    }

    private static List<ExpressionSyntax?>? TAuditGetterRead(PropertyDeclarationSyntax property)
    {
        if (property.ExpressionBody is { } body)
        {
            return [body.Expression];
        }

        AccessorDeclarationSyntax? getter = property.AccessorList?.Accessors
            .FirstOrDefault(accessor => accessor.IsKind(SyntaxKind.GetAccessorDeclaration));
        if (getter?.ExpressionBody is { } arrow)
        {
            return [arrow.Expression];
        }

        return getter?.Body?.DescendantNodes(node => node is not (AnonymousFunctionExpressionSyntax
                or LocalFunctionStatementSyntax))
            .OfType<ReturnStatementSyntax>()
            .Select(exit => exit.Expression)
            .ToList();
    }

    private static List<ExpressionSyntax?> TAuditArgumentRead(IMethodSymbol? method, string name)
    {
        List<ExpressionSyntax?> writes = [];
        List<SyntaxNode?> sites;
        if (method is { MethodKind: MethodKind.Ordinary })
        {
            sites = (TAuditSiteNames.GetValueOrDefault(method.Name) ?? [])
                .OfType<IdentifierNameSyntax>()
                .Where(identifier => !identifier.Ancestors().Any(TAuditTruthWalker.TAuditNameofCheck)
                                     && TAuditBinder.TAuditSymbolRead(identifier) is IMethodSymbol called
                                     && SymbolEqualityComparer.Default.Equals(
                                         (called.ReducedFrom ?? called).OriginalDefinition, method))
                .Select(identifier => identifier.Parent is MemberBindingExpressionSyntax binding
                    ? binding
                    : identifier.Parent is MemberAccessExpressionSyntax access && access.Name == identifier
                        ? access
                        : (ExpressionSyntax)identifier)
                .Select(callee => callee.Parent is InvocationExpressionSyntax call && call.Expression == callee
                    ? call
                    : null)
                .ToList<SyntaxNode?>();
            if (sites.Count == 0 || sites.Contains(null) || method.IsOverride || method.IsVirtual
                || method.IsAbstract || method.IsGenericMethod
                || method.ContainingType.AllInterfaces.SelectMany(face => face.GetMembers()).Any(member =>
                    SymbolEqualityComparer.Default.Equals(
                        method.ContainingType.FindImplementationForInterfaceMember(member), method)))
            {
                return [null];
            }
        }
        else
        {
            sites = (TAuditSiteNames.GetValueOrDefault(method?.ContainingType.Name ?? "") ?? [])
                .Select(node => node.Parent is QualifiedNameSyntax qualified ? qualified.Parent : node.Parent)
                .Concat(TAuditSiteNames.GetValueOrDefault("new") ?? [])
                .Where(site => site is not null
                               && SymbolEqualityComparer.Default.Equals(TAuditBinder.TAuditSymbolRead(site), method))
                .ToList();
        }

        foreach (SyntaxNode site in sites.OfType<SyntaxNode>())
        {
            ImmutableArray<IArgumentOperation>? built = TAuditBinder.TAuditModelRead(site).GetOperation(site) switch
            {
                IObjectCreationOperation creation => creation.Arguments,
                IInvocationOperation chained => chained.Arguments,
                _ => null
            };
            if (built is null)
            {
                writes.Add(null);
                continue;
            }

            foreach (IArgumentOperation argument in built.Value.Where(argument => argument.Parameter?.Name == name))
            {
                ExpressionSyntax? given = argument.ArgumentKind == ArgumentKind.DefaultValue
                    ? (argument.Parameter?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as ParameterSyntax)
                    ?.Default?.Value
                    : argument.Value.Syntax as ExpressionSyntax;
                if (given is null || !TAuditNeutralCheck(given))
                {
                    writes.Add(given);
                }
            }
        }

        return writes;
    }

    private static List<ExpressionSyntax?> TAuditLocalRead(SyntaxNode declaration)
    {
        if (declaration is VariableDeclaratorSyntax variable)
        {
            return variable.Initializer is { } start ? [start.Value] : [];
        }

        if (declaration is ForEachStatementSyntax loop)
        {
            return [loop.Expression];
        }

        if (declaration is not SingleVariableDesignationSyntax || declaration.Parent is DeclarationExpressionSyntax)
        {
            return [null];
        }

        return
        [
            declaration.Ancestors().Select(node => node switch
            {
                IsPatternExpressionSyntax test => test.Expression,
                SwitchStatementSyntax choice => choice.Expression,
                SwitchExpressionSyntax arms => arms.GoverningExpression,
                ForEachVariableStatementSyntax pair => pair.Expression,
                _ => null
            }).FirstOrDefault(tested => tested is not null)
        ];
    }

    private static bool TAuditNeutralCheck(ExpressionSyntax value)
    {
        if (TAuditBlankCheck(value))
        {
            return true;
        }

        Optional<object?> constant = TAuditBinder.TAuditModelRead(value).GetConstantValue(value);
        return constant.HasValue && constant.Value switch
        {
            false => true,
            bool or string or char => false,
            IConvertible number => number.ToDouble(System.Globalization.CultureInfo.InvariantCulture) == 0,
            _ => false
        };
    }

    private static bool TAuditBlankCheck(ExpressionSyntax value)
    {
        if (value.IsKind(SyntaxKind.NullLiteralExpression)
            || value.IsKind(SyntaxKind.DefaultLiteralExpression)
            || value is DefaultExpressionSyntax
            || TAuditBinder.TAuditSymbolRead(value) is IFieldSymbol
            {
                Name: "Empty", ContainingType.SpecialType: SpecialType.System_String
            })
        {
            return true;
        }

        Optional<object?> constant = TAuditBinder.TAuditModelRead(value).GetConstantValue(value);
        return constant.HasValue && constant.Value is null or "";
    }

    private static bool TAuditCapsuleCheck(SyntaxNode node)
    {
        string? source = node is InvocationExpressionSyntax
                         && TAuditBinder.TAuditSymbolRead(node) is IMethodSymbol { ContainingType: { } owner }
            ? TAuditBinder.TAuditSourceRead(owner.OriginalDefinition)
            : null;
        return source is not null && TAuditBinder.TAuditRootRead(TAuditTruthSetting.TAuditCapsuleInclude)
            .Any(folder => source.StartsWith(folder + "/", StringComparison.OrdinalIgnoreCase));
    }
}
