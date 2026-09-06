# PPhonologyBrowse.cs

## `public partial class PPhonology`

Browsing behavior of the phonology panel.
The search box and the ordering dropdown refill the pronunciation inventory.
A chosen inventory row is loaded back from the workspace and rendered read-only.
The mode toggle swaps that display for the editor, where the pronunciation is corrected.
This is the same round trip the library panel makes, entered through the pronunciation instead of the word.

## Inline notes

### `private string? _pDisplayEntry;`

The entry the right-hand side stands on, or null when none is selected.
The display may show it and the editor may be correcting it.

### `private LCatalogOrder _pSequenceChoice;`

Which ordering the inventory is listed in, held as one of the orderings the engine supports.
It starts as whatever the workspace stored, which the window applies before the panel is first shown.
The dropdown tag is read into that set, so the panel offers nothing the engine cannot do.
The ordering is the same one whatever language names it.

### `private void PInventoryFind(string query)`

The engine returns each matching entry already carrying the pronunciation stored for it.
An entry with no pronunciation sorts last under a pronunciation ordering.
It sorts first under the ordering that looks for what is still missing.

### `internal void PSequenceRestore(LCatalogOrder order)`

Puts the panel back on the ordering the workspace stored, and moves the dropdown mark onto it.
The window calls it once on attach, so the panel never reads the stored state for itself.

### `internal void PPhonologyScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored, because an empty editor is the state a new record is written in.
