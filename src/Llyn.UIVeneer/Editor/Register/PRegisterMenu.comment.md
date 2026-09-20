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

The keys the caret answers.
The shelf comes first while it stands open, then Enter, then the edits an empty caret allows.
Backspace and Delete reach the neighbouring chips only when nothing is selected, so ordinary editing is untouched.

## `internal void PRegisterCloseHandle(object sender, RoutedEventArgs e)`

Commits what stands in the caret when focus leaves it, so text is never lost by clicking away.

## `internal void PRegisterFocusHandle(object sender, MouseButtonEventArgs e)`

Puts the caret at the end of the field when the empty space beside the chips is clicked.

## `internal PCard? PCardRegisterFind(object row)`

The card whose Register field holds this chip or caret, or `null` when no shown card does.

## `private bool PRegisterSend(PCard card, string text)`

Turns a typed name into a request, and answers whether one went out.
A name the card already carries goes nowhere.
One that names a stored Register exactly is picked, and any other is added new.

## `private void PRegisterSend(PCard card, long? id, string written)`

Sends the pick or the addition at the place the caret stands.

## `private void PRegisterRemove(PCard card, PRegister? chip)`

Asks the engine to unlink the chip, when there is one to unlink.

## `private void PRegisterCommit(PCard card)`

Turns what stands in the caret into a chip and sends the wording to the engine as written.
The engine decides whether the wording names a stored Register or starts a new one.
The panel holds no matching rule of its own, so every client of the engine gets the same answer.
