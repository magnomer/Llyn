# QImprint.cs
Hash: `9b39117b6be646e7`

## `internal sealed class QImprint : QChronicleHost`

The driver of the Source edit area, built by the Source panel over its veneer page.
It holds no state, since Conduct's `CImprint` holds the desk, the blank row and the byline.
`CImprint` also wires its desk's notices at build, so this driver attaches no observer.
Every handler hands what the page carries to one gate, and every update writes what Conduct answers.
The fields, the credit rows and the byline all live in this one file, since none of them holds anything.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.

## `internal QImprint(UserControl surface)`

Takes the page and pulls each part through `QContract.QContractFind`.
Seats the credit list, and builds the kind menu once from `CImprint.CImprintKindRead`.
Subscribes the four fields, typing, keys and focus of the credit list, and its hover and focus.
Ties the kind chip to its list and attaches the credit and byline item fills.
The credit fill subscribes the row's four handles and is followed by the handle update.
The byline fill subscribes this driver's press handler on each row.
The byline popup places itself under its field through the shared field helper.

## `internal void QImprintIntroduce(CImprint imprint)`

Takes the source editor the shelf composed and listens to its notices and its byline.
It calls no gate, so the panel's introduce hands the engine one request only.

## `internal void QImprintCloseRefine()`

Shuts the kind menu when the window closes.
The byline's close is the shelf's, run with the atelier's closures.

## `internal void QImprintClearRefine()`

Writes the editor's empty Source, so every field blanks and restores its placeholder.

## `internal void QImprintTallyRefine()`

Writes how many places cite the stored Source, as the editor answers it.
The Source panel also runs it whenever the shelf rows change.
A citation made elsewhere moves the count without a new draft.

## `private void QImprintDraftRefine(CReference reference)`

Writes the four fields, their placeholders, the kind chip and the tally from the Source the imprint announced.
A field whose text is unchanged keeps its caret.
The framework ignores a write of the same text.

## `private void QAuthorRefine()`

Splices the credit rows, so a row that kept its Author and place keeps its field.
The unsaved notice shows while no draft is held.

## `private void QAuthorShelfIntroduce(FrameworkElement container)`

Subscribes a realized credit row's four buttons, each to the handler of its own gate.
A row realized again is unsubscribed first, so no button fires twice.

## `private void QAuthorRestoreRefine()`

Writes the held name back into whichever credit field has the keyboard.

## `private void QBylineRefine()`

Reads the offered rows, targets the popup at the focused field, and opens or closes it.
The lit row follows the byline's index and is scrolled into view.
The frame takes the field's width as its least width, as the markup once bound it.

## `private void QImprintKindObserve(object sender, RoutedEventArgs e)`

A picked kind option, whose tag goes raw to `CImprintKindSet`.
It then hands off to `QImprintKindRefine`, which shuts the chip.

## `private void QAuthorAddObserve(object sender, RoutedEventArgs e)`

The add button of a credit row, handed to `CImprintAuthorAdd` with the row's place and Author.
The remove button hands its row to `CImprintAuthorRemove`.
The earlier and later buttons hand theirs to `CImprintAuthorMove`, one place back or on.

## `private void QAuthorTextObserve(object sender, TextChangedEventArgs e)`

Typing in a credit field, passed on with whether the field has the keyboard.
A field the edit area wrote has no keyboard, and the byline opens nothing for it.

## `private void QAuthorKeyObserve(object sender, KeyEventArgs e)`

A key in a credit field, handed to the gate of its key with the row it came from.
Enter finishes the credit with the text and the lit byline row, and Escape cancels it.
Down and Up move the lit byline row.
The gate answers whether it took the key, and any other key is left to the field.
Setting `Handled` from that verdict is routing, not a look.

## `private void QAuthorLeaveObserve(object sender, KeyboardFocusChangedEventArgs e)`

Leaving a credit field closes the byline through `CBylineClose`.
It then hands the field to the row's revert, which drops what was typed there.

## `private void QBylineObserve(object sender, MouseButtonEventArgs e)`

A press on an offered byline row, subscribed on each realized row.

## `private void QAuthorShelfRefine()`

Shows every credit row's handles while the pointer or the caret is in the list, and hides them otherwise.
It runs on the list's hover and focus changes and after every credit row fill.
