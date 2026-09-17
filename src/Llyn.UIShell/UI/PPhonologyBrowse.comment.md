# PPhonologyBrowse.cs

## `public partial class PPhonology`

Browsing behavior of the phonology panel.
The search box and the ordering dropdown refill the pronunciation inventory.
A chosen inventory row is loaded back from the workspace and rendered read-only.
The mode toggle swaps that display for the editor, where the pronunciation is corrected.
This is the same round trip the library panel makes, entered through the pronunciation instead of the word.

## Inline notes

### `private LVista? _pPhonologyVista;`

The engine's view state for the phonology tab: order, filter and query.
The panel keeps no copy of any of the three and reads each from the vista where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

### `private async void PPhonologyBulletinHandle(LBulletin bulletin)`

The panel answers the engine rather than its own visibility.
So a pronunciation corrected in another tab is in the inventory at once, with no tab switch to trigger it.
A vista announcement carrying this panel's vista id re-lists the inventory, since order, filter or query moved.
Another panel's vista is not this panel's business and is skipped.
A workspace that moved is the one announcement that empties the panel first.
Its flags are reloaded before any row is built.
A draft edit or a fetched frequency, paradigm, script, fanqie or reflex row changes no listed row.
Those announcements are skipped.

### `private string? _pDisplayEntry;`

The entry the right-hand side stands on, or null when none is selected.
The display may show it and the editor may be correcting it.

### `private void PProbeHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement re-lists the inventory.

### `private void PSequenceHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement re-lists the inventory.

### `private void PLensHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

### `private void PInventoryFind()`

Refills the inventory with the rows the engine returns for the vista, already filtered and sorted.
Each row carries the pronunciation stored for its entry.
An entry with no pronunciation sorts last under a pronunciation ordering.
It sorts first under the ordering that looks for what is still missing.
Rows arrive numbered and with their epithets, so the panel only copies them.

### `internal async void PPhonologyVistaRestore(LVista vista)`

Takes the vista the window started for this tab and puts the panel on it.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The inventory is then listed from the vista.

### `private void PSequenceRestore()`

Moves the dropdown mark onto the ordering the vista holds.

### `private void PLensRestore()`

Shows the filter mark while the vista hides any language.

### `internal void PPhonologyScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

### `private void PInventorySelect(string? id)`

Marks the catalog row the panel stands on and clears the mark from every other row.
A null id leaves no row marked, which is what a cleared panel shows.
It is called wherever the shown entry changes, so the mark and the right-hand side never disagree.
