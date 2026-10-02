# QRegister.cs
Hash: `8cb2a277996528d2`

## `internal sealed class QRegister`

The Register field's driver, torn apart by role as `docs-work/JobPrinciple.md` section 13 asks.
Each Observe hears one event and ends in one card gate, and each Refine only changes the look.
The entry's keys are heard by several handlers in order, and the first to handle a key ends its route.
The dropdown's key handlers come first, so an open dropdown hears a key before the entry does.
A chip or caret item's data context is the item, not the card.
So each handler finds the owning card by asking which card's collection holds the item.
Reaching the entry itself is shared with the Tag field, which is written the same way.

## `internal QRegister(`

Holds the editor scope, since this driver has no control of its own to wire.
It holds the two card lists the editor keeps, which its finds walk.

## `internal void QRegisterIntroduce(CEditor editor, QProffer proffer)`

Holds the Conduct editor whose card gates the handlers call, and the dropdown driver they paint.

## `internal void QRegisterTextObserve(PRegisterCaret caret, string text)`

Hears each edit of a Register caret and hands the raw text to the register gate, unsettled.
`QProfferRegisterRefine` then paints the answer.
That is the kept text and the offered Registers.

## `internal void QRegisterChipObserve(object sender, RoutedEventArgs e)`

Hears the close button of a chip and hands that Register to the erase gate.

## `internal void QRegisterCommitObserve(object sender, KeyEventArgs e)`

Enter hands the raw text standing in the entry to the register gate at the caret, settled.
It runs after the dropdown's handlers, so Enter on a chosen stored Register never reaches it.
`QProfferRegisterRefine` paints the gate's answer, so the entry empties and the dropdown shuts.
The wording goes to the engine as written, and the engine decides whether it names a stored Register.

## `internal void QRegisterEraseObserve(object sender, KeyEventArgs e)`

Backspace at the entry's start or Delete at its end hands the chip beside the caret to the erase gate.
`QCaretEdgeApply` decides the edge, and the key is taken at the edge even with no chip there.

## `internal void QRegisterCaretRefine(object sender, KeyEventArgs e)`

A Refine, since the arrows only walk the empty entry past a chip and ask no gate.
`QCaretStepApply` decides the step, and the caret is put back into the moved entry.

## `internal void QRegisterBlurRefine(object sender, RoutedEventArgs e)`

Shuts the dropdown when focus leaves the entry, since the field it was opened for is no longer typed into.
It is subscribed before the close observer, so the dropdown shuts first.

## `internal void QRegisterCloseObserve(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field, then paints the answer.
So a Register typed and abandoned is kept rather than silently dropped.

## `internal void QRegisterFocusRefine(object sender, MouseButtonEventArgs e)`

A Refine, since the click changes only where the caret stands and asks no gate.
Puts the caret at the end of the field when the empty space beside the chips is clicked.

## `internal PCard? QRegisterCardFind(object row)`

The card whose Register field holds this chip or caret, or `null` when no shown card does.

## `internal void QRegisterApply(FrameworkElement container, object item, string? _)`

Fills one item of a card's Register field, where bindings and event attributes stood.
An entry gets its hint, its text and its key and leaving handlers in role order.
A chip gets its wording, its close icon and its close observer.

## `internal void QRegisterFieldApply(ItemsControl list, PCard card)`

Readies a card's Register list once.
It sets the chip or entry selector, the item fill and the click on its surface.
The selector is set only while none is, since a new selector would rebuild every item.
