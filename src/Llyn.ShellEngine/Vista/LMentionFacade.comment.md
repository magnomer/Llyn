# LMentionFacade.cs
Hash: `d4c352210a3b3aba`

## `internal sealed class LMentionFacade`

The engine's facade for mention operations.
Every call takes the gate and hands the work to `LMentionClerk`, which holds the rules.

## `public LMentionFacade(LEngine engine)`

Stores the engine and its gate.

## `public LMentionResult LEngineMentionFind(long exampleId, int offset)`

The clerk's find over a stored Example, under the gate.

## `public LMentionResult LEngineMentionFind(LEntryDraft shown, long sentence, int offset)`

The clerk's find over one sentence row of the entry the reading view shows, under the gate.

## `public LMentionResult LEngineEtymologyFind(LEntryDraft shown, int offset)`

The clerk's find over the etymology prose of the entry the reading view shows, under the gate.

## `public LMentionDraft LEngineSpanRead(string text, int start, int length)`

A field's selection as the span a Mention request carries, measured by the engine so the shell never counts.

## `public bool LEngineSpanCheck(string text, int start, int length)`

Whether a field's selection spans any code point, so a link command may run.

## `public int LEngineUnitRead(string text, int offset)`

The UTF-16 index of a code-point offset in the text.

## `public int LEngineOffsetRead(string text, int unit)`

The code-point offset of a UTF-16 index in the text.

## `public IReadOnlyList<LMentionLabel> LEngineEtymologyResolve(LTenure held)`

The clerk's mention line for the held draft's etymology, under the gate.
The draft is read before the gate is taken, as the other held reads do.

## `public IReadOnlyList<LMentionLabel> LEngineMentionResolve(LTenure held, long card, long sentence)`

The clerk's mention line for one Example of the held draft, under the gate.
Card 0 and sentence 0 address the draft's own Example.
The draft is read before the gate is taken, as the other held reads do.

## `public IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>> LEngineMentionResolve(LTenure held)`

The clerk's mention lines for every sentence row of the held draft, under the gate.
The draft is read before the gate is taken, as the other held reads do.
