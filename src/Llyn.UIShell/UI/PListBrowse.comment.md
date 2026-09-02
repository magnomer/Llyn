# PListBrowse.cs

## `public partial class PList`

Browsing behavior of the list panel.
The search box and the ordering menu refill the entry index.
A chosen index row is loaded back from the workspace.
It is rendered read-only in the display area, which the mode toggle swaps for the editor.
This is the read half of the entry round trip.
The input panel writes an entry, and this reads it back and corrects it.

## Inline notes

### `private readonly MediaPlayer _pDisplayPlayer = new();`

This panel's own playback.
The input panel has one too.
Sharing a single player made the two panels each other's business.
Clearing the input recording, which typing a headword does, stopped what the List tab played.

### `private string? _pDisplayRecording;`

Full path of the audio the shown entry owns, or null when it has none.
That is what the display's play button plays.
It is kept apart from the input panel's own transient recording.

### `private string? _pDisplayEntry;`

The entry the right-hand side stands on, or null when none is selected.
The display may show it and the editor may be correcting it.
Either way it is the one entry this panel is on.

### `private string _pOrderChoice = "Headword";`

Which ordering the index is listed in.
It is the tag the chosen menu row carries, not the words that row showed.
The ordering is the same one whatever language names it.

### `private void PListHandle(object sender, DependencyPropertyChangedEventArgs e)`

The panel opens with whatever the database already holds.
So a save made in the input panel is visible the moment the tab is switched to.
Rebinding is idempotent, which keeps the binding out of the window constructor and beside the code that owns it.

### `private void POrderHandle(object sender, RoutedEventArgs e)`

A chosen ordering renames the button and re-lists the index.
Nothing about the selected entry changes.
An ordering says in which order the entries are offered, never which one is shown.

### `private IEnumerable<LEntry> POrderSort(IReadOnlyList<LEntry> entries)`

The found entries in the order the menu asked for.
The engine returns them by headword already, so that ordering is the one that costs nothing.
A timestamp an entry does not carry sorts as empty.
That puts an entry of unknown age at the end of a newest-first list.

### `private void PIndexFind(string query)`

Refills the index with the entries matching the typed text.
An empty box lists everything.
The display is not touched: the search says which entries are offered, never which one is shown.

### `if (!PListLeaveConfirm())`

Selecting another entry leaves whatever is being written behind, so it is asked about first.
The row that was clicked is not worth the correction that was typed.

### `private void PIndexEntryShow(string id)`

Puts the whole right-hand side on one entry.
The display is filled from the store.
An editor that is open moves onto the same entry, since the entry being edited is selected.

### `PDisplayClear();`

The entry went away between the search and the click.
The row is stale.
So the list is re-read rather than left offering a row that no longer loads.

### `private void PIndexEntryUpdate(string id)`

After a store, the headword the index lists may have changed.
So may the text the display shows.
Each is read again from what was written.
The editor keeps the entry it is on — the user corrected it, they did not leave it.

### `return;`

The write went through.
A display that cannot be filled again is not worth reporting as a failure that did not happen.

### `private void PEditorEntryRestore()`

The discard the editor hands over: the selected entry comes back as it is stored.
With no entry selected there is nothing stored to come back to, so the form is emptied instead.

### `private void PScribeHandle(object sender, RoutedEventArgs e)`

The mode toggle: reading becomes writing on the selected entry, and writing goes back to reading what is actually stored.
Leaving the editor is what the unsaved question stands in front of.

### `if (_pDisplayEntry is not null)`

A correction that was given up leaves the editor holding text the store never took.
So the display is filled from the store again rather than from what was on screen.

### `private void PScribeShow(bool editing)`

Which of the two halves the broader side shows, and what the toggle then offers.

### `private bool PListLeaveConfirm()`

The question put before the editing state is left.
That is selecting another entry, or toggling back to the display.
It is anything else that would leave typed corrections behind.
Nothing unsaved means nothing to ask about.

### `_pDisplayRecording = draft.LEntryDraftAudio.Length > 0 && File.Exists(draft.LEntryDraftAudio)`

The audio the entry was saved with, as a full path in the workspace open now.
The button appears only when that file is actually there.
So a workspace whose audio folder was removed shows no play control.
It does not show one that fails on click.

### `PDisplaySense.ItemsSource = draft.LEntryDraftSenses;`

The draft records carry exactly the fields the read view shows.
So the templates bind to them directly instead of copying each one into a second row model.

### `PScribe.IsEnabled = true;`

An entry is selected now, so it can be written as well as read.

### `private async void PDisplayLanguageShow(string language)`

Keeps the language on the same visual line as the headword.
It gives the language the flag resolved by the same path as the editor's picker.
Flag lookup is asynchronous.
So a result is painted only while the display still shows the language that asked for it.

### `private void PDisplayClear()`

The display must not assume an entry is selected.
With none chosen it shows only its prompt.
There is nothing to write, so the editor is closed and the toggle offers nothing.
