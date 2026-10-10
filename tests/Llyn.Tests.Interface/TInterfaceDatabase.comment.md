# TInterfaceDatabase.cs
Hash: `ca2c48221455bf3a`

## `internal static partial class TInterface`

The relays for the infrastructure layer.
They open a store and run one store operation.
Entry-owned rows, forms, speeches, examples, media, notes, inflections, lacunae and readings are relayed here.
The database session, the doctor, the realm, the revision log and the workspace root are relayed here too.
A retirement of a part of speech is built and applied here, against the speech store.
The fanqie store saves and reads a character's rows here, and sets a row's representative rank.
The stores of what hangs on a meaning or collocation are relayed in `TInterfaceMeaning.cs`.
Each relay is transparent and carries no test logic of its own.

## `internal static Guid TRealmValueRead(TWorkspace workspace)`

The realm value of a test workspace, which the migration and realm tests compare across a rebuild.

## `private static long TInterfaceIdentity = -1_000_000_000;`

The last temporary id a test built by hand.
It starts far below the ids a fresh workspace issues, so a hand-built id never names a real draft row.
A test counting on an unknown row would otherwise meet the row the engine just made, depending on test order.

## `internal static void TInflectionAnalysisSave(this LInflectionArchive inflectionArchive, long inflectionId, string? prediction, IReadOnlyList<LInflectionMark>? marks, string? stamp, bool regular)`

The archive owns serialization and null handling, so the relay preserves every supplied analysis value unchanged.

## `internal static IReadOnlyList<LRevisionDelta> TRevisionChangeRead(this TWorkspace workspace, long revisionId)`

Reads persisted revision changes directly in position order, independently of the archive adapter.
