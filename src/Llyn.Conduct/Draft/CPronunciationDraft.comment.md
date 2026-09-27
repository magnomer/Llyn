# CPronunciationDraft.cs

## `public sealed record CPronunciationDraft(`

One pronunciation of an entry, as the accent rows and the recorder read it.

**Parameters**

- `CPronunciationDraftId`: the stored pronunciation, zero for a fresh one.
- `CPronunciationDraftIpa`: the phonetic transcription.
- `CPronunciationDraftRespelling`: the respelling, empty when the language has none.
- `CPronunciationDraftVariety`: the variety, empty for the main pronunciation.
- `CPronunciationDraftAudio`: the recording's address, empty when none is held.
