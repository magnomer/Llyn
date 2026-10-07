# TEntryStore.cs
Hash: `d81f2ef0a7dc4daa`

## `public sealed class TEntryStore`

Covers what an Entry owns and what it must not take with it.
That is the cascade that removes everything beneath a deleted Entry.
It is also the refusal to report success for an update that reached no row.
A headword lookup lists entries tied on headword by language spelling, then by id.
So the import window never lists its candidates in storage order alone.

## Inline notes

### `Assert.NotNull(examples.TExampleRead(example.LExampleId));`

The Example was referenced, never owned, so it outlives the Entry that pointed at it.

### `Assert.Equal("Äpfel", Assert.Single(entries.TEntryFind("äpfel")).LEntryHeadword);`

SQLite's own lower() folds ASCII only, so this is the case the old matching could not make.

### `Assert.Equal(2, entries.TEntryFind("   ").Count);`

A query of whitespace alone lists everything, exactly as an empty box does.
A query is trimmed before it is matched.
