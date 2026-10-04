# TAuditTruthRelay.cs
Hash: `6941514019b8f5d9`

## `internal static partial class TAuditTruthWalker`

The relay half of the truth walker, covering which driver members stand in for logic and at which parameters.
Every set is keyed by symbol, so two members sharing a name never share a verdict.
It also holds the handle and alias rules that decide which fields the walker follows.

## `private static HashSet<ISymbol> TAuditRelayNames`

The driver members whose body requests logic, so a call to one is a request.
A local function, a delegate field, a delegate property and an event count too.
Invoking one of those is then a request, so a request behind a delegate cannot hide.

## `internal static HashSet<ISymbol> TAuditReaderNames`

The driver members whose body reads or requests logic, so a value from one is an engine value.
A request is a read too, so a relay is read through as well.

## `private static Dictionary<ISymbol, HashSet<int>> TAuditHotNames`

For each relay, the positions of the parameters that reach logic inside it.
An argument at any other position is shown, not sent, and is no sink.

## `private static void TAuditRelayRead(IReadOnlyList<TypeDeclarationSyntax> type)`

The requesting members of every driver, directly or through another relay, and the reading ones beside.
A delegate member becomes a relay when a driver assigns or subscribes a request, a relay or logic to it.
Only delegate-typed members of a shell type count, so a framework property never becomes a relay.
An interface event counts too, since a shell type raises it through its own implementation.
A delegate local becomes a relay when its initialiser or a later assignment is one.
A delegate parameter stored in a member or local passes on what its callers hand it.
Wiring, locals and seams are read over every tracked source, not only the walked drivers.
So a composition root that hands a gate to a driver constructor is traced too.
Every set and the hot positions grow to a fixed point, so a chain of wrappers is followed through.

## `private static Dictionary<ISymbol, List<ExpressionSyntax>> TAuditSeamRead(IReadOnlyList<SyntaxNode> roots)`

Every argument handed to a delegate parameter, grouped by that parameter.
It reads every tracked source, so a caller in another type or project counts too.
It lets a stored parameter be judged by what its callers pass.

## `private static bool TAuditSeamCheck(ExpressionSyntax value, IReadOnlyDictionary<ISymbol, List<ExpressionSyntax>> seams, HashSet<ISymbol> seen)`

True when a value handed to a delegate member or local is a relay.
A bare delegate parameter is one when any caller hands it one, followed through every hop.
A parameter is visited once, so a cycle of callers ends.
Only a stored parameter is judged this way.
A parameter only invoked in place stays plain, since its caller's lambda already requests there.

## `private static IParameterSymbol? TAuditParameterRead(ArgumentSyntax argument)`

The parameter an argument binds to, by name when named, else by position.
A call, a construction, a `base` or `this` initialiser and a primary base call all bind.
Null when the call does not resolve to a method.

## `private static bool TAuditDelegateCheck(ExpressionSyntax value)`

True when a value handed to a delegate member requests.
Naming a relay or a logic method as a group counts too.
Naming a relay member or local without calling it counts as well.

## `private static bool TAuditHotRead(ISymbol symbol, SyntaxNode method, ParameterListSyntax list)`

Marks a parameter hot when the body passes it, or its member, to logic or a hot relay position.
A method and a local function are read alike.
Returns true when a new position was found, so the caller loops again.

## `private static bool TAuditReadCheck(SyntaxNode member)`

True when the member's body reads a logic member or calls a reader.
A name inside `nameof` reads nothing and does not count.

## `internal static bool TAuditHandleCheck(ITypeSymbol type)`

True for a Conduct type, which a driver is meant to hold.
Also true for a listed handle, shown minimally and with any nullable mark dropped.

## `internal static HashSet<ISymbol> TAuditAliasRead(IFieldSymbol field, IReadOnlyList<TypeDeclarationSyntax> type)`

The field and every getter-only property of its class that reads it without requesting.
A getter reading such a property is followed too, to a fixed point.
Gatekeeping through one of them is gatekeeping on the field itself.
A parameter the field is passed to by `ref`, `out` or `in` is the field itself inside that method.
Such a parameter passed on again is followed too.
