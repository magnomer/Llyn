# TAuditTruthMutation.cs

## `internal static partial class TAuditTruthWalker`

The mutation half of the truth walker: what a shell line may not do to logic without a request.

## `private static void TAuditMutationScan(SyntaxNode root, List<TViolation> violations)`

Every plain assignment to a logic-named member outside an object initialiser.
An event subscription is a compound assignment and is not reported.
A tuple assignment over indexed slots is a swap, and a swap reorders a collection.
A `with` copy that sets a logic member rewrites logic the engine handed over.
A slot assignment on a store of rows overwrites what the engine ordered.
Also every order verb on a store of rows, since the engine alone owns an order.

## `private static bool TAuditStoreCheck(string name, SyntaxNode site)`

True when the named collection is declared to hold logic, shell rows or untyped objects.
The declaration is sought from the site outward, whether local, parameter, field or property.
A name with no declaration in sight is taken as a store, since the walk cannot clear it.

## `private static bool TAuditShellCheck(string name)`

True for a name that starts with `P` followed by a capital.
