# LEngineCard.cs

## `public sealed partial class LEngine`

The card half of `LEngineEntryUpdate`: reconciling an entry's stored Meanings and Collocations, and the Examples, Situations and Tags they reference, to the cards a draft lists. It sits in its own file because it is the bulk of the update and answers one question — which stored row each card is, and what to do with the rows no card names any more.

## Inline notes

### `private void LEngineCardUpdate(`

Reconciles one card set — the entry's Meanings or its Collocations — to the cards the draft lists. A card naming a stored row of this entry updates that row, a card naming nothing creates one, and a stored row the draft stopped naming is deleted; a card that has gone blank never reaches here, so clearing a card removes its row exactly as adding text to a blank one adds one.

Deletions run first so the creates that follow append onto a set already free of the rows that are going, and the whole surviving set is renumbered afterwards in one pass: the unique (owner, position) index rejects a swap done row by row, which is what LDatabaseOrder exists for.

### `if (row.LSenseParentId is not null)`

A sub-meaning belongs to its parent's group, not to the entry's card list, and no save writes one; leaving it out keeps this over the cards the form actually shows.

### `List<LCardDraft> kept = [];`

The cards the draft still names, in draft order. A card naming a row of another entry — or one already gone — names nothing here, so it is created rather than reaching across.

### `HashSet<string> applied = new(StringComparer.Ordinal);`

A card names a stored row once: two cards carrying one id would otherwise both write that row and the renumber would be handed the same member twice, so the second is a new card.

### `private static string LEngineCardApply(`

Writes a card the draft still names onto the row it names, keeping that row's id, and returns the id. The columns the form has no control for — a Meaning's gloss, definition language and labels — are carried over from the stored row rather than blanked by an edit that never saw them.

### `private static string LEngineCardCreate(`

Writes a card the draft added as a new row and returns its fresh id. The row is appended; the renumber that follows the whole set puts it where the draft holds it.

### `private void LEngineCardSync(string ownerId, LCardDraft card, string language, bool collocation)`

Re-attaches the Examples, Situations and Tags one card references so they match the draft. A row whose text the card still lists is kept and moved to its new place, a text the card gained gets a row of its own, and a row the card dropped is detached only — the rows are independent data the card references, so the last reference going does not take the row with it. A newly created card has nothing attached yet, so the same path attaches its whole set.

### `private static void LEngineFieldSync<TRow>(`

One card field reconciled against the rows it already references. Example, Situation and Tag differ only in which store creates a row and which pair of methods attaches and detaches it, so they are handed in and the reconciliation itself is written once.

A value is matched to a referenced row by its text, which is what the shell can say about it: the form holds typed text and no picker exists to name a row, so keeping the row a value already had is the most an update can honour. Each row matches at most one value, so a card listing the same text twice keeps one row and creates the second. Blank values are dropped on the same terms the save drops them.

### `for (int position = 0; position < targets.Count; position++)`

Attaching a row the card already references moves it: the association goes in at the end and the set is renumbered around the requested position, so draft order becomes stored order.
