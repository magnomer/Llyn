# PImprint.xaml.cs

## `public partial class PImprint : UserControl, PChronicleHost`

The Source edit area as a control of its own, held by the Source panel.
It holds no state, since the imprint deportment holds the desk, the blank row and the byline.
Every handler forwards what the control carries, and every update writes what the deportment answers.
The fields, the credit rows and the byline all live in this one file, since none of them holds anything.

## `public PImprint()`

Merges the byline row template, seats the credit list, and builds the kind menu once.
The byline popup places itself under its field through the shared field helper.

## `internal void PImprintAttach(PWindow host, LImprint imprint)`

Takes the deportment the shelf composed and listens to its notices and to its desk.
A desk failure goes straight to the window's failure dialog.

## `internal void PImprintClear()`

Blanks every field and restores each placeholder, for the moment no draft is held.

## `public void PChronicleUpdate()`

Asks the desk to raise its state again, so the rail reads the chronicle after a step.

## `private void PImprintStartUpdate(LTenure held)`

Attaches the desk's own updates to the tenure the desk just started, on the window's thread.

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

## `private void PAuthorCreditHandle(object sender, RoutedEventArgs e)`

A click on one of the four row buttons, passed on with the button's tag and its row.

## `private void PAuthorTextHandle(object sender, TextChangedEventArgs e)`

Typing in a credit field, passed on with whether the field has the keyboard.
A field the veneer wrote has no keyboard, and the deportment opens nothing for it.

## `private void PAuthorKeyHandle(object sender, KeyEventArgs e)`

A key in a credit field, passed on with the row, the text and the lit byline row.
The deportment answers whether it took the key.

## `private void PAuthorLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Leaving a credit field closes the byline and drops what was typed there.

## `internal void PBylineHandle(object sender, MouseButtonEventArgs e)`

A click on an offered byline row, routed here from the row template's own class.
