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

### `private LCatalogOrder _pSequenceChoice = LCatalogOrder.LCatalogOrderHeadword;`

Which ordering the inventory is listed in, held as one of the orderings the engine supports.
The dropdown tag is read into that set, so the panel offers nothing the engine cannot do.
The ordering is the same one whatever language names it.

### `private void PInventoryFind(string query)`

The engine returns each matching entry already carrying the pronunciation stored for it.
An entry with no pronunciation sorts last under a pronunciation ordering.
It sorts first under the ordering that looks for what is still missing.
