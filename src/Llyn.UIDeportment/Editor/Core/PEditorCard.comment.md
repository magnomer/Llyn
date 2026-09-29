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

### `private void PCardRefine(ObservableCollection<PCard> cards, string prefix, IReadOnlyList<CCardDraft> drafts)`

Brings one list into line with the draft's cards, pairing each control with its card by id.
The ids are the draft's, and the draft never holds two cards under one id.
A card the form does not show yet is built empty and filled like a kept one.
Its notices are subscribed as it is built.
So a typed situation and a Gloss pick reach the editor.
A card the draft no longer names is dropped.
A card that moved is moved to the place the ready list holds it at.
The rest keep their controls, so the caret stays in a card while its neighbours change.
Every card, new or kept, is then painted, since any of its values may have changed.

### `private void PCardDraftRefine(PCard card, CCardDraft draft)`

Paints one card from its ready draft card, and the card redraws only a value that changed.
What was waiting is written before the read, so the draft already holds what the field shows.
The sentence frame's order goes with the rows, as the last `CSentenceFrameRead` answered it.
The frame is painted before the cards show, so a new row is built in its language's order.
The links come ready on the draft card, so the paint asks Conduct nothing per card.
The chip lines under the rows are redrawn after, because the rows hold no engine to read headwords from.
A picture or film row is built with the atelier, which the card itself does not hold.
Each card takes the number the draft carries, not its place in the loop.
