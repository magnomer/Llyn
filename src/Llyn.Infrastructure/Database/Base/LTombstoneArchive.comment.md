# LTombstoneArchive.cs

## `public sealed class LTombstoneArchive : LTombstoneVault`

It is the adapter of `LTombstoneVault`, the port the engine holds.

Persists the record that an Entry was deleted.
A tombstone is written after the Entry row is gone and names it as recorded text.
So it survives the deletion it describes.
The revision it is filed under is a real reference and must already exist.

One deleted Entry leaves exactly one tombstone, because `entry_ref` is the primary key.
So recording the same Entry twice is a conflict rather than a second row.
This store never deletes lexical data — `LEntryArchive.LEntryDelete` does that, and the caller records the tombstone afterwards.

## `public LTombstoneArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LTombstone LTombstoneRecord(long entryId, long revisionId)`

Records that the Entry identified by `entryId` was deleted under `revisionId`.
It is stamped with the current UTC time.
Returns the stored tombstone.

## `public LTombstone? LTombstoneRead(long entryId)`

Reads the tombstone for the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `public IReadOnlyList<LTombstone> LTombstoneRevisionRead(long revisionId)`

Reads every tombstone filed under `revisionId`, oldest deletion first.
