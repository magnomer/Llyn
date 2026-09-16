# TInterfaceDatabase.cs

## `internal static partial class TInterface`

The relays for the infrastructure layer.
They open a store and run one store operation.
The stores of the entry itself stand here: its rows, forms, speeches, examples, media, notes, inflections and readings.
The database session, the doctor, the realm, the revision log and the workspace root are relayed here too.
The stores of what hangs on a meaning or collocation are relayed in `TInterfaceMeaning.cs`.
Each relay is transparent and carries no test logic of its own.

## `internal static Guid TRealmValueRead(TWorkspace workspace)`

The realm value of a test workspace, which the migration and realm tests compare across a rebuild.
