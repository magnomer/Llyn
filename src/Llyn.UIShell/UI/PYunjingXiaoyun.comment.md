# PYunjingXiaoyun.cs

## `public partial class PYunjing`

The entry side of the yunjing panel: the entries at the chosen cell, the reader, the editor and their buttons.
It mirrors the tenor panel's entry side, so an entry is read, edited, made and deleted the same way here.

## `private void PXiaoyunFind()`

Lists the entries at the cell the chosen onset and rime name together, or nothing while neither is chosen.
The entry search narrows them by headword, and entries sharing a headword are numbered apart.
The empty text says to choose, that the cell is empty, or that nothing matches.

## `private void PXiaoyunSelect(long? id)`

Marks the entry row the reader stands on.

## `private void PXiaoyunHandle(object sender, RoutedEventArgs e)`

Reads the clicked entry, after an unsaved draft is settled.

## `private void PXiaoyunEntryShow(long id)`

Reads one entry into the reader, and into the editor too when it is the shown side.
An entry gone meanwhile clears the reader and reloads the columns.

## `private void PXiaoyunEntryUpdate(long id)`

An entry the editor just saved becomes the shown one, then the columns reload and the reader is read again.

## `private void PYunjingEntryShow(long id, LEntryDraft draft)`

Shows a draft in the reader and lets the mode switch be used.

## `private void PYunjingClear()`

Empties the reader and the editor, returns to the reading side, and disables what needs an entry.

## `private void PYunjingFreshHandle(object sender, RoutedEventArgs e)`

Opens an empty editor for a new entry, after an unsaved draft is settled.

## `private void PYunjingStoreHandle(object sender, RoutedEventArgs e)`

Saves the editor's draft.

## `private void PYunjingBinHandle(object sender, RoutedEventArgs e)`

Deletes the shown entry after the user confirms, then empties the panel.

## `private void PYunjingScribeHandle(object sender, RoutedEventArgs e)`

Switches between reading and editing the shown entry.
Leaving the editor with a change asks first, and a refusal keeps the editor up.

## `private void PYunjingScribeShow(bool editing)`

Shows one side, hides the other, sets the switch and remembers the split.

## `internal void PYunjingScribeRestore(bool editing)`

The side the last session kept, applied when the tab is restored, editing only when an entry is shown.

## `internal bool PYunjingLeaveConfirm()`

Whether the user may leave the editor: true at once without a change, else after the discard prompt.
