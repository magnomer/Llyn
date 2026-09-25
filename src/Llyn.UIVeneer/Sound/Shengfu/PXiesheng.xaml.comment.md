# PXiesheng.xaml.cs

## `public partial class PXiesheng : UserControl`

The xiesheng panel: the workspace browsed by the phonetic series its characters belong to.
It is shown only while a loaded language pack declares a series source, since without one there is no series.
Every decision lives in [LXiesheng](../../Llyn.UIDeportment/LXiesheng.comment.md), and this file writes controls on notice.
The series column, the entry list, the series page, the reader and the editor are served from one file.

## `internal void PXieshengAttach(PWindow host)`

Builds the deportment, subscribes the notices, and attaches the reader, the page and the editor.

## `private void PXieshengStoreUpdate()`

Enables the save button while the editor holds something storable.

## `internal bool PXieshengCheck()`

True while the tab may be shown at all, so the navigation can hide its button.

## `internal void PXieshengVistaRestore()`

Starts the vistas, attaches the observers, and builds the ordering menu of the series column.

## `private async void PXieshengWorkspaceUpdate()`

Reloads the flags and resets the panel, as the workspace changes under it.

## `internal bool PXieshengDraftFinish(bool store)`

Closes the held draft, saving it or dropping it, as the shell asks while leaving.

## `internal bool PXieshengChangeCheck()`

True while the panel holds an unsaved change.

## `internal bool PXieshengLeaveConfirm()`

Asks the user about an unsaved change before the panel is left.

## `private bool PXieshengDiscardConfirm()`

The seam the deportment discards a draft through.

## `internal void PXieshengStemShow(string language, string key)`

Opens that series, clearing the column's query first so the series can be listed.

## `internal void PXieshengScribeRestore(bool editing)`

Restores the editing side the posture was saved in.

## `internal void PXieshengClose()`

Closes the editor and the reader, as the window exits.

## `private bool PXieshengShownCheck()`

True while the panel is the visible tab.

## `private void PXieshengColumnUpdate()`

Copies the series column, its empty line, the series page and the mode again.

## `private void PKindredUpdate()`

Copies the entry list and its empty line again.

## `private void PStemUpdate()`

Writes the page of the chosen series onto the page control.

## `private void PXieshengModeUpdate()`

Shows the reader, the page or the editor, and enables the mode and bin buttons.

## `private void PXieshengClearUpdate()`

Clears the reader and writes the page again, as the panel is cleared.

## `private void PXieshengEntryUpdate(LDraft draft)`

Shows the held draft in the reader.

## `private void PLodestarHandle(object sender, TextChangedEventArgs e)`

Narrows the series column as the field is typed into.

## `private void PSextantHandle(object sender, TextChangedEventArgs e)`

Narrows the entry list as the field is typed into.

## `private void PRungHandle(object sender, RoutedEventArgs e)`

Closes the dropper and lists the series column in the ordering picked.

## `private void PGroveHandle(object sender, RoutedEventArgs e)`

Chooses the series of the pressed row.

## `private void PKindredHandle(object sender, RoutedEventArgs e)`

Records the voyage station and opens the entry of the pressed row.

## `internal long PXieshengVoyageRead()`

The entry the panel would return to, as the shell records a station.

## `internal void PKindredEntryShow(long id)`

Opens that entry in the panel, for a jump the window makes from another panel.
It asks nothing, because the window asks before it jumps.

## `private void PXieshengFreshHandle(object sender, RoutedEventArgs e)`

Starts a fresh entry in the editor.

## `private void PXieshengScribeHandle(object sender, RoutedEventArgs e)`

Switches between reading and editing.

## `private void PXieshengStoreHandle(object sender, RoutedEventArgs e)`

Saves what the editor holds.

## `private void PXieshengBinHandle(object sender, RoutedEventArgs e)`

Deletes the entry the panel holds.

## `private void PXieshengPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Allows printing and exporting only while an entry is read.

## `private async void PXieshengPressHandle(object sender, ExecutedRoutedEventArgs e)`

Prints the read entry through the shell's press.

## `private async void PXieshengPortraitHandle(object sender, ExecutedRoutedEventArgs e)`

Exports the read entry as a portrait file.

## `internal void PXieshengVoyageShow(bool past, bool future)`

Enables the back and forward buttons of the rail.

## `private void PXieshengRetreatHandle(object sender, RoutedEventArgs e)`

Sails one station back.

## `private void PXieshengAdvanceHandle(object sender, RoutedEventArgs e)`

Sails one station forward.

## `private void PXieshengUndoHandle(object sender, RoutedEventArgs e)`

Undoes one editor change.

## `private void PXieshengRedoHandle(object sender, RoutedEventArgs e)`

Redoes one editor change.

## `private void PXieshengChronicleUpdate()`

Enables the undo and redo buttons as the editor's chronicle changes.
