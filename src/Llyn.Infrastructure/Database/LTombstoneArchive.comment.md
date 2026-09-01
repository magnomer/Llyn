# LTombstoneArchive.cs

## `public sealed class LTombstoneArchive`

Persists the record that an Entry was deleted. A tombstone is written after the Entry row is gone and names it as recorded text, so it survives the deletion it describes; the revision it is filed under is a real reference and must already exist.

One deleted Entry leaves exactly one tombstone: `entry_id` is the primary key, so recording the same Entry twice is a conflict rather than a second row. This store never deletes lexical data — `LEntryArchive.LEntryDelete` does that, and the caller records the tombstone afterwards.

## `public LTombstoneArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LTombstone LTombstoneRecord(string entryId, string revisionId)`

Records that the Entry identified by `entryId` was deleted under `revisionId`, stamped with the current UTC time, and returns the stored tombstone.

## `public LTombstone? LTombstoneRead(string entryId)`

Reads the tombstone for the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `public IReadOnlyList<LTombstone> LTombstoneRevisionRead(string revisionId)`

Reads every tombstone filed under `revisionId`, oldest deletion first.
