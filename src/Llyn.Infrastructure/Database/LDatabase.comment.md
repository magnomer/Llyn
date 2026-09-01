# LDatabase.cs

## `public sealed class LDatabase`

Owns the SQLite database that lives as `llyn.db` inside the user's workspace folder — the only place it ever lives (`LWorkspaceRoot.LWorkspaceDatabaseRead`). Hands out ready connections with the workspace pragmas already applied, and initializes the file's schema on first use. Every later data store reaches the file through here so no code reaches past the workspace for the database.

It also holds the *ambient session*: the unit of work an operation runs in. A store asks for a session rather than a connection (`LDatabaseSessionStart`), so several stores called one after another share one connection and one transaction and either all land or none do. The ambient session is a plain field, so a single `LDatabase` instance belongs to one thread — which is what the shell engine does with it.

## `public LDatabase(string root)`

Binds this database to `root`, the resolved workspace folder that owns the database file. The file is not opened until a connection or a session is asked for.

## `public SqliteConnection LDatabaseConnectionRead()`

Opens the database (creating the file if absent), applies the workspace pragmas, and returns the connection open and ready to use. The caller owns the connection and disposes it. Stores use `LDatabaseSessionStart` instead, so this is for the schema runner and for tests.

Write-ahead logging lets a reader run while a writer holds the file; the busy timeout gives a blocked statement time to wait rather than failing at once; `synchronous = NORMAL` is the mode write-ahead logging is designed for. Foreign keys are per connection and must be enabled on every one of them.

## `public LDatabaseSession LDatabaseSessionStart()`

Starts the unit of work a store operation runs in. With no session open this opens a connection, begins a transaction, and becomes the ambient session; while one is already open this returns a nested session that shares that connection and transaction, whose commit and disposal do nothing so the outermost session alone decides whether the work lands.

## `public void LDatabaseCreate()`

Initializes the database on first use: opens a connection and runs the schema so the file, its tables, and its indexes exist and its version is current. Idempotent — running it against an already-initialized database creates nothing and migrates only what is behind.

## `internal void LDatabaseSessionClear(LDatabaseSession session)`

Clears the ambient session when the session that owns it ends.

## Inline notes

### `connection.CreateFunction<string?, string?>(`

SQLite's own lower() folds ASCII only, so a query typed as "Ä" would never find a headword stored as "ä" — unusable in an application whose subject is other languages. lfold() hands the fold to .NET, which knows the whole of Unicode. It is registered on every connection because a user-defined function lives on the connection that declared it.
