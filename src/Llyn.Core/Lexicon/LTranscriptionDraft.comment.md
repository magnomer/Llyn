# LTranscriptionDraft.cs

## `public sealed record LTranscriptionDraft(`

One transcription of an entry as one value the input form carries.
An entry holds an ordered list of these, one per scheme the form shows.
A row with a scheme but no text is a row still being filled.
It is kept in the draft, and stored empty on commit when the user asked for it.
A seeded row is the one the form offers before anything was asked for, and it is dropped while empty.

**Parameters**

- `LTranscriptionDraftScheme` — Name of the scheme, taken from the language pack or typed.
- `LTranscriptionDraftText` — The reading spelled in that scheme, as typed.
- `LTranscriptionDraftId` — Id of the stored transcription row, empty when none stands yet.
- `LTranscriptionDraftSeeded` — True when the form offered the row unasked, so it counts only once filled.

## `public bool LTranscriptionDraftEmpty`

True when the row carries no text.
