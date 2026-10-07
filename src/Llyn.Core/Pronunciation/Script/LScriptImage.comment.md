# LScriptImage.cs
Hash: `553c5fd7d2eb218a`

## `public sealed record LScriptImage(string LScriptImageCharacter, string LScriptImageStyle, int LScriptImagePosition, string LScriptImageCaption, string LScriptImageGloss, byte[] LScriptImageData, string LScriptImageEpoch = "", long LScriptImageId = 0)`

One glyph picture of one character in one style, as fetched and as stored.
The picture belongs to the character, not to the entry, so every entry holding that character shares it.
The bytes are the original the source drew, and the view scales them down itself.

**Parameters**

- `LScriptImageCharacter` — The single Han character the picture draws.
- `LScriptImageStyle` — The style name of the pack row the picture came from.
- `LScriptImagePosition` — The picture's place among the style's pictures, in source order from zero.
  It records arrival order only, so no view orders by it.
- `LScriptImageCaption` — The caption the source printed under the picture, without its chronology, or empty.
  A bronze inscription names its vessel, a stele its name.
  A chronology the pack does not list stays inside the caption, in the source's own language.
- `LScriptImageGloss` — The gloss the source printed beside the style's results, repeated on every picture of the style, or empty.
- `LScriptImageData` — The picture's bytes, a PNG with the glyph in black on a clear ground.
- `LScriptImageEpoch` — The stored code of the chronology cut from the caption, or empty for none.
  The code is stored once and read back, so the age is never matched again and never fetched again.
  The view turns it into the reader's language, so changing that language changes what is printed.
- `LScriptImageId` — The stored row id, or zero for a picture just fetched and not yet stored.
  It is arrival order, so it only breaks the last tie of the sort.

## `public static IReadOnlyList<string> LScriptCharacterScan(IReadOnlyList<LScriptImage> images)`

The distinct characters the pictures draw, in first-seen order.
The grouping follows that order, so the blocks stand as the headword spells them.

## `public static IReadOnlyList<LScriptImage>? LScriptImageScan(IReadOnlyList<LScriptImage> images, string character, string style)`

The pictures of `character` in `style`, or null when there are none.
Null rather than empty lets the grouper skip a missing block in one pattern test.

## `public static IReadOnlyList<LScriptImage> LScriptImageSort(IReadOnlyList<LScriptImage> images, IReadOnlyList<LScriptStyle> styles, IReadOnlyList<string> spelled)`

The pictures in the one declared order every view shows them in.
Characters follow `spelled`, the headword's characters in the order the headword spells them.
A character the headword lacks follows, by code point through `LGlyphOrder`.
So the order of the input never decides where a character's block stands.
Within a character, styles follow the pack's declared list, and an undeclared style follows by name.
Within a style, epochs follow the first place their code holds in the style's epoch table.
The pack lists that table from the oldest age to the latest, so the pictures read as a chronology.
An undeclared epoch code follows the declared ones by code, and a picture without an epoch comes last.
The stored position is web hit order, so it is never a key.
The stored row id is the last key, only to make the result stable.
