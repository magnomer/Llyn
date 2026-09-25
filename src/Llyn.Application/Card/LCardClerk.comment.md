# LCardClerk.cs

## `public sealed class LCardClerk`

The clerk over Collocations and over what any card references.
It reconciles an entry's stored Collocations to the cards a draft lists.
It also writes the rows, chips, marks and links one card of either kind carries.
It runs over the ports of one rig and composes the tag, register, translation and example clerks.
The engine calls it under its own gate.

Which stored row each card is comes from `LCardDraft.LCardDraftId`, never from its place in the list.
A card naming a stored row updates that row in place, so its id survives.
A card naming nothing is created, and a stored row the draft no longer names is deleted.

## `public LCardClerk(LRig rig, LTagClerk tags, LRegisterClerk registers, LTranslationClerk translations, LExampleClerk examples)`

Reads the collocation, image, video, sentence and situation ports out of `rig`.
The four clerks handed in write the lines a card carries.

## `public void LCollocationSave(long entryId, IReadOnlyList<LCardDraft> cards, string language, List<LRevisionChange> changes, Dictionary<long, long> identity)`

Reconciles the entry's Collocations to the cards the draft lists.
A card naming a stored row of this entry updates that row when its text or its place moved.
A row the card left as it was is neither rewritten nor recorded, so a no-op save leaves no history.
An unreadable value is still sent, because the store is what refuses it.
A card naming nothing creates one.
A stored row the draft stopped naming is deleted.
A card that has gone blank never reaches here.
So clearing a card removes its row, exactly as adding text to a blank one adds one.

Deletions run first, so the creates that follow append onto a set already free of the going rows.
The whole surviving set is renumbered afterwards in one pass.
The unique (owner, position) index rejects a swap done row by row.
That is what `LCollocationOrderSet` on the collocation vault exists for.

A card names a stored row once.
Two cards carrying one id would otherwise both write that row.
So the second is a new card.

## `public long LCollocationInsert(long entryId, LCardDraft card, Dictionary<long, long> identity)`

Writes one new Collocation row from `card` and records its id in the map.
The entry clerk's create and the reconcile above both append through it.

## `public void LCardClerkSync(long ownerId, LCardDraft card, string language, bool collocation, Dictionary<long, long> identity)`

Re-attaches the rows and Situations one card references and rewrites the Tags it carries so they match the draft.
A row the card names by a positive id is kept and moved to its new place.
A row the card names by a negative id gets a row of its own.
The map records which one.
A row the card dropped is detached only.
Those rows are independent data the card references.
So the last reference going does not take the row with it.
Tags are not reconciled that way.
A Tag is its own text and lives on the card.
So the card's whole Tag line is written over.
Translations are written over the same way, because a link is an id the card holds.
A name that is no Entry is dropped rather than written.
A newly created card has nothing attached yet, so the same path attaches its whole set.
A stale positive id anywhere in the card is refused rather than rebound.
The card therefore never points where the user did not.

## `private void LSentenceSync(long ownerId, IReadOnlyList<LSentenceDraft> drafts, string language, bool collocation, Dictionary<long, long> identity)`

Writes the whole row list of one card over what the store holds for it.
The store hands back the id of every row it wrote.
A negative row id is mapped to its new row.
A row states which Example it quotes, and the example clerk resolves that Example.
A row naming no Example is written with none, which is what the store's check allows for a bare frame.

## `private void LSituationSync(long ownerId, IReadOnlyList<LSituationDraft> drafts, bool collocation, Dictionary<long, long> identity)`

The card's Situation chips reconciled against the rows it already references, by the shared field sync.
