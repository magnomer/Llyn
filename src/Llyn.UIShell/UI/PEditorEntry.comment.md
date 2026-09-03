# PEditorEntry.cs

## `public partial class PEditor`

Which entry the editor stands on, and the two buttons that end an edit of it.
A form standing on an entry modifies that entry.
A form standing on none writes a new one.
It is one decision, made from one field.
That is why a session that opened on an entry no longer saves a second copy of it.
What a store and a discard leave behind afterwards differs by host.
So each is handed on rather than assumed here.

## `internal void PEditorEntryShow(string id)`

Opens the form on one entry and leaves it standing on that entry.
That is what makes the next store a modification of it rather than a copy of it.
An entry that no longer loads leaves the form empty.
An id that has gone stale is a normal cost of remembering one, not an error.

## Inline notes

### `private string? _pEditorEntry;`

Id of the entry the form stands on, or null when it stands on none.
It is the whole difference between a store that creates and a store that modifies.
A host that mounts the editor over an entry it loaded sets it through PEditorEntryShow.
A form that was never put on one creates.
That is the input panel's form, and any form left blank by a store.

### `string? entry = _pEditorEntry;`

The form already knows which entry it was opened on, and that is the whole decision.
A form standing on an entry is an edit of it.
A form standing on nothing is a new entry.
Saving unconditionally wrote a second entry with the same headword on every ordinary session.
Such a session is launch, correct a typo, press Save.

### `stored = entry is null`

The write is deliberately synchronous.
LDatabase keeps its ambient session in a plain instance field.
LEngine is built on the UI thread, so it stays on it.

### `_pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);`

A refused write leaves the form exactly as typed.
So the missing field can be filled in and the write repeated.
An update whose entry vanished between load and save is reported as that.
It is never quietly turned back into a create.

### `PEditorEntryShow(stored.LEntryId);`

The user corrected an entry, and did not finish one.
So the form stays on it rather than resetting.
It is filled from the store again rather than left as typed.
The cards this update created carry stored ids now.
A form still holding none would create them a second time on the next save.

### `PEditorReset();`

An entry was finished, so the form comes up empty and standing on nothing.
The next thing typed here is the next entry, not a rewrite of the one just written.
PEditorReset detaches it, and nothing puts it back on.
A blank form still holding an id overwrote the first entry of a session with the second.

### `PTranslationFreshClear();`

A saved entry keeps every stub its links made, so the editor stops watching them.

### `private void PDiscardHandle(object sender, RoutedEventArgs e)`

A thrown-away edit takes its stubs with it, so they are dropped before the form is reset.
Loading another entry throws the current edit away too, so it drops them the same way.
What a discard means is the host's.
An input form comes up empty.
A browse-style panel puts the selected entry back as it is stored.
A host that says nothing gets the form it was given back.
