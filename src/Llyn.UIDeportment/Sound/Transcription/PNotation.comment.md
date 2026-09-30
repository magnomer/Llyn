# PNotation.cs

## `public partial class PEditor`

Pronunciation and transcription lookup as the editor shows it.
Every pronunciation row and every transcription row carries its own lookup button, and the menu opens under the one pressed.
The menu is one popup the editor owns, retargeted at the row that asked for it.
Opening it starts a search for the headword, the same search from whichever row.
A transcription row names its scheme, and the search then runs that scheme's sources instead of the IPA ones.
The search is a foray the desk's errand starts and keeps, so the menu holds no handle.
Candidates stream in from the engine, and the errand keeps the rows, one reading per variety a source returned.
Picking one hands it to the errand, which writes it into the row that opened the menu.
The errand marshals each step onto the dispatcher and raises the ready notation state.
So the menu implements no contract and decides nothing about a step.

## `private void PNotationAttach()`

Hands the notation list its row fill, and wires the lookup button and the menu's close.
The lookup icon is set here, where the markup held an icon lookup.

## `private void PPhoneticianRefine(object sender, RoutedEventArgs e)`

Opens the menu under the primary row's lookup button.
It is subscribed before `PPhoneticianObserve`, so the old search closes before the new one starts.

## `private void PPhoneticianObserve(object sender, RoutedEventArgs e)`

Asks the errand for a pronunciation search on the primary row, then paints the start.

## `private void PNotationClosedObserve(object? sender, EventArgs e)`

A closed menu stops the search, so a stale arrival finds no foray.

## `internal void PNotationSelectorObserve(object sender, RoutedEventArgs e)`

Hands the pressed reading's phonetic and variety to the errand, then closes the menu.
The template's reading fill subscribes it on each realized reading button.

## `private void PNotationOpenRefine(UIElement anchor)`

Places the menu under the pressed button.
The popup is shut first, so a press on another row's button moves it instead of leaving it put.
Shutting it stops the old search through `PNotationClosedObserve`, before the new start.

## `private async void PNotationStartRefine(CNotationRoll roll)`

Paints the state the start answered, then loads the flags and paints the state again.
Readings that landed before the flags thus gain them, since every paint rebuilds the rows.
The flags load after the start, as the clip popup's do.

## `internal void PNotationRefine(CNotationRoll roll)`

Paints the ready notation state: the rows, the progress line and the notice.
`QEditor` subscribes it to the errand's notation event, so each search step repaints the menu.
Each paint builds fresh rows, so a flag or notice that changed always shows.
