# PRepertoireBrowse.cs

## `public partial class PRepertoire`

Browsing and editing behavior of the Repertoire panel.
`PAtlas` lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown with everything referencing it, and `PRepertoireScribe` swaps that reading for the editor.
The editor's own fields live in `PRepertoireEditor.cs`.
The panel answers two questions rather than one: what this Situation is, and where it is used.

## Inline notes

### `private async void PRepertoireBulletinHandle(LBulletin bulletin)`

The panel answers the engine rather than its own visibility.
A draft bulletin is the panel's own typing coming back, so it redraws the editor and reads nothing else.
An entry stored in another tab may reference a Situation, so the catalog and its reference figures are read again.
A workspace that moved is the one announcement that empties the panel first.
Its flags are reloaded before any row is built.

### `private IReadOnlyDictionary<long, int> _pAtlasCount = new Dictionary<long, int>();`

How many places reference each Situation, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

### `private long? _pVignetteSituation;`

The Situation the reading stands on, held as an id and never as a record.
Every showing reads it back, so nothing stale is drawn and no comparison is made against a cached copy.

## `private void PAtlasFind(string query)`

Refills the catalog with the rows the engine returns, already matched and already ordered.
What a title, a description or a kind answers is decided below the shell.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open editor keeps its selection either way, because the row may be the one being written.

## `internal void PAtlasSituationShow(long id)`

Opens one Situation for a caller outside the panel, which is how a chip elsewhere reaches this reading.
The guard belongs to the caller, because the panel is left before the tab is switched, not after.

## `private void PRepertoireShow(long id)`

Reads one Situation back and shows it with the sides referencing it.
A Situation that is gone leaves the panel unselected rather than showing a stale reading.
An open editor is restarted on the Situation shown, so the held draft and the reading never name different records.

## `private void PVignetteValueShow(TextBlock field, LStateValue value, string? shown = null)`

Draws one stored field as a row of the reading.
A written value reads itself, or the name resolved for it when the value is an id.
An unknown one reads the mark, and one never written reads the unrecorded text in the muted colour.
The row is never hidden, because a missing row and an empty field are different claims about the store.

## `private void POccurrenceFind(long id)`

Reads the referring sides of one Situation, itemized rather than counted.
An empty result is shown rather than hidden: a Situation nothing references is reachable only here.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.

## `private void PCitationFind()`

Reads the whole shelf of Sources the editor offers.
A Source is referenced, never owned, so nothing here creates or changes one.

### `internal void PTierRestore(LCatalogOrder order)`

Puts the panel back on the ordering the workspace stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PRepertoireScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `private void PAtlasSelect(long? id)`

Marks the catalog row the panel stands on and clears the mark from every other row.
A null id leaves no row marked, which is what a cleared panel shows.
It is called wherever the shown situation changes, so the mark and the right-hand side never disagree.
