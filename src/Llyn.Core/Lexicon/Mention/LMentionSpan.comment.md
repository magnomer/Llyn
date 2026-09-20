# LMentionSpan.cs

## `public static class LMentionSpan`

Finds the word around one offset of a sentence.
It is the fallback the engine uses when no headword matches the text at that point.
It knows no language of its own.
The one language fact it needs arrives as a flag read off the language pack.

## `public static (int LMentionSpanOffset, int LMentionSpanLength) LMentionSpanResolve(string text, int offset, bool separated)`

The span of the word at code-point `offset` of `text`, as a start and a length.
Both count Unicode scalar values, so a surrogate pair is one character.
An offset on whitespace, on punctuation, or past the end answers `(offset, 0)`.
`separated` says whether the language writes a space between its words.
For a separated language a word is a run of letters, digits, apostrophes and hyphens.
For an unseparated language a word is the longest run of letters with no script change.
So a kanji run stops where hiragana begins, and the particle stands alone.

## `public static IReadOnlyList<LMentionPiece> LMentionSpanDivide(string text, IReadOnlyList<LMention> mentions)`

Cuts `text` at every Mention boundary into pieces that cover it end to end.
Each piece carries its offset, its length and its Mention, or null for a gap.
No piece is empty, so a sentence with no Mention is one piece and an empty sentence is none.
A Mention reaching past the end of the text is trimmed to it rather than refused.
The store never writes such a Mention, and a stale one must not blank the sentence.
Overlapping Mentions are a caller error and throw, with `LMention.LMentionOverlapCheck` as the guard.

## `public static int LMentionOffsetRead(string text, int unit)`

The code-point offset of the character holding UTF-16 unit `unit` of `text`.
A unit inside a surrogate pair answers the offset of that one character.
A unit at or past the end answers the number of characters.

## `public static int LMentionUnitRead(string text, int offset)`

The UTF-16 unit index where code point `offset` of `text` begins.
An offset at or past the end answers the length of the text in units.
WPF measures text in UTF-16 units, and the engine and the store count code points.
These two methods are the only place the conversion lives.

## Inline notes

### `private static bool LMentionJoinCheck(Rune left, Rune right, bool separated)`

Answers whether two neighbouring characters belong to one word.
Both must be word characters, and in an unseparated language both must share one script.

### `private static bool LMentionWordCheck(Rune rune, bool separated)`

A letter is always a word character.
A digit, an apostrophe, a right single quotation mark or a hyphen counts only in a separated language.

### `private static int LMentionScriptRead(Rune rune)`

A small script number for a letter: hiragana, katakana, Han, Hangul, another letter without case, or a cased letter.
The Unicode block ranges name the scripts an unseparated pack ships today.
Every other letter without case falls into one bucket, and every cased letter into another.
