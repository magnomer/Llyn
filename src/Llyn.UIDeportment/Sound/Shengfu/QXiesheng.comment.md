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

## `internal void QXieshengAttach(PWindow host)`

Builds the Conduct session, wraps its editor, and subscribes the notices.
The lectern is built here to follow the panel, so the session names no driver type.
It then attaches the reader, the series page and the editor.

## `private void QXieshengStoreUpdate()`

Enables the save button while the editor holds something storable.

## `internal void QXieshengVistaRestore()`

Attaches the observers, and builds the ordering menu of the series column.
The series column and the entry list observe through Conduct subjects, so the driver names no engine type.

## `private async void QXieshengWorkspaceUpdate()`

Reloads the flags and resets the panel, as the workspace changes under it.

## `internal bool QXieshengDraftFinish(bool store)`

Closes the held draft, saving it or dropping it, as the shell asks while leaving.

## `internal bool QXieshengChangeCheck()`

True while the panel holds an unsaved change.

## `private void QLodestarRefine()`

Answers the area's opening of a series a chip names by emptying the column's query field.

## `internal void QXieshengClose()`

Closes the editor and the reader, as the window exits.

## `private bool QXieshengShownCheck()`

True while the panel is the visible tab.

## `private void QXieshengColumnUpdate()`

Copies the series column, its empty line, the series page and the mode again.

## `private void QKindredUpdate()`

Copies the entry list and its empty line again.

## `private void QStemUpdate()`

Hands the page of the chosen series to the series page driver.

## `private void QXieshengModeUpdate()`

Shows the reader, the page or the editor, and enables the mode and bin buttons.

## `private void QXieshengClearUpdate()`

Writes the page again, as the panel is cleared.
The lectern empties the reader itself.

## `private void QLodestarHandle(object sender, TextChangedEventArgs e)`

Narrows the series column as the field is typed into.

## `private void QSextantHandle(object sender, TextChangedEventArgs e)`

Narrows the entry list as the field is typed into.

## `private void QRungHandle(object sender, RoutedEventArgs e)`

Closes the dropper and lists the series column in the ordering picked.

## `private void QGroveHandle(object sender, RoutedEventArgs e)`

Chooses the series of the pressed row.

## `private void QKindredHandle(object sender, RoutedEventArgs e)`

Hands the pressed row to the panel's row gate, which records the voyage station and opens it.

## `private void QXieshengFreshHandle(object sender, RoutedEventArgs e)`

Starts a fresh entry in the editor.

## `private void QXieshengScribeHandle(object sender, RoutedEventArgs e)`

Switches between reading and editing.

## `private void QXieshengStoreHandle(object sender, RoutedEventArgs e)`

Saves what the editor holds.

## `private void QXieshengBinHandle(object sender, RoutedEventArgs e)`

Deletes the entry the panel holds.

## `private void QXieshengPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Allows printing and exporting only while an entry is read.
It answers no before the panel is attached, since the commands are bound at construction.

## `private async void QXieshengPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the read entry on the printer the gate asks for.

## `private async void QXieshengPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the read entry as a portrait file.

## `internal void QXieshengVoyageShow(bool past, bool future)`

Enables the back and forward buttons of the rail.

## `private void QXieshengRetreatHandle(object sender, RoutedEventArgs e)`

Sails one station back.

## `private void QXieshengAdvanceHandle(object sender, RoutedEventArgs e)`

Sails one station forward.

## `private void QXieshengUndoHandle(object sender, RoutedEventArgs e)`

Undoes one editor change.

## `private void QXieshengRedoHandle(object sender, RoutedEventArgs e)`

Redoes one editor change.

## `private void QXieshengChronicleUpdate()`

Enables the undo and redo buttons as the editor's chronicle changes.
