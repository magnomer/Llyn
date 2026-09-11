# LRealmArchive.cs

## `public sealed class LRealmArchive`

Reads the realm standing for this workspace.

The realm is read once when the engine opens the workspace and never changes after.
Rows carry the realm's value itself, so no lookup from a local number to the value is needed.

## `public LRealmArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LRealm LRealmRead()`

Reads the realm standing for this workspace.

Throws when the row is missing, because a database without its own realm cannot stamp anything it creates.
Only a database built by an older schema can be in that state.
