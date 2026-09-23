# LMentionClerk.cs

## `public sealed class LMentionClerk`

Answers "what may this word mean" for a clicked position of a sentence.
Also reads what a chip line shows for the Mentions a sentence already holds.
Nothing here writes, and linking goes through `LRequestMentionAddition` on an open draft.
It runs over the entry, example, meaning and translation ports of one rig and the language cache beside it.
The engine calls it under its own gate.

## `private const int LMentionClerkReach = 8;`

How many code points before the click a headword may begin in a language without word separators.

## `public LMentionClerk(LRig rig, LLanguageCache languages)`

Reads the four ports out of `rig` and keeps the cache that says whether a language separates words.

## `public LMentionResult LMentionClerkFind(long exampleId, int offset)`

Reads the stored Example and asks the text form below with its language and its Mentions.
An id naming no Example is refused.

## `public LMentionResult LMentionClerkFind(string text, string language, int offset, IReadOnlyList<LMention> mentions)`

The answer for `offset` in `text`, which is what a draft-backed form calls with the text it shows.
A Mention in `mentions` covering the offset wins.
The form navigates on it, or does nothing for a word standing for nothing.
Otherwise the word is resolved.
In a language without separators the longest stored headword of that language covering the offset is the word.
Only when no headword matches, or the language separates words, does the letter run of `LMentionSpan` decide.
The candidates are every Entry of the language whose headword is that word, case folded, in headword order.

## `public static IReadOnlyList<LMentionPiece> LMentionClerkDivide(string text, IReadOnlyList<LMention> mentions)`

The pieces a sentence falls into around its Mentions, as `LMentionSpan` divides them.

## `public static int LMentionUnitRead(string text, int offset)`

The UTF-16 index of a code-point offset in the text.

## `public static int LMentionOffsetRead(string text, int unit)`

The code-point offset of a UTF-16 index in the text.

## `public static LMentionDraft LMentionSpanRead(string text, int start, int length)`

A selection of the text in UTF-16 units, read as a span in code points without its outer whitespace.

## `public IReadOnlyList<LMentionLabel> LMentionClerkResolve(string text, IReadOnlyList<LMentionDraft> mentions)`

Reads the span text, headword and sense of every Mention in one pass, in the order given.
The headwords are read in one batched call, each Meaning once however many Mentions narrow to it.
A Mention standing for nothing gets an empty name, for the line to substitute its own word.
The offsets count code points, so the span is cut at the UTF-16 units the sentence maps them to.

## `private string LMentionSenseRead(long sense)`

The title of one Meaning, its definition when the title is empty, empty when the Meaning is gone.

## `private static (int LMentionClerkOffset, int LMentionClerkLength) LMentionClerkScan(LEntryVault entries, List<Rune> runes, string language, int offset)`

The longest headword of the language that begins at or before the offset and covers it.
A window around the offset is read, and every headword the window contains is fetched in one query.
Each is tried at every start that could cover the click.
Longer headwords come first, so the first hit is the longest.

## `private static bool LMentionRuneMatch(List<Rune> runes, int start, List<Rune> headword)`

Whether the headword sits in the text at `start`, letter case folded.

## `private static string LMentionRuneFormat(List<Rune> runes, int start, int length)`

The code points from `start` as a string, clipped at the end of the text.

## `public static List<Rune> LMentionRuneRead(string text)`

The text as a list of code points, so offsets count what the store counts.
The example clerk and the markup link count a sentence through it too.
