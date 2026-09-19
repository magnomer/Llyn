# PYunjing.xaml.cs

## `public partial class PYunjing : UserControl`

The yunjing panel: the workspace browsed as a rime table, by onset and rime of the user's reconstruction.
It is shown only while a loaded language pack carries rime books, since without them there is no table.
Every decision lives in [LYunjing](../../Llyn.UIDeportment/LYunjing.comment.md), and this file writes controls on notice.
The two columns, the entry list, the category page, the reader and the editor are all served from one file.

## `internal void PYunjingAttach(PWindow host, LEngine engine)`

News the deportment with the window's dialogs as seams, subscribes its notices, and wires the lists and the page.

## `internal bool PYunjingCheck()`

Whether any loaded language pack carries rime books, so the window knows to show the tab.

## `internal void PYunjingVistaRestore(LVista shengmu, LVista yunmu, LVista xiaoyun)`

Hands the vistas down, attaches the bulletins the panel follows, restores the order menus, and loads.

## `internal bool PYunjingLeaveConfirm()`

Asks the window whether an unsaved entry may be left, in the words every panel uses.

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
