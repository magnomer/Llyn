# PRepertoireBrowse.cs

## `public partial class PRepertoire`

Browsing and editing behavior of the Repertoire panel.
`PAtlas` lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown, and the middle column narrows to the entries referencing it.
`PRepertoireScribe` swaps the reading for the editor.
The editor's own fields live in `PRepertoireEditor.cs`.
The entry column and the entry display live in `PRepertoireEntry.cs`.
The panel answers two questions rather than one: what this Situation is, and where it is used.

## Inline notes

### `private async void PRepertoireBulletinHandle(LBulletin bulletin)`

The panel answers the engine rather than its own visibility.
A draft bulletin is the panel's own typing coming back, so it redraws the editor and reads nothing else.
A fetched frequency, paradigm, script, fanqie or reflex row changes no situation or entry row, so those read nothing.
An entry stored in another tab may reference a Situation, so the catalog and its reference figures are read again.
A workspace that moved is the one announcement that empties the panel first.
Its flags are reloaded before any row is built.
An Entry stored while the editor holds a fresh one and shows nothing is that fresh one, and is adopted.
Only an Entry announcement is read that way, since a frequency or tag announcement carries another id.

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

Reads one Situation back, shows it, and narrows the middle column to the entries referencing it.
A Situation that is gone leaves the panel unselected rather than showing a stale reading.
An open editor is restarted on the Situation shown, so the held draft and the reading never name different records.

## `private void PRepertoireBinHandle(object sender, RoutedEventArgs e)`

Deletes the shown Situation.
A Situation nothing references is deleted outright.
One something references is deleted only after the user is told how many places that reaches and says yes.
The detaching and the delete are one operation in the store, because between two steps the count can change.

## `private string? PVignetteTextRead(LStateValue value)`

What one stored field reads as, or null for one never written.
A written value reads itself, and an unknown one reads the mark.
A card field does the same in the entry display.

## `private void PVignetteTitleShow(LStateValue value)`

Draws the title in the headword's place.
A never-written title reads the untitled text in the muted colour, because the head of the page cannot stand empty.

## `private void PVignetteKindShow(LStateValue value)`

Draws the kind in its chip, or hides the chip while no kind was ever written.
An entry with no speech draws no speech chip, and the kind follows that.

## `private void PVignetteDescriptionShow(LStateValue value)`

Renders the description from Markdown into its card, or hides the section while none was ever written.
The entry note is rendered the same way, and the same renderer keeps the two alike.

## `private void PVignetteMediaShow(LSituation? situation)`

Hands the Situation to the two media lists, which draw its pictures and videos through the card's own converters.
Each list hides itself while its side of the Situation is empty, so the page carries no empty media block.
A null Situation empties both, which also stops any video still playing.

## `private string PRepertoireTallyRead(long? id)`

How many places reference one Situation, read from the count the catalog fill already holds, worded as a sentence.
None, one and many are three texts, because a number alone beside a title says nothing about what it counts.
It is drawn on both sides of the panel, so the reader and the writer see the same figure.

### `internal void PMeshRestore(LCatalogFilter filter)`

Puts the language filter back on the languages the settings stored, and builds the menu over the loaded languages.
The window calls it once on attach, so the panel never reads the stored state for itself.

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
