# LRevisionArchive.cs

## `public sealed class LRevisionArchive`

Persists revisions and the ordered changes recorded under them. A revision is written once, complete: `LRevisionRecord` stamps it and inserts its changes in list order inside one transaction, so a half-written revision never reaches the database.

The change rows name their targets as recorded text rather than by foreign key, so a revision stays readable after the rows it describes are deleted — which is exactly the case the history exists for. Nothing here deletes lexical data; the stores that own that data do, and the caller hands the resulting change list to this store afterwards.

## `public LRevisionArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LRevision LRevisionRecord(IReadOnlyList<LRevisionChange> changes)`

Opens a revision with a fresh opaque id and the current UTC timestamp and records `changes` under it in list order — each stored position comes from that order, not from the position the caller happened to set. Returns the stored revision. The whole write is one transaction; an empty change list records an empty revision.

## `public LRevision? LRevisionRead(string id)`

Reads the revision for `id`, or `null` when no revision has that id.

## `public LRevision? LRevisionLatestRead()`

Reads the most recently opened revision, or `null` when the workspace has none yet. Ordering is by the stamped timestamp, then by id, so two revisions opened in the same tick still read back in one stable order.

## `public IReadOnlyList<LRevisionChange> LRevisionChangeRead(string revisionId)`

Reads the changes recorded under `revisionId`, in the order they were recorded.
