# PByline.cs

## `public partial class PImprint`

The dropdown of authors the workspace already holds that a typed credit may already name, and the rows it offers.
It is one popup the edit area owns rather than one per row.
Only one field is typed into at a time.
The popup is retargeted at the field that opened it.

A row carries the id of a stored Author, so choosing one credits that Author rather than its spelling.
Two Sources by the same person should credit one Author, and the dropdown is how the second finds the first.

The list opens as the name is typed rather than only when it is entered.
A user cannot pick from a catalogue they were never shown.

## `internal void PBylineHandle(object sender, MouseButtonEventArgs e)`

Takes the row the pointer chose out of the dropdown and credits that Author in the field's row.

## Inline notes

### `private bool PBylineHandle(Key key)`

The dropdown never takes focus, so the field's key handler drives it.
Nothing is selected while the list merely stands open, so enter still credits the typed name.
The user reaches the list with the arrows, and only then does enter take a row.
Down from nothing selects the first row and up selects the last.

### `if (name.Length == 0 || PAuthorCreditFind(author.LAuthorId) is not null)`

An Author the Source already credits is not offered again.
Crediting them twice would be refused, and offering them would only say so later.

### `PByline.PlacementTarget = PBylineFrameFind(box) ?? box;`

The popup hangs from the field's drawn surface, so it lines up with the border the user sees.

## `private static CustomPopupPlacement[] PBylinePlace(Size popup, Size target, Point offset)`

Below the field first, and above it when the screen leaves no room below.
The offsets pull the popup back by its own shadow margin.
So the surface and not the shadow meets the field.
