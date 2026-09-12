# LPronunciation.cs

## `public sealed record LPronunciation(`

One way the entry's reading sounds, written in IPA.
An entry owns an ordered list of these, and the first is the primary one every summary shows.
`LPronunciationId` is the identity, an opaque and program-generated stable id.
`LPronunciationEntryId` names the owning entry and `LPronunciationPosition` its place in that entry's list.
`LPronunciationVariety` says why this row stands beside the others, such as a region or a register.
A row may carry no IPA yet, because a recording is often fetched before the reading is typed.
It owns its ordered `LPronunciationSyllables`.

**Parameters**

- `LPronunciationId` — Opaque, program-generated stable id.
- `LPronunciationEntryId` — Owning entry id.
- `LPronunciationPosition` — Order within the entry, zero first.
- `LPronunciationVariety` — Optional label telling this pronunciation from the entry's others.
- `LPronunciationIpa` — Optional IPA transcription of the whole pronunciation, always phonetic.
- `LPronunciationSyllables` — Ordered syllables owned by this pronunciation.
