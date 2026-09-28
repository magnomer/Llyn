# PLabel.cs

## `public partial class PEditor`

The Tag field's driver, torn apart by role as `docs-work/JobPrinciple.md` section 13 asks.
Each Observe hears one event and ends in one card gate, and the Refine only moves the caret.
The trim, the blank check and the duplicate check live in the Application clerk, not here.
The templates that draw the field cannot reach the card the Tags belong to.
An item's data context is the Tag or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.
Reaching the entry itself is shared with the Situation field, which is written the same way.

## `internal void PLabelAttach(PCard card)`

Hands the card a way to say what is being typed into its Tag entry.
The dropdown of stored tags answers that, so the card need not know the dropdown exists.
The Situation field is wired the same way.

## `private void PLabelChipObserve(object sender, RoutedEventArgs e)`

Hears the close button of a chip and erases that Tag.

## `private void PLabelCaretObserve(object sender, KeyEventArgs e)`

The dropdown is offered the key first while it stands open, so the arrows and enter reach the list.
Enter commits what is standing in the entry.
The chip keys go through `PCaretKeyApply`.
Every other key is left to the text box.

## `private void PLabelCloseObserve(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field.
The dropdown is shut first, because the field it was opened for is no longer being typed into.
So a Tag typed and abandoned is kept rather than silently dropped.

## `private void PLabelFocusRefine(object sender, MouseButtonEventArgs e)`

A Refine, since the click changes only where the caret stands and asks no gate.
Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.

## `private void PLabelCommitObserve(PCard card)`

Hands the raw text standing in the entry to the card gate at the caret, then empties the entry.
It never trims or checks the text, since the clerk owns that rule.

## `private void PLabelEraseObserve(PCard card, PLabelChip? chip)`

Hands the chip to the card gate to unlink, when there is one.

## `private readonly PLabelTemplate _pLabelTemplate`

The Tag dictionary, held for its chip and entry templates.

## `private void PLabelApply(FrameworkElement container, object item, string? _)`

Fills one item of a card's Tag field, where bindings and event attributes stood.
An entry gets its hint, its text and its key and leaving observers.
A chip gets its wording, its close icon and its close observer.

## `private void PLabelFieldApply(ItemsControl list)`

Readies a card's Tag list once: the chip or entry selector, the item fill and the click on its surface.
The selector is set only while none is, since a new selector would rebuild every item.
