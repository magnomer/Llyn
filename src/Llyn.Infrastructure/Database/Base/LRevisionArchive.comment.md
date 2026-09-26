# LRevisionArchive.cs

## `public sealed class LRevisionArchive : LRevisionVault`

It is the adapter of `LRevisionVault`, the port the engine holds.

Persists revisions and the ordered changes recorded under them.
A revision is written once, complete.
`LRevisionRecord` stamps it and inserts its changes in list order inside one transaction.
So a half-written revision never reaches the database.

The change rows name their targets as recorded text rather than by foreign key.
So a revision stays readable after the rows it describes are deleted.
That is exactly the case the history exists for.
Nothing here deletes lexical data.
The stores that own that data do.
The caller hands the resulting change list to this store afterwards.

## `public LRevisionArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LRevision LRevisionRecord(IReadOnlyList<LRevisionDelta> changes)`

Opens a revision with a fresh opaque id and the current UTC timestamp.
It records `changes` under it in list order.
Each stored position comes from that order, not from the position the caller set.
Returns the stored revision.
The whole write is one transaction.
An empty change list records an empty revision.
