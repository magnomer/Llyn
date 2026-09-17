# TAuditStrictWalker.cs

## `internal static class TAuditStrictWalker`

Parses the shell sources with Roslyn and applies the strict rules.
No semantic model is built: a name that starts with `L` and a capital is a logic name.
Partial classes are joined by class name, and one veneer part makes the whole class a veneer.

## `private static readonly SyntaxKind[] TAuditFlowKinds =`

The syntax a veneer member may not contain: branches, loops, operators, patterns and local functions.

## `public static IReadOnlyList<TViolation> TAuditRun(IEnumerable<string> sourcePaths, out List<string> veneers)`

Parses every file, scans it for treatment, groups the classes, then scans storage and flow per class.
The veneer class names come back for the report.

## `private static bool TAuditVeneerCheck(ClassDeclarationSyntax type)`

True when the file has a `.xaml` beside it or the class derives from a listed veneer base.

## `private static void TAuditStorageScan(ClassDeclarationSyntax part, bool veneer, List<TViolation> violations)`

Every mutable field of a veneer class, and every mutable static field anywhere.
A `readonly` or `const` field is a fixture, and a `null!` field is wired once and skipped.

## `private static void TAuditFlowScan(ClassDeclarationSyntax part, List<TViolation> violations)`

One hit per veneer member, naming each flow kind found in it.

## `private static void TAuditTreatScan(SyntaxNode root, List<TViolation> violations)`

One hit per line where a logic value is an operand, a query source, or a condition.
Null checks, coalescing and the logical connectives are not treatment.

## `private static bool TAuditConditionCheck(ExpressionSyntax condition)`

True when the condition treats a logic value, and false for a null check or a bare verdict.

## `private static bool TAuditVerdictCheck(ExpressionSyntax condition)`

True when the condition, once unwrapped from parentheses and negation, is one logic call or member.
Asking the engine a question is not treating its data.

## `private static bool TAuditPatternCheck(PatternSyntax pattern)`

True for a null constant, its negation, or a declaration that only names the value.

## `private static bool TAuditNullCheck(ExpressionSyntax expression)`

True for the `null` or `default` literal.

## `private static bool TAuditDataCheck(SyntaxNode node)`

True when the node contains a logic member access or a logic call.

## `private static bool TAuditWiredCheck(VariableDeclaratorSyntax variable)`

True for a field initialised to `null!`.

## `private static string? TAuditTypeRead(TypeSyntax type)`

The rightmost identifier of a type name.

## `private static string TAuditMemberRead(MemberDeclarationSyntax member)`

A short label for a member, for the report.

## `private static bool TAuditLogicCheck(string name)`

True for a name that starts with `L` followed by a capital.

## `private static int TAuditLineRead(SyntaxNode node)`

The one-based line of the node.
