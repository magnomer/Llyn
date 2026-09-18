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

### `private LVista? _pReferenceVista;`

The engine's view state for the reference tab: order, query, and the languages hidden from the entry column.
The panel keeps no copy of any of the three and reads each from the vista where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `private void PReferenceWorkspaceUpdate()`

A workspace that moved empties the panel, then the shelf and the edit area's author catalog are re-read.

## `private void PShelfReferenceUpdate()`

A stored Source updates its row and the credits under it, so the author catalog is read before the shelf.
That is what the panel used to do by re-reading straight after its own commit.
Only an Entry announcement is read that way, since a frequency or tag announcement carries another id.

### `private void PSurveyHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the shelf.

### `private void PTrellisHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

### `private void PGradeHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement refills the shelf.

## `private void PShelfFind()`

Refills the shelf with the rows the engine returns for the vista, already matched and already ordered.
Before a vista is handed over nothing is asked.
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

### `internal async void PReferenceVistaRestore(LVista vista, LVista footnote)`

Takes the vista the window started for this tab and puts the panel on it.
The panel re-reads the shelf and the authors through the vistas whenever the engine announces a change.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A vista announcement refills the shelf, since order, filter or query moved.
The entry column is refilled with it, so a moved filter reaches it the same way.
The edit area's draft and tenure bulletins are attached for it, since the area holds no vista of its own.
An author bulletin redraws the area's credits and refills the shelf, whose rows carry the credits.
A stored Example or entry may move the usage count beside a row, so each refills the shelf.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each refills too.
An entry announcement goes to the footnote vista, whose chosen row is the entry it may name.
A fetched frequency, paradigm, script or fanqie row changes no source or entry row, and is not attached.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The edit area's author catalog and the shelf are then listed.
The footnote vista is kept for the entry column and handed to the display, which reads its chosen entry.

### `private void PGradeRestore()`

Moves the dropdown mark onto the ordering the vista holds.

### `private void PTrellisRestore()`

Shows the filter mark while the vista hides any language.

### `internal void PReferenceScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `private void PShelfChosenApply()`

Marks the row of the Source the vista stands on and clears the mark from every other row.
No row is marked when the vista stands on none, which is what a cleared panel shows.
It walks the rows already listed, so choosing a Source never re-reads the shelf.
