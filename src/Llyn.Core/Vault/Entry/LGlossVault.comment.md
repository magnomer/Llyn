# LGlossVault.cs

## `public interface LGlossVault`

The persistence port for the Gloss rows the engine reads and writes.
It lists exactly what the engine asks of gloss storage, and nothing about how rows are kept.
`LGlossArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LGloss> LGlossExampleRead(long exampleId);`

Every Gloss of the Example identified by `exampleId`, in position order.

## `IReadOnlyList<long> LGlossExampleSave(long exampleId, IReadOnlyList<LGloss> glosses);`

Rewrites the Gloss list of one Example and answers the written ids in input order.
The position of each row is its index in the list.
So the caller states the order by the order it sends.
Existing rows are shelved by shifting their position far up, so the unique (example, position) index never trips.
A row whose positive id survives is rewritten in place, the rest are inserted, and the shelf is deleted.
This is the same shape as `LMentionExampleSave`.
