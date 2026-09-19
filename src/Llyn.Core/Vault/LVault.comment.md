# LVault.cs

## `public interface LVault`

The persistence port through which the engine opens one unit of work over storage.
A vault is what the engine may ask of storage for one lexicon base, with no knowledge of SQLite.
Its adapter is the `{Base}Archive` in Infrastructure, and `LDatabase` is the adapter of this root port.
The engine holds ports as fields and never builds an archive outside its constructor.

## `LVaultSession LVaultSessionStart();`

Opens a session that spans every vault call made until it is disposed.
A session opened inside another on the same thread joins it rather than starting a second one.
