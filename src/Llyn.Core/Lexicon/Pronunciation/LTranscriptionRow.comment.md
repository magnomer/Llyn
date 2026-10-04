# LTranscriptionRow.cs
Hash: `c4e44a0a8772aae4`

## `public sealed record LTranscriptionRow(LTranscriptionDraft LTranscriptionRowDraft, IReadOnlyList<LSchemeRow> LTranscriptionRowSchemes)`

One transcription row outside the glyph scheme, with the schemes its dropdown offers.

**Parameters**

- `LTranscriptionRowDraft` — The row as the draft holds it.
- `LTranscriptionRowSchemes` — Every scheme of the pack, each marked when another row holds it.
