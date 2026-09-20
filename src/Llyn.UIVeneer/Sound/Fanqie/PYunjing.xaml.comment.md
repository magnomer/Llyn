# PYunjing.xaml.cs

## `public partial class PYunjing : UserControl`

The yunjing panel: the workspace browsed as a rime table, by onset and rime of the user's reconstruction.
It is shown only while a loaded language pack carries rime books, since without them there is no table.
Every decision lives in [LYunjing](../../Llyn.UIDeportment/LYunjing.comment.md), and this file writes controls on notice.
The two columns, the entry list, the category page, the reader and the editor are all served from one file.

## `internal void PYunjingAttach(PWindow host)`

News the deportment with the window's dialogs as seams, subscribes its notices, and wires the lists and the page.
The print and portrait command bindings are added last, so no can-execute query ever meets a deportment not yet built.

## `internal bool PYunjingCheck()`

Whether any loaded language pack carries rime books, so the window knows to show the tab.

## `internal void PYunjingVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Hands the vistas down, attaches the bulletins the panel follows, restores the order menus, and loads.

## `internal bool PYunjingLeaveConfirm()`

Asks the panel whether the tab may be left, so an untouched entry is left without a word.
The panel asks the window only once its editor holds a change.

## `private bool PYunjingDiscardConfirm()`

The seam the panel calls once it finds a change: the window's leave dialog, in wording every panel shares.

## `internal void PYunjingDiweiShow(string language, string kind, string key)`

A glyph link from another panel: empties both search fields, then opens the page on the cell.

## `private void PYunjingColumnUpdate()`

Lists both columns afresh with their empty texts, then the page and the mode, which follow the chosen cell.

## `private void PXiaoyunUpdate()`

Lists the entries at the chosen cell afresh with the empty text the deportment names.

## `private void PDiweiUpdate()`

Hands the page its composed content, blank while it is hidden.

## `private void PYunjingModeUpdate()`

Writes every visibility and enablement off the deportment's verdicts.

## `private void PYunjingClearUpdate()`

The entry list was cleared: the reader empties, the editor resets, and the page is read again.

## `private void PYunjingHandle(object sender, RoutedEventArgs e)`

A click on either column hands the cell's id and side to the deportment.

## `internal void PYunjingVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PYunjingRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PYunjingAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.

## `private void PYunjingUndoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor back one step.

## `private void PYunjingRedoHandle(object sender, RoutedEventArgs e)`

Walks the chronicle of the editor forward one step.

## `private void PYunjingChronicleUpdate()`

Lights the two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `internal long PYunjingVoyageRead()`

The Entry the panel shows, read off the yunjing panel as the station of this panel.

## `internal void PXiaoyunEntryShow(long id)`

Shows one Entry by id, the same selection a click on its xiaoyun row makes.
The window's trail walks back into this panel through it.
