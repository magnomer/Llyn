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

### `private async void PPhonologyWorkspaceUpdate()`

A workspace that moved empties the panel and reloads its flags before any row is built.

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
Rows arrive numbered, with their epithets and marked, so the panel only copies them.
The display is not touched: the search says which entries are offered, never which one is shown.

### `internal async void PPhonologyVistaRestore(LVista vista)`

Takes the vista the window started for this tab and puts the panel on it.
The panel answers the engine through the vista rather than its own visibility.
So a pronunciation corrected in another tab is in the inventory at once, with no tab switch to trigger it.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A vista announcement re-lists the inventory, since order, filter or query moved.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each re-lists too.
A draft edit or a fetched frequency, paradigm, script or fanqie row changes no listed row, and is not attached.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The inventory is then listed from the vista.
The same vista is handed to the display, which reads its chosen entry from it.

### `private void PInventoryEntryUpdate(LBulletin bulletin)`

After a store, the row the inventory lists may have changed, so it is re-listed.
An entry stored while the editor is open becomes the chosen row first, since the user did not leave it.

### `private void PPhonologyEntryUpdate(LBulletin bulletin)`

Reached only for the entry the panel stands on, or for a store that named no entry.
The entry is read back and shown again, or the panel clears when it is gone.

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

### `private void PInventoryChosenApply()`

Marks the row of the entry the vista stands on and clears the mark from every other row.
No row is marked when the vista stands on none, which is what a cleared panel shows.
It walks the rows already listed, so choosing an entry never re-reads the inventory.

### `private void PPhonologyCommandApply()`

The delete button acts on the entry being read.
It is derived from that one fact rather than switched on at each place an entry appears.
An entry can be shown by a click, by an announcement, or by a restored tab.
Deriving the state means no such path can leave the button contradicting the panel.
