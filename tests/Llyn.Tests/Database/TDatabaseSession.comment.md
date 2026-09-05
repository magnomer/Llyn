# TDatabaseSession.cs

## `public sealed class TDatabaseSession`

Covers the unit of work and the ordering it makes possible.
A session spanning several stores lands as one transaction or not at all.
A nested session leaves the decision to the outermost one.
Every ordered set stays numbered `0 … n-1` across a move, an insert, and a removal.
Those are the reorders the unique position indexes used to make impossible.

## Inline notes

### `Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));`

The store committed its own nested session.
The outermost one never does.

### `private static IReadOnlyList<long> TDatabasePositionRead(`

The stored positions of one referrer's association rows, in order.
