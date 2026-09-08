# PContextMenu.cs

## `public partial class PEditor`

The Situation field's handlers.
The templates that draw the field cannot reach the card the Situations belong to.
An item's data context is the Situation or the entry, not the card.
So each handler finds the owning card by asking which card's collection holds the item.

## `internal void PContextAttach(PCard card)`

Hands the card the way to ask for candidates as its Situation caret is typed into.
The card holds no engine, so it reports what was typed and the editor answers with the dropdown.

## `internal void PContextChipHandle(object sender, RoutedEventArgs e)`

Closes the Situation whose button was pressed.

## `internal void PContextCaretHandle(object sender, KeyEventArgs e)`

The dropdown is offered the key first, so the arrows and escape reach the candidate list.
Enter commits what is standing in the entry, unless a candidate is selected in the dropdown.
Backspace at the start of the entry drops the Situation before it.
Delete at the end of the entry drops the Situation after it.
The arrow keys walk the entry past a Situation once the text runs out.
So a Situation is crossed the way a character is, in either direction.
Every other key is left to the text box.

## `internal void PContextCloseHandle(object sender, RoutedEventArgs e)`

Commits what is standing in the entry when focus leaves the field.
So a Situation typed and abandoned is kept rather than silently dropped.
Choosing a candidate empties the entry first, so the click that chose it commits nothing twice.

## Inline notes

### `private void PContextCommit(PCard card)`

A wording already stored is attached under its own id rather than written again.
So two cards sharing a wording share the Situation, and the workspace does not fill with twins.
A wording nothing matches is committed as it stands, which is what writing a new Situation means.

### `private string? PContextResolve(string written)`

Only a title equal to the whole wording counts, ignoring case: a partial match is a suggestion, not an answer.
The most used one wins when a workspace already holds several with that title.
A workspace that cannot be read leaves the wording to be written as new, rather than stopping the user.

## `internal void PContextFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret in the entry when the field's empty space is clicked.
So the whole box behaves as the one input it looks like, not only its trailing text.
