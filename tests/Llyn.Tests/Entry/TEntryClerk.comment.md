# TEntryClerk.cs

## `public sealed class TEntryClerk`

Covers the entry clerk on its own, over a rig of fakes and no engine.

## `public void EntryClerkRead_AfterCreate_ReturnsStoredEntry()`

An entry created through the clerk is read back under the id the vault gave it.

## `public void EntryClerkFind_ReverseOrder_ListsLastHeadwordFirst()`

The ordering is applied over what the vault answered, so a reverse order lists the later headword first.

## `public void EntryClerkDelete_StoredEntry_LeavesTombstoneAndRevision()`

A delete drops the row, files a tombstone under a new revision and moves the workspace row onto it.
The revision carries one change naming the deleted headword.

## `public void EntryClerkDelete_ZeroId_ThrowsOutOfRange()`

A delete of nothing is an argument error before any store is touched.
