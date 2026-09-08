# TDraft.cs

## `public sealed class TDraft`

Covers the drafts folder, the store that keeps unsaved work outside the database.
It also covers the court beneath it, the register of links pointing at records that are still tentative.

## `public void DraftArchiveSave_WholeDraft_ReadsBackAsWritten()`

A saved draft comes back field for field, nested content included.
The record holds lists and state values.
A shallow round trip would pass while losing the part the user typed.

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

## `public void DraftCommit_HeldDraft_StoresEntryAndDeletesFile()`

Held work that reaches the database leaves the drafts folder empty and the entry stored.
A commit that wrote the entry but kept the file would offer the same work back.
The next session would read it as unfinished.

## `public void DraftCommit_WriteRefused_KeepsDraftFile()`

A commit the database refuses leaves the file exactly where it was, and writes no entry.
A blank headword is refused, and that is the moment the user most needs what they typed to survive.

## `public void DraftMove_CardMoved_RenumbersAllCards()`

Moving one card reorders the list and renumbers every position contiguously from `1`.
The renumbered cards are returned to the caller and written to the file.
The form and the folder never disagree.

## `public void DraftCheck_OnlyVideoAdded_ReportsChanged()`

A card given nothing but a video reads as changed against the entry it was opened from.
The comparison decides whether the editor offers to store at all.
A field it skips is a field the user cannot save.

## `public void DraftCheck_NewDraftCarryingLanguage_ReportsUnchanged()`

A fresh draft holding nothing but the tongue the shell chose is not changed work.
The editor writes that tongue on its own, so counting it would warn about edits the user never made.

## `public void LeftoverRead_DraftStillHeldOpen_PassesOverIt()`

An engine never offers back the drafts it started itself.
A fresh engine over the same folder offers every one that was left changed.
That is the shape of a crash.
The process that held the files is gone, and only the next launch sees them as leftovers.
The untouched draft the third panel started is not counted, because a blank form costs nothing to lose.

## `public void LeftoverSweep_DraftMatchingStoredEntry_SweepsIt()`

A draft whose content already matches the entry it names is deleted by the sweep.
Such a draft is what a kill just after a commit leaves, and nothing else ever collects it.
It is not reported as a leftover either, because the file it was is gone.
A draft holding work the entry does not have survives the sweep and is still offered back.

## `public void LeftoverSweep_StalePendingFile_RemovesIt()`

A half-written `.json.tmp` older than an hour is deleted from the drafts folder and from the court.
One written a moment ago is left.
A save in the other copy of the program may still be in flight.

## `public void DraftCommit_NamesStoredEntry_UpdatesIt()`

A draft naming an entry that already exists commits as an update, and the word is stored once.
That is the file a kill between the database write and the delete leaves behind.
Committing it again would store a second copy if the file still named nothing.

## `public void DraftCommit_NamesDeletedEntry_StoresNewEntry()`

A draft opened on an entry that is deleted while it is open commits as a new entry.
Taking the update branch would refuse it forever, and the work would sit unsaved with no way out.

## `public void DraftCommit_TargetAnotherEditorHolds_KeepsTargetFile()`

A commit that reaches a target another editor is holding stores that target and leaves its file where it is.
Deleting it would pull the file out from under the panel typing into it.
Every later keystroke would be lost.
The file it leaves names the entry it became.
The holder's own commit updates that entry instead of storing the word twice.
The chip still lands on a real id.
The court row is settled exactly as a held target's would be.

## `public void DraftDelete_DraftOwningLinks_DropsThem()`

Dropping a held draft takes the court rows it owns with it, and the tentative target nothing else wants.
A row left behind names an owner no call can reach, so nothing would ever collect it.

## `public void DraftDelete_OwnerDraftGone_CollectsRow()`

A row whose owner draft is already gone is collected by the next draft that ends.
That is what an earlier launch stranded, and neither resolving nor cancelling can find it by target alone.

## `public void DraftSave_EveryCardNamed_ReturnsTheContentSent()`

Content whose cards already carry names comes back as the same instance the editor sent.
The editor redraws only when the instance differs, so a fresh instance here would redraw the page on every keystroke.

## `public void DraftSave_UnnamedCard_ReturnsFreshContentWithEveryCardNamed()`

Content holding one unnamed card comes back as a different instance with every card named.
The untouched list inside it is still the sent one, so the editor rebuilds only the part that changed.

## `private static LCardDraft TDraftCardCreate(string title, string id)`

A card carrying a title and a name already given, for the save that must change nothing.

## `private static LCardDraft TDraftCardCreate(string title)`

A card carrying nothing but a title, for order that is read by title alone.

## `private static LDraft TDraftCreate(string origin, string headword)`

A draft with enough nesting to prove the whole shape survives the file.
