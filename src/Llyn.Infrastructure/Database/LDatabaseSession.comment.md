# LDatabaseSession.cs

## `public sealed class LDatabaseSession : IDisposable`

One unit of work against the workspace database: a connection and the transaction every statement in the unit runs in. A store asks `LDatabase.LDatabaseSessionStart` for one, runs its statements against `LDatabaseSessionConnection`, and commits; disposal without a commit rolls the whole unit back.

Sessions nest. The first session opens the connection and owns it; a session started while that one is open shares its connection and transaction and owns neither — its commit and its disposal do nothing. So a store method that commits its own work still lands inside a caller's larger unit, and an operation spanning several stores is atomic without any store knowing about it. Only the outermost session decides.

## `internal LDatabaseSession(LDatabase owner, SqliteConnection connection)`

Opens the outermost session: the transaction the whole unit of work runs in.

## `internal LDatabaseSession(LDatabaseSession session)`

Opens a nested session over `session`, owning neither of its resources.

## `public SqliteConnection LDatabaseSessionConnection => _lDatabaseSessionLink;`

The open connection every statement of this unit of work runs on.

## `public void LDatabaseSessionCommit()`

Finalizes the unit of work. On the outermost session the transaction is committed and everything written since it started becomes visible; on a nested session nothing happens, because the outermost session has not finished yet.

## `public void Dispose()`

Ends the session. An outermost session that was never committed rolls back, so a store method that throws part-way leaves nothing behind; a nested session releases nothing, since it owns nothing.
