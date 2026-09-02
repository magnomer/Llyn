# TEntryArchive.cs

## `public sealed class TEntryArchive`

Covers what an Entry owns and what it must not take with it.
That is the cascade that removes everything beneath a deleted Entry.
It is also the guard that refuses the delete while another Entry still links to it.
It is also the refusal to report success for an update that reached no row.

## Inline notes

### `Assert.NotNull(examples.LExampleRead(example.LExampleId));`

The Example was referenced, never owned, so it outlives the Entry that pointed at it.

### `Assert.Equal("Äpfel", Assert.Single(entries.LEntryFind("äpfel")).LEntryHeadword);`

SQLite's own lower() folds ASCII only, so this is the case the old matching could not make.

### `Assert.Equal(2, entries.LEntryFind("   ").Count);`

A query of whitespace alone lists everything, exactly as an empty box does.
A query is trimmed before it is matched.
