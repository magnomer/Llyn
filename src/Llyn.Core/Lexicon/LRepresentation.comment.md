# LRepresentation.cs

## `public sealed record LRepresentation(`

One notational representation of a pronunciation, ordered within it. Identity is `(pronunciation_id, position)`: the representation is subordinate to its `LRepresentationPronunciationId` parent, and reordering changes `LRepresentationPosition` only. It records a value in a named transcription `LRepresentationSystem` playing a given `LRepresentationRole`.

**Parameters**

- `LRepresentationPronunciationId` — Parent pronunciation id.
- `LRepresentationPosition` — Order within the parent pronunciation.
- `LRepresentationSystem` — The transcription system the text is written in.
- `LRepresentationRole` — The role this representation plays for the pronunciation.
- `LRepresentationText` — The representation text.
- `LRepresentationLocalTone` — Optional local tone notation; NULL when absent.
