# LTranscriptionDraft.cs

## `public sealed record LTranscriptionDraft(`

One transcription of an entry as one value the input form carries.
An entry holds an ordered list of these, one per scheme the form shows.
A row with a scheme but no text is a row still being filled, kept in the draft and dropped on commit.

**Parameters**

- `LTranscriptionDraftScheme` — Name of the scheme, taken from the language pack or typed.
- `LTranscriptionDraftText` — The reading spelled in that scheme, as typed.
- `LTranscriptionDraftId` — Id of the stored transcription row, empty when none stands yet.

## `public bool LTranscriptionDraftEmpty`

True when the row carries no text worth storing.
