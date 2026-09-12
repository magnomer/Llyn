# LRequestTranscription.cs

The transcription requests, one per change to the entry's ordered transcription list.
Each names the row by its id, so a change lands on that row wherever the list has moved it.

## `public sealed record LRequestTranscriptionAddition(long LRequestDraftId, string LRequestScheme, int LRequestPosition)`

Adds a new transcription row for `LRequestScheme` at `LRequestPosition`, its text still to be typed.
The engine refuses a scheme the entry already carries, because one scheme spells the reading one way.

## `public sealed record LRequestTranscriptionRemoval(long LRequestDraftId, long LRequestTranscriptionId)`

Removes one transcription row.

## `public sealed record LRequestTranscriptionShift(long LRequestDraftId, long LRequestTranscriptionId, int LRequestPosition)`

Moves one transcription row to `LRequestPosition`.

## `public sealed record LRequestTranscriptionScheme(long LRequestDraftId, long LRequestTranscriptionId, string LRequestText)`

Renames the scheme of one row.
The engine refuses a name another row of the entry already carries.

## `public sealed record LRequestTranscriptionText(long LRequestDraftId, long LRequestTranscriptionId, string LRequestText)`

Replaces the spelled reading of one row.
