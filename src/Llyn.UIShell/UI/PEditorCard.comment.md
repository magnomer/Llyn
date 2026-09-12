# PEditorCard.cs

## `public partial class PEditor`

The two card lists of the form, rendered from the draft's cards and read back for their chips.
A Meaning card and a Collocation card are the same card, so one pair of paths serves both lists.
The panel owns this and not the card, because only the panel knows which list a card stands in.

## Inline notes

### `private void PCardShow(`

Brings one list into line with the draft's cards, matching each card by id.
A card the form does not show yet is built.
A card the draft no longer names is dropped.
A card that moved is moved in place.
The rest keep their controls, so the caret stays in a card while its neighbours change.
Each card takes the number the draft carries, not its place in the loop.

### `private void PCardTextShow(PCard card, LCardDraft draft)`

Writes a card's three text fields only where the card does not already show the draft's value.
A field with a request still waiting is skipped, because the draft is about to change to what it holds.

### `private PCard PCardCreate(`

Builds one card from a draft card and gives it the way back to the editor.
A card that cannot report its own edits would be typed into without ever being written.
A card resolves no typed word on its own, so it is attached before it is shown.
Each card takes its own words out of the one translation answer, in the order the card holds them.
An id the answer does not name is passed over, as a chip with no Entry has nothing to say.

### `private void PCardPrepare()`

Asks for one empty card in every list that shows none.
The panel is an editor.
An editor with nothing to type into is not a state the form has.
The card is minted by the engine like any other, so it is named before anything is typed into it.

### `private static IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards, IReadOnlyList<LCardDraft> held)`

The draft's cards with each shown card's chip lists written over them.
Title, Expression and Meaning are not read here, because they reached the engine as requests.
A card the form does not show goes back exactly as it came.
So a field the editor never drew is never blanked by an edit that never saw it.
