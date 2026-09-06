# PRepertoireBrowse.cs

## `public partial class PRepertoire`

Browsing and editing behavior of the Repertoire panel.
`PAtlas` lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown with everything referencing it, and `PRepertoireScribe` swaps that reading for the editor.
The panel answers two questions rather than one: what this Situation is, and where it is used.

## Inline notes

### `private IReadOnlyDictionary<string, int> _pAtlasCount = new Dictionary<string, int>();`

How many places reference each Situation, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

### `private string? _pVignetteSituation;`

The Situation the reading stands on, held as an id and never as a record.
Every showing reads it back, so nothing stale is drawn and no comparison is made against a cached copy.

### `private bool _pScenarioLoading;`

Set while fields are being filled from a stored Situation.
Filling a field raises the same change the user typing raises, and only the second may clear an unreadable mark.

## `private void PAtlasFind(string query)`

Refills the catalog with the rows the engine returns, already matched and already ordered.
What a title, a description or a kind answers is decided below the shell.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open editor keeps its selection either way, because the row may be the one being written.

## `private void PRepertoireShow(string id)`

Reads one Situation back and shows it with the sides referencing it.
A Situation that is gone leaves the panel unselected rather than showing a stale reading.
An open editor is restarted on the Situation shown, so the held draft and the reading never name different records.

## `private void PVignetteValueShow(TextBlock field, LStateValue value, string? shown = null)`

Draws one stored field as a row of the reading.
A written value reads itself, or the name resolved for it when the value is an id.
An unreadable one reads the mark, and one never written reads the unrecorded text in the muted colour.
The row is never hidden, because a missing row and an empty field are different claims about the store.

## `private void POccurrenceFind(string id)`

Reads the referring sides of one Situation, itemized rather than counted.
An empty result is shown rather than hidden: a Situation nothing references is reachable only here.

## `private void PCitationFind()`

Reads the whole shelf of Sources the editor offers.
A Source is referenced, never owned, so nothing here creates or changes one.

## `private LSituation PScenarioRead(LSituation held)`

What the editor says the Situation is, written onto the Situation the engine holds.
Identity comes from the held draft rather than from the panel, because the panel mints nothing.
A field standing empty says nothing was recorded, unless it still carries the mark it was loaded with.
Then it says instead that something was recorded that cannot be read back.

## `private void PScenarioStoreHandle(object sender, RoutedEventArgs e)`

Commits the held Situation, which stores it over the selected one or creates one nothing references yet.
Typing still waiting to be written is written first, so the commit carries the last keystroke.
Whether this is a create or a rewrite is the engine's reading of the draft, not the panel's.
Saving does not change the id, what references it, or the order it takes for any referrer.

## `private void PScenarioRemovalHandle(object sender, RoutedEventArgs e)`

Deletes the shown Situation.
A Situation nothing references is deleted outright.
One something references is deleted only after the user is told how many places that reaches and says yes.
The detaching and the delete are one operation in the store, because between two steps the count can change.

### `internal void PTierRestore(LCatalogOrder order)`

Puts the panel back on the ordering the workspace stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PRepertoireScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored, because an empty editor is the state a new record is written in.
