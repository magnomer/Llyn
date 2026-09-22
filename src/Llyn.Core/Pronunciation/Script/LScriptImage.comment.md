# LScriptImage.cs

## `public sealed record LScriptImage(`

One glyph picture of one character in one style, as fetched and as stored.
The picture belongs to the character, not to the entry, so every entry holding that character shares it.
The bytes are the original the source drew, and the view scales them down itself.

**Parameters**

- `LScriptImageCharacter` — The single Han character the picture draws.
- `LScriptImageStyle` — The style name of the pack row the picture came from.
- `LScriptImagePosition` — The picture's place among the style's pictures, in source order from zero.
- `LScriptImageCaption` — The caption the source printed under the picture, without its chronology, or empty.
  A bronze inscription names its vessel, a stele its name.
  A chronology the pack does not list stays inside the caption, in the source's own language.
- `LScriptImageGloss` — The gloss the source printed beside the style's results, repeated on every picture of the style, or empty.
- `LScriptImageData` — The picture's bytes, a PNG with the glyph in black on a clear ground.
- `LScriptImageEpoch` — The stored code of the chronology cut from the caption, or empty for none.
  The code is stored once and read back, so the age is never matched again and never fetched again.
  The view turns it into the reader's language, so changing that language changes what is printed.
