# PEditorCard.cs

## `public partial class PEditor`

The two card lists of the form, rendered from the draft's cards down to the last chip.
A Meaning card and a Collocation card are the same card, so one pair of paths serves both lists.
The panel owns this and not the card, because only the panel knows which list a card stands in.
Nothing is read back from a card, because every edit reached the engine as a request.

## Inline notes

### `private void PCardShow(`

Brings one list into line with the draft's cards, matching each card by id.
A card the form does not show yet is built.
A card the draft no longer names is dropped.
A card that moved is moved in place.
The rest keep their controls, so the caret stays in a card while its neighbours change.
Every card, new or kept, is then shown its text and its lists, since any of them may have changed.
Each card takes the number the draft carries, not its place in the loop.

### `private void PCardTextShow(PCard card, LCardDraft draft)`

Writes a card's three text fields only where the card does not already show the draft's value.
A field with a request still waiting is skipped, because the draft is about to change to what it holds.

### `private PCard PCardCreate(`

Builds one card from a draft card and gives it the way back to the editor.
A card that cannot report its own edits would be typed into without ever being written.
Every list a card carries is attached here, so each row change and each chip commit has somewhere to go.
The card is built empty and filled by the list render.
So a new card and a kept one take one path.

### `private void PCardListShow(`

Shows one card every list the draft card holds, each list diffed by id.
A field with a request still waiting is handed the check so the row is not overwritten under the caret.
Each card takes its own words out of the one translation answer, in the order the card holds them.
An id the answer does not name is passed over, as a chip with no Entry has nothing to say.

### `private void PCardPrepare()`

Asks for one empty card in every list that shows none.
The panel is an editor.
An editor with nothing to type into is not a state the form has.
The card is minted by the engine like any other, so it is named before anything is typed into it.

