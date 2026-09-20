# LGlyph.cs

## `public sealed record LGlyph(`

The glyph section a language pack written in Han characters declares.
It names the storing scheme, the language each character is a word of, and the lookup sources.
The form is stored as one transcription row carrying `LGlyphName` as its scheme.
So the draft, the database, the sync and the markup carry it without a store of their own.
The engine holds no list of Han languages of its own.

**Parameters**

- `LGlyphName` — The scheme name the glyph row carries, such as `Traditional` or `Kanji`.
  The transcription rows skip a row of this scheme, and the glyph row shows it instead.
- `LGlyphLanguage` — The language each character opens an entry in, such as `Classical Chinese`.
- `LGlyphSources` — The sources the pack declares for looking the traditional form up.
  A section declared without sources carries an empty list, and the headword's own characters stand.
- `LGlyphFont` — The typography the section declares under `font` for its character chips, held as an [LFont](LFont.comment.md).
  It is `null` when the section declares none, and the pack's example typography stands for the chips.

## `public static IReadOnlyList<string> LGlyphScan(string text)`

The distinct Han characters of a text, in first-seen order, each as its own string.
Runes are walked rather than chars, because ideographs beyond the basic plane take two chars.
Kana, Latin letters, punctuation and repeats are dropped.

## `public static bool LGlyphRowCheck(LGlyph? glyph, IReadOnlyList<LTranscriptionDraft> rows)`

Whether a transcription row already stands under the glyph section's own scheme.

## `public static bool LGlyphOtherCheck(LGlyph? glyph, IReadOnlyList<LTranscriptionDraft> rows)`

Whether a row stands under any scheme but the glyph section's, or under any scheme when there is no section.

## `public bool LGlyphSchemeCheck(string scheme)`

Whether `scheme` is this section's own name, so a transcription under it is the glyph row.

## `public IReadOnlyList<LSourceSpec>? LGlyphSourceRead(string scheme)`

The section's sources when `scheme` is its name, else null so the pack's scheme list alone decides.
The engine asks it when a transcription lookup names a scheme no transcription entry declares.

## `private static bool LGlyphHanCheck(int value)`

Whether one code point sits in a Han block.
The blocks are the unified ideographs, extension A, the compatibility ideographs, extensions B to H, and the compatibility supplement.
