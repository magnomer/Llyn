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

## `private void PAnthologyFind(string query)`

Refills the catalog with the rows the engine returns, already matched and already ordered.
The panel names the ordering and the query and decides nothing else about either.
A selection that survives the fill is kept, and one that no longer stands is dropped.
An open editor keeps its selection either way, because the row may be the one being written.

## `internal void PAnthologyExampleShow(string id)`

Reads one Example back and shows it with the sides quoting it.
An open editor is restarted on the Example chosen, so what it writes into is the draft for that sentence.
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
Entering the editor starts a draft on the selected Example, and leaving it discards that draft.
Leaving the editor asks first, so unsaved wording is never lost silently.

## `private void PCorpusClear()`

Drops the selection and empties the reading, the usage and the editor together.
The held draft goes first, because nothing may write into a draft the panel no longer stands on.
`PCorpusScribe` is disabled with it, because there is nothing to edit while nothing is selected.

### `internal void PRankRestore(LCatalogOrder order)`

Puts the panel back on the ordering the workspace stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PCorpusScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored, because an empty editor is the state a new record is written in.
