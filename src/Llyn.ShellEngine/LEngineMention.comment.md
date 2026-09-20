# LEngineMention.cs

## `public sealed partial class LEngine`

Answers "what may this word mean" for a clicked position of a sentence.
Also reads what a chip line shows for the Mentions a sentence already holds.
Nothing here writes, and linking goes through `LRequestMentionAddition` on an open draft.
So the corpus panel and the card editor link through one path.
A read-only display opens a draft the way an edit does.

## `private const int LEngineMentionReach = 8;`

How many code points before the click a headword may begin in a language without word separators.

## `public LMentionResult LEngineMentionFind(long exampleId, int offset)`

Reads the stored Example and asks the text form below with its language and its Mentions.
An id naming no Example is refused.

## `public LMentionResult LEngineMentionFind(`

The answer for `offset` in `text`, which is what a draft-backed form calls with the text it shows.
A Mention in `mentions` covering the offset wins.
The form navigates on it, or does nothing for a word standing for nothing.
Otherwise the word is resolved.
In a language without separators the longest stored headword of that language covering the offset is the word.
Only when no headword matches, or the language separates words, does the letter run of [LMentionSpan](../Llyn.Core/Lexicon/LMentionSpan.comment.md) decide.
The candidates are every Entry of the language whose headword is that word, case folded, in headword order.

## `public IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions)`

The pieces a sentence falls into around its Mentions, as `LMentionSpan` divides them.

## `public int LEngineUnitRead(string text, int offset)`

The UTF-16 index of a code-point offset in the text.

## `public int LEngineOffsetRead(string text, int unit)`

The code-point offset of a UTF-16 index in the text.

## `public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions)`

Reads the span text, headword and sense of every Mention in one pass, in the order given.
The headwords are read in one batched call, each Meaning once however many Mentions narrow to it.
A Mention standing for nothing gets an empty name, for the line to substitute its own word.
The offsets count code points, so the span is cut at the UTF-16 units the sentence maps them to.

## `private static string LEngineSenseRead(LMeaningVault meanings, long sense)`

The title of one Meaning, its definition when the title is empty, empty when the Meaning is gone.

## `private static (int LEngineMentionOffset, int LEngineMentionLength) LEngineMentionScan(`

The longest headword of the language that begins at or before the offset and covers it.
A window around the offset is read, and every headword the window contains is fetched in one query.
Each is tried at every start that could cover the click.
Longer headwords come first, so the first hit is the longest.

## `private static bool LEngineRuneMatch(List<Rune> runes, int start, List<Rune> headword)`

Whether the headword sits in the text at `start`, letter case folded.

## `private static string LEngineRuneFormat(List<Rune> runes, int start, int length)`

The code points from `start` as a string, clipped at the end of the text.

## `private static List<Rune> LEngineRuneRead(string text)`

The text as a list of code points, so offsets count what the store counts.
