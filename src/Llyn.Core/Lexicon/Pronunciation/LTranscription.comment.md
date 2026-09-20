# LTranscription.cs

## `public sealed record LTranscription(`

The entry's reading spelled in one named scheme, such as Jyutping or Pinyin.
It is not a pronunciation.
A pronunciation is IPA and says how the word sounds.
A transcription is how that sound is written in a scheme.
An entry owns an ordered list of these, one row per scheme, because a scheme spells one reading one way.
`LTranscriptionId` is the identity, an opaque and program-generated stable id.
The scheme name is stored on the row, so a row stays readable after its language pack changes.

**Parameters**

- `LTranscriptionId` — Opaque, program-generated stable id.
- `LTranscriptionEntryId` — Owning entry id.
- `LTranscriptionPosition` — Order within the entry, zero first.
- `LTranscriptionScheme` — Name of the scheme the text is written in, unique within the entry.
- `LTranscriptionText` — The reading spelled in that scheme, never empty.
