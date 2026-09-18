# PYunjingBrowse.cs

## `public partial class PYunjing`

The catalog side of the yunjing panel: the onset and rime columns, each with its own sorting and search.

## `private const string PYunjingFailure = "Yunjing.LoadFailed";`

The message shown when the categories or an entry cannot be read.

## `private LVista? _pShengmuVista;`

The engine's view state for the onset column: its ordering, its search text and the chosen onset.
The column keeps no copy of any of the three and reads each from the vista where it needs it.
It is saved under the `yunjing` tab, the panel's own record.
It is null until the window hands one over, so the handlers do nothing before that.

## `private LVista? _pYunmuVista;`

The engine's view state for the rime column, saved under the `yunmu` tab since one record holds one ordering.
It is null until the window hands one over, so the handlers do nothing before that.

## `private async void PYunjingWorkspaceUpdate()`

A moved workspace reloads the flags and resets the panel.

## `private void PYunjingLoad()`

Reads both kinds of category for the panel's language as each column's vista lists them, then rebuilds the columns.
The rows arrive from the engine already narrowed by the search and in the chosen order.
A chosen row the search has hidden is let go of through its vista, and the column is built unmarked.
So the cell never names a row nobody can see.
The cell is then listed and an open category page is read again, so stored placements reach it.
Before both vistas are handed over nothing is asked.

## `private static void PYunjingListBuild(`

Rebuilds one column from the rows the engine returned, the chosen one marked.

## `private static void PYunjingEmptyShow(TextBlock label, int count, string? query, string vacant, string unmatched)`

The empty text under a column says nothing is there, or nothing matches when a search is typed.

## `private void PYunjingHandle(object sender, RoutedEventArgs e)`

A click chooses the row through its column's vista, or clears a choice already standing.
The column is told by which list holds the row.
Its rows are re-marked in place and the cell relisted, so no column is rebuilt for a click.
A chosen onset or rime opens its category page, and a cleared one hides it.
The page follows the last click, so choosing a rime after an onset shows the rime.

## `internal void PYunjingDiweiShow(string language, string kind, string key)`

Chooses one category by kind and key, as a fanqie link asks, and clears the other column's choice.
The column searches are emptied first so the chosen row is listed.
Each choice goes through its vista, and the columns are loaded once more for a language that changed.
The category then opens its page.
A key the store does not hold changes nothing.

## `private void PPlumbHandle(object sender, TextChangedEventArgs e)`

Typing in the onset search hands the text to the onset vista, whose announcement rebuilds the columns.

## `private void PFathomHandle(object sender, TextChangedEventArgs e)`

Typing in the rime search hands the text to the rime vista, whose announcement rebuilds the columns.

## `private void PBeaconHandle(object sender, TextChangedEventArgs e)`

Typing in the entry search relists the cell.

## `private void PLadderHandle(object sender, RoutedEventArgs e)`

A pick in the onset ordering menu closes the menu and hands the ordering to the onset vista.
The vista saves it and announces it, and the announcement rebuilds the columns.

## `private void PStairHandle(object sender, RoutedEventArgs e)`

A pick in the rime ordering menu closes the menu and hands the ordering to the rime vista.

## `internal void PYunjingVistaRestore(LVista shengmu, LVista yunmu, LVista xiaoyun)`

Takes the two vistas the window started for the two columns and puts the panel on them.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A vista announcement from either column rebuilds the columns, since an order or a search moved.
Each column's vista forwards its own announcement only, so no other panel's vista is seen.
The subjects shared by the panel are attached to the onset vista alone, so none is answered twice.
Stored placements reload the columns, and a reflex fill rewrites an epithet, so it reloads them too.
A settings change fills the category page again, since the respelling switch decides whether its own shows.
Every settings switch raises that bulletin, so a language or epithet change fills the page again as well.
An entry announcement goes to the xiaoyun vista, whose chosen row is the entry it may name.
A draft edit or a fetched frequency, paradigm or script row changes no listed row, and is not attached.
Both dropdown marks are drawn from them first.
Search text still standing in either box is handed to its vista, so a switched workspace keeps the search.
The columns are then loaded once for both.
The xiaoyun vista is kept for the entry column and handed to the display, which reads its chosen entry.

## `private void PLadderRestore()`

Moves the onset dropdown mark onto the ordering the onset vista holds.

## `private void PStairRestore()`

Moves the rime dropdown mark onto the ordering the rime vista holds.
