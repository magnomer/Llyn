# PEditorEntry.cs

## `public partial class PEditor`

Which entry the editor stands on, and the two buttons that end an edit of it. A form standing on an entry modifies that entry; a form standing on none writes a new one — one decision, made from one field, which is why a session that opened on an entry no longer saves a second copy of it. What a store and a discard leave behind afterwards differs by host, so each is handed on rather than assumed here.

## `internal void PEditorEntryShow(string id)`

Opens the form on one entry and leaves it standing on that entry, which is what makes the next store a modification of it rather than a copy of it. An entry that no longer loads leaves the form empty: an id that has gone stale is a normal cost of remembering one, not an error.

## Inline notes

### `private string? _pEditorEntry;`

Id of the entry the form stands on, or null when it stands on none. It is the whole difference between a store that creates and a store that modifies: a host that mounts the editor over an entry it loaded sets it through PEditorEntryShow, and a form that was never put on one - the input panel's, and any form left blank by a store - creates.

### `string? entry = _pEditorEntry;`

The form already knows which entry it was opened on, and that is the whole decision: a form standing on an entry is an edit of it, a form standing on nothing is a new entry. Saving unconditionally is what wrote a second entry with the same headword on every ordinary session - launch, correct a typo, press Save.

### `stored = entry is null`

The write is deliberately synchronous: LDatabase keeps its ambient session in a plain instance field and LEngine is built on the UI thread, so it stays on it.

### `_pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);`

A refused write leaves the form exactly as typed, so the missing field can be filled in and the write repeated. An update whose entry vanished between load and save is reported as that, and never quietly turned back into a create.

### `PEditorEntryShow(stored.LEntryId);`

The user corrected an entry; they did not finish one, so the form stays on it rather than resetting. It is filled from the store again rather than left as typed, because the cards this update created carry stored ids now and a form still holding none would create them a second time on the next save.

### `PEditorReset();`

An entry was finished, so the form comes up empty and standing on nothing: the next thing typed here is the next entry, not a rewrite of the one just written. PEditorReset detaches it, and nothing puts it back on - a blank form still holding an id is what overwrote the first entry of a session with the second.

### `private void PDiscardHandle(object sender, RoutedEventArgs e)`

What a discard means is the host's: an input form comes up empty, a browse-style panel puts the selected entry back as it is stored. A host that says nothing gets the form it was given back.
