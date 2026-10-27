using Microsoft.CodeAnalysis;

namespace Convention.Tests;

internal sealed class TAuditModuleType(
    TAuditModuleRow row,
    INamedTypeSymbol entrance,
    IReadOnlyList<INamedTypeSymbol> contracts,
    IReadOnlyList<INamedTypeSymbol> declarations)
{
    public TAuditModuleRow TAuditRow { get; } = row;

    public INamedTypeSymbol TAuditEntrance { get; } = entrance;

    public IReadOnlyList<INamedTypeSymbol> TAuditContracts { get; } = contracts;

    public IReadOnlyList<INamedTypeSymbol> TAuditDeclarations { get; } = declarations;

    public bool TAuditDeclarationCheck(INamedTypeSymbol type) =>
        TAuditDeclarations.Contains(TAuditTopRead(type), SymbolEqualityComparer.Default);

    public bool TAuditContractCheck(INamedTypeSymbol type)
    {
        INamedTypeSymbol top = TAuditTopRead(type);
        return SymbolEqualityComparer.Default.Equals(top, TAuditEntrance)
            || TAuditContracts.Contains(top, SymbolEqualityComparer.Default);
    }

    public static INamedTypeSymbol TAuditTopRead(ISymbol symbol)
    {
        INamedTypeSymbol? type = symbol switch
        {
            INamedTypeSymbol named => named,
            IAliasSymbol alias when alias.Target is INamedTypeSymbol named => named,
            _ => symbol.ContainingType,
        };
        if (type is null)
        {
            throw new InvalidOperationException($"{symbol} has no containing type.");
        }

        while (type.ContainingType is not null)
        {
            type = type.ContainingType;
        }

        return type.OriginalDefinition;
    }

    public static string TAuditNameRead(INamedTypeSymbol type)
    {
        Stack<string> names = new();
        INamespaceOrTypeSymbol? owner = type;
        while (owner is not null)
        {
            if (owner is INamespaceSymbol space && space.IsGlobalNamespace)
            {
                break;
            }

            names.Push(owner.MetadataName);
            owner = owner.ContainingSymbol as INamespaceOrTypeSymbol;
        }

        return string.Join('.', names);
    }

    public static IReadOnlyList<ITypeSymbol> TAuditSignatureRead(ISymbol member)
    {
        List<ITypeSymbol> types = [];
        switch (member)
        {
            case INamedTypeSymbol type:
                if (type.BaseType is not null)
                {
                    types.Add(type.BaseType);
                }
                types.AddRange(type.Interfaces);
                foreach (ITypeParameterSymbol parameter in type.TypeParameters)
                {
                    types.AddRange(parameter.ConstraintTypes);
                }
                break;
            case IMethodSymbol method:
                types.Add(method.ReturnType);
                types.AddRange(method.Parameters.Select(parameter => parameter.Type));
                foreach (ITypeParameterSymbol parameter in method.TypeParameters)
                {
                    types.AddRange(parameter.ConstraintTypes);
                }
                break;
            case IPropertySymbol property:
                types.Add(property.Type);
                types.AddRange(property.Parameters.Select(parameter => parameter.Type));
                break;
            case IEventSymbol signal:
                types.Add(signal.Type);
                break;
            case IFieldSymbol field:
                types.Add(field.Type);
                break;
        }

        return types;
    }

    public static IReadOnlyList<INamedTypeSymbol> TAuditShapeRead(IEnumerable<ITypeSymbol> roots)
    {
        HashSet<ITypeSymbol> visited = new(SymbolEqualityComparer.Default);
        HashSet<INamedTypeSymbol> found = new(SymbolEqualityComparer.Default);
        Stack<ITypeSymbol> pending = new(roots);
        while (pending.TryPop(out ITypeSymbol? type))
        {
            if (!visited.Add(type))
            {
                continue;
            }

            switch (type)
            {
                case IArrayTypeSymbol array:
                    pending.Push(array.ElementType);
                    break;
                case IPointerTypeSymbol pointer:
                    pending.Push(pointer.PointedAtType);
                    break;
                case IFunctionPointerTypeSymbol function:
                    pending.Push(function.Signature.ReturnType);
                    foreach (IParameterSymbol parameter in function.Signature.Parameters)
                    {
                        pending.Push(parameter.Type);
                    }
                    break;
                case ITypeParameterSymbol parameter:
                    foreach (ITypeSymbol constraint in parameter.ConstraintTypes)
                    {
                        pending.Push(constraint);
                    }
                    break;
                case INamedTypeSymbol named:
                    found.Add(TAuditTopRead(named));
                    foreach (ITypeSymbol argument in named.TypeArguments)
                    {
                        pending.Push(argument);
                    }
                    break;
            }
        }

        return found.OrderBy(TAuditNameRead, StringComparer.Ordinal).ToList();
    }

    public static bool TAuditVisibleCheck(ISymbol member) => member.DeclaredAccessibility is
        Accessibility.Public or Accessibility.Internal or Accessibility.Protected
        or Accessibility.ProtectedAndInternal or Accessibility.ProtectedOrInternal;
}

