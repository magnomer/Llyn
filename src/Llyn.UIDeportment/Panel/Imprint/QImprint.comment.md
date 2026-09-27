# QImprint.cs

## `internal sealed class QImprint : QChronicleHost`

The driver of the Source edit area, built by the Source panel over its veneer page.
It holds no state, since the imprint deportment holds the desk, the blank row and the byline.
Every handler forwards what the page carries, and every update writes what the deportment answers.
The fields, the credit rows and the byline all live in this one file, since none of them holds anything.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.

## `internal QImprint(UserControl surface)`

Takes the page and pulls each part through `QContract.QContractFind`.
Seats the credit list, and builds the kind menu once.
Subscribes the four fields, typing, keys and focus of the credit list, and its hover and focus.
Ties the kind chip to its list and attaches the credit and byline item fills.
The credit fill subscribes the row's four handles and is followed by the handle update.
The byline fill subscribes this driver's press handler on each row.
The byline popup places itself under its field through the shared field helper.

## `internal void QImprintAttach(PWindow host, LImprint imprint)`

Takes the deportment the shelf composed and listens to its notices and to its desk.
A desk failure goes straight to the window's failure dialog.

## `internal void QImprintClear()`

Writes the deportment's empty Source, so every field blanks and restores its placeholder.

## `public void QChronicleUpdate()`

Asks the desk to raise its state again, so the rail reads the chronicle after a step.

## `private void QImprintDraftUpdate(CImprint reference)`

Writes the four fields, their placeholders, the kind chip and the tally from the Source the imprint announced.
A field whose text is unchanged keeps its caret.
The framework ignores a write of the same text.

## `private void QAuthorUpdate()`

Splices the credit rows, so a row that kept its Author and place keeps its field.
The unsaved notice shows while no draft is held.

## `private void QAuthorShelfAttach(FrameworkElement container)`

Subscribes a realized credit row's four buttons, each to the handler of its own gate.
A row realized again is unsubscribed first, so no button fires twice.

## `private void QAuthorRestore()`

Writes the held name back into whichever credit field has the keyboard.

## `private void QBylineUpdate()`

Reads the offered rows, targets the popup at the focused field, and opens or closes it.
The lit row follows the deportment's index and is scrolled into view.
The frame takes the field's width as its least width, as the markup once bound it.

## `private void QAuthorAddHandle(object sender, RoutedEventArgs e)`

The add button of a credit row, handed to `LImprintAuthorAdd` with the row's place and Author.
The remove, earlier and later buttons each hand their row to their own gate the same way.

## `private void QAuthorTextHandle(object sender, TextChangedEventArgs e)`

Typing in a credit field, passed on with whether the field has the keyboard.
A field the edit area wrote has no keyboard, and the deportment opens nothing for it.

## `private void QAuthorKeyHandle(object sender, KeyEventArgs e)`

A key in a credit field, passed on with the row, the text and the lit byline row.
The deportment answers whether it took the key.

## `private void QAuthorLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Leaving a credit field closes the byline and drops what was typed there.

## `private void QBylineHandle(object sender, MouseButtonEventArgs e)`

A press on an offered byline row, subscribed on each realized row.

## `private void QAuthorShelfUpdate()`

Shows every credit row's handles while the pointer or the caret is in the list, and hides them otherwise.
It runs on the list's hover and focus changes and after every credit row fill.
