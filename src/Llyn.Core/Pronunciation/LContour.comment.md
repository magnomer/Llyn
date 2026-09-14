# LContour.cs

## `public sealed record LContour(`

One syllable of an IPA reading with the pitch levels its tone marks spell.
It is derived from the reading text alone and never stored.
The engine holds no language facts, so the parse knows only the IPA tone notations themselves.

**Parameters**

- `LContourText` — The syllable as written, its segments followed by its tone marks.
  A tone mark run keeps every mark, so a sandhi form such as `²¹⁴⁻²¹` stays readable.
- `LContourLevels` — The Chao levels the marks spell, from `LContourFloor` to `LContourCeiling`, in speaking order.
  A level tone carries one entry and a contour tone carries one entry per turning point.
  The list is empty for a syllable without a tone or with a mark outside the five levels.

## `public const int LContourFloor = 1;`

The lowest Chao level.

## `public const int LContourCeiling = 5;`

The highest Chao level.

## `public static IReadOnlyList<LContour> LContourParse(string ipa)`

Splits a reading into syllables and reads the tone of each.
Three notations count as tone marks.
Superscript digits are what Wiktionary writes for Mandarin and Cantonese.
Chao tone letters `˩˨˧˦˥` are what it writes for Thai and Vietnamese.
Plain digits serve a reading typed by hand.
A syllable ends where its tone marks end, or at a space, a dot or a foot bar.
Slashes, brackets and parentheses are fences and are dropped from the text.
A reading with no marks at all still yields its syllables, each without levels.

## `public static bool LContourToneCheck(IReadOnlyList<LContour> syllables)`

Whether any syllable carries a tone worth drawing.

## `private static IReadOnlyList<int> LContourLevelRead(string marks)`

A joiner, `⁻` or `-`, separates a citation tone from its sandhi surface form.
Only the surface form after the last joiner is read, because that is what is spoken.
A mark outside the five levels empties the list rather than guessing.
