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
A card naming a stored row of this entry updates that row.
A card naming nothing creates one.
A stored row the draft stopped naming is deleted.
A card that has gone blank never reaches here.
So clearing a card removes its row, exactly as adding text to a blank one adds one.

Deletions run first, so the creates that follow append onto a set already free of the going rows.
The whole surviving set is renumbered afterwards in one pass.
The unique (owner, position) index rejects a swap done row by row.
That is what LDatabaseOrder exists for.

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

### `private void LEngineCardSync(string ownerId, LCardDraft card, string language, bool collocation)`

Re-attaches the rows and Situations one card references and rewrites the Tags it carries so they match the draft.
A row whose text the card still lists is kept and moved to its new place.
A text the card gained gets a row of its own.
A row the card dropped is detached only.
Those rows are independent data the card references.
So the last reference going does not take the row with it.
Tags are not reconciled that way.
A Tag is its own text and lives on the card.
So the card's whole Tag line is written over.
Translations are written over the same way, because a link is an id the card holds.
Nothing is created or detached for one, and no target row is touched.
A newly created card has nothing attached yet, so the same path attaches its whole set.

### `private void LEngineSentenceSync(`

Writes the whole row list of one card over what the store holds for it.
Each row carries the id of the stored row it edits, so an untouched save rewrites the same ids.
A row states which Example it quotes, and two cards quoting one sentence name one Example row.
A row naming no Example is written with none, which is what the store's check allows for a bare frame.

### `private static void LEngineFieldSync<TRow>(`

One card field reconciled against the rows it already references.
Example, Situation and Image differ only in two things.
Those are which store creates a row, and which pair of methods attaches and detaches it.
So they are handed in and the reconciliation itself is written once.

A value is matched to a referenced row by its text, which is what the shell can say about it.
The form holds typed text and no picker exists to name a row.
So keeping the row a value already had is the most an update can honour.
Each row matches at most one value.
So a card listing the same text twice keeps one row and creates the second.
Blank values are dropped on the same terms the save drops them.

### `for (int position = 0; position < targets.Count; position++)`

Attaching a row the card already references moves it.
The association goes in at the end and the set is renumbered around the requested position.
So draft order becomes stored order.
