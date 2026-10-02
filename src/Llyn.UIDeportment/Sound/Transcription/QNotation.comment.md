# QNotation.cs

## `internal sealed class QNotation`

The editor's driver for the pronunciation and transcription lookup menu.
Every pronunciation, transcription and glyph row carries its own lookup button, and the menu opens under the one pressed.
The menu is one popup the editor owns, retargeted at the row that asked for it.
Opening it starts a search for the headword, the same search from whichever row.
The search is a foray the desk's errand starts and keeps, so the menu holds no handle.
Candidates stream in from the engine, and the errand keeps the rows, one reading per variety a source returned.
Picking one hands it to the errand, which writes it into the row that opened the menu.
The errand marshals each step onto the dispatcher and raises the ready notation state.
So the menu implements no contract and decides nothing about a step.

## `internal QNotation(FrameworkElement surface)`

Hands the notation list its row fill, and wires the lookup button and the menu's close.
The lookup icon is set here, where the markup held an icon lookup.

## `internal void QNotationIntroduce(CEditor editor)`

Holds the Conduct editor and repaints the menu on each search step the errand raises.

## `private void QPhoneticianRefine(object sender, RoutedEventArgs e)`

Opens the menu under the primary row's lookup button.
It is subscribed before `QPhoneticianObserve`, so the old search closes before the new one starts.

## `private void QPhoneticianObserve(object sender, RoutedEventArgs e)`

Asks the errand for a pronunciation search on the primary row, then paints the start.

## `private void QNotationClosedObserve(object? sender, EventArgs e)`

A closed menu stops the search, so a stale arrival finds no foray.

## `private void QNotationSelectorObserve(object sender, RoutedEventArgs e)`

Hands the pressed reading's phonetic and variety to the errand, then closes the menu.
The template's reading fill subscribes it on each realized reading button.

## `internal void QNotationOpenRefine(UIElement anchor)`

Places the menu under the pressed button.
The popup is shut first, so a press on another row's button moves it instead of leaving it put.
Shutting it stops the old search through `QNotationClosedObserve`, before the new start.

## `private void QNotationCloseRefine()`

Shuts the menu.

## `internal async void QNotationStartRefine(CNotationRoll roll)`

Paints the state the start answered, then loads the flags and paints the state again.
Readings that landed before the flags thus gain them, since every paint rebuilds the rows.
The flags load after the start, as the clip popup's do.

## `private void QNotationRefine(CNotationRoll roll)`

Paints the ready notation state: the rows, the progress line and the notice.
Each paint builds fresh rows, so a flag or notice that changed always shows.
