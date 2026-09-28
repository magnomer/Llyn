# QRepertoireBrowse.cs

## `internal sealed partial class QRepertoire`

Browsing behavior of the Repertoire panel.
`QAtlas` lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown, and the middle column narrows to the entries referencing it.
`QRepertoireScribe` swaps the reading for the editor.
The editor's own fields live in `QRepertoireEditor.cs`.
The entry column lives in `QRepertoireEntry.cs`.
The reading side that draws the chosen Situation lives in `QRepertoireVignette.cs`.
The panel answers two questions rather than one: what this Situation is, and where it is used.

## Inline notes

### `private CRepertoire _cRepertoire = null!;`

The repertoire Conduct, holding the atlas, the occurrence list, the desk, the session and the panel's mode.
The atlas's vista carries the order, the inquest, and the languages hidden from the entry column.
The panel keeps no copy of any of them and asks the Conduct for each where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

### `private IReadOnlyDictionary<long, int> _qAtlasCount = new Dictionary<long, int>();`

How many places reference each Situation, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

## `private async void QRepertoireWorkspaceUpdate()`

A workspace that moved reloads the flags first, since they do not belong to the old folder's rows.
The panel is then emptied and listed again.

### `private void QInquestHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

### `private void QTierHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement refills the catalog.

### `internal async void QRepertoireVistaRestore()`

The Conduct starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Takes the vistas the window started for this tab and puts the panel on them.
The dropdown mark and the filter mark are drawn from the atlas first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in either box is handed to its vista, so a switched workspace keeps the search.
The catalog is then listed.

### `private void QRepertoireObserverAttach()`

Attaches each subject the panel cares about once, so no handler sorts announcements by subject.
A vista announcement refills the catalog, since order, filter or inquest moved.
A stored Situation refills the catalog and its reference figures.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each refills the catalog too.
An entry announcement goes to the occurrence panel first, which may adopt a freshly stored Entry.
The catalog follows, since the store may reference a Situation.
The catalog's row notice re-lists the entry column, so the column is read once per announcement.
The chosen entry's own announcement reaches the Conduct last, which redraws or drops it.
The occurrence vista carries the entry search box, so its announcement refills the entry column alone.
A fetched frequency, paradigm, script or fanqie row changes no situation or entry row, and is not attached.

### `private void QMeshRestore()`

Shows the filter mark while the vista hides any language.

### `private void QMeshBuild()`

Builds the language menu from the loaded packs, ticked as the vista's filter reads.
The restore and the dropped inquest share it.

### `private void QInquestClear()`

Empties the inquest box and rebuilds the language menu after an arrival dropped both on the vista.
The menu is read back from the vista's cleared filter, and the filter mark follows.

### `private void QSortieHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the entry search text to the occurrence vista, whose announcement refills the entry column.

### `private void QMeshHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu of the clicked box and handed to the vista.
The vista saves and announces them.
The mark on the button is redrawn from the vista at once.

## `private void QAtlasFind()`

Refills the catalog with the rows the engine returns for the vista, already matched and already ordered.
The panel hands over its vista and decides nothing about the ordering or the inquest.
Before a vista is handed over nothing is asked.
What a title, a description or a kind answers is decided below the shell.
The rows arrive as `CCatalogSituation` shapes, the kind carrying its own unknown mark.
The Conduct read drops a shown selection whose row no longer stands, and keeps one an open editor holds.
Its clear announces fresh rows that list the same Situations, so the nested refill is harmless.
Both tally chips are rewritten from the fresh counts, so a reference added elsewhere shows at once.
The entry column follows through the atlas's row notice, so it is not refilled here.

## `private void QAtlasHandle(object sender, RoutedEventArgs e)`

A clicked row asks the Conduct to select it, handing over the trail's record.
The Conduct asks about unsaved work first, and records the station only when the move goes ahead.

## `private void QAtlasApply(FrameworkElement container, object item, string? _)`

Fills one atlas row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The kind is written as it is, and the look sheet collapses it while empty.

## `private void QRepertoireBinHandle(object sender, RoutedEventArgs e)`

Hands the delete to the Conduct, which acts only while a Situation and not an Entry is shown.
A Situation something references is named to the user first, through the removal question the panel lent it.

## `private void QRepertoireViewerHandle(object sender, RoutedEventArgs e)`

Swaps the editor for the reading, on whichever side the Conduct stands.
Each segment has its own handler, so no control is read to decide which was pressed.

## `private void QRepertoireScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the editor, on whichever side the Conduct stands.
The Conduct asks before leaving an editor, so unsaved wording is never lost silently.

## `internal void QRepertoireVoyageShow(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void QRepertoireRetreatHandle(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QRepertoireAdvanceHandle(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.
