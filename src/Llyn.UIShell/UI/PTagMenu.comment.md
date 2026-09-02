# PTagMenu.cs

## `public partial class PEditor`

The Tag field's handlers.
The templates that draw the field cannot reach the card the Tags belong to.
An item's data context is the Tag or the entry, not the card.
So each handler finds the owning card the way the Example and Situation rows are found.
It asks which card's collection holds the item.

## `internal void PTagChipHandle(object sender, RoutedEventArgs e)`

Closes the Tag whose button was pressed.

## `internal void PTagEntryHandle(object sender, KeyEventArgs e)`

Enter commits what is standing in the entry.
Backspace on an empty entry reaches back past the caret and drops the last Tag.
Every other key is left to the text box.

## `internal void PTagCloseHandle(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field.
So a Tag typed and abandoned is kept rather than silently dropped.

## `internal void PTagFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.

## Inline notes

### `private static TextBox? PTagEntryFind(DependencyObject root)`

The entry is found by walking the drawn field, because it is one item of a templated collection.
It has no name to bind to, and its position moves as Tags are added.
