# TInterfaceVault.cs
Hash: `77bad45b5f2ce34c`

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

## `internal static LLanguageVault TLanguageVaultCreate(string root, HttpClient client) =>`

The language loader over the workspace `root` and the given `client`, handed out as the port.

## `internal static LLanguage TLanguageCreate(IReadOnlyList<LVariety> varieties) =>`

A pack with no flag and blank fonts that declares `varieties`, for a fake language port to answer.

## `internal static LSettingsVault TSettingsVaultCreate(string root) =>`

The settings loader over the workspace `root`, handed out as the port.

## `internal static LPostureState? TPostureRead(this LPostureVault postureVault, string name) =>`

Relays the posture port's read of the posture saved under `name`.

## `internal static void TPostureSave(this LPostureVault postureVault, string name, LPostureState state)`

Relays the posture port's save of `state` under `name`.

## `internal static LWorkspaceVault TWorkspaceVaultCreate(LDatabase database) =>`

The workspace archive over `database`, handed out as the port the identity counts over.

## `internal static LIdentity TIdentityCreate(LWorkspaceVault workspaces) =>`

The identity issuer over a workspace port, as the engine builds it.

## `internal static LVaultFault TVaultFaultCreate(string message) =>`

The vault fault a port raises when its disk refuses, carrying an I/O cause with `message`.
