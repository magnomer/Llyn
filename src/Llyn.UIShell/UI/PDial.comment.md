# PDial.cs

## `private void PDialShow(string child)`

Brings the card of one group to the front and marks that group's row in the catalog.
The five cards share one cell, so showing one means collapsing the other four.
A card is found by its name, which is the child name behind the `PDial` prefix.
Marking the row here rather than in the click keeps the first card, shown on attach, marked as well.

## `private void PDialFolderHandle(object sender, RoutedEventArgs e)`

Opens the workspace folder in the shell's file browser.
The path is handed to the shell as is, so the user's default browser for folders is what opens.

## `private void PDialWidthHandle(object sender, RoutedEventArgs e)`

Drops every stored panel width and puts every tab back at the width its markup declared.
Orderings and hidden languages are untouched, because the button promises widths and nothing more.
The engine record is cleared first, so a tab reopened later starts from the design as well.
No confirmation is asked, because the widths are recovered by dragging again.
