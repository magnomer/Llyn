# LDatabase.cs

## `public sealed class LDatabase`

Owns the SQLite database that lives as `llyn.db` inside the user's workspace folder.
That is the only place it ever lives (`LWorkspaceRoot.LWorkspaceDatabaseRead`).
Hands out ready connections with the workspace pragmas already applied, and initializes the file's schema on first use.
Every later data store reaches the file through here so no code reaches past the workspace for the database.

It also holds the *ambient session*: the unit of work an operation runs in.
A store asks for a session rather than a connection (`LDatabaseSessionStart`).
So several stores called one after another share one connection and one transaction.
Either all of them land or none do.
The ambient session is guarded by a gate and belongs to the thread that opened it.
A start from another thread while one is open is refused rather than silently joined.
Two overlapping units of work would otherwise share a connection and a transaction.
The first to finish would close it under the second.

## `public LDatabase(string root)`

Binds this database to `root`, the resolved workspace folder that owns the database file.
The file is not opened until a connection or a session is asked for.

## `public string LDatabaseFile`

The path of the database file this instance is bound to.
`LDoctor` needs it to move an unusable database aside, which is work on the file rather than on its contents.

## `public SqliteConnection LDatabaseConnectionRead()`

Opens the database, creating the file if absent.
Applies the workspace pragmas.
Returns the connection open and ready to use.

## `public static SqliteConnection LDatabaseConnectionRead(string file)`

The same opening for a file named by the caller.
The migration builds its fresh file through it, so the fresh file carries the same pragmas and functions.
The caller owns the connection and disposes it.
Stores use `LDatabaseSessionStart` instead, so this is for the schema runner and for tests.

Write-ahead logging lets a reader run while a writer holds the file.
The busy timeout gives a blocked statement time to wait rather than failing at once.
`synchronous = NORMAL` is the mode write-ahead logging is designed for.
Foreign keys are per connection and must be enabled on every one of them.

## `public static string LDatabaseIdFormat(IEnumerable<long> ids)`

The JSON text a list of ids travels in as one parameter.
A statement reads it back through `json_each`, so a list of any length costs one placeholder.
Binding one parameter per id would hit SQLite's cap on a corpus a few tens of thousands of rows deep.

## `public LDatabaseSession LDatabaseSessionStart()`

Starts the unit of work a store operation runs in.
With no session open this opens a connection and begins a transaction.
It then becomes the ambient session.
While one is already open on the same thread this returns a nested session.
The nested session shares that connection and transaction.
Its commit and disposal do nothing.
So the outermost session alone decides whether the work lands.
A start from any other thread while one is open throws instead.
Nesting is how one operation composes several stores, and that only ever happens on the thread running it.

## `public void LDatabaseCreate()`

Initializes the database on first use.
A file written at another schema version is rebuilt in the current shape first.
It then opens a connection and runs the schema.
So the file, its tables, and its indexes exist and its version is current.
A file that had no version table before this ran is a new workspace.
Such a file is given the Unknown Source once.
Idempotent — running it against an already-initialized database creates nothing and migrates only what is behind.

## `internal void LDatabaseSessionClear(LDatabaseSession session)`

Clears the ambient session and the thread that owned it when the session that owns it ends.

## Inline notes

### `catch`

The connection is opened here but the pragmas run afterwards.
A file that is not a database fails on the first of them.
An undisposed connection keeps the file open, and Windows refuses to move a file that is open.
So a failed setup would leave `LDoctor` unable to set the broken database aside.

### `connection.CreateFunction<string?, string?>(`

SQLite's own lower() folds ASCII only.
So a query typed as "Ä" would never find a headword stored as "ä".
That is unusable in an application whose subject is other languages.
lfold() hands the fold to .NET, which knows the whole of Unicode.
It is registered on every connection because a user-defined function lives on the connection that declared it.

### `connection.CreateFunction<string?, string?, bool>(`

lmatch() hands the query test to `LCatalogTextMatch`, so the wildcards a panel honors are the wildcards a query honors.
SQLite's own LIKE and GLOB fold differently and spell their wildcards differently.
One matcher on both sides keeps the headword catalog and the in-memory catalogs answering alike.
