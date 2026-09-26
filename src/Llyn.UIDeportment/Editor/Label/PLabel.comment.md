# PLabel.cs

## `public partial class PEditor`

The Tag field's handlers.
The templates that draw the field cannot reach the card the Tags belong to.
An item's data context is the Tag or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.
Reaching the entry itself is shared with the Situation field, which is written the same way.

## `internal void PLabelAttach(PCard card)`

Hands the card a way to say what is being typed into its Tag entry.
The dropdown of stored tags answers that, so the card need not know the dropdown exists.
The Situation field is wired the same way.

## `internal void PLabelChipHandle(object sender, RoutedEventArgs e)`

Closes the Tag whose button was pressed.

## `internal void PLabelCaretHandle(object sender, KeyEventArgs e)`

The dropdown is offered the key first while it stands open, so the arrows and enter reach the list.
Enter commits what is standing in the entry.
The chip keys go through `PCaretKeyApply`.
Every other key is left to the text box.

## `internal void PLabelCloseHandle(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field.
The dropdown is shut first, because the field it was opened for is no longer being typed into.
So a Tag typed and abandoned is kept rather than silently dropped.

## `internal void PLabelFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.

## `private void PLabelCommit(PCard card)`

Closes what is standing in the entry into a request and clears the entry.

## `private bool PLabelSend(PCard card, string text)`

Asks the engine to add a new Tag with the typed text at the caret.
A text the card already carries goes nowhere.

## `private void PLabelSend(PCard card, long id)`

Asks the engine to link the stored Tag the user picked, at the caret.

## `private void PLabelRemove(PCard card, PLabelChip? chip)`

Asks the engine to unlink the chip, when there is one to unlink.

## `private readonly PLabelTemplate _pLabelTemplate`

The Tag dictionary, held so its fill can subscribe the dictionary's forwarders.

## `private void PLabelApply(FrameworkElement container, object item, string? _)`

Fills one item of a card's Tag field, where bindings and event attributes stood.
An entry gets its hint, its text and its key and leaving handlers.
A chip gets its wording, its close icon and its close handler.

## `private void PLabelFieldApply(ItemsControl list)`

Readies a card's Tag list once: the chip or entry selector, the item fill and the click on its surface.
The selector is set only while none is, since a new selector would rebuild every item.
