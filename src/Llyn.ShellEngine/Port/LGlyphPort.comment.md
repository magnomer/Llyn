# LGlyphPort.cs
Hash: `eadbd2da3f745279`

## `public interface LGlyphPort`

The slice of the engine a deportment sees when it shows an entry's glyph row and transcriptions.
`LLanguageFacade` implements it, since the glyph section belongs to the language.

## `LGlyph? LEngineGlyphRead(LEntryDraft draft);`

The glyph section of the draft's language, or none when the language declares no glyph section.

## `IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft);`

The cells of the draft's glyph row, empty when its language declares no glyph section.

## `IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft);`

The filled transcription rows a reading view lists, without the glyph row.
