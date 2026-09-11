# LRealmArchive.cs

## `public sealed class LRealmArchive`

Reads the realms the workspace knows and admits new ones as merges bring them.

The workspace's own realm is read far more often than it changes, because it stamps every row created.
A foreign realm appears only when an import carries rows minted elsewhere.

## `public LRealmArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LRealm LRealmRead()`

Reads the realm standing for this workspace.

Throws when the row is missing, because a database without its own realm cannot stamp anything it creates.
Only a database built by an older schema can be in that state.

## `public static LRealm? LRealmSingleRead(SqliteConnection connection, long id)`

Reads the realm carrying the local surrogate `id`, or `null` when none does.

Takes a connection rather than opening a session, so a merge can read realms inside the transaction it is already running.

## `public static long LRealmResolve(SqliteConnection connection, Guid value)`

Reads the local surrogate standing for the global realm `value`, admitting the realm when it is new.

An import calls this once per incoming workspace and stamps every row it brings with the answer.
Calling it twice for the same workspace yields the same surrogate, so an import repeated is not an import doubled.
