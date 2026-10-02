# LGlyphBlock.cs
Hash: `3efd800ff721c845`

## `public sealed record LGlyphBlock(`

A draft's transcription rows split by its language's glyph section, as an editor lists them.
[LGlyph](LGlyph.comment.md) builds it, so the scheme match has one owner.
Blank rows stay on both sides, since an editor offers them to type into.

**Parameters**

- `LGlyphBlockShown`: whether the language declares a glyph section.
- `LGlyphBlockSourced`: whether the section names sources to look glyphs up in.
- `LGlyphBlockRows`: the rows under the section's scheme, in draft order.
- `LGlyphBlockOther`: every other row, in draft order.
