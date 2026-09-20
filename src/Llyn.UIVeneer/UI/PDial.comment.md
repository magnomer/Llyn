# PDial.cs

## `private (string PDialChild, StackPanel PDialPage, string[] PDialKeys)[] PDialTableRead()`

The one table of setting groups: the child name, the page that shows it, and the keys a search reads.
The ledger rows, the page swap and the search all read it, so a group is added in one place.

## `private void PDialShow(string child)`

Brings the page of one group to the front and marks that group's row in the catalog.
The pages share one cell, so showing one means collapsing the others.
A page is found through the table, never by guessing a name from the child.
Marking the row here rather than in the click keeps the first page, shown on attach, marked as well.

## `private void PDialFolderHandle(object sender, RoutedEventArgs e)`

Opens the workspace folder in the shell's file browser.
The path is handed to the window deportment as is, so the user's default browser for folders is what opens.
A folder the shell cannot open is reported as a notice, since the folder may have been moved away meanwhile.

## `private void PDialWidthHandle(object sender, RoutedEventArgs e)`

Drops every stored panel width and puts every tab back at the width its markup declared.
Orderings and hidden languages are untouched, because the button promises widths and nothing more.
The posture record is cleared first, so a tab reopened later starts from the design as well.
No confirmation is asked, because the widths are recovered by dragging again.
