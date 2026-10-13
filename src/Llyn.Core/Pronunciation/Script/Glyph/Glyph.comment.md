# Glyph

This folder holds the Han-character glyph section, its row split into cells and blocks, and the character order.

## `LGlyph`

The glyph section a Han-character pack declares.
It names the storing scheme, the language each character is a word of, and the lookup sources.
The form is stored as one transcription row carrying `LGlyphName` as its scheme.
So the draft, the database, the sync and the markup carry it without a store of their own.
The engine holds no list of Han languages of its own.
The transcription rows skip a row of this scheme, and the glyph row shows it instead.
A section declared without sources carries an empty list, and the headword's own characters stand.
Without a font the pack's example typography stands for the chips.

## `LGlyphScan`

The distinct Han characters of a text, in first-seen order, each as its own string.
Runes are walked rather than chars, because ideographs beyond the basic plane take two chars.
Kana, Latin letters, punctuation and repeats are dropped.

## `LGlyphSingleCheck`

Whether the text holds exactly one distinct Han character.
Other runes and repeats are ignored, so a lone rune passes only when it is Han.

## `LGlyphOtherRead`

The filled rows a reading view lists as transcriptions, in draft order.
A row under the glyph section's scheme is left out, since the glyph row shows it.
Without a section, every filled row stays.

## `LGlyphBlockRead`

Splits rows into those under the section's scheme and all others, blank rows kept.
Without a section, every row is another row and the block reads hidden.

## `LGlyphSchemeCheck`

Whether the scheme is this section's own name, so a transcription under it is the glyph row.

## `LGlyphDivide`

The cells of the draft's glyph row, one per rune.
The text is the first filled transcription under this section's scheme, else the headword.
Without a filled glyph transcription, a headword lacking Han characters yields no cells.
A filled glyph transcription supplies cells regardless of the headword's script.
A lone Han character carries this section's language, and any other rune carries a blank one.

## `LGlyphSourceRead`

The section's sources when the scheme is its name, else null so the pack's scheme list alone decides.
The engine asks it when a transcription lookup names a scheme no transcription entry declares.

## `LGlyphHanCheck`

Whether one code point sits in a Han block.
The blocks are the unified ideographs, extension A, the compatibility ideographs, extensions B to G, and the compatibility supplement.

## `LGlyphBlock`

A draft's transcription rows split by its language's glyph section, as an editor lists them.
`LGlyph` builds it, so the scheme match has one owner.
Blank rows stay on both sides, since an editor offers them to type into.

## `LGlyphCell`

One rune of a glyph row, as `LGlyph` divides it.
The language is the one the character's entry lives in.
It is blank for a rune that is not Han, so the cell opens nothing.

## `LGlyphCellLinked`

Whether the cell opens an entry, which holds while it names a language.
A driver asks this one verdict, so no chip decides it from the blank language.

## `LGlyphOrder`

The one declared rule ordering lists of characters for every view.
Characters list by Unicode code point, so storage and fetch order never decide.

## `LGlyphOrderComparer`

Compares two texts rune by rune on the code point value.
UTF-16 ordinal order would put ideographs beyond the basic plane before the compatibility block.
Walking runes keeps a surrogate pair whole and sorts it by its true value.
A text that is a prefix of the other sorts first.
A caller holding rows sorts by this comparer first and by the stored id last.

## `LGlyphOrderSort`

The characters in code point order.
The sort is stable, so equal texts keep the order they arrived in.

## Stamp

Commit: `c2f335c331fe`
- `LGlyph.cs` `15bc35e2790759b1`
- `LGlyphBlock.cs` `3efd800ff721c845`
- `LGlyphCell.cs` `a600c36e569d2001`
- `LGlyphOrder.cs` `9adea187dbce619b`
