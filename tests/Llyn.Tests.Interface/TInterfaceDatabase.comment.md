# TInterfaceDatabase.cs
Hash: `540701a1d06613ff`

## `internal static partial class TInterface`

The relays for the infrastructure layer.
They open a store and run one store operation.
The stores of the entry itself stand here: its rows, forms, speeches, examples, media, notes, inflections and readings.
The database session, the doctor, the realm, the revision log and the workspace root are relayed here too.
A retirement of a part of speech is built and applied here, against the speech store.
The stores of what hangs on a meaning or collocation are relayed in `TInterfaceMeaning.cs`.
Each relay is transparent and carries no test logic of its own.

## `internal static Guid TRealmValueRead(TWorkspace workspace)`

The realm value of a test workspace, which the migration and realm tests compare across a rebuild.

## `private static long TInterfaceIdentity = -1_000_000_000;`

The last temporary id a test built by hand.
It starts far below the ids a fresh workspace issues, so a hand-built id never names a real draft row.
A test counting on an unknown row would otherwise meet the row the engine just made, depending on test order.
