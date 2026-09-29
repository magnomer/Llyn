# QWing.cs

## `internal sealed class QWing`

Drives one side of the duplex panel: a search bar, the matches it finds and the Entry picked from them.
The two sides are peers, so each place of the veneer's `PWing` gets its own driver.
Each driver has its own vista and its own display.
The panel never writes, so there is no editor and nothing to discard.
The match list, the field's keys and focus live here, since they are medium.
The vista, the rows and the saved standing live in Conduct's `CWing`.

## `internal QWing(UserControl surface)`

Takes one `PWing` place the duplex pulled by its contract ID.
It builds the match list over the place's list and empty notice, since both are there from the start.
It ties both droppers to their popups, sets both icons, and subscribes the field and list events.
It attaches the index row fill and watches the list's visibility for the tray.

## `private ToggleButton QWingOrderDropper`

Each named part is pulled from the place by its contract ID on every read.

## `internal void QWingIntroduce(PWindow host, bool left)`

Puts the side to work on the host, building Conduct's side over the host's atelier and envoy.
`left` tells Conduct which place this is, so the side picks its own tab and saved entry.
The side's announcements re-list the matches on the list's thread.
A loaded entry re-lists the matches, and the display's area opens it on the lectern itself.
A key move re-lists the matches on the spot, since it is heard on the list's thread.
Each workspace open repaints the side's menus through `QWingRefine`.
The display is its own subscriber, so it stays current on its own.

## `private async void QWingRefine()`

Answers every workspace open, once Conduct has handed the side its vista.
The list is cleared, then the flags are loaded before any row is built.
The language menu is built from the languages that load answers.
The dropdown lists the shared entry orderings, and the filter mark is drawn from the side's verdict.
The field is emptied, since the fresh vista holds no query.
Conduct restores the entry itself, so this paint calls no gate.

## `private void QWingIndexRefine()`

Lists the matches from the vista, already filtered, sorted, numbered and marked by the engine.
The empty notice shows as `CWingEmpty` answers.
It runs on each announcement, after each entry the side loaded, and after each key move.

## `private void QWingSieveRefine()`

Shows the mark on the sieve button while the vista hides any language.

## `private void QWingOrderObserve(object sender, RoutedEventArgs e)`

A clicked order row sets the ordering its tag carries, then closes the dropdown.
The announcement then re-lists the matches.

## `private void QWingOrderRefine()`

Closes the order dropdown.

## `private void QWingSieveObserve(object sender, RoutedEventArgs e)`

A clicked language row sets the filter its list now stands for, then redraws the mark at once.

## `private void QWingQueryObserve(object sender, TextChangedEventArgs e)`

Hands the text to the vista, then shows the list while the vista holds a query and hides it otherwise.

## `private void QWingKeyRefine(object sender, KeyEventArgs e)`

The field's first key handler: Escape hides an open list and takes the key.
A handled key ends the route, so the key observe never hears it.

## `private void QWingKeyObserve(object sender, KeyEventArgs e)`

The keyboard path from the field into the open list, heard after the key refine.
A closed list lets every key through to the field, so a hidden list is never picked from.
The index remembers whether it is shown, since a control's state never gates a request.
Each key picks one gate, and the gate answers whether it took the key.
Enter hands the chosen row to `CWingEntryOpen`, and an opened entry hides the list.
Down and Up hand the direction to `CWingRowMove`, and the answered entry is scrolled into view.
The gate re-marks the rows through `CWingRowsChanged`.
Which key means which action is the medium's own routing, so it stays here.

## `private void QWingLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)`

Focus leaving the field and the list hides the list.
Focus moving between the two is not a leave, since a clicked row takes focus before its click lands.

## `private void QWingIndexObserve(object sender, RoutedEventArgs e)`

A clicked row hands its id to `CWingEntryOpen`, and an opened entry hides the list.

## `private void QWingRowRefine(FrameworkElement container, object item, string? change)`

Fills one result row through the shared index fill, then subscribes its click once.

## `private void QWingTrayRefine(object? sender, EventArgs e)`

Copies the list's visibility onto the seam and the tray, the work their bindings did before.
