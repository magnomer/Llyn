# LFontLoader.cs
Hash: `24fdea5b38e44d4d`

## `internal static class LFontLoader`

The font side of the pack loader.
It reads the typography blocks a pack declares.
One block names a family, a size and a style.
`LLanguageLoader` and `LGlyphLoader` call it.

## `public const string LFontKey = "font";`

The key of the word typography block, in the pack and in a glyph section.

## `public const string LFontExample = "example";`

The key of the sentence typography block.

## `public const string LFontGloss = "gloss";`

The key of the gloss typography block.

## `public static LFont LFontBlank => new(null, 0);`

The font that stands for no typography.
`LLanguageLoader` also uses it for a pack it cannot read.

## `public static LFont LFontPackRead(JsonElement root, string key)`

One reader serves every typography block the pack declares.
The blocks carry the same shape, so a second reader would only repeat this one.
A block the pack omits reads as blank, and the theme's own typography stands.
