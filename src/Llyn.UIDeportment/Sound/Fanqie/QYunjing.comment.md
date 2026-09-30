# QYunjing.cs

## `internal sealed class QYunjing`

The yunjing panel's driver: the workspace browsed as a rime table, by onset and rime of the user's reconstruction.
It is shown only while a loaded language pack carries rime books, since without them there is no table.
Every decision lives in [CYunjing](../../../Llyn.Conduct/Panel/CYunjing.comment.md), and this file writes controls on notice.
It drives the Veneer page `PYunjing`, so the page holds only markup.
The two columns, the entry list, the category page, the reader and the editor are all driven from one file.

## `internal QYunjing(UserControl surface)`

Takes the Veneer page the main window places and pulls its parts by contract ID.
It points the export and print buttons at their commands.
It ties the droppers to their popups, sets every icon, and attaches the row fills.
Row clicks are taken on each list, and every button and search field is subscribed here.

## `private Border QLadder`

Each named part of the page is pulled through `QContract` under the page's own `x:Name`.

## `internal void QYunjingIntroduce(PWindow host)`

Builds the Conduct session, wraps its editor, and subscribes the notices.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The lectern is built here to follow the panel, so the session names no driver type.
Both order menus are built once from `CYunjingOrderRead`, since the offered orders need no session.
Each column, the page and the mode answer the area's change with their own read.
It then wires the lists, the page and the editor.
The print and portrait command bindings are added last, so no can-execute query meets a session not yet built.

## `private void QYunjingStoreRefine()`

Lights the store button only while the desk holds something to save.
It runs whenever the desk reports its state again.

## `internal void QYunjingVistaRefine()`

Answers the workspace's opening once the session restored its vistas and attached its observers.
It marks the ordering of both columns and paints the mode and the entry list.
The columns and the page answer the same opening with their own Refines.

## `private async void QYunjingWorkspaceRefine()`

Answers the area's workspace change by drawing the flags of the languages again.
The area has already let both columns and the chosen entry go.
Once the flags are in, it repaints the entry list, the only list whose rows carry a flag.
So rows built while the load ran, after a cell was chosen, gain their flags.
A failed load throws before the repaint, as the old load before the reset did.
Its one request is `CYunjingXiaoyunLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QYunjingQueryRefine()`

Answers the area's opening of a cell a fanqie chip names by emptying both search fields.
The area has already emptied both queries, so the fields only show it.
The fields' own handlers still hear the change, and their gates find the queries already empty.

## `internal void QYunjingExitRefine()`

Releases the editor's player as the window closes.
The area's own close, run by the atelier, lets the draft go and stops the playback.

## `private bool QYunjingShownCheck()`

Tells the session whether the page is on screen, since the session cannot see the window.

## `internal void QShengmuRefine()`

Lists the initial column afresh with the empty text the session names.

## `internal void QYunmuRefine()`

Lists the rime column afresh with the empty text the session names.

## `private void QXiaoyunRefine()`

Lists the entries at the chosen cell afresh with the empty text the session names.

## `private void QXiaoyunRefine(IReadOnlyList<CVistaRow> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `internal void QYunjingDiweiRefine()`

Hands the page its composed content, blank while it is hidden.
It answers the area's change, a cleared entry list and the workspace's opening.

## `private void QYunjingModeRefine()`

Writes every visibility and enablement off the session's verdicts.

## `private void QPlumbObserve(object sender, TextChangedEventArgs e)`

A change of the initial search field hands its text to the find gate.

## `private void QFathomObserve(object sender, TextChangedEventArgs e)`

A change of the rime search field hands its text to the find gate.

## `private void QBeaconObserve(object sender, TextChangedEventArgs e)`

A change of the entry search field hands its text to the find gate.

## `private void QLadderObserve(object sender, RoutedEventArgs e)`

Hands the picked ordering to the initial column's gate, then closes the menu.

## `private void QLadderRefine()`

Closes the initial column's order menu.

## `private void QStairObserve(object sender, RoutedEventArgs e)`

Hands the picked ordering to the rime column's gate, then closes the menu.

## `private void QStairRefine()`

Closes the rime column's order menu.

## `private void QYunjingObserve(object sender, RoutedEventArgs e)`

A click on either column hands the cell's id and side to the select gate.

## `private void QXiaoyunObserve(object sender, RoutedEventArgs e)`

A click on the entry list hands the row's id to the select gate.

## `private void QYunjingFreshObserve(object sender, RoutedEventArgs e)`

The fresh button asks the panel for a new entry.

## `private void QYunjingViewerObserve(object sender, RoutedEventArgs e)`

The view switch hands the scribe gate a false.
Each switch has its own handler, so no sender is compared.

## `private void QYunjingScribeObserve(object sender, RoutedEventArgs e)`

The edit switch hands the scribe gate a true.

## `private void QYunjingStoreObserve(object sender, RoutedEventArgs e)`

The store button asks the editor to save the entry.

## `private void QYunjingBinObserve(object sender, RoutedEventArgs e)`

The bin button asks the panel to delete the entry.

## `private void QYunjingPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Tells the print and portrait commands whether the panel allows them now.

## `private async void QYunjingPressObserve(object sender, ExecutedRoutedEventArgs e)`

The print command asks the session to print the portrait.

## `private async void QYunjingPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

The portrait command asks the session to export the portrait.

## `internal void QYunjingVoyageRefine(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void QYunjingRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QYunjingAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.

## `private void QYunjingUndoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void QYunjingRedoObserve(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void QYunjingChronicleRefine()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.
