# Base

This folder holds the database core.
It covers the connection, the session, association links, ordered rows, state columns and recovery.
It stands apart from the Entry and Context folders because nothing here is lexical content.
Every other store reaches the file through here.

## `LDatabase`

It owns the SQLite database, which lives only as `llyn.db` inside the workspace folder (`LWorkspaceRoot.LWorkspaceDatabaseRead`).
It hands out ready connections with the workspace pragmas applied and initializes the schema on first use.
No code reaches past the workspace for the database.
The file is not opened until a connection or a session is asked for.

`LDatabaseFile` gives the bound path.
`LDoctor` needs it to move an unusable database aside, which is work on the file rather than its contents.

`LDatabaseMigrated` tells whether `LDatabaseCreate` rebuilt the file from an older schema version.
The engine reads it once after opening, to derive the stored strings the new columns hold.
A file made or opened at the current version leaves it false.

`LDatabaseConnectionRead` opens the database, creating the file if absent, and applies the pragmas.
The static form opens a file the caller names, and the migration builds its fresh file through it.
So the fresh file carries the same pragmas and functions.
The caller owns that connection and disposes it.
Stores use `LDatabaseSessionStart` instead, so the open call is for the schema runner and for tests.

The pragmas have reasons.
Write-ahead logging lets a reader run while a writer holds the file.
The busy timeout gives a blocked statement time to wait rather than failing at once.
`synchronous = NORMAL` is the mode write-ahead logging is designed for.
Foreign keys are per connection and must be enabled on every one of them.

`LDatabaseIdFormat` gives the JSON text a list of ids travels in as one parameter.
A statement reads it back through `json_each`, so a list of any length costs one placeholder.
One parameter per id would hit SQLite's cap on a corpus a few tens of thousands of rows deep.

`LDatabaseCreate` initializes the database on first use and is idempotent.
A file written at another schema version is rebuilt in the current shape first.
Then it runs the schema, so the file, tables and indexes exist and the version is current.
A file that had no version table before is a new workspace, and it is given the Unknown Source once.
Against an initialized database it creates nothing and migrates only what is behind.

## The ambient session

`LDatabase` holds the unit of work an operation runs in.
A store asks for a session rather than a connection.
So several stores called one after another share one connection and one transaction.
Either all of them land or none do.

The ambient session is guarded by a gate and belongs to the thread that opened it.
A start from another thread while one is open throws rather than silently joining.
Two overlapping units of work would otherwise share a transaction.
The first to finish would close it under the second.

`LDatabaseSessionStart` opens a connection and begins a transaction when no session is open.
On the same thread while one is open, it returns a nested session sharing that connection and transaction.
Nesting is how one operation composes several stores, and that only happens on the thread running it.

`LDatabaseSessionClear` clears the ambient session and its thread when the owning session ends.
`LVault.LVaultSessionStart` and `LVault.LVaultMigrated` are the port faces the engine uses, so it never names SQLite.

## `LDatabaseSession`

It is one unit of work, holding a connection and the transaction every statement runs in.
A store runs its statements on `LDatabaseSessionConnection` and commits.
Disposal without a commit rolls the whole unit back.

Sessions nest.
A nested session owns neither the connection nor the transaction, and its commit and disposal do nothing.
So a store method that commits its own work still lands inside a caller's larger unit.
An operation spanning several stores is atomic without any store knowing, and only the outermost session decides.

If the outermost transaction cannot begin, the connection is disposed and the fault rethrown, so no file stays held.

`LDatabaseSessionCommit` commits on the outermost session and does nothing on a nested one.
`LVaultSession.LVaultSessionCommit` is its port face.

`Dispose` rolls back an outermost session never committed, so a store that throws part-way leaves nothing behind.
A rollback that itself fails is swallowed.
The connection may already be gone, or the commit may have failed and taken the transaction with it.
The connection is released and the ambient slot cleared regardless.
A slot left held would refuse every later session on another thread.
A rollback fault thrown here would hide the commit fault the caller is about to see.

## `LDatabaseLink`

It is one ordered association table, seen from the store that owns its member rows.
Every such table differs only in its name and its two key columns.
So one link type serves each of them.
The table and column names are store-owned literals, never caller input.
So composing them into statement text opens no injection seam, and every value still travels as a parameter.

An owner's order is a unique index, so attaching and detaching renumber that owner's whole set through `LDatabaseOrder`.
A caller names the index it wants and never has to find a free position or leave a gap.
Every method starts its own session and nests inside a caller's, so a delete and its guard stay one transaction.

`LDatabaseLinkAttach` puts the row beyond the end of the set and renumbers around it, so the requested index is honoured.
An occupied position is not a unique-index failure.
A member the owner already references keeps its single link and moves to the position.

`LDatabaseLinkDetach` removes the reference and closes the gap.
The member row and its other references survive.

`LDatabaseLinkDelete` drops every reference to the member and renumbers what each owner has left.
The owners are read before the delete, because afterwards nothing names them.
A leftover gap would be a unique-index failure waiting for the next attach.

## `LDatabaseOrder`

It rewrites the `position` column of an ordered row set to read `0, 1, 2, …` again.
This happens after an insert, move or removal.
Every ordered table carries a unique index over `(owner, position)`.
So a one-row-at-a-time reorder is impossible, since swapping two neighbours collides on the first statement.
The set is rewritten in two passes.
Every position is first shifted clear of the range by a constant.
A constant shift keeps the set unique against itself and clear of the final values.
Each position is then written back as its index in the intended order.

`LDatabaseOrderShift` is that constant, one billion.

A set is named by a scope, a store-owned SQL predicate over `$owner`.
One example is `entry_parent = $owner AND sense_parent IS NULL`.
Scopes and column names are literals the calling store chooses, never caller input, and every value travels as a parameter.

`LDatabaseOrderRead` reads the members in stored order.
The member column is the id column of a stable-id table, or the member column of an association table.
`LDatabaseOrderInsert` places a member at a target, clamping the target into the list, and returns the order to normalize.
`LDatabaseOrderNormalize` gives the listed members positions `0 … n-1` in that order.
A row in the scope left out of the list keeps its shifted position, past the last.

## `LDoctor`

It gets the program to a database it can run against, whatever state the workspace was left in.
The schema is still moving, and a rebuild from another version can fail on a file it cannot read.
That would stop the program at startup with nothing the user can do.
So the doctor sets the unusable file aside under a dated name and starts a clean one.

Nothing is deleted.
The old file stays in the workspace beside the new one, so a database is never lost to a launch.
This is the recovery of last resort, and `LSchemaMigration` still carries a database forward whenever it can.

`LDoctorDatabaseCreate` initializes the database, resetting it when it cannot be initialized, and returns what had to be done.
The caller can then tell the user their old data is no longer in front of them.
The clean database is created by the same call that failed.
So a fault in the schema itself surfaces rather than being mistaken for a second corruption.

`LDoctorRescueCheck` allows only the corrupt and not-a-database result codes.
Those are the two ways a broken or unknown file announces itself.
A busy file, a full disk, a read error and a failed rebuild propagate.
Resetting on one of those would set aside a healthy database on a bad morning.

`LDoctorBusyCheck` says another connection holds the database, or another program holds the workspace lease.
The launch names that case to the user, since closing the other program is the cure.

`LDoctorDatabaseSave` moves the file aside and returns where it went.
The connection pool is cleared first, because a pooled connection holds the file open.
Windows refuses to move an open file.
The write-ahead log and shared-memory files move with it.
One left behind would be read as part of the clean database that takes its name.

`LDoctorBackupResolve` picks a free name carrying the moment of the rescue.
So a workspace rescued twice keeps every copy in order.
A counter is added when the stamp alone is taken.
Two rescues in the same second would otherwise erase the first.

## `LStateColumn`

It reads and writes a value that knows what is known about it, through the two columns the schema keeps.
The state is always written, and the text only when the state says the value states one.
Every table storing such a value uses the same pair and wording, so the pair is handled in one place.
The schema guards the pair with a `CHECK` that no column holds text unless its state is `specified`.
So a row can never claim a value is unknown while carrying its text.

`LStateColumnCheck` tests that a wording is one of the three the store writes.
Any other wording means the row was damaged, and the reading side reports it as unreadable.
`LStateColumnParse` assumes the check passed.

The apply methods refuse an unreadable value, anchor or mark, since a diagnosis is never a value to store.
A value goes into `$<field>State` and `$<field>`, with `NULL` text when it states none.
An anchor keeps its Reference id where a value keeps text, `NULL` when it points nowhere, as the `CHECK` demands.
A bare mark sets only `$<field>State`, for a field whose content lives in another table.

The read methods take the state at a column and the text or id directly after it.
That is the order every `SELECT` lists them in.
An unknown state wording yields an unreadable result, carrying whatever text stood there.
An anchor has no unknown answer, because "source not known" is a Reference row to point at.
So the `unknown` wording older workspaces wrote reads as unreadable.
Migration rewrites that wording to a link before the row is read this way.

## Inline notes

### LDatabase.cs: catch

The connection is opened before the pragmas run.
A file that is not a database fails on the first of them.
An undisposed connection keeps the file open, and Windows refuses to move an open file.
So a failed setup would leave `LDoctor` unable to set the broken database aside.

### LDatabase.cs: connection.CreateFunction<string?, string?>(

SQLite's own lower() folds ASCII only.
So a query typed as "Ä" would never find a headword stored as "ä".
That is unusable in an application whose subject is other languages.
lfold() hands the fold to .NET, which knows the whole of Unicode.
It is registered on every connection because a user-defined function lives on the connection that declared it.

### LDatabase.cs: connection.CreateFunction<string?, string?, bool>(

lmatch() hands the query test to `LCatalogTextMatch`, so the wildcards a panel honors are the wildcards a query honors.
SQLite's own LIKE and GLOB fold differently and spell their wildcards differently.
One matcher on both sides keeps the headword catalog and the in-memory catalogs answering alike.

## Stamp

Commit: `52c21d68b4c4`
- `LDatabase.cs` `7ed9662f8bae6357`
- `LDatabaseLink.cs` `6213269c5836f43a`
- `LDatabaseOrder.cs` `d02d69fd03dc1028`
- `LDatabaseSession.cs` `2d0a0294da057251`
- `LDoctor.cs` `de8fc5f895a0fac4`
- `LStateColumn.cs` `346a55dfd3d09118`
