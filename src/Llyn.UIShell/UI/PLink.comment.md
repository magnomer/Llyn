# PLink.cs

## `public partial class PEditor`

The editor's half of the Translation field: chip removal, caret keys, focus routing and resolution.
The templates that draw the field cannot reach the card the links belong to.
An item's data context is the link or the entry, not the card.
So each handler asks which card's collection holds the item, as the Tag field does.

Resolution lives here rather than on the card because it is the editor that holds the engine.
A card holds ids and knows nothing about how a word becomes one.

## `internal void PLinkAttach(PCard card)`

Gives a card the way back to the editor its typed words are resolved through.
Every card the editor builds is attached, whether it was loaded or added by hand.

## `internal void PLinkChipHandle(object sender, RoutedEventArgs e)`

Closes the link whose button was pressed.

## `internal void PLinkCaretHandle(object sender, KeyEventArgs e)`

An open dropdown is offered every key first, so it answers the arrows and enter before the field does.
Enter resolves what is standing in the entry once no dropdown is open.
Escape closes an open dropdown and leaves the word alone.
Backspace at the start of the entry drops the link before it.
Delete at the end of the entry drops the link after it.
The arrow keys walk the entry past a link once the text runs out.
Every other key is left to the text box.

## `internal void PLinkCloseHandle(object sender, RoutedEventArgs e)`

Resolves what is standing in the entry when focus leaves the field.
A word one Entry answers becomes a link, as it would on enter.
Anything else is left in the entry rather than opening a dropdown the user has already walked away from.

## `internal void PLinkFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like.

## `internal void PLinkFlagUpdate()`

Redraws every card's chips once the workspace's flags have finished loading.

## `internal void PLinkFreshClear()`

Forgets the stubs made while editing, because a saved entry keeps every stub it linked.

## `internal void PLinkFreshDelete()`

Drops the stubs made while editing when the edit is thrown away.
A stub something else already links to is left standing.
A stub that will not delete is passed over rather than stopping the discard.

## Inline notes

### `private bool PLinkResolve(PCard card, string text, bool offered)`

One Entry whose whole headword is the word is not a choice, so it is linked without asking.
Anything else is a question, and a question is only asked when the user is still in the field.
That is what `offered` says.
An unasked question leaves the word standing in the entry, where the user can still see it.
A search the workspace refuses raises a notice, because a silent nothing reads as no match.

### `private TextBox? PLinkBoxFind(PCard card)`

The dropdown hangs off the caret it was opened from.
The caret has no name to bind to, so the focused box is asked whether it is that card's.

### `private static void PLinkCaretApply(TextBox box, PLinkCaret row, int caret)`

Moving the entry rebuilds its item, so the focused box is gone by the time the move lands.
The caret is put back on the newly drawn entry once the field is laid out again.

### `private static TextBox? PLinkCaretFind(DependencyObject root)`

The entry is found by walking the drawn field, because it is one item of a templated collection.
Its position moves as links are added.
