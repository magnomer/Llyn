# CTranscriptionDraft.cs

## `public sealed record CTranscriptionDraft(`

One transcription of an entry, as the transcription and glyph rows show it.

**Parameters**

- `CTranscriptionDraftId`: the stored transcription, zero for a fresh one.
- `CTranscriptionDraftScheme`: the scheme the transcription follows.
- `CTranscriptionDraftText`: the transcribed text.

## `public string CTranscriptionDraftKey`

The localization key the scheme of the row is labelled under.
