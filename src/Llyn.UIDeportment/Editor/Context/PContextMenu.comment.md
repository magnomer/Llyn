# PContextMenu.cs

## `public partial class PEditor`

The Situation field's driver, torn apart by role as `docs-work/JobPrinciple.md` section 13 asks.
Each Observe hears one event and ends in one card gate, and each Refine only changes the look.
The entry's keys are heard by several handlers in order, and the first to handle a key ends its route.
The dropdown's key handlers come first, so an open dropdown hears a key before the entry does.
An item's data context is the Situation or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.

## `private void PContextTextObserve(PContextCaret caret, string text)`

Hears each edit of a Situation caret and hands the raw text to the situation gate, unsettled.
The gate adds every Situation a comma completed and answers what the entry keeps.
The editor's `PProfferSituationRefine` then paints that answer and the dropdown of stored Situations.

## `private void PContextChipObserve(object sender, RoutedEventArgs e)`

Hears the close button of a Situation chip and hands that Situation to the remove gate.

## `private void PContextCommitObserve(object sender, KeyEventArgs e)`

Enter hands the text standing in the entry to the situation gate, settled, then empties the entry.
It runs after the dropdown's handlers, so Enter on a chosen stored Situation never reaches it.
The wording goes to the engine as written, and the engine decides which Situation it names.
A wording already stored is attached under its own id there, so the workspace does not fill with twins.

## `private void PContextEraseObserve(object sender, KeyEventArgs e)`

Backspace at the entry's start or Delete at its end hands the chip beside the caret to the remove gate.
`QCaretEdgeApply` decides the edge, and the key is taken at the edge even with no chip there.

## `private void PContextCaretRefine(object sender, KeyEventArgs e)`

A Refine, since the arrows only walk the empty entry past a chip and ask no gate.
`QCaretStepApply` decides the step, and the caret is put back into the moved entry.

## `private void PContextBlurRefine(object sender, RoutedEventArgs e)`

Shuts the dropdown when focus leaves the entry.
It is subscribed before the close observer, so the dropdown shuts first.

## `private void PContextCloseObserve(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field, then empties it.
So a Situation typed and abandoned is kept rather than silently dropped.
Choosing a row empties the entry first, so the click that chose it commits nothing twice.

## `private readonly PContextTemplate _pContextTemplate`

The Situation dictionary, held for the two item templates its selector hands out.

## `private void PContextApply(FrameworkElement container, object item, string? _)`

Fills one item of a card's Situation field, where bindings and event attributes stood.
An entry gets its hint, its text and its key and leaving handlers.
The dropdown's key Refine and key Observe come first, then the commit, erase and caret handlers.
A chip gets its wording, its close icon and its close observer.

## `private void PContextFieldApply(ItemsControl list)`

Readies a card's Situation list once: the chip or entry selector, the item fill and the click on its surface.
The selector is set only while none is, since a new selector would rebuild every item.

## `private void PContextFocusRefine(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.
