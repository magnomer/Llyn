# LEngineUpdate.cs

## `public sealed partial class LEngine`

The one write path that changes a stored entry.
The same draft the save consumes goes in.
The rows the entry is already made of are reconciled to it.
It is the counterpart of `LEngineEntrySave`: the save creates, and this one changes.
It lives beside it rather than inside it because the two share nothing but the draft shape.
Creating writes rows in card order.
Changing has to work out which stored row each card is first.

## `public LEntry LEngineEntryUpdate(string id, LEntryDraft draft)`

Applies `draft` to the entry identified by `id` and returns the stored entry as it now stands.
The entry keeps its opaque id and its `added_utc`.
Only `updated_utc` moves, so an edit is the same record, not a new one.

A blank headword and an id no entry carries are both refused before anything is written.
Each carries a reason key the shell localizes.
The refusal for a missing entry matters.
The form may have been opened on an entry that has since been deleted.
Falling back to creating a copy is exactly the defect this seam exists to remove.

Everything after the refusals runs inside one session and commits once.
So an update is whole or it never happened.
A card that cannot be deleted is one another entry still links to.
Such a card leaves the stored entry exactly as it was, down to its timestamps.

Which stored card a draft card is comes from `LCardDraft.LCardDraftId`.
It never comes from its place in the list.
A card naming a stored Meaning or Collocation of this entry updates that row in place.
So its id survives and every row referencing it keeps pointing at the same Meaning.
A card naming nothing is created.
A stored card the draft no longer names is deleted.
A card gone entirely blank counts as dropped, as the save counts it as unwritten.
The survivors are then renumbered to draft order in one pass through `LDatabaseOrder`.
The unique `(owner, position)` index makes moving one row at a time collide at once.

A card's Examples, Situations and Tags are re-attached to match the draft.
A value the card still lists keeps the row it already referenced.
A new value creates a new row.
A value it dropped is detached and nothing more.
Detaching the last reference never deletes the row.
Those are independent data the card references and does not own.
So a Tag typed once and cleared survives its last referrer.

The revision records one change per altered child.
That is each Meaning and Collocation created, updated or deleted.
It also covers the note and pronunciation when they moved.
It is not one change saying the entry changed, so the history says what an edit did.
The workspace row is moved onto that revision, as the save and the delete both do.

Forms, parts of speech, inflections and the pronunciation are each compared before they are written.
A field that did not change writes no row and records no revision change.

## Inline notes

### `private static void LEngineFormUpdate(LEntryArchive entries, string entryId, LEntryDraft draft, List<LRevisionChange> changes)`

The forms of the entry as the draft holds them, replacing what stood before.
Positions are rewritten by the archive, so a reordered list stores as the new order.
The comparison is over the text, the role and the label, because those are what a form is.

### `private void LEngineInflectionUpdate(string entryId, LEntryDraft draft, List<LRevisionChange> changes)`

The inflections of the entry as the draft holds them, features and all.
A feature list that differs at one place is a different inflection, so the whole set is rewritten.

### `entries.LEntryUpdate(stored with`

The row is updated in place.
The archive stamps updated_utc and touches neither the id nor added_utc.
So the entry stays the record it was.

### `private void LEngineSpeechUpdate(`

The entry's part of speech reconciled to the draft.
The assignments are one owned set with an (entry_id, position) identity and nothing referencing them.
So they are replaced wholesale rather than matched row by row.
The field holds one part of speech, and rewriting it is what the user did.
A field cleared leaves the entry with no assignment, which is the set replaced by an empty one.
A field that comes back the same row writes nothing and records no change.

### `private static bool LEngineSpeechMatch(IReadOnlyList<LSpeech> one, IReadOnlyList<LSpeech> other)`

Two sets of assignments compared as they are stored.
A row is the same row when it names the same preset id at the same position.
It is also the same when it carries the same typed text at the same position.
The generated record equality would do this too.
But it also compares the entry id a resolved row carries.
That id is empty on the way in and filled on the way out.

### `private void LEngineNoteUpdate(string entryId, LEntryDraft draft, List<LRevisionChange> changes)`

The entry's note reconciled to the draft.
Text replaces whatever was stored.
A note the user cleared is deleted rather than left standing as their last words.

### `private void LEnginePronunciationUpdate(`

The entry's pronunciation reconciled to the draft.
The row is what a recording hangs from.
So it is created when either the IPA or a recording is present.
It is deleted only when both are gone.
Its id survives an edit of the IPA.
So the recording hanging from it is not re-downloaded to stay attached.

### `string file = LEngineRecordingFormat(draft.LEntryDraftAudio);`

The draft carries the recording as a full path and the row stores it relative to the workspace.
So the comparison is made in stored terms.
An unchanged recording writes nothing.
