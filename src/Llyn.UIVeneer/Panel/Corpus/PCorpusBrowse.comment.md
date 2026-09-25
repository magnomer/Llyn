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

The panel's deportment, holding the anthology, the quotation and the desk the window restored.
The anthology's vista carries the order, the query, and the languages hidden from the entry column.
The panel keeps no copy of any of them and asks the deportment for each where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.
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
The applied rows then go to the deportment, so a clear it makes never re-enters this fill.
It drops a shown selection whose row no longer stands, and keeps one an open editor holds.
Both tally chips are rewritten from the fresh counts, so a quotation added elsewhere shows at once.
The entry column follows through the anthology's row notice, so it is not refilled here.

## `private void PAnthologyHandle(object sender, RoutedEventArgs e)`

A clicked row asks the deportment to select it, handing over the trail's record.
The deportment asks about unsaved work first, and records the station only when the move goes ahead.
## `internal void PAnthologyExampleShow(long id)`

A jump from another panel: opens the Example without asking, since the window has already asked.
The deportment keeps the side the panel stood on, so an open editor restarts on the Example.
## `private void PCorpusBinHandle(object sender, RoutedEventArgs e)`

Hands the delete to the deportment, which acts only while an Example and not an Entry is shown.
An Example something quotes is named to the user first, through the removal question the panel lent it.
## `private string PCorpusTallyRead(long? id)`

The usage count as the tally chip reads it, a sentence rather than a bare number.
It reads the same count the delete confirmation reads, so the chip and the warning cannot disagree.

## `private void PCorpusStoreHandle(object sender, RoutedEventArgs e)`

The rail's save, standing for whichever editor is in front.
The deportment saves an open Entry through the entry editor, and otherwise commits the held Example.
## `private void PCorpusScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the editor and back, on whichever side the deportment stands.
Both segments answer here, so the click is read off which one was pressed.
The deportment asks before leaving an editor, so unsaved wording is never lost silently.
### `internal async void PCorpusVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Takes the vistas the window started for this tab and puts the panel on them.
The dropdown mark and the filter mark are drawn from the anthology first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The speakers, the citations and the catalog are then listed.

### `private void PCorpusObserverAttach()`

Attaches each subject the panel cares about once, so no handler sorts announcements by subject.
A vista announcement refills the catalog, since order, filter or query moved.
A stored Example or Source refills the citations and the catalog, because a row cites a Source.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each refills the catalog too.
An entry announcement goes to the quotation panel first, which may adopt a freshly stored Entry.
The citations and the catalog follow, since the store may quote an Example.
The catalog's row notice re-lists the entry column, so the column is read once per announcement.
The chosen entry's own announcement reaches the deportment last, which redraws or drops it.
A fetched frequency, paradigm, script or fanqie row changes no example or entry row, and is not attached.
### `private void PGauzeRestore()`

Shows the filter mark while the vista hides any language.

### `private void PGauzeBuild()`

Builds the language menu from the loaded packs, ticked as the vista's filter reads.
The restore and the dropped search share it.

### `private void PQueryClear()`

Empties the search box and rebuilds the language menu after an arrival dropped both on the vista.
The menu is read back from the vista's cleared filter, and the filter mark follows.

### `internal void PCorpusScribeRestore(bool editing)`

Puts the Example side back on the reader or the editor it was left standing on.
A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `internal bool PCorpusLeaveConfirm()`

Asks the deportment whether the panel may be left, which asks the window only over unsaved work.
## `internal long PCorpusVoyageRead()`

The Example the anthology panel shows, as the station the window records before a jump away.
Zero says no Example is shown, so there is no place to come back to.

## `internal void PCorpusVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PCorpusRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PCorpusAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.
