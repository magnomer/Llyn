# Mention

Mention holds the links from words of a sentence to the Entries they stand for.
Offsets and lengths count Unicode scalar values, not UTF-16 units.
So a surrogate pair is one character on both sides of the store boundary.
WPF measures in UTF-16 units, so the conversion lives only in `LMentionSpan`.

## `LMention`

One word inside an Example that stands for an Entry.
It holds the span of the sentence text and the Entry, which may be narrowed to one Meaning.
The word may instead be marked as standing for nothing, such as a proper name or a typo.
A Mention lives on the Example, never on the card that quotes it.
So every card citing one sentence reads the same words.
A Mention has no state of its own, since a row exists or it does not.
`LMentionOverlapCheck` is the one rule both the archive and the engine share, and the archive refuses an overlapping list.
`LMentionSort` orders by start, which every store read and every normalize uses.

## `LMentionDraft`

One Mention as a draft carries it, with the same five facts under the draft prefix.
The id is negative for a row the database has not stored.
It is zero for a row the engine has not minted.
`LMentionDraftCreate` keeps the id, so a word already linked stays linked.

## `LMentionLabel`

One Mention of a sentence as the engine hands it to a chip line, its words already read.
The line shows it and reads nothing more.

## `LMentionPiece`

One stretch of a sentence as drawn, either a Mention or the gap between two.
The pieces of one sentence cover its text end to end, and none of them is empty.
The control keeps one text run per piece.

## `LMentionResult`

The engine's answer for a clicked word of a sentence, naming the span the click resolves to.
A stored Mention comes with an empty candidate list, because the word is already decided.
A word with no Mention comes with every Entry of the language whose headword is that word, in headword order.
Zero candidates is a legitimate answer, and the form shows nothing for it.
A click on a space or a mark answers no candidates.

## `LMentionSpan`

Finds the word around one offset of a sentence, as the fallback when no headword matches the text there.
It knows no language of its own.
So the one language fact arrives as a flag read off the language pack.

## `LMentionSpanResolve`

An offset on whitespace, on punctuation, or past the end answers `(offset, 0)`.
For a separated language a word is a run of letters, digits, apostrophes and hyphens.
For an unseparated language a word is the longest run of letters with no script change.
So a kanji run stops where hiragana begins, and the particle stands alone.

## `LMentionSpanRead`

Whitespace at either end is dropped, because a double click selects the space after a word too.
The span carries no id and no entry, since it only measures where a Mention would stand.

## `LMentionSpanDivide`

Cuts the text at every Mention boundary into pieces that cover it end to end.
No piece is empty, so a sentence with no Mention is one piece and an empty sentence is none.
A Mention reaching past the end of the text is trimmed to it rather than refused.
The store never writes such a Mention, and a stale one must not blank the sentence.
Overlapping Mentions are a caller error and throw, with `LMentionOverlapCheck` as the guard.

## `LMentionSpanFind`

The Mention that a span lies wholly inside, or nothing.
A caret with no length counts as inside anywhere on a Mention, its ends included.
So a right click on a linked word finds it without the word being selected first.
Every draft that holds Mentions answers its selection through this one test.

## `LMentionOffsetRead`

A unit inside a surrogate pair answers the offset of that one character.
A unit at or past the end answers the number of characters.
`LMentionUnitRead` answers the text length in units for an offset at or past the end.

## Inline notes

### LMentionSpan.cs: `private static bool LMentionJoinCheck(Rune left, Rune right, bool separated)`

Both neighbours must be word characters, and in an unseparated language both must share one script.

### LMentionSpan.cs: `private static bool LMentionWordCheck(Rune rune, bool separated)`

A letter is always a word character.
A digit, an apostrophe, a right single quotation mark or a hyphen counts only in a separated language.

### LMentionSpan.cs: `private static int LMentionScriptRead(Rune rune)`

The Unicode block ranges name the scripts an unseparated pack ships today.
Every other letter without case falls into one bucket, and every cased letter into another.

## Stamp

Commit: `c2f335c331fe`
- `LMention.cs` `1d50d63feeca5f28`
- `LMentionDraft.cs` `02129a16e4839db0`
- `LMentionLabel.cs` `3b945f646dfa9f9b`
- `LMentionPiece.cs` `666600a76c09993f`
- `LMentionResult.cs` `399eeb22d50b4440`
- `LMentionSpan.cs` `4b41f8d5dad0330e`
