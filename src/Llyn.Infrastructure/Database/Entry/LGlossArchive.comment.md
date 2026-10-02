# LGlossArchive.cs
Hash: `71ec037cb2ae9ae1`

## `public sealed class LGlossArchive : LGlossVault`

Persists Glosses, the renderings of an Example's sentence in other languages.
A Gloss is the Example's own row, so it is read and written by Example.
`LExampleArchive` calls the static reader and saver here for every Example it returns or writes.
The rows sit in the `example_translation` table, named after the Translation the schema speaks of.

## `public LGlossArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LGloss> LGlossExampleRead(long exampleId)`

Every Gloss of the Example identified by `exampleId`, in position order.

## `public IReadOnlyList<long> LGlossExampleSave(long exampleId, IReadOnlyList<LGloss> glosses)`

Rewrites the Gloss list of one Example and answers the written ids in input order.
The position of each row is its index in the list.
So the caller states the order by the order it sends.
Existing rows are shelved by shifting their position far up, so the unique (example, position) index never trips.
A row whose positive id survives is rewritten in place, the rest are inserted, and the shelf is deleted.
This is the same shape as `LMentionExampleSave`.

## Inline notes

### `internal static IReadOnlyDictionary<long, IReadOnlyList<LGloss>> LGlossExampleRead(`

Every Gloss of a set of Examples in one statement, keyed by Example.
The ids travel as one JSON parameter, so the whole corpus fits in one statement.
An Example with no Gloss has no key, and the caller reads that as an empty list.

### `internal static IReadOnlyList<long> LGlossExampleSave(`

The rewrite on a connection the caller already holds.
So an Example insert and its Glosses commit together.
