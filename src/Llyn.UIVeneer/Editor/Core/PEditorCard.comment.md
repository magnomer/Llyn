# PEditorCard.cs

## `public partial class PEditor`

The two card lists of the form, rendered from the draft's cards down to the last chip.
A Meaning card and a Collocation card are the same card, so one pair of paths serves both lists.
The panel owns this and not the card, because only the panel knows which list a card stands in.
Nothing is read back from a card, because every edit reached the engine as a request.

## `private void PEditorFieldHandle(TextBox box)`

A field inside a card, sentence, gloss, picture or film row changed, so the request its binding names is deferred.
Only a field with the keyboard in it reports, since a write from the draft echoes through the same event.

## `private void PCardChangeHandle(PCard card, string field, LStateWritten written)`

The card's own three fields, each deferred as its own request.

## Inline notes

### `private void PCardShow(`

Brings one list into line with the draft's cards, matching each card by id.
A card the form does not show yet is built.
A card the draft no longer names is dropped.
A card that moved is moved in place.
The rest keep their controls, so the caret stays in a card while its neighbours change.
Every card, new or kept, is then shown its text and its lists, since any of them may have changed.
Each card takes the number the draft carries, not its place in the loop.

### `private static void PCardTextShow(PCard card, LCardDraft draft)`

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
The draft's language goes with the rows, since a new row reads its field order from the engine under it.
Each card takes its own words out of the one translation answer, in the order the card holds them.
The chip lines under the rows are redrawn after, because the rows hold no engine to read headwords from.
An id the answer does not name is passed over, as a chip with no Entry has nothing to say.

### `private void PCardPrepare(LEntryDraft draft)`

Asks for one empty card in every list of the draft that holds none.
The panel is an editor.
An editor with nothing to type into is not a state the form has.
The card is minted by the engine like any other, so it is named before anything is typed into it.
The draft is checked rather than the drawn lists, because the ask now runs before the render.


# PEditorCard.xaml

## `ResourceDictionary`

The button that adds a card, apart from the two lists it stands under.
A meaning list and a collocation list end the same way, so the ending is written once.

## Inline notes

### `<Style x:Key="Editor.Card.Add" TargetType="Button">`

A dashed outline says the card is not there yet and the plus says it can be.
Only the wording and the list it adds to differ, and both stay in the markup.
