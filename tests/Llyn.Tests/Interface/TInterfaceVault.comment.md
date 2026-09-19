# TInterfaceVault.cs

## `internal static partial class TInterface`

The relays for the persistence ports.
They open an adapter typed as its port and run one port operation.
A vault test proves the adapter keeps the port's promise, so it never sees the adapter's own type.
Each relay is transparent and carries no test logic of its own.

## `internal static LEntryVault TEntryVaultCreate(LDatabase database)`

The entry archive over `database`, handed out as the port the engine holds.

## `internal static LDraftVault TDraftVaultCreate(string root)`

The draft archive over the workspace `root`, handed out as the port the engine holds.
