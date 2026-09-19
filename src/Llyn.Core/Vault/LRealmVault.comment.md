# LRealmVault.cs

## `public interface LRealmVault`

The persistence port for the Realm rows the engine reads and writes.
It lists exactly what the engine asks of realm storage, and nothing about how rows are kept.
`LRealmArchive` in Infrastructure is its adapter over the workspace database.

## `LRealm LRealmRead();`

Reads the realm standing for this workspace.

Throws when the row is missing, because a database without its own realm cannot stamp anything it creates.
Only a database built by an older schema can be in that state.
