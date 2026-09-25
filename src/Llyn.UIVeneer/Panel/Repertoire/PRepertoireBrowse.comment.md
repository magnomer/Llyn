# PRepertoireBrowse.cs

## `public partial class PRepertoire`

Browsing behavior of the Repertoire panel.
`PAtlas` lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown, and the middle column narrows to the entries referencing it.
`PRepertoireScribe` swaps the reading for the editor.
The editor's own fields live in `PRepertoireEditor.cs`.
The entry column lives in `PRepertoireEntry.cs`.
The reading side that draws the chosen Situation lives in `PRepertoireVignette.cs`.
The panel answers two questions rather than one: what this Situation is, and where it is used.

## Inline notes

### `private LRepertoire _lRepertoire = null!;`

The panel's deportment, holding the atlas, the occurrence and the desk the window restored.
The atlas's vista carries the order, the inquest, and the languages hidden from the entry column.
The panel keeps no copy of any of them and asks the deportment for each where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

### `private IReadOnlyDictionary<long, int> _pAtlasCount = new Dictionary<long, int>();`

How many places reference each Situation, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

## `private async void PRepertoireWorkspaceUpdate()`

A workspace that moved reloads the flags first, since they do not belong to the old folder's rows.
The panel is then emptied and listed again.

### `private void PInquestHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

### `private void PTierHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement refills the catalog.

### `internal async void PRepertoireVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Takes the vistas the window started for this tab and puts the panel on them.
The dropdown mark and the filter mark are drawn from the atlas first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in either box is handed to its vista, so a switched workspace keeps the search.
The catalog is then listed.

### `private void PRepertoireObserverAttach()`

Attaches each subject the panel cares about once, so no handler sorts announcements by subject.
A vista announcement refills the catalog, since order, filter or inquest moved.
A stored Situation refills the catalog and its reference figures.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each refills the catalog too.
An entry announcement goes to the occurrence panel first, which may adopt a freshly stored Entry.
The catalog follows, since the store may reference a Situation.
The catalog's row notice re-lists the entry column, so the column is read once per announcement.
The chosen entry's own announcement reaches the deportment last, which redraws or drops it.
The occurrence vista carries the entry search box, so its announcement refills the entry column alone.
A fetched frequency, paradigm, script or fanqie row changes no situation or entry row, and is not attached.

### `private void PMeshRestore()`

Shows the filter mark while the vista hides any language.

### `private void PMeshBuild()`

Builds the language menu from the loaded packs, ticked as the vista's filter reads.
The restore and the dropped inquest share it.

### `private void PInquestClear()`

Empties the inquest box and rebuilds the language menu after an arrival dropped both on the vista.
The menu is read back from the vista's cleared filter, and the filter mark follows.

### `private void PSortieHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the entry search text to the occurrence vista, whose announcement refills the entry column.

### `private void PMeshHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

## `private void PAtlasFind()`

Refills the catalog with the rows the engine returns for the vista, already matched and already ordered.
The panel hands over its vista and decides nothing about the ordering or the inquest.
Before a vista is handed over nothing is asked.
What a title, a description or a kind answers is decided below the shell.
The applied rows then go to the deportment, so a clear it makes never re-enters this fill.
It drops a shown selection whose row no longer stands, and keeps one an open editor holds.
Both tally chips are rewritten from the fresh counts, so a reference added elsewhere shows at once.
The entry column follows through the atlas's row notice, so it is not refilled here.

## `private void PAtlasHandle(object sender, RoutedEventArgs e)`

A clicked row asks the deportment to select it, handing over the trail's record.
The deportment asks about unsaved work first, and records the station only when the move goes ahead.

## `internal void PAtlasSituationShow(long id)`

A jump from another panel: opens the Situation without asking, since the window has already asked.
The deportment keeps the side the panel stood on, so an open editor restarts on the Situation.

## `private void PRepertoireBinHandle(object sender, RoutedEventArgs e)`

Hands the delete to the deportment, which acts only while a Situation and not an Entry is shown.
A Situation something references is named to the user first, through the removal question the panel lent it.

## `private void PRepertoireScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the editor and back, on whichever side the deportment stands.
Both segments answer here, so the click is read off which one was pressed.
The deportment asks before leaving an editor, so unsaved wording is never lost silently.

### `internal void PRepertoireScribeRestore(bool editing)`

Puts the Situation side back on the reader or the editor it was left standing on.
A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `internal bool PRepertoireLeaveConfirm()`

Asks the deportment whether the panel may be left, which asks the window only over unsaved work.

## `internal long PRepertoireVoyageRead()`

The Situation the atlas panel shows, as the station the window records before a jump away.
Zero says no Situation is shown, so there is no place to come back to.

## `internal void PRepertoireVoyageShow(bool past, bool future)`

Lights the two trail buttons from the stacks the window keeps.
The window owns the trail, so the panel only shows what it is told.

## `private void PRepertoireRetreatHandle(object sender, RoutedEventArgs e)`

Steps the window's trail back one station.

## `private void PRepertoireAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the window's trail forward one station.
