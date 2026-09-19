# LEngineCard.cs

## `public sealed partial class LEngine`

The card half of `LEngineEntryUpdate`.
It reconciles an entry's stored Meanings and Collocations to the cards a draft lists.
It does the same for the Examples and Situations they reference and the Tags they carry.
It sits in its own file because it is the bulk of the update and answers one question.
That question is which stored row each card is.
It also asks what to do with the rows no card names any more.

## Inline notes

### `private void LEngineCardUpdate(`

Reconciles one card set — the entry's Meanings or its Collocations — to the cards the draft lists.
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

A Meaning list is a tree and is reconciled in `LEngineMeaningCard.cs`.
What stays here is the flat half, which is what a Collocation list is.

### `LEngineCardValidate(cards, collocation);`

A Collocation carrying a card inside it is refused before anything is written.
Only a Meaning nests, because only a sense names a parent in the store.

### `List<LCardDraft> kept = [];`

The cards the draft still names, in draft order.
A card naming a row of another entry names nothing here, and so does one already gone.
So it is created rather than reaching across.

### `HashSet<string> applied = new(StringComparer.Ordinal);`

A card names a stored row once.
Two cards carrying one id would otherwise both write that row.
The renumber would then be handed the same member twice.
So the second is a new card.

### `bool reuse = named.Contains(card.LCardDraftId) && applied.Add(card.LCardDraftId);`

A card the draft still names is written onto the row it names and keeps that row's id.
Any other card is appended as a new row.
The renumber that follows the whole set puts it where the draft holds it.

### `private void LEngineCardSync(`

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
Nothing is created or detached for one, and no target row is touched.
A name that is no Entry is dropped rather than written.
An import points a card at an Entry the same file declares, and that Entry may not exist yet.
A newly created card has nothing attached yet, so the same path attaches its whole set.
A stale positive id anywhere in the card is refused rather than rebound.
The card therefore never points where the user did not.

### `private void LEngineSentenceSync(`

Writes the whole row list of one card over what the store holds for it.
The store hands back the id of every row it wrote.
A negative row id is mapped to its new row.
A row states which Example it quotes, and two cards quoting one sentence name one Example row.
A row naming no Example is written with none, which is what the store's check allows for a bare frame.

### `private void LEngineTagSave(`

Writes the whole Tag line of one card over what the store holds for it.
The store resolves each Tag to its row, by id when it has one and by wording otherwise.
The wording is the Tag's identity in the store, so a written Tag can only ever name one row.
A negative Tag id is mapped to the row the store answered with.

### `private static void LEngineFieldSync<TRow>(`

One card field reconciled against the rows it already references.
Situation, Register, Image and Video differ only in two things.
Those are which resolver turns a value into a row id, and which pair of methods attaches and detaches it.
So they are handed in and the reconciliation itself is written once.

A value names its row by id and nothing else.
A positive id keeps that row, and a negative id creates one.
A card naming one id twice keeps one association.
Blank values are dropped on the same terms the save drops them.

### `for (int position = 0; position < targets.Count; position++)`

Attaching a row the card already references moves it.
The association goes in at the end and the set is renumbered around the requested position.
So draft order becomes stored order.
