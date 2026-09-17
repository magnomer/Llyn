# TAuditTruthWalker.cs

## `internal static class TAuditTruthWalker`

Parses the shell sources with Roslyn and follows each mutable `_p` field to where its value goes.
No semantic model is built: a name that starts with `L` and a capital is a logic name.
Partial classes are joined by class name, so a field declared in one part is followed in every part.

## `private sealed record TAuditTruthField(`

One audited field: its name, its declared type without `?`, and where it is declared.

## `public static IReadOnlyList<TViolation> TAuditRun(IEnumerable<string> sourcePaths)`

Parses every file, scans it for logic mutation, groups the classes, and checks every field of each.

## `private static IEnumerable<TAuditTruthField> TAuditFieldRead(IReadOnlyList<ClassDeclarationSyntax> type)`

The mutable `_p` fields of a class.
A `readonly`, `const` or `static` field is a fixture and is skipped.
A field initialised to `null!` is wired once after construction and is skipped as identity.

## `private static void TAuditFieldCheck(`

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

## `private static (string TSinkKind, string TSinkReason)? TAuditSinkRead(SyntaxNode reference)`

Walks up from the reference to the member and names the first sink met.
A reference that is only the receiver of a member access is not a sink.
The owner of that member audits its own field.
An argument of a logic call, a record copy or a logic construction is an argument sink.
The condition of an if, ternary or switch whose branch requests is a guard sink.

## `private static bool TAuditGuardCheck(IfStatementSyntax branch)`

True when a branch requests, or when the branch jumps out and the member requests anywhere.

## `private static bool TAuditRequestCheck(SyntaxNode node)`

True when the node contains a logic call or a logic construction.

## `private static string? TAuditCallRead(ExpressionSyntax call)`

The logic name a call or construction invokes, else null.

## `private static string? TAuditNameRead(ExpressionSyntax expression)`

The rightmost identifier of a name, member access, binding or nullable type.

## `private static string TAuditWriterResolve(ExpressionSyntax? value, MemberDeclarationSyntax scope)`

`clear` for null or default, `engine` when the value names logic or a logic-typed parameter or local, else `plain`.
A compound assignment, increment or out argument has no value and is plain.

## `private static bool TAuditSourceCheck(HashSet<string> names, MemberDeclarationSyntax scope)`

True when any named identifier is a parameter of logic type or a local initialised from logic.

## `private static void TAuditMutationScan(SyntaxNode root, List<TViolation> violations)`

Every plain assignment to a logic-named member outside an object initialiser.
An event subscription is a compound assignment and is not reported.

## `private static SyntaxNode TAuditReferenceRead(IdentifierNameSyntax identifier)`

The reference node: the `this.` access when present, else the identifier.

## `private static bool TAuditWriteCheck(SyntaxNode reference, out ExpressionSyntax? value)`

True when the reference is assigned, incremented or passed by out or ref.
The value is the right side of a plain assignment only.

## `private static bool TAuditNameCheck(SyntaxNode node, string name)`

True when the node contains an identifier of that name.

## `private static bool TAuditLogicCheck(string name)`

True for a name that starts with `L` followed by a capital.

## `private static int TAuditLineRead(SyntaxNode node)`

The one-based line of the node.
