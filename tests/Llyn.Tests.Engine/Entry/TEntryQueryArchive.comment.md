# TEntryQueryArchive.cs
Hash: `b3d51eb9f87237ac`

## `public sealed class TEntryQueryArchive`

Covers the read-only entry queries over a real workspace database.
A find folds case beyond ASCII and trims the query before it matches.
A headword lookup lists entries tied on headword by language spelling, then by id.
So the import window never lists its candidates in storage order alone.

## Inline notes

### `Assert.Equal("Äpfel", Assert.Single(queries.TEntryFind("äpfel")).LEntryHeadword);`

SQLite's own lower() folds ASCII only, so this is the case the old matching could not make.

### `Assert.Equal(2, queries.TEntryFind("   ").Count);`

A query of whitespace alone lists everything, exactly as an empty box does.
A query is trimmed before it is matched.
