# TDraftArchive.cs

## `public sealed class TDraftArchive`

Covers the drafts folder, the store that keeps unsaved work outside the database.
It also covers the court beneath it, the register of links pointing at records that are still tentative.
Each test reads the files back through the archive alone, with no engine in between.

## `public void DraftArchiveSave_WholeDraft_ReadsBackAsWritten()`

A saved draft comes back field for field, nested content included.
The record holds lists and state values.
A shallow round trip would pass while losing the part the user typed.

### `public void DraftArchiveSave_DerivedProperties_LeavesThemOutOfTheFile()`

The file carries the stored fields and none of the derived ones.
The adapter's serializer option does that, and this pins it so the option is not lost.

## `public void DraftArchiveSave_SituationWithMedia_ReadsBackBothLists()`

A Situation draft keeps its pictures and clips through the file, so a crash loses no picture.
The lists round-trip as records, ids and spans included.

## `public void DraftArchiveScan_HeldDrafts_ListsAll()`

Every draft written is returned by the listing.
Recovery after a crash is only useful if it finds all of them.

## `public void DraftArchiveDelete_OneOfSeveral_LeavesOthers()`

Deleting one draft leaves the others in place.

## `public void DraftArchiveScan_BrokenFile_SkipsIt()`

A file holding invalid JSON is skipped instead of throwing.
A crash mid-write is exactly when a truncated file appears, which is the same moment the listing matters most.

## `public void CourtArchiveSettle_WaitingLinks_DropsAll()`

Two links naming one tentative target are both settled, and both files disappear.
A settlement that stopped at the first link would leave a second pointing at a draft that is gone.
The returned links carry their owners, which is what the engine needs to rewrite the drafts holding them.

## `public void CourtArchiveSettle_OtherTargetLink_LeavesIt()`

A link to an unrelated target survives a settlement aimed elsewhere.
The court is shared by every pending link, so settling one target must not empty the register.
Settling the same target twice returns nothing the second time.

## `public void CourtArchiveSave_TentativeLink_ReadsBackAsWritten()`

A saved link comes back field for field.
The headword and language are stored because they are shown before the target is real.
