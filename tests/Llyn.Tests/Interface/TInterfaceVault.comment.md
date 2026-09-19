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

## `internal static LLanguageVault TLanguageVaultCreate()`

The language loader over the packs the test build copies beside it, handed out as the port.

## `internal static LSettingsVault TSettingsVaultCreate(string root)`

The settings loader over the workspace `root`, handed out as the port.

## `internal static LWorkspaceVault TWorkspaceVaultCreate(LDatabase database)`

The workspace archive over `database`, handed out as the port the identity counts over.

## `internal static LIdentity TIdentityCreate(LWorkspaceVault workspaces)`

The identity issuer over a workspace port, as the engine builds it.
