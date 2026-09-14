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

## `internal void PEditorEntryShow(long id)`

Starts a draft on one entry and renders the form from it.
That is what makes the next store a modification of it rather than a copy of it.
The controls are filled from the draft rather than from a second read of the entry.
The sentence frames are loaded before the cards are built, because each card takes their order as it is built.
A list the entry leaves empty is given one card to type into, asked for like any other.
An entry that no longer loads leaves the form empty.
An id that has gone stale is a normal cost of remembering one, not an error.

## `internal void PEditorFrequencyShow()`

Fills the frequency chip under the parts of speech from the stored entry, since the draft does not carry it.
The chip is worded by the shared label and sits where the view puts it.
So the two modes read alike.
A form standing on nothing has no entry to ask, so the chip is hidden rather than emptied.
Hiding it moves nothing below the header, because it holds its own row.
An entry with no value asks the engine to fill it, and the fill announces itself when done.

## `internal void PEditorFavoriteShow()`

Puts the heart on the mark the entry already carries, so the editor opens showing what the view showed.
The entry is read off the draft rather than kept, exactly as every other entry question here is.
A form standing on nothing has no entry to mark, so the heart is shown disabled rather than hidden.
Hiding it would move the controls beside it, and the heart is meant to sit still across the two modes.
A favourite that cannot be read is shown unmarked, since a heart is not worth failing an open over.

## `internal void PEditorGraspShow()`

Puts the star row on the step the entry already carries, so the editor opens showing what the view showed.
A form standing on nothing has no entry to rate.
So the row is shown disabled and empty rather than hidden.
A grasp that cannot be read is shown as unrated, for the same reason the heart is.

### `private string PEditorGraspFormat(int step)`

The words a step is shown as beside the stars, empty while the form stands on nothing.

### `private void PEditorHoverHandle(object sender, RoutedEventArgs e)`

Re-words the label as the pointer moves across the stars, and back to the stored step when it leaves.

### `private void PEditorGraspHandle(object sender, RoutedEventArgs e)`

The rating is written straight to the engine rather than into the draft, exactly as the favourite is.
A refused write re-reads the stored step, so the row never shows a rating the workspace does not hold.
The engine announces the change, so the view showing the same entry follows without being told here.

## Inline notes

### `private void PEditorFavoriteHandle(object sender, RoutedEventArgs e)`

The mark is written straight to the engine rather than into the draft.
A favourite is not part of the entry being edited.
It neither waits for a store nor falls with a discard.
A refused write puts the heart back, because the toggle had already moved itself before this was called.
The engine announces the change, so the view showing the same entry follows without being told here.

### `private string? PEditorEntryRead()`

The entry the held draft was started from, or null when it was started from nothing.
It is asked of the engine rather than kept, so the two can never disagree.

### `PEditorChangeSave();`

Everything typed reaches the draft before the draft is committed.
A store within a keystroke of the last change would otherwise write the form as it stood before it.
A hold that failed while flushing halts the form, and a halted form commits nothing.
The draft would otherwise be stored missing the edits the failed flush dropped.

### `stored = _lEngine.LEngineDraftCommit(held);`

The answer is the stored entry with the map from every temporary id to its real one.
This form only needs the entry, because it reloads the whole record from the store.
The write is deliberately synchronous.
LDatabase keeps its ambient session in a plain instance field.
LEngine is built on the UI thread, so it stays on it.
Committing settles the tentative entries this draft links to before writing it.

### `_pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);`

A refused write leaves the form exactly as typed, and the draft file where it was.
So the missing field can be filled in and the write repeated.
An update whose entry vanished between load and save is reported as that.
It is never quietly turned back into a create.

### `_pEditorDraft = 0;`

The form stops naming the draft before the commit rather than after it.
The commit announces the entry while it runs, and a listener may start the form on a fresh draft.
Clearing afterwards would drop that fresh draft's id and leave it held forever.
A refused commit puts the id back, so the form goes on with the same draft.
The inflection fill is the engine's own step after a commit, so nothing starts it here.

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
