# PYunjingBrowse.cs

## `public partial class PYunjing`

The catalog side of the yunjing panel: the onset and rime columns, each with its own sorting and search.

## `private const string PYunjingFailure = "Yunjing.LoadFailed";`

The message shown when the categories or an entry cannot be read.

## `private async void PYunjingBulletinHandle(LBulletin bulletin)`

A moved workspace resets the panel, stored placements reload the columns, anything else refreshes the entry side.

## `private void PYunjingLoad()`

Reads both kinds of category for the panel's language and rebuilds the columns, then lists the cell.
A chosen row that the search has hidden is forgotten, so the cell never names a row nobody can see.
An open category page is read again, so stored placements reach it.

## `private static long? PYunjingListBuild(`

Rebuilds one column: the rows holding the search text, in the chosen order, the chosen one marked.
Name order sorts by key, reverse the other way, usage by entry count with the key as tie-break.
Returns the choice, or `null` when its row was filtered away.

## `private static void PYunjingEmptyShow(TextBlock label, int count, string? query, string vacant, string unmatched)`

The empty text under a column says nothing is there, or nothing matches when a search is typed.

## `private void PYunjingHandle(object sender, RoutedEventArgs e)`

A click chooses the row in its column, or clears the choice when the row was already chosen.
The column is told by which list holds the row.
A chosen onset opens its category page, and a cleared one hides it.

## `internal void PYunjingDiweiShow(string language, string kind, string key)`

Chooses one category by kind and key, as a fanqie link asks, and clears the other column's choice.
The column filters are emptied so the chosen row is listed.
An onset then opens its category page.
A key the store does not hold changes nothing.

## `private static void PYunjingSelect(ObservableCollection<PYunjingItem> list, long? chosen)`

Marks the chosen row of one column and unmarks the rest.

## `private void PPlumbHandle(object sender, TextChangedEventArgs e)`

Typing in the onset search rebuilds the columns.

## `private void PFathomHandle(object sender, TextChangedEventArgs e)`

Typing in the rime search rebuilds the columns.

## `private void PBeaconHandle(object sender, TextChangedEventArgs e)`

Typing in the entry search relists the cell.

## `private void PLadderHandle(object sender, RoutedEventArgs e)`

A pick in the onset ordering menu is stored and applied at once.

## `private void PStairHandle(object sender, RoutedEventArgs e)`

A pick in the rime ordering menu is stored and applied at once.

## `internal void PLadderRestore(LCatalogOrder order)`

The onset ordering the last session kept, applied before the panel is shown.

## `internal void PStairRestore(LCatalogOrder order)`

The rime ordering the last session kept, applied before the panel is shown.
