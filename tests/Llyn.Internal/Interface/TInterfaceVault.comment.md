# TInterfaceVault.cs
Hash: `e8007546d884076f`

## `internal static partial class TInterface`

The relays for the persistence ports.
They open an adapter typed as its port and run one port operation.
A vault test proves the adapter keeps the port's promise, so it never sees the adapter's own type.
Each relay is transparent and carries no test logic of its own.

## `internal static LEntryVault TEntryVaultCreate(LDatabase database) =>`

The entry archive over `database`, handed out as the port the engine holds.

## `internal static LDraftVault TDraftVaultCreate(string root) =>`

The draft archive over the workspace `root`, handed out as the port the engine holds.

## `internal static LLanguageVault TLanguageVaultCreate() =>`

The language loader over the temp folder with a source client that always answers not found.
It is handed out as the port.

## `internal static LLanguage TLanguageCreate(IReadOnlyList<LVariety> varieties) =>`

A pack with no flag and blank fonts that declares `varieties`, for a fake language port to answer.

## `internal static LSettingsVault TSettingsVaultCreate(string root) =>`

The settings loader over the workspace `root`, handed out as the port.

## `internal static LWorkspaceVault TWorkspaceVaultCreate(LDatabase database) =>`

The workspace archive over `database`, handed out as the port the identity counts over.

## `internal static LIdentity TIdentityCreate(LWorkspaceVault workspaces) =>`

The identity issuer over a workspace port, as the engine builds it.
