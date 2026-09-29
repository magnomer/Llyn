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

## `internal void QWingAttach(PWindow host)`

Puts the side to work on the host, building Conduct's side over the host's atelier and envoy.
The side's announcements re-list the matches on the list's thread.
A loaded entry re-lists the matches, and the display's area opens it on the lectern itself.
A key move re-lists the matches on the spot, since it is heard on the list's thread.
The vista arrives with each restore, since it belongs to the workspace open then.
The display is its own subscriber, so it stays current on its own.

## `internal async void QWingRestore(string tab, long? id)`

Puts the side back on the workspace open now, on the vista of `tab`, standing on the Entry `id` names.
A switched workspace hands a fresh vista, so the order and filter are the new workspace's own.
A vista announcement re-lists the matches, since order, filter or query moved.
A stored entry, a reflex fill or a flipped setting can change a listed row, so each re-lists too.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
The dropdown lists the shared entry orderings, and the filter mark is drawn from the side's verdict.
The query is emptied, the display's area closes its entry, and the side loads the Entry `id` names.
The entry is closed first, so a failed load never leaves another workspace's entry standing.
The fresh vista starts with nothing typed and nothing chosen, so neither needs setting.
A different workspace has its own database.
So the Entry the side was comparing came from a workspace no longer open.

## `internal void QWingClose()`

Stops the side: the display releases its playback.

## `private void QWingIndexShow()`

Lists the matches from the vista, already filtered, sorted, numbered and marked by the engine.
The empty notice shows as `CWingEmpty` answers.
It runs on each announcement, after each entry the side loaded, and after each key move.

## `private void QWingSieveShow()`

Shows the mark on the sieve button while the vista hides any language.

## `private void QWingOrderHandle(object sender, RoutedEventArgs e)`

A clicked order row closes the dropdown and sets the ordering its tag carries.
The announcement then re-lists the matches.

## `private void QWingSieveHandle(object sender, RoutedEventArgs e)`

A clicked language row sets the filter its list now stands for, then redraws the mark at once.

## `private void QWingQueryHandle(object sender, TextChangedEventArgs e)`

Hands the text to the vista, then shows the list while the vista holds a query and hides it otherwise.

## `private void QWingKeyHandle(object sender, KeyEventArgs e)`

The keyboard path from the field into the open list.
A closed list lets every key through to the field, so a hidden list is never picked from.
The index remembers whether it is shown, since a control's state never gates a request.
Escape hides the list and takes the key.
Enter opens the chosen row while the rows still mark it.
Down and Up hand the direction to `CWingRowMove`.

## `private void QWingLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Focus leaving the field and the list hides the list.
Focus moving between the two is not a leave, since a clicked row takes focus before its click lands.

## `private bool QWingRowMove(bool down)`

Hands the direction to `CWingRowMove` and scrolls the entry it answers into view.
The gate re-marks the rows through `CWingRowsChanged`.
It answers whether a row was there, so the key is taken only then.

## `private void QWingRowOpen(long? id)`

A picked row hides the list, shows its entry and saves the side's standing.
A click hands over its row's id and Enter the chosen one, so no control decides the request.
No row picks nothing.

## `private void QWingIndexApply(FrameworkElement container, object item, string? change)`

Fills one result row through the shared index fill, then subscribes its click once.

## `private void QWingTrayHandle(object? sender, EventArgs e)`

Copies the list's visibility onto the seam and the tray, the work their bindings did before.
