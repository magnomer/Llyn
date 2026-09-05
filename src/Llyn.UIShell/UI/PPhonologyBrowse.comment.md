# PPhonologyBrowse.cs

## `public partial class PPhonology`

Browsing behavior of the phonology panel.
The search box and the ordering menu refill the pronunciation inventory.
A chosen inventory row is loaded back from the workspace and rendered read-only.
The mode toggle swaps that display for the editor, where the pronunciation is corrected.
This is the same round trip the library panel makes, entered through the pronunciation instead of the word.

## Inline notes

### `private string? _pDisplayEntry;`

The entry the right-hand side stands on, or null when none is selected.
The display may show it and the editor may be correcting it.

### `private string _pSequenceChoice = "Headword";`

Which ordering the inventory is listed in.
It is the tag the chosen menu row carries, not the words that row showed.
The ordering is the same one whatever language names it.

### `private IEnumerable<PInventoryItem> PSequenceSort(...)`

The orderings are read off the items rather than off the entries.
A pronunciation ordering needs the pronunciation, and the entry row does not carry one.
An entry with no pronunciation sorts last under a pronunciation ordering.
It sorts first under the ordering that looks for what is still missing.

### `private void PInventoryFind(string query)`

Every matching entry is asked for its pronunciation, because the row shows it.
The list is built whole and then ordered, so the ordering may read what the rows show.
