# LEngineDraftCard.cs

## `public sealed partial class LEngine`

Card order inside one held draft.
The form no longer counts its own cards, so every position comes from here.
Both the move and the renumber end in one write, so the two cannot drift apart.
The draft file itself is read and written by `LEngineDraftHold.cs`.

## `public IReadOnlyList<LCardDraft> LEngineDraftMove(long id, bool collocation, int from, int target)`

Reorders one card inside the held content and hands the whole list back renumbered.
Positions are rewritten from `1` so they stay contiguous whatever the drag did.
This is the only place card order is computed, so the form no longer keeps its own count.
`collocation` picks which of the two lists is reordered.
Both ends are clamped into range, because a drag can land past the last card.
An empty list is written back untouched.

## `public IReadOnlyList<LCardDraft> LEngineDraftNormalize(long id, bool collocation)`

Renumbers one card list without moving anything, and hands the whole list back.
A removed card leaves a gap in the numbering that nothing else closes.
Asking for a move of nothing said the same thing by accident, and read as a reorder that never happened.

## `public string LEngineCardCreate()`

Mints one card id for a card the form has just added.
The form cannot mint one itself, because identity is the workspace's to give.
An id given at the moment a card appears is what lets a later answer name that card back.

## `private IReadOnlyList<LCardDraft> LEngineCardApply(LDraft draft, bool collocation, List<LCardDraft> cards)`

Writes one card list onto a held draft, numbered from `1` and named throughout.
Positions are rewritten so they stay contiguous whatever the caller did to the list.
This is the only place card order is computed, so the form no longer keeps its own count.
A card still carrying no id is named here too.
The answer is about to be matched by id.
Both the move and the renumber end here, so the two cannot drift apart.
