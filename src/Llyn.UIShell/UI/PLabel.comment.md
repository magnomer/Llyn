# PLabel.cs

## `public partial class PEditor`

The Tag field's handlers.
The templates that draw the field cannot reach the card the Tags belong to.
An item's data context is the Tag or the entry, not the card.
So each handler finds the owning card the way the Example and Situation rows are found.
It asks which card's collection holds the item.

## `internal void PLabelChipHandle(object sender, RoutedEventArgs e)`

Closes the Tag whose button was pressed.

## `internal void PLabelCaretHandle(object sender, KeyEventArgs e)`

Enter commits what is standing in the entry.
Backspace at the start of the entry drops the Tag before it.
Delete at the end of the entry drops the Tag after it.
The arrow keys walk the entry past a Tag once the text runs out.
So a Tag is crossed the way a character is, in either direction.
Every other key is left to the text box.

## `internal void PLabelCloseHandle(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field.
So a Tag typed and abandoned is kept rather than silently dropped.

## `internal void PLabelFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.

## Inline notes

### `private static void PLabelCaretApply(TextBox box, PLabelCaret row, int caret)`

Moving the entry rebuilds its item, so the focused box is gone by the time the move lands.
The caret is put back on the newly drawn entry once the field is laid out again.
Without it a step across a Tag would drop the user out of the field.

### `private static TextBox? PLabelCaretFind(DependencyObject root)`

The entry is found by walking the drawn field, because it is one item of a templated collection.
It has no name to bind to, and its position moves as Tags are added.
