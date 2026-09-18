# PTenorCohort.cs

## `public partial class PTenor`

The entry list of the tenor panel: the Entries carrying the chosen Register, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.

## `private LVista? _pCohortVista;`

The child vista of the entry column, handed to the display so it reads the chosen entry from it.
It is the only holder of the shown entry, so every reader here asks it.

## `private void PCohortChosenApply()`

Marks the entry row the vista has chosen and clears the mark from every other row.
No chosen entry leaves no row marked, which is what a cleared panel shows.

## `private void PCohortHandle(object sender, RoutedEventArgs e)`

A clicked row is shown once the leave check has settled any unsaved draft.

## `private void PCohortEntryShow(long id)`

Loads the chosen Entry and hands it to the reader, and to the editor when that side is open.
An Entry the workspace no longer holds clears the panel and rebuilds the catalog without it.

## Inline notes

### `private void PCohortFind()`

The Entries are never refilled on their own.
They are refilled whenever the catalog is, because the chosen Register may have just changed or vanished.
A Register carrying no id stands for the whole workspace, which is what the engine reads an empty id as.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.

### `private void PCohortEntryUpdate(LBulletin bulletin)`

A store is answered by re-reading the catalog rather than the entry list alone.
Storing may have marked a card with a Register nothing carried, or taken away the last card that carried one.
An entry stored while the editor is open becomes the chosen row first, since the user did not leave it.

### `private void PTenorEntryUpdate()`

Reached only for the entry the panel stands on, or for a store that named no entry.
The entry is read back and shown again, or the panel clears when it is gone.

### `private void PCohortEntryCreate()`

Starts a fresh entry in the editor, marked with the chosen register on its first sense.
The clear leaves the register choice standing, so the list still shows that register's entries beside the fresh one.
An entry started with no register chosen comes up blank, since every entry is listed then.
