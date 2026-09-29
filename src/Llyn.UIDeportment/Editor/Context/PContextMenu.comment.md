# PContextMenu.cs

## `public partial class PEditor`

The Situation field's handlers.
The templates that draw the field cannot reach the card the Situations belong to.
An item's data context is the Situation or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.

## `private void PContextTextObserve(PContextCaret caret, string text)`

Hears each edit of a Situation caret and hands the raw text to the situation gate, unsettled.
The gate adds every Situation a comma completed and answers what the entry keeps.
The card's `PCardContextRefine` then shows that answer and offers candidates for it.

## `private void PContextChipObserve(object sender, RoutedEventArgs e)`

Hears the close button of a Situation chip and erases that chip.

## `private void PContextCaretObserve(object sender, KeyEventArgs e)`

The dropdown is offered the key first, so the arrows and escape reach the candidate list.
Enter commits what is standing in the entry, unless a candidate is selected in the dropdown.
The chip keys go through `PCaretKeyApply`, whose erase is the situation remove gate.
Every other key is left to the text box.

## `private void PContextCloseObserve(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field.
So a Situation typed and abandoned is kept rather than silently dropped.
Choosing a candidate empties the entry first, so the click that chose it commits nothing twice.

## `private readonly PContextTemplate _pContextTemplate`

The Situation dictionary, held for the two item templates its selector hands out.

## `private void PContextApply(FrameworkElement container, object item, string? _)`

Fills one item of a card's Situation field, where bindings and event attributes stood.
An entry gets its hint, its text and its key and leaving observers.
A chip gets its wording, its close icon and its close observer.

## `private void PContextFieldApply(ItemsControl list)`

Readies a card's Situation list once: the chip or entry selector, the item fill and the click on its surface.
The selector is set only while none is, since a new selector would rebuild every item.

## `private void PContextFocusRefine(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.

## Inline notes

### `private void PContextCommitObserve(PCard card)`

Hides the dropdown, hands the entry's text to the situation gate settled, then empties the entry.
The wording goes to the engine as written, and the engine decides which Situation it names.
A wording already stored is attached under its own id there, so the workspace does not fill with twins.
The panel holds no matching rule of its own, so every client of the engine gets the same answer.

### `private void PContextEraseObserve(PCard card, PContext? chip)`

Hands the chip's Situation to the remove gate, when there is a chip to erase.
