# TAuditChainWalker.cs

## `internal static class TAuditChainWalker`

Reads, for every type one ring names from another, which ring declares it and whether it is data.
The verdict comes from the binder, not from the position of the name in the line.

## `public static IReadOnlyList<TAuditHit> TAuditRun()`

Scans each tree of a ring on its own thread and returns every hit ordered by file and line.

## `public static IReadOnlyList<TAuditHit> TAuditDeclaredRead()`

Every type declared inside a ring, as a `declared` hit naming the ring and the type.

## `public static IReadOnlyDictionary<string, int> TAuditFileRead()`

The source file count of every ring.

## `public static IReadOnlyList<TAuditHit> TAuditSurfaceScan()`

Walks every public member of each surface type written for a cut pair.
Each return, parameter, property or event type from a ring below the neighbour is an `expose` hit.
The hit sits at the member line and names the cut ring and the deeper ring.

## `private static IEnumerable<INamedTypeSymbol> TAuditSignatureRead(ISymbol member)`

Every named type in a member signature, with array elements and type arguments unwrapped.

## `private static Dictionary<string, HashSet<string>> TAuditInnerRead()`

The rings each ring may see at any depth: its reach, then the reach of that reach, and so on.

## `private static List<TAuditHit> TAuditTreeScan(SemanticModel model, string relative, string ring, Dictionary<string, HashSet<string>> inner)`

Walks one tree and gives every simple name bound to a type of another ring its verdict.
It is `root` in a root file and `neighbour` for the ring the table names.
Past the neighbour it is `outward` beyond the closure, `carry` for data and `reach` for behaviour.
In a cut ring, a name from outside the UI rings is `cross` ahead of `carry`.
One hit per line, kind, target and name, so a name used twice on a line counts once.
