# PDuplexBrowse.cs

## `public partial class PDuplex`

Browsing behavior of the duplex panel.
A query refills the matches of its own side only.
A chosen match is loaded back from the workspace and rendered read-only on that side.
The panel never writes, so there is no editor and nothing to discard.

## Inline notes

### `private readonly ObservableCollection<PIndexItem> _pLeftIndex = [];`

The matches the left query found, held per side because the sides are peers.
Changing one side leaves the other where it was.

## `private void PDuplexIndexFind(string query, ObservableCollection<PIndexItem> catalog, ItemsControl index)`

Refills one side's matches from its query.
An empty query stands for no matches rather than for every entry.
The list hides itself when it has nothing to offer.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.

## `private bool PDuplexEntryShow(long id, PDisplay display)`

Loads one Entry back from the workspace onto the side that asked for it.
An Entry that is gone leaves that side empty rather than showing what it was.
It reports whether the side now stands on that Entry.
The caller stores an id only when something is shown.
A side that could not load stores nothing rather than an id pointing at what is gone.
