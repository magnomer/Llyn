# TVaultFakePronunciation.cs

## `internal sealed class TVaultFakePronunciation : LPronunciationVault`

An in-memory pronunciation store keyed from one, so a clerk save can run without SQLite.
Rows are kept by id and read back by entry, and the order set is accepted and ignored.
A recording save is refused, since no clerk test writes audio through it.
