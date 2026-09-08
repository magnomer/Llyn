# PLabel.cs

## `public partial class PEditor`

The Tag field's handlers.
The templates that draw the field cannot reach the card the Tags belong to.
An item's data context is the Tag or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.
Reaching the entry itself is shared with the Situation field, which is written the same way.

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
