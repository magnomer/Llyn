# QXiesheng.cs

## `internal sealed class QXiesheng`

The xiesheng panel: the workspace browsed by the phonetic series its characters belong to.
It is shown only while a loaded language pack declares a series source, since without one there is no series.
Every decision lives in [CXiesheng](../../../Llyn.Conduct/Panel/CXiesheng.comment.md), and this file writes controls on notice.
The series column, the entry list, the reader and the editor are served from one file.
The series page has its own driver, `QStem`, which this one builds over the nested page.

## `internal QXiesheng(UserControl surface)`

Takes the veneer page as its surface, builds the series page driver, and binds the print and export commands.
It points the export and print buttons at their commands.
It ties the droppers to their popups, sets every icon, and attaches the row fills.
Row clicks are taken on each list, and every button and search field is subscribed here.

## `private Border QRungBar`

Each named part of the page is pulled through `QContract.QContractFind`.

## `internal void QXieshengIntroduce(PWindow host)`

Builds the Conduct session, wraps its editor, and subscribes the notices.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The lectern is built here to follow the panel, so the session names no driver type.
It builds the ordering menu of the series column once, from the orders the session offers.
The column, the series page and the mode each answer the session's change with their own Refine.
It then attaches the reader, the series page and the editor.

## `private void QXieshengStoreRefine()`

Enables the save button while the editor holds something storable.

## `internal void QXieshengVistaRefine()`

Answers the workspace's opening once the session restored its vistas and attached its observers.
It marks the column's ordering and paints the mode and the entry list.
The column and the series page answer the same opening with their own Refines.

## `private async void QXieshengWorkspaceRefine()`

Answers the area's workspace change by drawing the flags of the languages again.
The area has already let the series and the chosen entry go.
Once the flags are in, it repaints the entry list, whose rows carry a flag.
So rows built while the load ran, after a series was chosen, gain their flags.
A failed load throws before the repaint, as the old load before the reset did.

## `private void QLodestarRefine()`

Answers the area's opening of a series a chip names by emptying the column's query field.
The area has already emptied the query, so the field only shows it.
The field's own handler still hears the change, and its gate finds the query already empty.

## `internal void QXieshengExitRefine()`

Releases the editor's recording player as the window exits.
The session's close already let the draft go and stopped the playback.

## `private bool QXieshengShownCheck()`

True while the panel is the visible tab.

## `internal void QGroveRefine()`

Copies the series column and its empty line again.

## `private void QKindredRefine()`

Copies the entry list and its empty line again.

## `internal void QXieshengStemRefine()`

Hands the page of the chosen series to the series page driver.
It also answers the panel's clearing, while the lectern empties the reader itself.

## `private void QXieshengModeRefine()`

Shows the reader, the page or the editor, and enables the mode and bin buttons.

## `private void QLodestarObserve(object sender, TextChangedEventArgs e)`

Narrows the series column as the field is typed into.

## `private void QSextantObserve(object sender, TextChangedEventArgs e)`

Narrows the entry list as the field is typed into.

## `private void QRungObserve(object sender, RoutedEventArgs e)`

Lists the series column in the ordering picked, then closes the dropper.

## `private void QRungRefine()`

Closes the dropper once an ordering is picked.

## `private void QGroveObserve(object sender, RoutedEventArgs e)`

Chooses the series of the pressed row.

## `private void QKindredObserve(object sender, RoutedEventArgs e)`

Hands the pressed row to the panel's row gate, which records the voyage station and opens it.

## `private void QXieshengFreshObserve(object sender, RoutedEventArgs e)`

Starts a fresh entry in the editor.

## `private void QXieshengViewerObserve(object sender, RoutedEventArgs e)`

Switches to reading.

## `private void QXieshengScribeObserve(object sender, RoutedEventArgs e)`

Switches to editing.

## `private void QXieshengStoreObserve(object sender, RoutedEventArgs e)`

Saves what the editor holds.

## `private void QXieshengBinObserve(object sender, RoutedEventArgs e)`

Deletes the entry the panel holds.

## `private void QXieshengPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Allows printing and exporting only while an entry is read.
It answers no before the panel is attached, since the commands are bound at construction.

## `private async void QXieshengPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the read entry on the printer the gate asks for.

## `private async void QXieshengPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the read entry as a portrait file.

## `internal void QXieshengVoyageRefine(bool past, bool future)`

Enables the back and forward buttons of the rail.

## `private void QXieshengRetreatObserve(object sender, RoutedEventArgs e)`

Sails one station back.

## `private void QXieshengAdvanceObserve(object sender, RoutedEventArgs e)`

Sails one station forward.

## `private void QXieshengUndoObserve(object sender, RoutedEventArgs e)`

Undoes one editor change.

## `private void QXieshengRedoObserve(object sender, RoutedEventArgs e)`

Redoes one editor change.

## `private void QXieshengChronicleRefine()`

Enables the undo and redo buttons as the editor's chronicle changes.
