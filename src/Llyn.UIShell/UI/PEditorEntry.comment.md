# PEditorEntry.cs

## `public partial class PEditor`

Which entry the editor stands on, and the two buttons that end an edit of it.
The form no longer remembers that entry itself.
The draft it is editing carries it, and the draft is the engine's.
A draft started from an entry modifies that entry.
A draft started from nothing writes a new one.
It is one decision, and the engine already holds it.
That is why a session that opened on an entry no longer saves a second copy of it.
What a store and a discard leave behind afterwards differs by host.
So each is handed on rather than assumed here.

## `internal void PEditorEntryShow(string id)`

Starts a draft on one entry and fills the form from it.
That is what makes the next store a modification of it rather than a copy of it.
The controls are filled from the draft rather than from a second read of the entry.
An entry that no longer loads leaves the form empty.
An id that has gone stale is a normal cost of remembering one, not an error.

## Inline notes

### `private string? PEditorEntryRead()`

The entry the held draft was started from, or null when it was started from nothing.
It is asked of the engine rather than kept, so the two can never disagree.

### `PEditorChangeSave();`

Everything typed reaches the draft before the draft is committed.
A store within a keystroke of the last change would otherwise write the form as it stood before it.

### `stored = _lEngine.LEngineDraftCommit(held);`

The write is deliberately synchronous.
LDatabase keeps its ambient session in a plain instance field.
LEngine is built on the UI thread, so it stays on it.
Committing settles the tentative entries this draft links to before writing it.

### `_pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);`

A refused write leaves the form exactly as typed, and the draft file where it was.
So the missing field can be filled in and the write repeated.
An update whose entry vanished between load and save is reported as that.
It is never quietly turned back into a create.

### `_pEditorDraft = string.Empty;`

A committed draft has no file left, so the form stops naming it.
The next line starts the draft the form goes on with.

### `PEditorEntryShow(stored.LEntryId);`

The user corrected an entry, and did not finish one.
So the form stays on it rather than resetting.
It is filled from a fresh draft of the stored entry rather than left as typed.
The cards this update created carry stored ids now.
A form still holding none would create them a second time on the next save.

### `PEditorReset();`

An entry was finished, so the form comes up empty on a new draft.
The next thing typed here is the next entry, not a rewrite of the one just written.

### `private void PEditorDiscardHandle(object sender, RoutedEventArgs e)`

A thrown-away edit takes its draft and its tentative entries with it.
The entry the draft stood on is read before that, because the draft is what names it.
The pending write is called off next, so nothing lands after the draft is gone.
A form that was correcting an entry comes back to it as it is stored.
A form that was creating one comes up empty, since there is nothing stored to come back to.
Either path starts the next draft, so the form is never left without one.
No host is asked what a discard means, because the draft already says which of the two it was.
