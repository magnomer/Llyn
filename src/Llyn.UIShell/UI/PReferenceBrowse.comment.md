# PReferenceBrowse.cs

## `public partial class PReference`

Browsing behavior of the Source panel.
`PShelf` lists every Source the workspace holds, including one nothing cites.
A chosen row is read back and shown, and the middle column narrows to the entries citing it.
`PReferenceScribe` swaps that reading for the edit area.
The entry column and the entry display live in `PReferenceEntry.cs`.
An uncited Source is reachable only through this panel, so it is never hidden.

## Inline notes

### `private IReadOnlyDictionary<string, int> _pShelfCount = new Dictionary<string, int>();`

How many places cite each Source, read once per shelf fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

### `private IReadOnlyDictionary<string, IReadOnlyList<LAuthor>> _pShelfCredit`

The credits of every Source, read whole rather than one Source at a time.
The catalog, the ordering, and the search all need them, so a per-row read would be one query per row.

## `private void PReferenceBulletinHandle(LBulletin bulletin)`

Re-reads the shelf and the authors whenever the engine announces a change, wherever it was made.
A draft bulletin is the edit area's own typing coming back.
It is handed to the area and nothing is re-read.
Every other bulletin is handed to the area as well, so it re-reads its author catalog beside the shelf.
The usage count beside a row and the credits under it are read with the shelf.
A stored Source updates its own row.
That is what the panel used to do by re-reading straight after its own commit.
A workspace that moved empties the panel before the same re-read runs.
An Entry stored while the editor holds a fresh one and shows nothing is that fresh one, and is adopted.
Only an Entry announcement is read that way, since a frequency or tag announcement carries another id.

## `private void PShelfFind(string query)`

Refills the shelf with the rows the engine returns, already matched and already ordered.
Each row carries its own name, credits and citation count, so the panel derives none of them.
The held counts and credits are read again with it.
The read area and the editor also stand on them.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open edit area keeps its selection either way, because the row may be the one being written.

## `internal void PReferenceShow(long id)`

Reads one Source back, shows it, and narrows the middle column to the entries citing it.
An id the store no longer knows clears the selection and refills the shelf rather than failing.
A selection made while the edit area is open starts held work on the Source shown.

## `private void PReferenceBinHandle(object sender, RoutedEventArgs e)`

Deletes the Source the panel stands on, from the reading side or the editing side alike.
It answers only while a Source and not an Entry is shown, because an Entry is deleted from the library.
An uncited Source is simply deleted.
A cited one is named to the user first, then detached and deleted in one store call.

## `internal string PReferenceTallyRead(long? id)`

The citation count as the tally chip reads it, a sentence rather than a bare number.
The edit area reads it through the panel, because the shelf's counts are what the panel holds.

## `private void PReferenceScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the edit area and back.
Leaving the edit area asks first, so unsaved wording is never lost silently.
Held work is started on entering and discarded on leaving, so no draft outlives the area that fills it.

## `if (editing == (PImprint.Visibility == Visibility.Visible))`

Both segments answer here, so the click is read off which one was pressed.
The segment already standing for what is on screen changes nothing.

## `internal void PReferenceClear()`

Drops the selection, empties the reading and the edit area, and widens the entry list back to every Entry.
Held work is discarded first, because the panel is leaving behind everything the draft was opened on.
The mode toggle and the bin are disabled with it.
There is nothing to edit or delete while nothing is selected.

### `internal void PTrellisRestore(LCatalogFilter filter)`

Puts the language filter back on the languages the settings stored, and builds the menu over the loaded languages.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PGradeRestore(LCatalogOrder order)`

Puts the panel back on the ordering the workspace stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PReferenceScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `private void PShelfSelect(string? id)`

Marks the catalog row the panel stands on and clears the mark from every other row.
A null id leaves no row marked, which is what a cleared panel shows.
It is called wherever the shown source changes, so the mark and the right-hand side never disagree.
