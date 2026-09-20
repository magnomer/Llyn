# TAuditBinder.cs

## `internal static class TAuditBinder`

One Roslyn compilation of every tracked source under `src`, shared by every audit that binds.
The chain and the frame each walk the same trees, so the parse and the bind are paid once.
Nothing here names a project: the folder, the exclusions and the project name come from the settings.

## `private const string TAuditBinderSource = "src/";`

The folder every ring lives under.

## `private static readonly CSharpParseOptions TAuditSyntaxOptions`

The parse options every source is read with.

## `private static readonly Lazy<CSharpCompilation> TAuditBinderCompilation`

The compilation, built on first use and kept for the run.

## `public static string TAuditRoot { get; }`

The Git working tree the sources are read from.

## `public static IReadOnlyList<SyntaxTree> TAuditTrees`

Every parsed source, in path order.

## `public static SemanticModel TAuditModelRead(SyntaxTree tree)`

The semantic model of one tree, safe to read from many threads.

## `public static string TAuditRelativeRead(string path)`

The repo-relative path with forward slashes.

## `public static string? TAuditRingRead(string relative, IEnumerable<string> rings)`

The ring whose folder holds the file, or null outside every ring given.
A ring is its project folder under `src`, matched whole, so `Llyn.UI` never claims `Llyn.UIVeneer`.

## `public static bool TAuditHeaderCheck(SimpleNameSyntax name)`

True for a name inside a using directive or a namespace declaration, which names no type.

## `public static bool TAuditDataCheck(INamedTypeSymbol type)`

True for an enum, a struct, a record or a delegate: a type a value is the whole of.
A class or an interface is behaviour, and a static class is behaviour reached through its name.

## `public static INamedTypeSymbol? TAuditTypeRead(ISymbol symbol)`

The type a name stands for: the type itself, or the type owning the member the name picks.
A local or a parameter is a name the file gave itself, so its uses are not counted again.

## `public static ISymbol? TAuditSymbolRead(SemanticModel model, SimpleNameSyntax name)`

The symbol a name binds to, or the first candidate when the bind is ambiguous.

## `public static string? TAuditSourceRead(INamedTypeSymbol type)`

The repo-relative file declaring the type, or null for a type outside the sources.

## `private static CSharpCompilation TAuditCompilationRead()`

Enumerates the sources with Git, parses them in parallel and binds them against the runtime assemblies.
An empty enumeration fails rather than passing vacuously.
