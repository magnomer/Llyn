# LEngineMention.cs

## `public sealed partial class LEngine`

Answers "what may this word mean" for a clicked position of a sentence.
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

## `private static (int LEngineMentionOffset, int LEngineMentionLength) LEngineMentionScan(`

The longest headword of the language that begins at or before the offset and covers it.
A window around the offset is read, and every headword the window contains is fetched in one query.
Each is tried at every start that could cover the click.
Longer headwords come first, so the first hit is the longest.

## `private static bool LEngineRuneMatch(List<Rune> runes, int start, List<Rune> headword)`

Whether the headword sits in the text at `start`, letter case folded.

## `private static string LEngineRuneFormat(List<Rune> runes, int start, int length)`

The code points from `start` as a string, clipped at the end of the text.
