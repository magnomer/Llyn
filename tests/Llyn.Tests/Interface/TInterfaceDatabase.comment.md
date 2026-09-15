# TInterfaceDatabase.cs

## `internal static partial class TInterface`

The relays for the infrastructure layer.
They open a store and run one store operation.
Each relay is transparent and carries no test logic of its own.

## `internal static Guid TRealmValueRead(TWorkspace workspace)`

The realm value of a test workspace, which the migration and realm tests compare across a rebuild.
