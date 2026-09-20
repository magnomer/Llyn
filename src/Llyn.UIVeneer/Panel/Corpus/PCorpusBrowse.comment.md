# PCorpusBrowse.cs

## `public partial class PCorpus`

Browsing behavior of the Corpus panel.
`PAnthology` lists every Example the workspace holds, including one nothing quotes.
A chosen row is read back and shown, and the middle column narrows to the entries quoting it.
`PCorpusScribe` swaps the reading for the editor.
The panel answers two questions rather than one: what this sentence is, and where it is quoted.

## Inline notes


### `private IReadOnlyDictionary<long, int> _pAnthologyCount = new Dictionary<long, int>();`

How many places quote each Example, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

### `private LCorpus _lCorpus = null!;`

The panel's deportment, holding the example vista, the Quotation vista and the desk the window restored.
The vista carries the order, the query, and the languages hidden from the entry column.
The panel keeps no copy of any of the three and asks the deportment for each where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `private void PTranscriptDraftUpdate(LBulletin bulletin)`

A draft bulletin for the held draft is the panel's own typing coming back, so it redraws the editor.
Another draft is some other panel's and is left alone.

## `private void PTranscriptTenureUpdate(LBulletin bulletin)`

A tenure bulletin for the held draft says its state moved, so the rail's buttons are settled.

## `private async void PCorpusWorkspaceUpdate()`

A workspace that moved reloads the flags and the language menu first, since neither belongs to the old folder's rows.
The panel is then emptied and listed again.

### `private void PQueryHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

### `private void PRankHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement refills the catalog.

### `private void PGauzeHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

## `private void PAnthologyFind()`

Refills the catalog with the rows the engine returns for the vista, already matched and already ordered.
The panel hands over its vista and decides nothing about the ordering or the query.
Before a vista is handed over nothing is asked.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open editor keeps its selection either way, because the row may be the one being written.
Rows sharing a sentence are numbered afterwards, so the reader can tell them apart.
Both tally chips are rewritten from the fresh counts, so a quotation added elsewhere shows at once.

## `internal void PAnthologyExampleShow(long id)`

Reads one Example back and shows it with the sides quoting it.
An open editor is restarted on the Example chosen, so what it writes into is the draft for that sentence.
An id the store no longer knows clears the selection and refills the catalog rather than failing.
The window calls it so a Source citing row can land on the Example it names.

## `private void PCorpusBinHandle(object sender, RoutedEventArgs e)`

Deletes the Example the panel stands on, from the reading side or the editing side alike.
It answers only while an Example and not an Entry is shown, because an Entry is deleted from the library.
An Example nothing quotes is simply deleted.
One something quotes is named to the user first, and only then detached and deleted in one operation.

## `private string PCorpusTallyRead(long? id)`

The usage count as the tally chip reads it, a sentence rather than a bare number.
It reads the same count the delete confirmation reads, so the chip and the warning cannot disagree.

## `private void PCorpusStoreHandle(object sender, RoutedEventArgs e)`

The rail's save, standing for whichever editor is in front.
An Entry open in `PEditor` saves itself, and otherwise the held Example is committed.

## `private void PCorpusScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the editor and back.
Entering the editor starts a draft on the selected Example, and leaving it discards that draft.
Leaving the editor asks first, so unsaved wording is never lost silently.

## `if (editing == (PTranscript.Visibility == Visibility.Visible))`

Both segments answer here, so the click is read off which one was pressed.
The segment already standing for what is on screen changes nothing.

## `private void PCorpusClear()`

Drops the selection and empties the reading, the usage and the editor together.
The held draft goes first, because nothing may write into a draft the panel no longer stands on.
The mode toggle and the bin are disabled with it.
There is nothing to edit or delete while nothing is selected.

### `internal async void PCorpusVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Takes the vista the window started for this tab and puts the panel on it.
The panel re-reads the workspace through the vistas whenever the engine announces a change, wherever it was made.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A vista announcement refills the catalog, since order, filter or query moved.
The entry column is refilled with it, so a moved filter reaches it the same way.
A stored Example or Source refills the citations and the catalog, because a row cites a Source.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each refills the catalog too.
An entry announcement goes to the quotation vista, whose chosen row is the entry it may name.
A fetched frequency, paradigm, script or fanqie row changes no example or entry row, and is not attached.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The speakers, the citations and the catalog are then listed.
The quotation vista is kept for the entry column and handed to the display, which reads its chosen entry.
The quotation vista carries the entry search box, so its announcement refills the entry column alone.

### `private void PGauzeRestore()`

Shows the filter mark while the vista hides any language.

### `internal void PCorpusScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `private void PAnthologyChosenApply()`

Marks the row of the Example the vista stands on and clears the mark from every other row.
No row is marked when the vista stands on none, which is what a cleared panel shows.
It walks the rows already listed, so choosing an Example never re-reads the anthology.

## `internal long PCorpusVoyageRead()`

The Example the panel shows, as the station the window records before a jump away.
Zero says no Example is shown, so there is no place to come back to.

## `internal void PCorpusVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PCorpusRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PCorpusAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.
