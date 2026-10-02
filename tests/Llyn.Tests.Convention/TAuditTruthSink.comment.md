# TAuditTruthSink.cs

## `internal static partial class TAuditTruthWalker`

The sink half of the truth walker: where a driver value may not go.

## `private static (string TViolationKind, string TViolationReason)? TAuditSinkRead(SyntaxNode reference)`

Walks up from the reference to the member and names the first sink met.
A reference that is only the receiver of a member access is not a sink.
Parentheses and a null-forgiving `!` between the two do not change that.
The owner of that member audits its own field.
The target of `?.` whose tail requests is gatekeeping, since its presence decides the request.
An argument of a logic call, a record copy or a logic construction is an argument sink.
The condition of an if, ternary or switch whose branch requests is a gatekeeping sink.
So is the condition of a while, do or for loop whose body requests.
So is a `when` clause on a switch arm or a catch that requests.
So is the left side of `&&`, `||` or `??` whose right side requests.
A condition that only tests for null or for a type is not a gatekeeping sink.

## `private static bool TAuditGatekeepingCheck(IfStatementSyntax branch)`

True when a branch requests, or jumps out while the scope it leaves requests from its condition on.
A `return` or `throw` leaves the member.
A `continue` leaves its loop body, and a `break` leaves its loop or its switch section.
A request after that loop or in the rest of that section runs whatever the branch decides.
A request in a later case is not skipped by the `break`, so it does not count.
A request before the branch runs whatever the branch decides, so it is not gatekeeping.
A condition that only tests presence decides nothing the engine holds and is never gatekeeping.

## `private static bool TAuditPresenceCheck(ExpressionSyntax condition)`

True when the condition, past parentheses and negation, only tests for null or matches a type.
An `&&` or `||` chain counts only when every operand is such a test.

## `private static bool TAuditRequestCheck(SyntaxNode node)`

True when the node contains a logic call, a logic construction or a call to a relay.

## `private static bool TAuditHotCheck(ISymbol callee, ArgumentSyntax argument)`

True for any argument of a logic call, and for a relay argument standing at a hot position.
A named argument is matched to its parameter by name.

## `internal static ISymbol? TAuditCallRead(ExpressionSyntax call)`

The logic or relay symbol a call or construction invokes, else null.
Invoking a delegate member that is a relay returns that member.
Raising an event that implements a relay interface event returns that event.

## `private static ISymbol? TAuditDelegateRead(ExpressionSyntax call)`

The delegate member a call invokes, directly, through `Invoke` or through `?.Invoke`.

## `private static bool TAuditInterfaceCheck(ISymbol held)`

True when an event implements an interface event that a driver subscribed a relay to.
A subscription through `INotifyPropertyChanged` thus reaches every type that raises it.

## `internal static bool TAuditNameofCheck(SyntaxNode node)`

True for a `nameof` expression, which yields a name and reads no value.
A method a driver itself names `nameof` resolves to a symbol and is not one.
