# LDraftClerk.cs

## `public sealed class LDraftClerk`

The interactor that applies one `LRequest` to one held draft over the ports of a rig.
A form sends one edit at a time, and the clerk answers with a new draft, never a mutated one.
Each request kind is applied by a pure function, one per kind, returning a new draft or new content.
Cards and the rows inside them are addressed by id wherever they nest, never by place.
So a form and the clerk cannot disagree about which item is meant.
The clerk holds no gate, no observer and no file.
The engine keeps those and calls the clerk under its own gate.
The entry-level fields and the card body are applied here.
The lists inside a card are applied by the concern classes beside this one, one per former engine part.
A clerk is built over one rig and is rebuilt when the engine takes a new rig.
It reads no port after construction that the rig did not hand it.

## `public LDraftClerk(LRig rig, LIdentity identity, LLanguageCache languages)`

Seats the concern classes over the ports of `rig` and the two shared services.
`identity` mints the negative id every new row carries.
`languages` answers the pack of a language, cached, for respelling and anatomy.
Neither is a port, so the engine hands them in beside the rig.

## `public LDraft LDraftClerkApply(LDraft draft, LRequest request)`

The switch over the kinds that reach past the entry content.
The example and source panels edit their own field of the draft, and the credits edit its author list.
The authors panel renames the one Author its draft holds.
The body records lay every field of a panel over the held one, so the caller decides what changed.
A situation field request may mean the panel's situation or a chip, so it is routed by id.
A situation body on a draft holding a Situation lands on that Situation, whatever id it carries.
So the panel need not read the draft back to learn the id before every keystroke.
A media request on a draft holding a Situation lands on that Situation's lists before the switch runs.
The card id such a request carries is ignored there, because a Situation draft holds no card.
Everything else is a change to the entry content and falls through to the content switch.
A headword or language change drops no recording here.
The engine does that after the apply, since only it can read the stored entry the draft edits.

## `public LEntryDraft LDraftClerkApply(LEntryDraft content, LRequest request)`

One switch over the entry and card kinds, ending in the reading, reflex and list switches in turn.
A language change derives every respelling again and recuts every reflex row under the new pack.
A kind no switch knows is a programming error, not a refusal, since no form can send one.
A null text or value is read as empty.
So a request can never leave a null where the draft holds text.

## `private LEntryDraft LCardInsert(LEntryDraft content, LRequestCardAddition request)`

Mints the id of the new card here rather than leaving it to the normalize.
The answer must name the card, and a card named at birth cannot be confused with one named later.

## `private LEntryDraft LDraftListApply(LEntryDraft content, LRequest request)`

The switch over every list kind inside a card.
A kind it does not know is a programming error, not a refusal, since no form can send one.

## `private LEntryDraft LCardResolve(LEntryDraft content, Func<long, LRequest> retarget)`

Applies a request that named card zero to the first Meaning card, made when the draft has none.
A browsing panel planting a Tag, Register, Situation, Example or Source into a fresh entry asks this way.
The panel need not read the draft back to learn which card the reset made.

## `private LEntryDraft LSentenceResolve(LEntryDraft content, long cardId, Func<long, LRequest> retarget)`

Applies a request that named sentence zero to the card's first sentence row, made when the card has none.
A card the draft does not hold is refused, as any card request is.
