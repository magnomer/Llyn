# PReferenceBrowse.cs

## `public partial class PReference`

Browsing behavior of the Source panel.
`PShelf` lists every Source the workspace holds, including one nothing cites.
A chosen row is read back and shown, and the middle column narrows to the entries citing it.
`PReferenceScribe` swaps that reading for the edit area.
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

## `private void PShelfFind(string query)`

Refills the shelf with the rows the engine returns, already matched and already ordered.
Each row carries its own name, credits and citation count, so the panel derives none of them.
The held counts and credits are read again with it.
The read area and the editor also stand on them.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open edit area keeps its selection either way, because the row may be the one being written.

## `internal int PShelfReachRead(long author)`

How many Sources one Author is credited on.
The count is answered here, because the credit map is held here and the edit area only asks.

## `internal void PReferenceShow(long id)`

Reads one Source back, shows it, and narrows the middle column to the entries citing it.
An id the store no longer knows clears the selection and refills the shelf rather than failing.
A selection made while the edit area is open starts held work on the Source shown.

## `private void PColophonValueShow(TextBlock field, LStateValue value)`

Writes one three-state field into the read area.
An unwritten value reads the unrecorded mark in the muted colour, so a blank line never stands for two facts.

## `private void PFootnoteFind()`

Refills the middle column with the entries citing the chosen Source, or every Entry while none is chosen.
A card cites by quoting an Example that names the Source.
So the row is the Entry the card belongs to.
The engine matches the typed text and drops the hidden languages, so the panel decides nothing about what matches.
An empty result is shown rather than hidden, and its text says whether nothing cites the Source or nothing matches.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.

### `private long? _pDisplayEntry;`

The Entry the right-hand side stands on, held as an id, while it shows an Entry and not a Source.
The right-hand side shows one or the other and never both.
An Entry row swaps the Source reading for the entry display in place, and a Source row swaps it back.
The chosen Source keeps its mark meanwhile, because it still narrows the middle column.

## `private void PFootnoteEntryShow(long id)`

Reads one Entry back and shows it in the panel's own display, without leaving the tab.
An open Source editor is cancelled first, after the caller has asked about its draft.
An editing side already open stays open, so the Entry lands in `PEditor` rather than in the display.
An Entry that is gone leaves the right-hand side on the Source and refills the middle column.

## `private void PFootnoteEntryUpdate(long id)`

A store announced by the engine redraws the shown Entry when the store touched it.
A store that deleted it falls back to the chosen Source, or clears the panel when none is chosen.

## `private void PFootnoteScribeHandle(bool editing)`

The mode toggle while an Entry is shown, mirroring the tenor panel.
Entering the editor starts a draft on the shown Entry, and leaving it asks first, then re-reads it.
`PReferenceScribeHandle` routes here whenever an Entry rather than a Source is shown.

## `private void PFootnoteScribeShow(bool editing)`

Swaps the entry display for the entry editor and back, and shows the rail save button with the editor.
The toggle marks follow, so the rail and the cell never disagree.

## `private void PReferenceStoreHandle(object sender, RoutedEventArgs e)`

Saves the Entry open in the editor, which the editor does for itself.

## `private void PFootnoteEntryHide()`

Puts the right-hand side back on the Source side, on the same reading or editing side it stood on.
An open entry editor is reset first, so no entry draft outlives the Entry it was opened on.
The mode toggle comes back only while a Source is chosen.

## `private void PReferenceScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the edit area and back.
Leaving the edit area asks first, so unsaved wording is never lost silently.
Held work is started on entering and discarded on leaving, so no draft outlives the area that fills it.


## `if (editing == (PImprint.Visibility == Visibility.Visible))`

Both segments answer here, so the click is read off which one was pressed.
The segment already standing for what is on screen changes nothing.
## `private void PReferenceFreshHandle(object sender, RoutedEventArgs e)`

Opens the edit area on a Source nothing has stored yet.
The held work is started before the controls are filled, so the first keystroke already has somewhere to go.

## `internal void PReferenceClear()`

Drops the selection, empties the reading and the edit area, and widens the entry list back to every Entry.
Held work is discarded first, because the panel is leaving behind everything the draft was opened on.
`PReferenceScribe` is disabled with it, because there is nothing to edit while nothing is selected.

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
