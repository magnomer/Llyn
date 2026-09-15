# PTenorCohort.cs

## `public partial class PTenor`

The entry list of the tenor panel: the Entries carrying the chosen Register, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.

## `private void PCohortSelect(long? id)`

Marks the entry row the reader stands on and clears the mark from every other row.
A null id leaves no row marked, which is what a cleared panel shows.

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

### `private void PCohortEntryUpdate(long id)`

A store is answered by re-reading the catalog rather than the entry list alone.
Storing may have marked a card with a Register nothing carried, or taken away the last card that carried one.

### `private void PCohortEntryCreate()`

Starts a fresh entry in the editor, marked with the chosen register on its first sense.
The clear leaves the register choice standing, so the list still shows that register's entries beside the fresh one.
An entry started with no register chosen comes up blank, since every entry is listed then.
