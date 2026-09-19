# LVault.cs

## `public interface LVault`

The persistence port through which the engine opens one unit of work over storage.
A vault is what the engine may ask of storage for one lexicon base, with no knowledge of SQLite.
Its adapter is the `{Base}Archive` in Infrastructure, and `LDatabase` is the adapter of this root port.
The engine holds ports as fields handed in by the rig and never builds an archive itself.

## `LVaultSession LVaultSessionStart();`

Opens a session that spans every vault call made until it is disposed.
A session opened inside another on the same thread joins it rather than starting a second one.

## `bool LVaultMigrated { get; }`

Whether opening the storage rebuilt it from an older schema.
The engine refills its derived strings once after such an open.
