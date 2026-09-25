# LDatabaseSession.cs

## `public sealed class LDatabaseSession : LVaultSession, IDisposable`

One unit of work against the workspace database: a connection and the transaction every statement in the unit runs in.
A store asks `LDatabase.LDatabaseSessionStart` for one.
It runs its statements against `LDatabaseSessionConnection` and commits.
Disposal without a commit rolls the whole unit back.

Sessions nest.
The first session opens the connection and owns it.
A session started while that one is open shares its connection and transaction.
It owns neither, and its commit and its disposal do nothing.
So a store method that commits its own work still lands inside a caller's larger unit.
An operation spanning several stores is atomic without any store knowing about it.
Only the outermost session decides.

## `internal LDatabaseSession(LDatabase owner, SqliteConnection connection)`

Opens the outermost session: the transaction the whole unit of work runs in.

## `internal LDatabaseSession(LDatabaseSession session)`

Opens a nested session over `session`, owning neither of its resources.

## `public SqliteConnection LDatabaseSessionConnection => _lDatabaseSessionLink;`

The open connection every statement of this unit of work runs on.

## `public void LDatabaseSessionCommit()`

Finalizes the unit of work.
On the outermost session the transaction is committed.
Everything written since it started becomes visible.
On a nested session nothing happens, because the outermost session has not finished yet.

## `void LVaultSession.LVaultSessionCommit()`

The port face of `LDatabaseSessionCommit`, called by the engine on the session its vault opened.

## `public void Dispose()`

Ends the session.
An outermost session that was never committed rolls back.
So a store method that throws part-way leaves nothing behind.
A nested session releases nothing, since it owns nothing.

A rollback that itself fails is swallowed.
The connection may already be gone, or the commit may have failed and taken the transaction with it.
Either way the connection is released and the ambient slot cleared regardless.
A slot left held would refuse every later session on another thread.
And a rollback fault thrown here would hide the commit fault the caller is about to see.
