# TVaultFakeDraft.cs

## `internal sealed class TVaultFakeDraft : LDraftVault`

An in-memory drafts folder keyed by draft id.
A clerk test can hold and finish a draft on it without a workspace.
The sweep answers the ids a test marked stale, as the archive answers the files of another version.

## `internal void TDraftStaleSet(long id)`

Marks one draft as what the next sweep drops, standing in for a file of another version.
