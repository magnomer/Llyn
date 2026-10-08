# QAuthor.cs
Hash: `b182e1ab73d67219`

## `internal sealed class QAuthor`

The credit rows and the byline of the Source edit area, built by `QImprint` over the `PAuthor` grid.
It holds no state beyond the spliced rows, since Conduct's `CImprint` holds the credits and the byline.
Every handler hands what the page carries to one gate, and every update writes what Conduct answers.
It subscribes what it paints itself, so the owner `QImprint` only builds and introduces it.

## `internal QAuthor(FrameworkElement author)`

Takes the author grid and pulls each part through `QContract.QContractFind`.
The byline popup sits inside the grid, so every part it drives is found from the grid.
Seats the credit list.
Subscribes the typing, keys and focus of the credit list, and its hover and focus.
Attaches the credit and byline item fills.
The credit fill subscribes the row's four handles and is followed by the handle update.
The byline fill subscribes this driver's press handler on each row.
The byline popup places itself under its field through the shared field helper.

## `internal void QAuthorIntroduce(CImprint imprint)`

`QImprintIntroduce` calls it with the source editor the shelf composed.
It listens to the editor's credit, focus and revert notices and to its byline.
It calls no gate, so the panel's introduce still hands the engine one request only.

## `private void QAuthorRefine()`

Splices the credit rows, so a row that kept its Author and place keeps its field.
The unsaved notice shows while no draft is held.

## `private void QAuthorFocusRefine()`

Gives the keyboard to the blank credit row once the rows are spliced, when Conduct announces the focus.

## `private void QAuthorShelfIntroduce(FrameworkElement container)`

Subscribes a realized credit row's four buttons, each to the handler of its own gate.
A row realized again is unsubscribed first, so no button fires twice.

## `private void QAuthorRestoreRefine()`

Writes the held name back into whichever credit field has the keyboard.

## `private void QBylineRefine()`

Reads the offered rows, targets the popup at the focused field, and opens or closes it.
The lit row follows the byline's index and is scrolled into view.
The frame takes the field's width as its least width, as the markup once bound it.

## `private void QAuthorAddObserve(object sender, RoutedEventArgs e)`

The add button of a credit row, handed to `CImprintAuthorAdd` with the row's place and Author.
No button of a credit row hands the row's blank mark, so Conduct decides blankness from its own marker.

## `private void QAuthorRemoveObserve(object sender, RoutedEventArgs e)`

The remove button of a credit row, handed to `CImprintAuthorRemove` with the row's Author.

## `private void QAuthorRetreatObserve(object sender, RoutedEventArgs e)`

The earlier button of a credit row, handed to `CImprintAuthorMove` with the row's place and Author, one place back.

## `private void QAuthorAdvanceObserve(object sender, RoutedEventArgs e)`

The later button of a credit row, handed to `CImprintAuthorMove` with the row's place and Author, one place on.

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
