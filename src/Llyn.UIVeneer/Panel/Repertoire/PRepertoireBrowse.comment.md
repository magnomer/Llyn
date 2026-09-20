# PRepertoireBrowse.cs

## `public partial class PRepertoire`

Browsing and editing behavior of the Repertoire panel.
`PAtlas` lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown, and the middle column narrows to the entries referencing it.
`PRepertoireScribe` swaps the reading for the editor.
The editor's own fields live in `PRepertoireEditor.cs`.
The entry column and the entry display live in `PRepertoireEntry.cs`.
The reading side that draws the chosen Situation lives in `PRepertoireVignette.cs`.
The panel answers two questions rather than one: what this Situation is, and where it is used.

## Inline notes

### `private LRepertoire _lRepertoire = null!;`

The panel's deportment, holding the situation vista, the Occurrence vista and the desk the window restored.
The vista carries the order, the query, and the languages hidden from the entry column.
The panel keeps no copy of any of the three and asks the deportment for each where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

### `private void PScenarioDraftUpdate(LBulletin bulletin)`

A draft bulletin for the held draft is the panel's own typing coming back, so it redraws the editor.
Another draft is some other panel's and is left alone.

### `private void PScenarioTenureUpdate(LBulletin bulletin)`

A tenure bulletin for the held draft settles the rail's buttons and the editor's enabled state.

### `private async void PRepertoireWorkspaceUpdate()`

A workspace that moved empties the panel and reloads its flags before any row is built.

### `private IReadOnlyDictionary<long, int> _pAtlasCount = new Dictionary<long, int>();`

How many places reference each Situation, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

### `private void PInquestHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement refills the catalog.

### `private void PMeshHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

### `private void PTierHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement refills the catalog.

## `private void PAtlasFind()`

Refills the catalog with the rows the engine returns for the vista, already matched and already ordered.
Before a vista is handed over nothing is asked.
What a title, a description or a kind answers is decided below the shell.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open editor keeps its selection either way, because the row may be the one being written.

## `internal void PAtlasSituationShow(long id)`

Opens one Situation for a caller outside the panel, which is how a chip elsewhere reaches this reading.
The guard belongs to the caller, because the panel is left before the tab is switched, not after.

## `private void PRepertoireShow(long id)`

Reads one Situation back, shows it, and narrows the middle column to the entries referencing it.
A Situation that is gone leaves the panel unselected rather than showing a stale reading.
An open editor is restarted on the Situation shown, so the held draft and the reading never name different records.

## `private void PRepertoireBinHandle(object sender, RoutedEventArgs e)`

Deletes the shown Situation.
A Situation nothing references is deleted outright.
One something references is deleted only after the user is told how many places that reaches and says yes.
The detaching and the delete are one operation in the store, because between two steps the count can change.

### `internal async void PRepertoireVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Takes the vista the window started for this tab and puts the panel on it.
The panel answers the engine through the vistas rather than its own visibility.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A vista announcement refills the catalog, since order, filter or query moved.
The entry column is refilled with it, so a moved filter reaches it the same way.
A stored Situation, or an entry that may reference one, refills the catalog and its reference figures.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each refills too.
An entry announcement goes to the occurrence vista, whose chosen row is the entry it may name.
A fetched frequency, paradigm, script or fanqie row changes no situation or entry row, and is not attached.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The catalog is then listed from the vista.
The occurrence vista is kept for the entry column and handed to the display, which reads its chosen entry.
The occurrence vista carries the entry search box, so its announcement refills the entry column alone.

### `private void PMeshRestore()`

Shows the filter mark while the vista hides any language.

### `internal void PRepertoireScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `private void PAtlasChosenApply()`

Marks the row of the Situation the vista stands on and clears the mark from every other row.
No row is marked when the vista stands on none, which is what a cleared panel shows.
It walks the rows already listed, so choosing a Situation never re-reads the atlas.

## `internal long PRepertoireVoyageRead()`

The Situation the panel shows, as the station the window records before a jump away.
Zero says no Situation is shown, so there is no place to come back to.
