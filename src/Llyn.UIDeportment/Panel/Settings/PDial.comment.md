# PDial.cs

## `private (string PDialChild, StackPanel PDialPage)[] PDialTableRead()`

Maps each settings page Conduct names to the part of the markup that shows it.
The page swap reads it, while the page list and the search keys belong to `CLedger`.

## `private void PDialRefine(string child)`

Brings the page of one group to the front and marks that group's row in the catalog.
The pages share one cell, so showing one means collapsing the others.
A page is found through the table, never by guessing a name from the child.
Marking the row here rather than in the click keeps the first page, shown on attach, marked as well.

## `private void PDialFolderObserve(object sender, RoutedEventArgs e)`

The folder button was pressed, so `CLedgerFolderOpen` opens the workspace folder in the shell.
It hands only the envoy, since the gate reads the folder and owns the failure notice.

## `private void PDialWidthRefine(object sender, RoutedEventArgs e)`

Drops every stored panel width and puts every tab back at the width its markup declared.
Orderings and hidden languages are untouched, because the button promises widths and nothing more.
One call clears the posture record, and `PLayout` hears it and resets the columns on screen.
So a tab reopened later starts from the design as well.
No confirmation is asked, because the widths are recovered by dragging again.
