# TChronicle.cs

## `public sealed class TChronicle`

Covers the chronicle of a held draft: the snapshots undo steps back to and redo steps forward to.
What matters is which draft is on disk after each step, and when the chronicle is empty.

## `public void RequestApply_ThenUndo_RestoresHeldDraft()`

One edit undone puts the draft as it was before the edit back on disk.
The past is then empty and the future holds the edit.

## `public void Undo_ThenRedo_ReturnsToLatest()`

A redo after an undo puts the undone edit back on disk.
The future is then empty and the past holds the edit again.

## `public void RequestApply_AfterUndo_ClearsFuture()`

An edit made after an undo forgets what the undo left ahead.
Redo then steps nowhere, and the new edit stands.

## `public void RequestApply_NoChange_RecordsNothing()`

A request that changes nothing leaves no snapshot behind.
The one undo steps over the repeated request straight to the start.

## `public void RequestApply_SameTypeWithinWindow_MergesStep()`

Two headword edits a second apart are one step, and a third two seconds later is another.
The clock is held by the test, so the window is measured and not waited for.

## `public void RequestApply_OtherType_StartsStep()`

A note edit right after a headword edit is its own step, since the request types differ.

## `public void RequestApply_AfterUndo_StartsStep()`

An edit right after an undo and redo is its own step even though the type and the moment match.
Undo steps back to the redone draft, not past it.

## `public void CardShift_ThenUndo_RestoresOrder()`

A card move is an edit, so undo puts the cards back in their earlier order.
The card ids are what is compared, since the move touched only their places.

## `public void DraftCommit_ClearsChronicle()`

Committing a draft drops its chronicle, so neither step has anywhere to go.

## `public void Undo_EmptyPast_ReturnsNull()`

Both steps answer null on a draft nothing has edited, and the draft is left as it was.

## `public void Record_PastCap_DropsOldest()`

Past the cap the oldest snapshot is forgotten.
After a hundred and one edits, a hundred undos walk back to the first edit and no further.
The clock advances two seconds per edit, so no two edits merge.
