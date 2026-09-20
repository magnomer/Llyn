# LTombstoneVault.cs

## `public interface LTombstoneVault`

The persistence port for tombstones, the records that an entry was deleted.
`LTombstoneArchive` in Infrastructure is its adapter over the workspace database.

## `LTombstone LTombstoneRecord(long entryId, long revisionId);`

Records that the entry identified by `entryId` was deleted under `revisionId`.
Returns the stored tombstone.

## `LTombstone? LTombstoneRead(long entryId);`

Reads the tombstone for `entryId`, or `null` when that entry has never been deleted.
