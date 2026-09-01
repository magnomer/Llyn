# LEngineUpdate.cs

## `public sealed partial class LEngine`

The one write path that changes a stored entry: the same draft the save consumes goes in, and the rows the entry is already made of are reconciled to it. It is the counterpart of `LEngineEntrySave` — the save creates, this one changes — and it lives beside it rather than inside it because the two share nothing but the draft shape: creating writes rows in card order, changing has to work out which stored row each card is first.

## `public LEntry LEngineEntryUpdate(string id, LEntryDraft draft)`

Applies `draft` to the entry identified by `id` and returns the stored entry as it now stands. The entry keeps its opaque id and its `added_utc`; only `updated_utc` moves, so an edit is the same record, not a new one.

A blank headword and an id no entry carries are both refused before anything is written, each with a reason key the shell localizes. The refusal for a missing entry matters: the form may have been opened on an entry that has since been deleted, and falling back to creating a copy is exactly the defect this seam exists to remove.

Everything after the refusals runs inside one session and commits once, so an update is whole or it never happened: a card that cannot be deleted — one another entry still links to — leaves the stored entry exactly as it was, down to its timestamps, rather than half-applied.

Which stored card a draft card is comes from `LCardDraft.LCardDraftId`, never from its place in the list: a card naming a stored Meaning or Collocation of this entry updates that row in place, so its id survives and every row referencing it keeps pointing at the same Meaning. A card naming nothing is created, a stored card the draft no longer names is deleted, and a card gone entirely blank counts as dropped on the same terms the save counts it as unwritten. The survivors are then renumbered to draft order in one pass through `LDatabaseOrder`, because the unique `(owner, position)` index makes moving one row at a time collide on the first statement.

A card's Examples, Situations and Tags are re-attached to match the draft: a value the card still lists keeps the row it already referenced, a new value creates a new row, and a value it dropped is detached and nothing more. Detaching the last reference never deletes the row — those are independent data the card references and does not own, so a Tag typed once and cleared survives its last referrer.

The revision records one change per altered child — each Meaning and Collocation created, updated or deleted, and the note and pronunciation when they moved — rather than one change saying the entry changed, so the history says what an edit actually did. The workspace row is moved onto that revision, as the save and the delete both do.

## Inline notes

### `entries.LEntryUpdate(stored with`

The row is updated in place: the archive stamps updated_utc and touches neither the id nor added_utc, so the entry stays the record it was.

### `private void LEngineSpeechUpdate(`

The entry's part of speech reconciled to the draft. The assignments are one owned set with an (entry_id, position) identity and nothing referencing them, so they are replaced wholesale rather than matched row by row: the field holds one part of speech, and rewriting it is what the user did. A field cleared leaves the entry with no assignment, which is the set replaced by an empty one, and a field that comes back the same row writes nothing and records no change.

### `private static bool LEngineSpeechMatch(IReadOnlyList<LSpeech> one, IReadOnlyList<LSpeech> other)`

Two sets of assignments compared as they are stored: a row is the same row when it names the same preset id, or carries the same typed text, at the same position. The generated record equality would do this too, but it also compares the entry id a resolved row carries, which is empty on the way in and filled on the way out.

### `private void LEngineNoteUpdate(string entryId, LEntryDraft draft, List<LRevisionChange> changes)`

The entry's note reconciled to the draft: text replaces whatever was stored, and a note the user cleared is deleted rather than left standing as the last thing they typed.

### `private void LEnginePronunciationUpdate(`

The entry's pronunciation reconciled to the draft. The row is what a recording hangs from, so it is created when either the IPA or a recording is present and deleted only when both are gone — and its id survives an edit of the IPA, so the recording hanging from it is not re-downloaded to stay attached.

### `string file = LEngineRecordingFormat(draft.LEntryDraftAudio);`

The draft carries the recording as a full path and the row stores it relative to the workspace, so the comparison is made in stored terms; an unchanged recording writes nothing.
