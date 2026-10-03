# TDatabaseSession.cs
Hash: `1fa6062d74eacb4d`

## `public sealed class TDatabaseSession`

Covers the unit of work and the ordering it makes possible.
A session spanning several stores lands as one transaction or not at all.
A nested session leaves the decision to the outermost one.
Every ordered set stays numbered `0 … n-1` across a removal and a rewrite of the set.
Those are the reorders the unique position indexes used to make impossible.
A session whose connection is already closed still releases the ambient slot when disposed.
The next session must open cleanly, or every later store call would be refused.
An example update keeps its translations and the row ids of reordered ones.
The test identity counter hands out negative values that never repeat.

## Inline notes

### `Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));`

The store committed its own nested session.
The outermost one never does.

### `private static IReadOnlyList<long> TDatabasePositionRead(`

The stored positions of one referrer's association rows, in order.
