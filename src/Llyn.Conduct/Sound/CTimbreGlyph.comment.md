# CTimbreGlyph.cs

## `public sealed record CTimbreGlyph(`

The editor's glyph block for the held draft, ready to paint.
The engine picks the transcription rows under the pack's glyph scheme, so no driver matches schemes.
The other rows arrive through `CTranscription.CTranscriptionRead`.

**Parameters**

- `CTimbreGlyphShown`: whether the glyph row shows, while the pack declares a glyph section.
- `CTimbreGlyphSourced`: whether the section names sources, which shows the lookup button.
- `CTimbreGlyphRows`: the rows under the glyph scheme, blank ones kept.
