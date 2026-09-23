# LEngineMention.cs

## `public sealed partial class LEngine`

The mention facade of the engine.
Every call takes the gate and hands the work to `LMentionClerk`, which holds the rules.
Nothing here writes, and linking goes through `LRequestMentionAddition` on an open draft.
So the corpus panel and the card editor link through one path.

## `public LMentionResult LEngineMentionFind(long exampleId, int offset)`

The clerk's find over a stored Example, under the gate.

## `public LMentionResult LEngineMentionFind(string text, string language, int offset, IReadOnlyList<LMention> mentions)`

The clerk's find over the text a draft-backed form shows, under the gate.

## `public IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions)`

The pieces a sentence falls into around its Mentions.

## `public LMentionDraft LEngineSpanRead(string text, int start, int length)`

A field's selection as the span a Mention request carries, measured by the engine so the shell never counts.

## `public int LEngineUnitRead(string text, int offset)`

The UTF-16 index of a code-point offset in the text.

## `public int LEngineOffsetRead(string text, int unit)`

The code-point offset of a UTF-16 index in the text.

## `public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions)`

The clerk's labels for every Mention of a chip line, under the gate.
