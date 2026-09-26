# PContextMenu.cs

## `public partial class PEditor`

The Situation field's handlers.
The templates that draw the field cannot reach the card the Situations belong to.
An item's data context is the Situation or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.

## `internal void PContextAttach(PCard card)`

Hands the card the way to ask for candidates as its Situation caret is typed into.
The card holds no engine, so it reports what was typed and the editor answers with the dropdown.

## `internal void PContextChipHandle(object sender, RoutedEventArgs e)`

Closes the Situation whose button was pressed.

## `internal void PContextCaretHandle(object sender, KeyEventArgs e)`

The dropdown is offered the key first, so the arrows and escape reach the candidate list.
Enter commits what is standing in the entry, unless a candidate is selected in the dropdown.
The chip keys go through `PCaretKeyApply`.
Every other key is left to the text box.

## `internal void PContextCloseHandle(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field.
So a Situation typed and abandoned is kept rather than silently dropped.
Choosing a candidate empties the entry first, so the click that chose it commits nothing twice.

## `private readonly PContextTemplate _pContextTemplate`

The Situation dictionary, held so its fill can subscribe the dictionary's forwarders.

## `private void PContextApply(FrameworkElement container, object item, string? _)`

Fills one item of a card's Situation field, where bindings and event attributes stood.
An entry gets its hint, its text and its key and leaving handlers.
A chip gets its wording, its close icon and its close handler.

## `private void PContextFieldApply(ItemsControl list)`

Readies a card's Situation list once: the chip or entry selector, the item fill and the click on its surface.
The selector is set only while none is, since a new selector would rebuild every item.

## Inline notes

### `private bool PContextSend(PCard card, string text)`

Turns a typed wording into a request, and answers whether one went out.
A wording the card already carries goes nowhere.
One that names a stored Situation exactly is picked, and any other is added new.

### `private void PContextSend(PCard card, long? id, string written)`

Sends the pick or the addition at the place the caret stands.

### `private void PContextRemove(PCard card, PContext? chip)`

Asks the engine to unlink the chip, when there is one to unlink.

### `private void PContextCommit(PCard card)`

The wording is sent to the engine as written, and the engine decides which Situation it names.
A wording already stored is attached under its own id there, so the workspace does not fill with twins.
The panel holds no matching rule of its own, so every client of the engine gets the same answer.

## `internal void PContextFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.
