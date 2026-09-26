# TVaultFakeTombstone.cs

## `internal sealed class TVaultFakeTombstone : LTombstoneVault`

An in-memory tombstone shelf keyed by entry id, so a clerk test can prove a delete filed one.

## `internal LTombstone? TTombstoneRead(long entryId)`

The tombstone filed for that entry, or `null` when none was.
No production seam reads a tombstone back, so only the fake answers.
