# QProspect.cs
Hash: `6aea75238707bd16`

## `internal sealed class QProspect`

The driver of the dropdown of Entries a typed translation may link to, and the rows it offers.
It is one popup the editor owns rather than one per card.
Only one card is being typed into at a time.
So it is retargeted at the caret it was opened from, and a pick finds its card from there.
A card built at runtime could not declare a popup of its own anyway.

It sits beside the Proffer popup the editor also owns rather than under the field that opens it.
The pronunciation lookup, the recording search and the part-of-speech menu are the same shape.

It has two callers.
A typed translation opens it at a card's link caret, offering matches and then a create row per language.
A selected or typed word opens it at the selection, offering matches only, for a mention or an etymon.
The corpus scribe reaches the second through its own editor, so the popup is still declared once.
The etymology driver reads the mention offer itself.
It hands the offer to the dropdown, since that read is its user action.

## `internal void QProspectPlaceRefine(FrameworkElement anchor, Rect place)`

Shuts any open list and stands the mention picker at the selection's rectangle, where the word is.
An opener places it first, then paints the offer its Conduct read or gate answered.

## `internal void QProspectOpenRefine(CProspect prospect)`

Paints the dropdown from the ready `CProspect`, at the place the opener set.
The blank word, the find, the self filter, the languages and the failure notice are all settled below.
The stored rows come first, then one create row per offered language.
A mention's offer carries no language, because a Mention may only point at an Entry that already exists.
An offer not shown leaves the dropdown shut.
The corpus transcript raises its offer as `CTranscriptMentionOffered`, so its driver hands the editor no Conduct record.

The plus of a create row is shown in the fill, where a data trigger stood.
The create row carries the typed word untouched, because that word is what the tentative entry will be called.

## `internal void QProspectTranslationRefine(PCard card, CProspect prospect)`

Stands the dropdown under the card's focused link caret, then paints the typed translation's offer.
A dropdown opened while the user types selects nothing, so enter still means what was typed.
One opened because a committed word was ambiguous selects its first row, since the user must choose.

## `private void QProspectMissRefine(object sender, MouseButtonEventArgs e)`

Shuts the dropdown when a press lands on no row.

## `private void QProspectPickObserve(object sender, MouseButtonEventArgs e)`

Hands the row the pointer chose to the one gate its field names.
The field is found from the anchor the dropdown stands at.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.
A card's link caret takes the row through the translation insert gate, then the dropdown shuts and the entry empties.
Every pick hands the raw row, a nullable id with the word and language it carries.
A create row has a null id, and the gate alone decides what that means.
The translation gate starts a tentative entry for it first, then links it.
An etymon entry hands the raw id to its gate and empties itself.
A card sentence hands the raw id for the selected span on its own row.
Any other field runs the mention pick command on itself, with the raw nullable id as its parameter.
The driver never tests the id, so no value of it skips a gate or picks another.
The field's owner binds that command where it lives, so the anchor reaches its owner's gate alone.
The etymology text and the corpus transcript are such fields.
The field's text and selection are read at the pick, raw, since the dropdown never takes focus.

## `internal void QProspectKeyRefine(object sender, KeyEventArgs e)`

A Refine, since escape and the arrows only change the open dropdown and ask no gate.
The dropdown never takes focus, so the translation entry's keys drive it, heard first.
The arrows ask `CLanternMove` which row to light, sending the lit row, the row count and the direction.
A shut dropdown holds no rows, so the gate answers null and the arrow stays unhandled.
Escape shuts the dropdown only while it stands open.
A key it does not use falls through to the entry's later handlers.

## `private void QProspectCloseRefine(object? sender, EventArgs e)`

Drops the selection and the rows whenever the dropdown shuts, however it shut.
A click elsewhere shuts it without `QProspectShutRefine`, and a kept selection would let enter take a row no one sees.

## `internal void QProspectKeyObserve(object sender, KeyEventArgs e)`

Enter on a selected row hands the raw row to the translation insert gate.
That is its nullable id, its word and its language, and the gate decides between them.
It runs after the dropdown's Refine and before the entry's own enter, so a chosen row wins.
The dropdown shuts and the entry empties after the gate.

## `internal void QProspectShutRefine()`

Shuts the dropdown, drops its rows and selection, and takes back any offset a mention placing set.

## `private void QProspectApply(FrameworkElement container, object item, string? _)`

Fills one prospect row, where bindings and an event attribute stood, and subscribes its press.
The miss Refine is subscribed before the pick Observe, as on the Proffer dropdown.

The plus of a create row is shown here, where a data trigger stood.
The epithet takes its leading space here, where a string format stood.

## `internal QProspect(FrameworkElement surface, QSentence sentence)`

Hands the dropdown list its rows and fill, and clears its selection as it shuts.
It holds the sentence driver, which finds the card of the sentence row a mention pick links.

The Translation dropdown is one popup for the whole editor rather than one per card.
Only one caret is typed into at a time, so only one list of candidates is ever open.
It hangs off the caret it was opened from, which is why the markup names no placement target.
Its shutting drops the selection, so enter never takes a row the dropdown no longer shows.

## `internal void QProspectIntroduce(CCard card, CSentence sentence, QLink link)`

Holds the card and sentence facets whose gates a pick calls.
It also holds the Translation driver, which finds the card a Translation caret belongs to.
The link driver takes this dropdown in its constructor, so the pair is joined here without a forwarder.
