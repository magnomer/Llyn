# PRegisterMenu.cs

## `public partial class PEditor`

The editor's handling of a card's Register field: the keys typed in it and the chips clicked in it.
The templates raise events on the dictionary, which relays them here, because a template has no card of its own.
Which card a row belongs to is found from the row itself rather than passed through the visual tree.

## `internal void PRegisterAttach(PCard card)`

Points a card's Register field at the shelf, so typing offers what is already stored.

## `internal void PRegisterChipHandle(object sender, RoutedEventArgs e)`

Drops the chip whose cross was clicked.

## `internal void PRegisterCaretHandle(object sender, KeyEventArgs e)`

The keys the caret answers: the shelf first while it stands open, then Enter, then the edits an empty caret allows.
Backspace and Delete reach the neighbouring chips only when nothing is selected, so ordinary editing is untouched.

## `internal void PRegisterCloseHandle(object sender, RoutedEventArgs e)`

Commits what stands in the caret when focus leaves it, so text is never lost by clicking away.

## `internal void PRegisterFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret at the end of the field when the empty space beside the chips is clicked.

## `internal PCard? PCardRegisterFind(object row)`

The card whose Register field holds this chip or caret, or `null` when no shown card does.

## `private void PRegisterCommit(PCard card)`

Turns what stands in the caret into a chip, marking a stored Register when the wording names one exactly.
Otherwise the wording is committed as written, and the engine decides what row it becomes on save.

## `private string? PRegisterResolve(string written)`

The id of the stored Register whose name is exactly `written`, or `null` when none is.
A shelf that cannot be read is the same as a shelf holding nothing, because a chip must still be typeable.
