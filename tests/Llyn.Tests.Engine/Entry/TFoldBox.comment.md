# TFoldBox.cs
Hash: `cfb24f0580d8822c`

## `public sealed class TFoldBox`

Covers the engine's box fold seam for the Fanqie and Script boxes of the entry editor.
A box is opened, closed and read back per entry through `LEngine` relays alone.
Theories exercise both box kinds, which have separate tables.
Combined facts check reflex isolation and deletion of both boxes.
It covers the two boxes staying apart from each other and from the reflex fold.
It covers two entries keeping apart, so no entry shares a box state with another.
It covers an entry delete dropping the rows of both boxes.
It covers an id at or below zero, as an unsaved entry carries.
Such an id must write nothing and throw nothing.
It covers the fold bulletin, raised on the entry so other open views of it re-read their folds.

## `public void BoxSpread_OpenedEntry_AnswersOpened(LFoldBox box)`

Each box kind persists its own opening for the entry.

## `public void BoxSpread_ClosedAfterOpened_AnswersClosedAndKeepsNoRow(LFoldBox box, string table)`

Closing a box removes its opening record from that box table.

## `public void BoxSpread_OneBoxOpened_LeavesTheOtherBoxAndTheReflexFoldClosed(LFoldBox box, LFoldBox other)`

Opening one box leaves the other box and reflex opening closed.

## `public void ReflexSpread_OpenedEntry_LeavesBothBoxesClosed()`

Reflex opening does not open either reference box.

## `public void BoxSpread_TwoEntries_KeepsEachEntrysOwnState(LFoldBox box)`

Box openings remain independent between entries.

## `public void EntryDelete_SpreadBoxes_DropsTheirRows()`

Entry deletion removes both box-opening records.

## `public void BoxSpread_IdAtOrBelowZero_WritesNothingAndThrowsNothing(long entryId, LFoldBox box, string table)`

Nonpositive entry ids create no box-opening record.

## `public void BoxSpread_StoredEntry_RaisesFoldBulletinOnItsEntry(LFoldBox box)`

A box-opening write publishes a Fold bulletin with its entry id.
