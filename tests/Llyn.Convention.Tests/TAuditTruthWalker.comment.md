# TAuditTruthWalker.cs

## `internal static partial class TAuditTruthWalker`

Parses the shell sources with Roslyn and follows each mutable field to where its value goes.
A settable property is followed the same way, and no prefix earns a field a pass.
No semantic model is built: a name that starts with `L` and a capital is a logic name.
Partial classes are joined by class name, so a field declared in one part is followed in every part.
A nested struct, class or record is audited as part of its outermost class.

## `private static readonly object TAuditGate = new();`

The walker keeps its state in statics, and the truth and strict facts run on parallel threads.
Every entry point takes the gate so one walk finishes before another resets the state.

## `private static List<SyntaxNode> TAuditRoots = [];`

Every parsed file, so a write to a property from another class is still seen.

## `private static readonly char[] TAuditTypeBreaks = ['<', '>', ',', '.', '[', ']', ' ', '(', ')'];`

The characters that separate the names inside a declared type.

## `private sealed record TAuditTruthField(`

One audited field: its name, its declared type without `?`, and where it is declared.
A settable property is audited as a field under its own name and is marked shared.

## `public static IReadOnlyList<TViolation> TAuditRun(IEnumerable<string> sourcePaths, IEnumerable<string> controls)`

Parses every file, scans it for mutation and shape, groups the outermost types, and checks every field.
The controls are the markup names, so the shape scan knows which member access reads a control.
The relay names are read once for the shell, then each type has bases, locals and fields checked.

## `public static IReadOnlySet<string> TAuditReaderRead(IEnumerable<string> sourcePaths)`

The names of every shell member that reads logic or requests, for the taint scan on the strict side.

## `private static Dictionary<string, List<TypeDeclarationSyntax>> TAuditPartRead(IEnumerable<string> sourcePaths)`

Parses every file, keeps the roots, groups the outermost types by name and reads the relay names.

## `private static IEnumerable<TAuditTruthField> TAuditFieldRead(IReadOnlyList<ClassDeclarationSyntax> type)`

The mutable fields, the settable properties and the positional record parameters of a class, whatever their prefix.
A `readonly` or `const` field is a fixture only while nothing writes into it.
A slot assignment or a fill verb on a `readonly` collection is a write, so it is followed too.
A `static` field is shared state and is followed like any other.
A field or property initialised to `null!` is wired once after construction and is skipped as identity.

## `private static string TAuditTypeRead(TypeSyntax type)`

The declared type without its trailing `?`.

## `private static bool TAuditFillCheck(string name, IReadOnlyList<TypeDeclarationSyntax> type)`

True when any part of the class writes into the field, by assignment, slot or fill verb.

## `private static bool TAuditWiredCheck(ExpressionSyntax? value)`

True for an initialiser of `null!`.

## `private static void TAuditFieldCheck(`

A field whose declared type names logic, and is not a handle, is a mirror before any reference is read.
A field typed `object` is a mirror too, since an untyped slot hides what it holds.
A shared field has its writes read in every file, but its reads only in its own class.
A scope that writes the field, or calls a setter relay writing it, is checked for a toggle.
A field whose name ends in `State` is a mirror too, since only the engine resolves a state.
A `??=` whose right side requests caches the answer and is a mirror.
Visits every reference to the field across the class parts.
A write is sorted by its writer.
A field with an engine writer and a shell writer is a fork.
A read is checked once per member scope, unless the field is a handle.

## `private static void TAuditScopeCheck(`

Checks the field and the locals it taints inside one member for a sink.
A hit is reported once per line and kind.

## `private static HashSet<string> TAuditTaintRead(string fieldName, MemberDeclarationSyntax scope)`

Locals whose initialiser, assignment or pattern designation reads the field.
One hop only, so a local built from a tainted local is not followed.

## `private static string TAuditWriterResolve(ExpressionSyntax? value, MemberDeclarationSyntax scope)`

`clear` for null or default, else `engine` or `plain`.
`engine` when the value reads a logic member, calls logic or a reader, or names a logic-typed parameter or local.
A bare logic name is a constant the shell chose, so it is plain.
A compound assignment, increment or out argument has no value and is plain.

## `private static bool TAuditSourceCheck(HashSet<string> names, MemberDeclarationSyntax scope)`

True when any named identifier is a parameter of logic type or a local initialised from logic.

## `private static SyntaxNode TAuditReferenceRead(IdentifierNameSyntax identifier)`

The reference node: the access when the identifier is a member of `this` or a named owner, else itself.

## `private static bool TAuditWriteCheck(SyntaxNode reference, out ExpressionSyntax? value)`

True when the reference is assigned, incremented or passed by out or ref.
Also true when a slot of it is assigned or a fill verb is called on it.
The value is the right side of a plain assignment, or the last argument of a fill.
A fill without an argument empties the field and is sorted as a clear, not a writer.

## `private static bool TAuditNameCheck(SyntaxNode node, string name)`

True when the node contains an identifier of that name.

## `private static bool TAuditLogicCheck(string name)`

True for a name that starts with `L` followed by a capital.

## `private static int TAuditLineRead(SyntaxNode node)`

The one-based line of the node.
