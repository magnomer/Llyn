# TVaultFakeCourt.cs

## `internal sealed class TVaultFakeCourt : LCourtVault`

An in-memory court keyed by link id.
A clerk test can link a draft to a tentative target on it and cancel the draft.
Settling answers and removes the rows pointing at the target, as the archive does.
The sweep drops nothing, since the fake holds no files of another version.
