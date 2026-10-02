# TAuditOriginWalker.cs
Hash: `adf804a2e39b3931`

## `internal static class TAuditOriginWalker`

Sorts a value written to a judged field by where it comes from, for Contesting.
It follows the value through Deportment holders to their writers.
Only a member of a logic-side type is an engine source, as before.
It is a class of its own, so the walker's big partial type gains no part.

## `private static readonly Dictionary<ISymbol, bool> TAuditOriginNames = new(SymbolEqualityComparer.Default);`

The verdict of each holder resolved from an empty path, cached for the compilation.
A verdict reached inside another resolve is not cached, since a cycle cut there depends on the path.

## `private static Dictionary<string, List<SyntaxNode>> TAuditSiteNames = new(StringComparer.Ordinal);`

Every identifier in the tracked shell sources, keyed by its text.
Implicit `new(...)` creations and `this(...)` or `base(...)` calls sit under the key `new`, which no identifier can have.
Binding happens on demand, only for names that match a holder.

## `private static Compilation? TAuditOriginCompilation;`

The compilation the caches belong to.
An assay binds a new compilation, which empties both caches.

## `internal static string TAuditWriterResolve(ExpressionSyntax? value)`

`clear` for a blank value, else `engine` or `plain`.
`engine` when the value reads a logic member, calls logic or a reader, or names a logic-typed parameter or local.
It is also `engine` when the value names a holder whose origin is engine.
A compound assignment, increment or out argument has no value and is plain.
A name inside `nameof` reads no logic.
A call into the Capsule project is a shell source, and its arguments are not searched.
The Capsule reads and writes Deportment's own file, so its answer is the shell's whatever key goes in.
Writers outside the walked roots count, since every tracked shell source is indexed.

## `private static bool TAuditOriginCheck(ISymbol held, HashSet<ISymbol> visiting)`

True when the holder has at least one writer and every writer that is not a clear is engine.
A holder met again on the path is a cycle and reads plain.
A writer that only copies a holder already on the path is skipped as neutral.
Such a copy, as in `held.X = fresh.X`, adds no origin of its own.
A holder written only by such copies has no writer left and stays plain.

## `internal static List<ExpressionSyntax?>? TAuditOriginRead(ISymbol held)`

The written values of a holder, or null when the symbol is no holder.
A null entry is an unseen writer and reads plain.
A holder is a field, property, local, constructor parameter or method parameter declared in a shell source.
A method parameter is a by-value parameter of an ordinary method.
A setter's `value` stands for the property's own writes.
A non-private field or property of a type with generated markup code reads as unseen.
Its markup can bind or set it where no C# writer shows.
A dependency property field takes its metadata default and its `SetValue` calls.
A property with a getter body takes what the getter returns.
A positional record property and a constructor parameter take their `new` arguments.
A method parameter takes its arguments at every call of the method.
A local takes its initializer, its loop collection or the value its pattern tests.
Every holder except a getter body or dependency property also takes its assignments.

## `internal static List<ExpressionSyntax?>? TAuditHelperRead(IdentifierNameSyntax identifier, SyntaxNode reference)`

Null when the write is not a `ref` hand-off.
A `ref` argument into a plain helper writes the arguments the helper assigns to that parameter.
The helper's own assignment to its `ref` parameter yields nothing, since each call site already counts.
A helper that does anything else with the parameter makes the write unseen.

## `private static List<IParameterSymbol>? TAuditHelperScan(IMethodSymbol method, IParameterSymbol held)`

The parameters a helper assigns to its `ref` parameter, or null when the helper is not plain.
Plain means every write is a simple assignment from another by-value parameter of the same method.

## `private static IEnumerable<IdentifierNameSyntax> TAuditSiteRead(ISymbol held)`

Every use of the holder in the shell sources, outside `nameof`.

## `private static List<ExpressionSyntax?> TAuditPropertyRead(ISymbol held, SyntaxNode declaration)`

The writers of a dependency property field.
A non-neutral `defaultValue` in its metadata counts as a writer.
`SetValue` and `SetCurrentValue` write their second argument.
Reads, clears and coercions write nothing.
Any other call that takes the field, such as `SetBinding` or a `Setter`, is unseen.

## `private static List<ExpressionSyntax?>? TAuditGetterRead(PropertyDeclarationSyntax property)`

The returned expressions of a getter body, or null for an auto property.
Returns inside lambdas or local functions are not the getter's.

## `private static List<ExpressionSyntax?> TAuditArgumentRead(IMethodSymbol? method, string name)`

The arguments passed for one parameter at every call site.
A constructor's sites are its creations and chained calls.
An ordinary method's sites are the invocations of that method in the shell sources.
Its callers are unseen, as one null, when any caller may be hidden.
That holds when the method is used as a method group or has no seen call.
It holds when the method is generic, `override`, `virtual`, `abstract` or implements an interface member.
A neutral argument writes nothing, whether given or defaulted.
A site whose arguments cannot be read is unseen.

## `private static List<ExpressionSyntax?> TAuditLocalRead(SyntaxNode declaration)`

The value a local starts with.
An `out` variable or a catch variable is unseen.

## `private static bool TAuditNeutralCheck(ExpressionSyntax value)`

True for a blank value, `false` or zero.
A neutral default writes nothing.

## `private static bool TAuditBlankCheck(ExpressionSyntax value)`

True for null, default, the empty string and `string.Empty`.
A blank write says nothing about the content, so it is a clear.
`false` and zero are left out, since writing them can be a real shell decision.

## `private static bool TAuditCapsuleCheck(SyntaxNode node)`

True for an invocation whose callee is declared under `TAuditCapsuleInclude`.
