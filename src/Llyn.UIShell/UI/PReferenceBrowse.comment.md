# PReferenceBrowse.cs

## `public partial class PReference`

Browsing behavior of the Source panel.
`PShelf` lists every Source the workspace holds, including one nothing cites.
A chosen row is read back and shown with everything citing it.
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

## `internal int PShelfReachRead(string author)`

How many Sources one Author is credited on.
The count is answered here, because the credit map is held here and the edit area only asks.

## `internal bool PShelfCreditUpdate()`

Reads the credit map again after the edit area writes a credit, and refills the shelf.
The shelf shows credits too, so one write is one refill for both.
It answers false on a failure, so the caller stops rather than refilling from a stale map.

## `internal void PReferenceShow(string id)`

Reads one Source back and shows it with the sides citing it.
An id the store no longer knows clears the selection and refills the shelf rather than failing.
A selection made while the edit area is open starts held work on the Source shown.

## `private void PColophonValueShow(TextBlock field, LStateValue value)`

Writes one three-state field into the read area.
An unwritten value reads the unrecorded mark in the muted colour, so a blank line never stands for two facts.

## `private void PFootnoteFind(string id)`

Reads everything citing the Source, and names each row by the kind of side it is.
A Meaning, a Collocation and an Example cite on their own terms, so the row says which.
A card cites by holding the Example, so its row leads to the Entry the card belongs to.

## `private void PFootnoteHandle(object sender, RoutedEventArgs e)`

Leaves for the panel the chosen row belongs to.
The panel asks the window for the switch, because navigation is never driven by the pointer alone.

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

Drops the selection and empties the reading, the citing list and the edit area together.
Held work is discarded first, because the panel is leaving behind everything the draft was opened on.
`PReferenceScribe` is disabled with it, because there is nothing to edit while nothing is selected.

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
