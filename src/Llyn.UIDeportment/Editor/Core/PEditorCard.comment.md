# PEditorCard.cs

## `public partial class PEditor`

The two card lists of the form, rendered from the draft's cards down to the last chip.
A Meaning card and a Collocation card are the same card, so one pair of paths serves both lists.
The panel owns this and not the card, because only the panel knows which list a card stands in.
Nothing is read back from a card, because every edit reached the engine as a request.

## `private void PEditorFieldObserve(TextBox box)`

A field inside a card, sentence, gloss, picture or film row changed, so its raw text goes to one gate.
Only a field with the keyboard in it reports, since a write from the draft echoes through the same event.
The badge number is written straight back into the card, where a two-way binding stood.

## `private void PCardFieldObserve(PCard card, string field, string text)`

The card's own three fields, each handed to its own gate.

## Inline notes

### `private void PCardShow(`

Brings one list into line with the draft's cards, matching each card by id.
A card the form does not show yet is built.
A card the draft no longer names is dropped.
A card that moved is moved in place.
The rest keep their controls, so the caret stays in a card while its neighbours change.
Every card, new or kept, is then shown its text and its lists, since any of them may have changed.
Each card takes the number the draft carries, not its place in the loop.

### `private static void PCardTextShow(PCard card, CCardDraft draft)`

Hands a card the draft's three text values, and the card redraws only a field whose value changed.
What was waiting is written before the read, so the draft already holds what the field shows.

### `private PCard PCardCreate(`

Builds one card from a draft card and gives it the way back to the editor.
A card that cannot report its own edits would be typed into without ever being written.
Every list a card carries is attached here, so each row change and each chip commit has somewhere to go.
The card is built empty and filled by the list render.
So a new card and a kept one take one path.

### `private void PCardListShow(`

Shows one card every list the draft card holds, each list diffed by id.
The sentence frame's order goes with the rows, as the last `CSentenceFrameRead` answered it.
The frame is painted before the cards show, so a new row is built in its language's order.
Each card reads its own links, ready and in the order the card holds them.
The chip lines under the rows are redrawn after, because the rows hold no engine to read headwords from.
