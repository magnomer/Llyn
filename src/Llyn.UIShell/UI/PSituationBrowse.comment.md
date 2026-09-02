# PSituationBrowse.cs

## `public partial class PSituation`

Browsing and editing behavior of the Situations panel.
`PAtlas` lists every Situation the workspace holds, including one nothing references.
A chosen row is read back and shown with everything referencing it, and `PScribe` swaps that reading for the editor.
The panel answers two questions rather than one: what this Situation is, and where it is used.

## Inline notes

### `private IReadOnlyDictionary<string, int> _pAtlasUsage = new Dictionary<string, int>();`

How many places reference each Situation, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

### `private LSituation? _pEditorSituation;`

The Situation the editor stands on as the store knows it.
It stands empty while the editor is open over a Situation no card references yet and nothing has stored.
Comparing the written fields against it is what says whether anything is unsaved.

### `private bool _pEditorLoading;`

Set while fields are being filled from a stored Situation.
Filling a field raises the same change the user typing raises, and only the second may clear an unreadable mark.

## `private void PAtlasFind(string query)`

Refills the catalog from the workspace under the current ordering and query.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open editor keeps its selection either way, because the row may be the one being written.

## `private bool PInquestMatch(LSituation situation, string query)`

Whether one Situation answers the query, over its title, description, kind, and the resolved name of the Source it cites.

## `private void PSituationShow(string id)`

Reads one Situation back and shows it with the sides referencing it.
A Situation that is gone leaves the panel unselected rather than showing a stale reading.

## `private void PDisplayValueShow(TextBlock field, LStateValue value, string? shown = null)`

Draws one stored field as a row of the reading.
A written value reads itself, or the name resolved for it when the value is an id.
An unreadable one reads the mark, and one never written reads the unrecorded text in the muted colour.
The row is never hidden, because a missing row and an empty field are different claims about the store.

## `private void PUsageFind(string id)`

Reads the referring sides of one Situation, itemized rather than counted.
An empty result is shown rather than hidden: a Situation nothing references is reachable only here.

## `private void PCitationFind()`

Reads the whole shelf of Sources the editor offers.
A Source is referenced, never owned, so nothing here creates or changes one.

## `private LSituation PEditorRead()`

What the editor says the Situation is.
A field standing empty says nothing was recorded, unless it still carries the mark it was loaded with.
Then it says instead that something was recorded that cannot be read back.

## `private bool PEditorChangeCheck()`

Compares the written Situation against the stored one, ignoring the id.
A fresh Situation is compared against an empty one, so opening the editor and typing nothing is not a change.

## `private void PStoreHandle(object sender, RoutedEventArgs e)`

Stores the editor over the selected Situation, or creates one nothing references yet.
Saving does not change the id, what references it, or the order it takes for any referrer.

## `private void PRemovalHandle(object sender, RoutedEventArgs e)`

Deletes the shown Situation.
A Situation nothing references is deleted outright.
One something references is deleted only after the user is told how many places that reaches and says yes.
The detaching and the delete are one operation in the store, because between two steps the count can change.
