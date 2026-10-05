# TAuditBorderWalker.cs
Hash: `fa34a805c14b87ba`

## `internal static class TAuditBorderWalker`

Reads, for every type one ring names from another, which ring declares it and whether it is data.
The verdict comes from the binder, not from the position of the name in the line.

## `public static IReadOnlyList<TAuditHit> TAuditRun()`

Scans each tree of a ring on its own thread and returns every hit ordered by file and line.

## `public static IReadOnlyList<TAuditHit> TAuditOfferScan()`

Walks every public member of each offer type written for a cut pair.
Each signature type from a ring below the neighbour is a `Leaking` hit.
Nested public types are walked with their members.
The hit sits at the member line and names the cut ring and the deeper ring.

## `public static IReadOnlyList<TAuditHit> TAuditDriftScan()`

Reads every source type with its name, its ring and whether it is public through every containing type.
It hands those types and the three offer settings to `TAuditDriftRead`.

## `public static IReadOnlyList<TAuditHit> TAuditDriftRead(IReadOnlyDictionary<string, string[]> offers, IReadOnlyCollection<string> cut, IReadOnlyDictionary<string, string[]> prefixes, IReadOnlyList<(string TAuditDriftName, string? TAuditDriftRing, bool TAuditDriftPublic)> types)`

The pure drift comparison, which binds nothing, so a specimen can feed it a hand-written list.
Clause (a) holds every entry to exactly one public type of the neighbour carrying its prefix.
Each entry reports only its first failing check, so one broken entry gives one hit.
Clause (b) holds a cut pair to every public type of its neighbour.
It reports only the unlisted types, since (a) already catches every listed type that is not public there.
Clause (c) holds pairs sharing a neighbour to one list, reporting each entry the other pair adds.
The engine pair sits outside the cut, so it skips (b) and may omit engine types.
Every hit sits at the neighbour folder with line zero, as the script places it.

## `public static IReadOnlyList<TAuditHit> TAuditSealScan()`

Walks every public member of each public top-level type a seal prefix names in its ring.
Each signature type from a ring below the ring's neighbour is an `Unsealing` hit.
Nested public types are walked with their members, as the offer scan walks them.
A ring that names more or fewer than one neighbour is skipped, since it has no single neighbour.

## `public static IReadOnlyList<TAuditHit> TAuditLingerScan()`

Reads every `+=` and `-=` in Conduct whose left side binds to an event.
A `+=` is guarded when the event's declaring type sits in a ring below Conduct.
The ring comes from the declaring type's source path, never its name prefix, so L-named Conduct events stay unguarded.
A guarded `+=` is a `Lingering` hit unless its type removes the same event with the same handler.
The removal may sit in any partial part and any method, and its receiver need not match.
A lambda or anonymous method handler is always a hit, since no `-=` can name it.
A type subscribing to its own event is skipped.
The pair runs from Conduct to the event's ring, and one hit is kept per line and name.

## `private static bool TAuditSealCheck(string name, string[] prefixes)`

Whether a type name starts with one of the ring's seal prefixes.

## `private static bool TAuditPublicCheck(INamedTypeSymbol type)`

Whether the type and every type containing it are declared public, so the type is reachable from outside.

## `private static IEnumerable<ISymbol> TAuditPublicRead(INamedTypeSymbol type)`

Every member of the type and of each public type nested in it.

## `private static IEnumerable<INamedTypeSymbol> TAuditSignatureRead(ISymbol member)`

Every named type in a member signature, with array elements and type arguments unwrapped.
A plain method, a property, an event, a field and a nested type's bases each have a signature.
A property accessor is covered by its property.

## `private static Dictionary<string, HashSet<string>> TAuditInnerRead()`

The rings each ring may see at any depth, which are its neighbour, that neighbour's neighbour, and so on.

## `private static List<TAuditHit> TAuditTreeScan(SemanticModel model, string relative, string ring, Dictionary<string, HashSet<string>> inner)`

Walks one tree and gives every simple name bound to a type of another ring its verdict.
It is `Commuting` for the neighbour the table names and for the ring's capsule.
Past the neighbour it is `Trespassing` beyond the closure, `Ferrying` for data and `Leapfrogging` for behaviour.
A method called on a data type is behaviour, so it is `Leapfrogging`.
In a cut ring, a name from outside the UI rings is `Undercutting` ahead of `Ferrying`.
A `using` of a deeper ring counts as data.
It is `Undercutting` from a cut ring to an uncut one and `Ferrying` otherwise.
One hit per line, kind, target and name, so a name used twice on a line counts once.

## `private static string? TAuditSpaceRead(string space)`

The ring whose name is the longest prefix of the namespace, or null outside every ring.
