# PCorpusBrowse.cs

## `public partial class PCorpus`

Browsing behavior of the Corpus panel.
`PAnthology` lists every Example the workspace holds, including one nothing quotes.
A chosen row is read back and shown with everything quoting it, and `PCorpusScribe` swaps that reading for the editor.
The panel answers two questions rather than one: what this sentence is, and where it is quoted.

## Inline notes

### `private IReadOnlyDictionary<string, int> _pAnthologyCount = new Dictionary<string, int>();`

How many places quote each Example, read once per catalog fill rather than once per row.
It also decides which delete the panel offers, so it is held rather than asked for again.

## `private async void PCorpusHandle(object sender, DependencyPropertyChangedEventArgs e)`

Binds the sources and reads the workspace when the panel becomes visible.
The flags are loaded before the first fill, so no row is built without the flag it shows.

## `private IEnumerable<LExample> PAnthologySort(IReadOnlyList<LExample> examples)`

Orders the catalog under the current choice.
Ordering by Source orders by the resolved name rather than the id, because the id is never shown.

## `private void PAnthologyFind(string query)`

Refills the catalog from the workspace under the current ordering and query.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open editor keeps its selection either way, because the row may be the one being written.

## `private bool PQueryMatch(LExample example, string query)`

Whether one Example answers the query, over its sentence, its translation and its Source name.

## `internal void PAnthologyExampleShow(string id)`

Reads one Example back and shows it with the sides quoting it.
An id the store no longer knows clears the selection and refills the catalog rather than failing.
The window calls it so a Source citing row can land on the Example it names.

## `private void PExcerptValueShow(TextBlock field, LStateValue value, string? shown = null)`

Writes one three-state field into the display.
An unwritten value reads the unrecorded mark in the muted colour, so a blank row never stands for two facts.

## `private void PQuotationFind(string id)`

Reads everything quoting the Example, and names each row by the kind of side it is.
An Entry, a Meaning and a Collocation quote on their own terms, so the row says which.

## `private void PQuotationHandle(object sender, RoutedEventArgs e)`

Leaves for the Entry the chosen row belongs to.
The panel asks the window for the switch, because navigation is never driven by the pointer alone.

## `private void PCorpusScribeHandle(object sender, RoutedEventArgs e)`

Swaps the reading for the editor and back.
Leaving the editor asks first, so unsaved wording is never lost silently.

## `private void PCorpusClear()`

Drops the selection and empties the reading, the usage and the editor together.
`PCorpusScribe` is disabled with it, because there is nothing to edit while nothing is selected.
