# PLibraryBrowse.cs

## `public partial class PLibrary`

Browsing behavior of the library panel.
The search box and the ordering dropdown refill the entry index.
A chosen index row is loaded back from the workspace.
It is rendered read-only in the display area, which the mode toggle swaps for the editor.
This is the read half of the entry round trip.
The input panel writes an entry, and this reads it back and corrects it.

## Inline notes

### `private LVista? _pLibraryVista;`

The engine's view state for the library tab: order, filter, query and the entry the right-hand side stands on.
The panel keeps no copy of any of the four and reads each from the vista where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

### `private async void PLibraryBulletinHandle(LBulletin bulletin)`

The panel answers the engine rather than its own visibility.
So a save made in the input panel lands here at once, with no tab switch to trigger it.
A vista announcement carrying this panel's vista id re-lists the index, since order, filter or query moved.
Another panel's vista is not this panel's business and is skipped.
A workspace that moved is the one announcement that empties the panel first.
Its flags are reloaded before any row is built.
Every announcement that can change a listed row re-lists the index.
A draft edit or a fetched frequency, paradigm, script, fanqie or reflex row changes no listed row.
Those announcements are skipped.
Each keystroke in the input panel announces a draft change, so re-listing on it stalled every edit.

### `private void PInquiryHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement re-lists the index.

### `private void POrderHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement re-lists the index.
Nothing about the selected entry changes.
An ordering says in which order the entries are offered, never which one is shown.

### `private void PSieveHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

### `internal async void PLibraryVistaRestore(LVista vista)`

Takes the vista the window started for this tab and puts the panel on it.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The index is then listed from the vista.

### `private void POrderRestore()`

Moves the dropdown mark onto the ordering the vista holds.

### `private void PSieveRestore()`

Shows the filter mark while the vista hides any language.

### `private void PIndexChosenApply()`

Marks the row of the entry the vista stands on and clears the mark from every other row.
No row is marked when the vista stands on none, which is what a cleared panel shows.
It walks the rows already listed, so choosing an entry never re-reads the index.

### `private void PIndexFind()`

Refills the index with the rows the engine returns for the vista.
Each row arrives filtered, sorted, numbered and marked, so the panel only copies it.
An empty query lists everything.
The display is not touched: the search says which entries are offered, never which one is shown.
Before a vista is handed over the index is emptied and nothing is asked.

### `if (!PLibraryLeaveConfirm())`

Selecting another entry leaves whatever is being written behind, so it is asked about first.
The row that was clicked is not worth the correction that was typed.

### `private void PIndexEntryShow(long id)`

Puts the whole right-hand side on one entry.
The display is filled from the store.
An editor that is open moves onto the same entry, since the entry being edited is selected.

### `PLibraryClear();`

The entry went away between the search and the click.
The row is stale.
So the list is re-read rather than left offering a row that no longer loads.

### `private void PIndexEntryUpdate(LBulletin bulletin)`

After a store, the headword the index lists may have changed.
It is read again from what was written.
The display re-reads its own entry on the same announcement, so it is not filled twice here.
An Entry announcement alone carries an entry id, so only it may select a row or find the entry gone.
The editor keeps the entry it is on — the user corrected it, they did not leave it.

### `return;`

The write went through.
A display that cannot be filled again is not worth reporting as a failure that did not happen.

### `private void PEditorEntryRestore()`

The discard the editor hands over: the selected entry comes back as it is stored.
With no entry selected there is nothing stored to come back to, so the form is emptied instead.

### `private void PLibraryFreshHandle(object sender, RoutedEventArgs e)`

The new-entry command: the selection is dropped and the editor opens on a blank form.
Nothing selected means the discard falls back to an empty form, so the writer stays in the new entry.
The blank form starts a draft of its own under the `Library` origin.
A new word here and a new word in the input panel are two tentative entries.
Each carries its own id and its own file, so neither answer to the exit question can reach the other.
The origin is what a leftover reports itself by.
That is how recovery can say where the work was being typed.

### `private void PLibraryScribeHandle(object sender, RoutedEventArgs e)`

The mode toggle: reading becomes writing on the selected entry, and writing goes back to reading what is actually stored.
Leaving the editor is what the unsaved question stands in front of.


### `if (editing == (PEditor.Visibility == Visibility.Visible))`

Both segments answer here, so the click is read off which one was pressed.
The segment already standing for what is on screen changes nothing.
### `if (_pLibraryVista?.LVistaChosen is long chosen)`

A correction that was given up leaves the editor holding text the store never took.
So the display is filled from the store again rather than from what was on screen.

### `private void PLibraryScribeShow(bool editing)`

Which of the two halves the broader side shows, and which segment the toggle marks.
The half chosen is pushed downstream as it changes, so the panel opens on it next time.

### `internal bool PLibraryLeaveConfirm()`

The question put before the editing state is left.
That is selecting another entry, or toggling back to the display.
It is anything else that would leave typed corrections behind.
The window asks it too, for a jump that lands on the library the user is already writing in.
Nothing unsaved means nothing to ask about.

### `internal long PLibraryVoyageRead()`

The Entry the panel shows, as the station the window records before a jump away.
Zero says no Entry is shown, so there is no place to come back to.

### `private void PLibraryEntryShow(long id, LEntryDraft draft)`

The shared display draws the entry, and this panel decides what the toggle may then do.
An entry is selected now, so it can be written as well as read.

### `internal void PLibraryCommandApply()`

The delete button acts on the entry being read.
It is derived from that one fact rather than switched on at each place an entry appears.
An entry can be shown by a click, by an announcement, or by a restored tab.
Deriving the state means no such path can leave the button contradicting the panel.
It is applied again when the panel becomes visible, so a tab never returns holding a stale answer.

### `private void PLibraryClear()`

The panel must not assume an entry is selected.
With none chosen the display shows only its prompt.
There is nothing to write, so the editor is closed and the toggle offers nothing.

### `internal void PLibraryScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.
