# PLink.cs

## `public partial class PEditor`

The Translation field's driver, torn apart by role as `docs-work/JobPrinciple.md` section 13 asks.
Each Observe hears one event and ends in one card gate, and each Refine only changes the look.
The entry's keys are heard by several handlers in order, and the first to handle a key ends its route.
The trim, the blank check, the resolve and the search live below Conduct, not here.
The templates that draw the field cannot reach the card the links belong to.
An item's data context is the link or the entry, not the card.
So each handler asks which card's collection holds the item, as the Tag field does.

## `private void PLinkTextObserve(PLinkCaret caret, string text)`

Hears each edit of a Translation caret and hands the raw text to the translation gate.
The gate links each completed word that resolves and answers the words that stay, with the dropdown for them.
Every card the editor builds is heard, whether it was loaded or added by hand.

## `private void PLinkRefine(PCard card, CProspect prospect)`

Paints a translation gate's answer: the text the entry keeps, then the dropdown open or shut.
The rows, the word and the chosen row come ready, so nothing is decided here.

## `private void PLinkDropObserve(object sender, RoutedEventArgs e)`

Hears the close button of a chip and hands that link to the erase gate.

## `private void PLinkCommitObserve(object sender, KeyEventArgs e)`

Enter hands the raw text standing in the entry to the resolve gate, asking to see the choices.
It runs after the dropdown's handlers, so enter on a chosen row never reaches it.
The gate answers the dropdown, with one whole-headword match chosen already, and the Refine paints it.

## `private void PLinkEraseObserve(object sender, KeyEventArgs e)`

Backspace at the entry's start or Delete at its end hands the chip beside the caret to the erase gate.
`QCaretEdgeApply` decides the edge, and the key is taken at the edge even with no chip there.

## `private void PLinkCaretRefine(object sender, KeyEventArgs e)`

A Refine, since the arrows only walk the empty entry past a chip and ask no gate.
`QCaretStepApply` decides the step, and the caret is put back into the moved entry.

## `private void PLinkBlurRefine(object sender, RoutedEventArgs e)`

Shuts the dropdown when focus leaves the entry, since the user has walked away from it.
It is subscribed before the close observer, so the dropdown shuts first.

## `private void PLinkCloseObserve(object sender, RoutedEventArgs e)`

Hands what is standing in the entry to the resolve gate when focus leaves the field.
A word one Entry answers becomes a link, and the entry empties.
Anything else is left in the entry rather than opening a dropdown the user has already left.

## `private void PLinkFocusRefine(object sender, MouseButtonEventArgs e)`

A Refine, since the click changes only where the caret stands and asks no gate.
Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like.

## `internal void PLinkFlagRefine()`

Redraws every card's chips once the workspace's flags have finished loading.

## `private readonly PLinkTemplate _pLinkTemplate`

The Translation dictionary, held for its chip and entry templates.

## `private void PLinkApply(FrameworkElement container, object item, string? _)`

Fills one item of a card's Translation field, where bindings and event attributes stood.
An entry gets its hint, its text, and its key and leaving handlers in the order they must run.
A chip gets its wording, its close icon and its close observer.

## `private void PLinkFieldApply(ItemsControl list)`

Readies a card's Translation list once: the chip or entry selector, the item fill and the click on its surface.
The selector is set only while none is, since a new selector would rebuild every item.

## Inline notes

### `private TextBox? PLinkBoxFind(PCard card)`

The dropdown hangs off the caret it was opened from.
The caret has no name to bind to, so the focused box is asked whether it is that card's.
