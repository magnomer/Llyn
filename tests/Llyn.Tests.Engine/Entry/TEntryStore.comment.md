# TEntryStore.cs
Hash: `de0802e0a187965c`

## `public sealed class TEntryStore`

Covers what an Entry owns and what it must not take with it.
That is the cascade that removes everything beneath a deleted Entry.
It is also the refusal to report success for an update that reached no row.

## Inline notes

### `Assert.NotNull(examples.TExampleRead(example.LExampleId));`

The Example was referenced, never owned, so it outlives the Entry that pointed at it.
