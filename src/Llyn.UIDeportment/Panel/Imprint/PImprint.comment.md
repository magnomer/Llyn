# PImprint.cs

## `public class PImprint : UserControl, PChronicleHost`

The Source edit area as a control of its own, held by the Source panel.
It holds no state, since the imprint deportment holds the desk, the blank row and the byline.
Every handler forwards what the control carries, and every update writes what the deportment answers.
The fields, the credit rows and the byline all live in this one file, since none of them holds anything.

## `public PImprint()`

Loads the markup from the Veneer, wears it as content, and copies its name scope.
Registers the markup's styles with the look sheet, so a disabled credit handle fades.
Merges the byline row template, seats the credit list, and builds the kind menu once.
Subscribes the four fields, the four routed events of the credit list, and its hover and focus.
Ties the kind chip to its list and attaches the credit and byline item fills.
The credit fill is followed by the handle update, and the byline fill gets the template's forwarder.
The byline popup places itself under its field through the shared field helper.

## `internal void PImprintAttach(PWindow host, LImprint imprint)`

Takes the deportment the shelf composed and listens to its notices and to its desk.
A desk failure goes straight to the window's failure dialog.

## `internal void PImprintClear()`

Blanks every field and restores each placeholder, for the moment no draft is held.

## `public void PChronicleUpdate()`

Asks the desk to raise its state again, so the rail reads the chronicle after a step.

## `private void PImprintDraftUpdate(LDraft draft)`

Writes the four fields, their placeholders, the kind chip and the tally from the Source in the notice.
A field whose text is unchanged keeps its caret.
The framework ignores a write of the same text.

## `private void PAuthorUpdate()`

Splices the credit rows, so a row that kept its Author and place keeps its field.
The unsaved notice shows while no draft is held.

## `private void PAuthorRestore()`

Writes the held name back into whichever credit field has the keyboard.

## `private void PBylineUpdate()`

Reads the offered rows, targets the popup at the focused field, and opens or closes it.
The lit row follows the deportment's index and is scrolled into view.
The frame takes the field's width as its least width, as the markup once bound it.

## `private void PAuthorCreditHandle(object sender, RoutedEventArgs e)`

A click on one of the four row buttons, passed on with its action and its row.
The action is read from the clicked button's name, since the markup carries no action tag.

## `private void PAuthorTextHandle(object sender, TextChangedEventArgs e)`

Typing in a credit field, passed on with whether the field has the keyboard.
A field the edit area wrote has no keyboard, and the deportment opens nothing for it.

## `private void PAuthorKeyHandle(object sender, KeyEventArgs e)`

A key in a credit field, passed on with the row, the text and the lit byline row.
The deportment answers whether it took the key.

## `private void PAuthorLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Leaving a credit field closes the byline and drops what was typed there.

## `internal void PBylineHandle(object sender, MouseButtonEventArgs e)`

A click on an offered byline row, routed here from the byline template's forwarder.

## `private void PAuthorShelfUpdate()`

Shows every credit row's handles while the pointer or the caret is in the list, and hides them otherwise.
It runs on the list's hover and focus changes and after every credit row fill.

