# LScriptImage.cs

## `public sealed record LScriptImage(`

One glyph picture of one character in one style, as fetched and as stored.
The picture belongs to the character, not to the entry, so every entry holding that character shares it.
The bytes are the original the source drew, and the view scales them down itself.

**Parameters**

- `LScriptImageCharacter` — The single Han character the picture draws.
- `LScriptImageStyle` — The style name of the pack row the picture came from.
- `LScriptImagePosition` — The picture's place among the style's pictures, in source order from zero.
- `LScriptImageCaption` — The caption the source printed under the picture, or empty.
  A bronze inscription names its vessel and era, a stele its name and dynasty.
- `LScriptImageGloss` — The gloss the source printed beside the style's results, repeated on every picture of the style, or empty.
- `LScriptImageData` — The picture's bytes, a PNG with the glyph in black on a clear ground.
