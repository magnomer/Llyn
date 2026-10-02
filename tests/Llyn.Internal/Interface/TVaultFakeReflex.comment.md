# TVaultFakeReflex.cs
Hash: `f5c05bb32d9f3a49`

## `internal sealed class TVaultFakeReflex : LReflexVault`

An in-memory reflex store, so an entry clerk save can run its reflex sync without SQLite.
Rows are kept per entry and a set replaces the list, numbering the rows from one.
The anchor scan answers nothing, since no clerk test tallies anchors.
