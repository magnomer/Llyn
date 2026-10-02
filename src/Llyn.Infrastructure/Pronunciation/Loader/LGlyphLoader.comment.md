# LGlyphLoader.cs

## `internal static class LGlyphLoader`

The glyph side of the pack loader: the traditional form a simplified language shows beside its word.
`LLanguageLoader` calls it.

## `private const string LGlyphKey = "glyph";`

The key of the glyph section.

## `public static LGlyph? LGlyphPackRead(JsonElement root, IReadOnlyList<LRespellingRule> spelling)`

The `glyph` section: its `name`, its `language`, and its optional `sources` list and `font` block.
A section missing either the name or the language reads as null, and the glyph row stays off.
The sources list has the shape of a scheme's, so the traditional form is looked up as a transcription is.
A blank font block reads as none, so the chips fall back to the example typography.
