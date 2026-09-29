# PProspect.cs

## `public partial class PEditor`

The dropdown of Entries a typed translation may link to, and the rows it offers.
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

## `internal event Action<TextBox, long>? PProspectPicked;`

A mention picked in a field the editor does not own, raised with the field and the raw Entry id.
The corpus transcript is such a field, and its own driver hears the notice and calls its gate.

## `internal void PProspectPlaceRefine(FrameworkElement anchor, Rect place)`

Shuts any open list and stands the mention picker at the selection's rectangle, where the word is.
An opener places it first, then paints the offer its Conduct read or gate answered.

## `internal void PProspectOpenRefine(CProspect prospect)`

Paints the dropdown from the ready `CProspect`, at the place the opener set.
The blank word, the find, the self filter, the languages and the failure notice are all settled below.
The stored rows come first, then one create row per offered language.
A mention's offer carries no language, because a Mention may only point at an Entry that already exists.
An offer not shown leaves the dropdown shut.
The corpus hears its offer as `CCorpusMentionOffered`, so its driver hands the editor no Conduct record.

The plus of a create row is shown in the fill, where a data trigger stood.
The create row carries the typed word untouched, because that word is what the tentative entry will be called.

## `private void PProspectTranslationRefine(PCard card, CProspect prospect)`

Stands the dropdown under the card's focused link caret, then paints the typed translation's offer.
A dropdown opened while the user types selects nothing, so enter still means what was typed.
One opened because a committed word was ambiguous selects its first row, since the user must choose.

## `private void PProspectMissRefine(object sender, MouseButtonEventArgs e)`

Shuts the dropdown when a press lands on no row.

## `private void PProspectPickObserve(object sender, MouseButtonEventArgs e)`

Hands the row the pointer chose to the one gate its field names.
The field is found from the anchor the dropdown stands at.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.
A card's link caret takes the row through the translation insert gate, then the dropdown shuts and the entry empties.
The gate starts a tentative entry first on a create row, whose id is zero.
The etymology text links the selected span, and an etymon entry adds the Entry and empties itself.
A card sentence links the selected span on its own row.
Any other field is not the editor's, so the pick is raised through `PProspectPicked`.
The field's text and selection are read at the pick, raw, since the dropdown never takes focus.

## `private void PProspectKeyRefine(object sender, KeyEventArgs e)`

A Refine, since escape and the arrows only change the open dropdown and ask no gate.
The dropdown never takes focus, so the translation entry's keys drive it, heard first.
Escape shuts it, and the arrows walk the selection and wrap at either end.
A key it does not use falls through to the entry's later handlers.

## `private void PProspectCloseRefine(object? sender, EventArgs e)`

Drops the selection whenever the dropdown shuts, however it shut.
A click elsewhere shuts it without `PProspectShutRefine`, and a kept selection would let enter take a row no one sees.

## `private void PProspectKeyObserve(object sender, KeyEventArgs e)`

Enter on a selected row hands that row's Entry, word and language to the translation insert gate.
It runs after the dropdown's Refine and before the entry's own enter, so a chosen row wins.
The dropdown shuts and the entry empties after the gate.

## `private void PProspectShutRefine()`

Shuts the dropdown, drops its rows and selection, and takes back any offset a mention placing set.

## `private void PProspectApply(FrameworkElement container, object item, string? _)`

Fills one prospect row, where bindings and an event attribute stood, and subscribes its press.
The miss Refine is subscribed before the pick Observe, as on the Proffer dropdown.

The plus of a create row is shown here, where a data trigger stood.
The epithet takes its leading space here, where a string format stood.
