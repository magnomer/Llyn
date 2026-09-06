# PReferenceBrowse.cs

## `public partial class PReference`

Browsing behavior of the Source panel.
`PShelf` lists every Source the workspace holds, including one nothing cites.
A chosen row is read back and shown with everything citing it, and `PReferenceScribe` swaps that reading for the edit area.
An uncited Source is reachable only through this panel, so it is never hidden.

## Inline notes

### `private IReadOnlyDictionary<string, int> _pShelfCount = new Dictionary<string, int>();`

How many places cite each Source, read once per shelf fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

### `private IReadOnlyDictionary<string, IReadOnlyList<LAuthor>> _pShelfCredit`

The credits of every Source, read whole rather than one Source at a time.
The catalog, the ordering, and the search all need them, so a per-row read would be one query per row.

## `private void PReferenceHandle(object sender, DependencyPropertyChangedEventArgs e)`

Binds the collections and reads the workspace when the panel becomes visible.
The authors are read before the first fill, so no menu is offered empty while the store holds some.

## `private IEnumerable<LReference> PShelfSort(IReadOnlyList<LReference> references)`

Orders the shelf under the current choice.
Ordering by name uses the resolved name, so a Source naming itself by its programme is ordered under what is shown.
Ordering by author uses the first credit, because the credit order belongs to the Source.

## `private void PShelfFind(string query)`

Refills the shelf from the workspace under the current ordering and query.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open edit area keeps its selection either way, because the row may be the one being written.

## `private static bool PSurveyMatch(LReference reference, IReadOnlyList<LAuthor> credits, string query)`

Whether one Source answers the query, over its four texts and its credited names.
Only a field standing Specified carries text, so an Unknown field matches nothing.

## `private void PReferenceShow(string id)`

Reads one Source back and shows it with the sides citing it.
An id the store no longer knows clears the selection and refills the shelf rather than failing.
A selection made while the edit area is open starts held work on the Source shown.

## `private void PColophonValueShow(TextBlock field, LStateValue value)`

Writes one three-state field into the read area.
An unwritten value reads the unrecorded mark in the muted colour, so a blank line never stands for two facts.

## `private void PFootnoteFind(string id)`

Reads everything citing the Source, and names each row by the kind of side it is.
An Entry and an Example cite on their own terms, so the row says which.

## `private void PFootnoteHandle(object sender, RoutedEventArgs e)`

Leaves for the panel the chosen row belongs to.
The panel asks the window for the switch, because navigation is never driven by the pointer alone.

## `private void PReferenceScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the edit area and back.
Leaving the edit area asks first, so unsaved wording is never lost silently.
Held work is started on entering and discarded on leaving, so no draft outlives the area that fills it.

## `private void PReferenceClear()`

Drops the selection and empties the reading, the citing list and the edit area together.
Held work is discarded first, because the panel is leaving behind everything the draft was opened on.
`PReferenceScribe` is disabled with it, because there is nothing to edit while nothing is selected.
