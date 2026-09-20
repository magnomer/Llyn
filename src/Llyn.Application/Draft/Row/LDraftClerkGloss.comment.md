# LDraftClerkGloss.cs

## `public sealed class LDraftClerkGloss`

The Gloss requests, applied to the sentence a card row holds or to the corpus panel's own Example.
Every handler edits the Gloss list of one Example draft and hands the rest of the draft back untouched.

## `public LDraftClerkGloss(LIdentity identity)`

Holds the issuer that names a new Gloss, the only service a Gloss edit needs.

## `public LDraft LGlossAdd(LDraft draft, LRequestGlossAddition request)`

Adds an empty Gloss in the requested language at the requested position, under a freshly minted negative id.

## `public static LDraft LGlossRemove(LDraft draft, LRequestGlossRemoval request)`

Drops the Gloss named, and refuses when no row carries the id.

## `public static LDraft LGlossChange(LDraft draft, LRequestGlossText request)`

Replaces the text of the Gloss named, resolved from the written value.

## `public static LDraft LGlossChange(LDraft draft, LRequestGlossLanguage request)`

Replaces the language of the Gloss named.

## `private static LDraft LGlossChange(`

The one place a Gloss row is found by id and rewritten, refusing when no row carries it.

## `private static LDraft LGlossApply(`

Routes one list change to the Example the request names.
Card 0 and sentence 0 name the draft's own Example, held by the corpus panel.
Its stored rows are read as drafts and written back resolved.
Any other pair names a card row, whose Example draft takes the changed list directly.

## `public static IReadOnlyList<LGloss> LGlossRead(IReadOnlyList<LGlossDraft> drafts)`

The drafts resolved to Glosses, ids kept.
The engine's commit reads a row's Glosses the same way, so it calls here.
