# QLabel.cs

## `internal sealed class QLabel`

The Tag field's driver, torn apart by role as `docs-work/JobPrinciple.md` section 13 asks.
Each Observe hears one event and ends in one card gate, and each Refine only changes the look.
The entry's keys are heard by several handlers in order, and the first to handle a key ends its route.
The trim, the blank check and the duplicate check live in the Application clerk, not here.
The templates that draw the field cannot reach the card the Tags belong to.
An item's data context is the Tag or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.
Reaching the entry itself is shared with the Situation field, which is written the same way.

## `internal QLabel(`

Holds the editor scope, since this driver has no control of its own to wire.
It holds the two card lists the editor keeps, which its finds walk.

## `internal void QLabelIntroduce(CEditor editor, QSlate slate)`

Holds the Conduct editor whose card gates the handlers call.
Holds the slate driver too, since the slate finds its card through this one and cannot be built first.

## `internal void QLabelTextObserve(PLabelCaret caret, string text)`

Hears each edit of a Tag caret and hands the raw text to the tag gate, unsettled.
`QSlateRefine` then paints the answer: the text the entry keeps, and the dropdown of stored Tags.

## `internal void QLabelChipObserve(object sender, RoutedEventArgs e)`

Hears the close button of a chip and hands that Tag to the erase gate.

## `internal void QLabelCommitObserve(object sender, KeyEventArgs e)`

Enter hands the raw text standing in the entry to the card gate at the caret, settled.
It runs after the dropdown's handlers, so Enter on a chosen stored tag never reaches it.
The dropdown shuts and the entry empties after the gate.
It never trims or checks the text, since the clerk owns that rule.

## `internal void QLabelEraseObserve(object sender, KeyEventArgs e)`

Backspace at the entry's start or Delete at its end hands the chip beside the caret to the erase gate.
`QCaretEdgeApply` decides the edge, and the key is taken at the edge even with no chip there.

## `internal void QLabelCaretRefine(object sender, KeyEventArgs e)`

A Refine, since the arrows only walk the empty entry past a chip and ask no gate.
`QCaretStepApply` decides the step, and the caret is put back into the moved entry.

## `internal void QLabelBlurRefine(object sender, RoutedEventArgs e)`

Shuts the dropdown when focus leaves the entry, since the field it was opened for is no longer typed into.
It is subscribed before the close observer, so the dropdown shuts first.

## `internal void QLabelCloseObserve(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field, then empties it.
So a Tag typed and abandoned is kept rather than silently dropped.

## `internal void QLabelFocusRefine(object sender, MouseButtonEventArgs e)`

A Refine, since the click changes only where the caret stands and asks no gate.
Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.

The Tag dictionary, held for its chip and entry templates.

## `internal void QLabelApply(FrameworkElement container, object item, string? _)`

Fills one item of a card's Tag field, where bindings and event attributes stood.
An entry gets its hint, its text, and its key and leaving handlers in the order they must run.
A chip gets its wording, its close icon and its close observer.

## `internal void QLabelFieldApply(ItemsControl list, PCard card)`

Readies a card's Tag list once: the chip or entry selector, the item fill and the click on its surface.
The selector is set only while none is, since a new selector would rebuild every item.
