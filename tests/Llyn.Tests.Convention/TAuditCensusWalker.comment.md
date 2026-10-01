# TAuditCensusWalker.cs

## `internal static class TAuditCensusWalker`

Reads what each ring holds: its declared types and its source file count.
A ring is any project the border names, read from the bound trees.

## `public static IReadOnlyList<TAuditHit> TAuditDeclaredRead()`

Every type declared inside a ring, as a `declared` hit naming the ring and the type.

## `public static IReadOnlyDictionary<string, int> TAuditFileRead()`

The source file count of every ring.
