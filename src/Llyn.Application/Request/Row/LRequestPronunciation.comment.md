# LRequestPronunciation.cs

The field requests key `LRequestKey` by type and row id, so a deferred edit replaces only its own row.
The pronunciation requests, one per change to the entry's ordered pronunciation list.
Each names the row by its id, so a change lands on that row wherever the list has moved it.
`LRequestIpa` and `LRequestAudio` remain for a form that edits the primary row alone.

## `public sealed record LRequestPronunciationAddition(long LRequestDraftId, string LRequestText, int LRequestPosition)`

Adds a new pronunciation row at `LRequestPosition` with `LRequestText` as its reading, empty for a row still to be filled.

## `public sealed record LRequestPronunciationRemoval(long LRequestDraftId, long LRequestPronunciationId)`

Removes one pronunciation row, its recording with it.

## `public sealed record LRequestPronunciationShift(long LRequestDraftId, long LRequestPronunciationId, int LRequestPosition)`

Moves one pronunciation row to `LRequestPosition`.
The row moved to the front becomes the primary one.

## `public sealed record LRequestPronunciationIpa(long LRequestDraftId, long LRequestPronunciationId, string LRequestText)`

Replaces the typed reading of one row, leaving its recording and its stored detail alone.
The engine derives the row's respelling from the new reading again, so a hand-written respelling is replaced.

## `public sealed record LRequestPronunciationRespelling(`

Replaces the respelling of one row alone, leaving the original reading as it stands.
The form sends it in place of `LRequestPronunciationIpa` while the respelling switch is on.
The value stands until the reading or the variety changes and the engine derives the respelling again.

## `public sealed record LRequestPronunciationVariety(long LRequestDraftId, long LRequestPronunciationId, string LRequestText)`

Replaces the label that tells one row from the others, such as a region.
The respelling is derived again, because the pack's respelling groups are scoped by variety.

## `public sealed record LRequestPronunciationAudio(`

Replaces the recording of one row and the source it came from.
An empty file clears it.
