using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Convention.Tests;

internal static class TAuditBinder
{
    private const string TAuditBinderSource = "src/";

    private static readonly CSharpParseOptions TAuditSyntaxOptions = new(
        languageVersion: LanguageVersion.Preview,
        documentationMode: DocumentationMode.None,
        kind: SourceCodeKind.Regular);

    private static readonly Lazy<CSharpCompilation> TAuditBinderCompilation = new(TAuditCompilationRead);

    public static string TAuditRoot { get; } = TAuditSource.TAuditRootRead();

    public static IReadOnlyList<SyntaxTree> TAuditTrees => TAuditBinderCompilation.Value.SyntaxTrees.ToList();

    public static SemanticModel TAuditModelRead(SyntaxTree tree) =>
        TAuditBinderCompilation.Value.GetSemanticModel(tree);

    public static string TAuditRelativeRead(string path) =>
        Path.GetRelativePath(TAuditRoot, path).Replace('\\', '/');

    public static string? TAuditRingRead(string relative, IEnumerable<string> rings)
    {
        foreach (string ring in rings)
        {
            if (relative.StartsWith(TAuditBinderSource + ring + "/", StringComparison.OrdinalIgnoreCase))
            {
                return ring;
            }
        }

        return null;
    }

    public static bool TAuditHeaderCheck(SimpleNameSyntax name)
    {
        SyntaxNode? parent = name.Parent;
        if (parent is QualifiedNameSyntax qualified)
        {
            parent = qualified.Parent;
        }

        return parent is UsingDirectiveSyntax or NamespaceDeclarationSyntax or FileScopedNamespaceDeclarationSyntax;
    }

    public static bool TAuditDataCheck(INamedTypeSymbol type) =>
        type.TypeKind is TypeKind.Enum or TypeKind.Struct or TypeKind.Delegate || type.IsRecord;

    public static INamedTypeSymbol? TAuditTypeRead(ISymbol symbol) => symbol switch
    {
        IAliasSymbol alias => TAuditTypeRead(alias.Target),
        INamedTypeSymbol named => named.IsTupleType || named.IsAnonymousType ? null : named.OriginalDefinition,
        IArrayTypeSymbol array => TAuditTypeRead(array.ElementType),
        INamespaceSymbol or ITypeParameterSymbol or ILocalSymbol or IParameterSymbol => null,
        IRangeVariableSymbol or IDiscardSymbol or ILabelSymbol or IPreprocessingSymbol => null,
        _ => symbol.ContainingType?.OriginalDefinition,
    };

    public static ISymbol? TAuditSymbolRead(SemanticModel model, SimpleNameSyntax name)
    {
        SymbolInfo info = model.GetSymbolInfo(name);
        return info.Symbol ?? info.CandidateSymbols.FirstOrDefault();
    }

    public static string? TAuditSourceRead(INamedTypeSymbol type)
    {
        Location? source = type.Locations.FirstOrDefault(location => location.IsInSource);
        return source?.SourceTree is null ? null : TAuditRelativeRead(source.SourceTree.FilePath);
    }

    private static CSharpCompilation TAuditCompilationRead()
    {
        TAuditScope scope = new(
            [],
            [TAuditBinderSource + "*.cs"],
            TAuditNameSetting.TAuditExcludedSegments,
            TAuditNameSetting.TAuditExcludedSuffixes,
            TAuditNameSetting.TAuditExcludedPrefixes,
            []);
        IReadOnlyList<string> sources = TAuditSource.TAuditFileRead(TAuditRoot, scope);
        Assert.True(sources.Count > 0, TAuditConvention.TAuditReportFormat(
            "AUDITBINDER", "No tracked source file was enumerated; every bound audit would pass vacuously."));

        List<SyntaxTree> trees = sources
            .AsParallel()
            .AsOrdered()
            .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path), TAuditSyntaxOptions, path))
            .ToList();
        return CSharpCompilation.Create(
            "AuditBinder",
            trees,
            TAuditObjectWalker.TAuditReferenceRead(),
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }
}
