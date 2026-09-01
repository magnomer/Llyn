# LPronunciation.cs

## `public sealed record LPronunciation(`

The single pronunciation an entry owns: at most one per entry, with no order and no primary/default distinction. `LPronunciationId` is the identity — an opaque, program-generated stable id — and `LPronunciationEntryId` names the owning entry, unique across pronunciations so an entry never carries two. It carries an optional transcription `LPronunciationIpa` and `LPronunciationLevel`, and owns its ordered `LPronunciationSyllables` and `LPronunciationRepresentations`.

**Parameters**

- `LPronunciationId` — Opaque, program-generated stable id.
- `LPronunciationEntryId` — Owning entry id; unique — one pronunciation per entry.
- `LPronunciationLevel` — Optional transcription level (for example phonemic or phonetic).
- `LPronunciationIpa` — Optional IPA transcription of the whole pronunciation.
- `LPronunciationSyllables` — Ordered syllables owned by this pronunciation.
- `LPronunciationRepresentations` — Ordered notational representations owned by this pronunciation.
