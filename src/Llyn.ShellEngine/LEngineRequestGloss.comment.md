# LEngineRequestGloss.cs

## `public sealed partial class LEngine`

The Gloss requests, applied to the sentence a card row holds or to the corpus panel's own Example.
Every handler edits the Gloss list of one Example draft and hands the rest of the draft back untouched.

## `private LDraft LEngineGlossAdd(LDraft draft, LRequestGlossAddition request)`

Adds an empty Gloss in the requested language at the requested position, under a freshly minted negative id.

## `private static LDraft LEngineGlossRemove(LDraft draft, LRequestGlossRemoval request)`

Drops the Gloss named, and refuses when no row carries the id.

## `private static LDraft LEngineGlossChange(LDraft draft, LRequestGlossText request)`

Replaces the text of the Gloss named, resolved from the written value.

## `private static LDraft LEngineGlossChange(LDraft draft, LRequestGlossLanguage request)`

Replaces the language of the Gloss named.

## `private static LDraft LEngineGlossChange(`

The one place a Gloss row is found by id and rewritten, refusing when no row carries it.

## `private static LDraft LEngineGlossApply(`

Routes one list change to the Example the request names.
Card 0 and sentence 0 name the draft's own Example, held by the corpus panel, whose stored rows are read as drafts and written back resolved.
Any other pair names a card row, whose Example draft takes the changed list directly.

## `private static void LEngineGlossRecord(`

Records the stored id each new draft row received, matched by position after a save.
The identity map lets a later request that still names the negative id reach the stored row.

## `private static IReadOnlyList<LGloss> LEngineGlossRead(IReadOnlyList<LGlossDraft> drafts)`

The drafts resolved to Glosses, ids kept.
