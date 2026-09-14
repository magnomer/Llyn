# LEngineGlyph.cs

## `public sealed partial class LEngine`

The glyph side of the engine: the section a Han-script pack declares and the entry one character opens.
The glyph form itself is a transcription row, so its lookup and storage live in `LEngineTranscription.cs`.

## `public LGlyph? LEngineGlyphRead(string language)`

The glyph section the pack of `language` declares, or null when the language shows no glyph row.

## `public LEntry LEngineGlyphResolve(string character, string language)`

The entry `character` stands for in `language`, made when none exists yet.
Only an exact headword in that language counts, so a Mandarin entry of the same character is never taken.
Creation goes through `LEngineTranslationCreate`, so the revision is recorded and the frequency fetch starts as for any new entry.
