# CTranscriptionDraft.cs
Hash: `b28f91481756e9ef`

## `public sealed record CTranscriptionDraft(long CTranscriptionDraftId, string CTranscriptionDraftScheme, string CTranscriptionDraftText)`

One transcription of an entry, as the transcription and glyph rows show it.

**Parameters**

- `CTranscriptionDraftId`: the stored transcription, zero for a fresh one.
- `CTranscriptionDraftScheme`: the scheme the transcription follows.
- `CTranscriptionDraftText`: the transcribed text.

## `public string CTranscriptionDraftKey`

The localization key the scheme of the row is labelled under.

## `internal static IReadOnlyList<CTranscriptionDraft> CTranscriptionDraftRead(IReadOnlyList<LTranscriptionDraft> transcriptions)`

The transcriptions of a draft, shaped for the transcription rows and the lectern.
