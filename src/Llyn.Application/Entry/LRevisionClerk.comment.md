# LRevisionClerk.cs
Hash: `c92a29ecc0351b70`

## `public sealed class LRevisionClerk`

The revision stamp every clerk that writes history shares.
A stamp records one revision and moves the workspace row onto it.
Entries, translation stubs, citations and the markup import all stamp through here.
So the pointer to the current revision can never drift from the revision recorded.

## `public LRevisionClerk(LRig rig)`

Reads the root, the revision and the workspace ports out of `rig`.

## `public LRevision LRevisionClerkRecord(IReadOnlyList<LRevisionDelta> changes)`

Records one revision holding `changes` and points the workspace row at it.
Every recorded revision moves that pointer, so the current revision is read from where it is recorded.
Both writes share one session, so a caller's open session folds them into its own transaction.
Returns the recorded revision.

## `internal void LRevisionClerkRecord(long target, string subject, bool fresh, string? summary)`

Records one revision holding a single change for `target` of kind `subject`.
The change reads create when `fresh` holds and update otherwise.
Every citation commit stamps through here, so authors, examples, sources and situations word their change alike.
