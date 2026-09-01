# LTombstone.cs

## `public sealed record LTombstone(`

The record that an Entry was deleted: which Entry, under which revision, and when. A tombstone outlives the Entry it names — `LTombstoneEntryId` is recorded text, not a reference to a row that still exists — so the history stays readable after the Entry is gone. One deleted Entry leaves exactly one tombstone.

**Parameters**

- `LTombstoneEntryId` — Id of the deleted Entry; recorded text, not a reference.
- `LTombstoneRevisionId` — The `LRevision` the deletion was recorded under.
- `LTombstoneDeletedUtc` — Round-trip UTC timestamp of the deletion.
