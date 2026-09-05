# TDraft.cs

## `public sealed class TDraft`

Covers the drafts folder, the store that keeps unsaved work outside the database.
It also covers the court beneath it, the register of links pointing at records that are still tentative.

## `public void ADraftIsSavedAndReadBackExactlyAsItWasWritten()`

A saved draft comes back field for field, nested content included.
The record holds lists and state values, so a shallow round trip would pass while losing the part the user typed.

## `public void EveryHeldDraftIsListedFromTheWorkspaceFolder()`

Every draft written is returned by the listing.
Recovery after a crash is only useful if it finds all of them.

## `public void DeletingOneDraftLeavesTheOthersStanding()`

Deleting one draft leaves the others in place.

## `public void ABrokenDraftFileIsSkippedRatherThanFailingTheScan()`

A file holding invalid JSON is skipped instead of throwing.
A crash mid-write is exactly when a truncated file appears, which is the same moment the listing matters most.

## `public void ResolvingADraftIdRewritesEveryLinkWaitingOnIt()`

Two links naming one tentative target are both settled when that target becomes a real entry, and both files disappear.
A resolution that stopped at the first link would leave a second link pointing at a draft that no longer exists.
The returned links carry their owners, which is what job06 needs to rewrite the drafts holding them.

## `public void CancellingADraftDropsEveryLinkWaitingOnIt()`

Dropping a tentative target drops every link that named it, with nothing left in the court.
An abandoned entry must not leave links waiting for a record that will never arrive.

## `public void ALinkOnAnotherTargetSurvivesACancel()`

A link to an unrelated target survives both a resolution and a cancellation aimed elsewhere.
The court is shared by every pending link, so settling one target must not empty the register.

## `public void ATentativeLinkIsSavedAndReadBackAsItWasWritten()`

A saved link comes back field for field.
The headword and language are stored because they are shown before the target is real.

## `public void CommittingADraftLeavesTheDraftFolderEmpty()`

Held work that reaches the database leaves the drafts folder empty and the entry stored.
A commit that wrote the entry but kept the file would offer the same work back as unfinished on the next session.

## `public void ARefusedCommitLeavesTheDraftFileWhereItWas()`

A commit the database refuses leaves the file exactly where it was, and writes no entry.
A blank headword is refused, and that is the moment the user most needs what they typed to survive.

## `public void MovingACardRenumbersEveryCardInTheHeldDraft()`

Moving one card reorders the list and renumbers every position contiguously from `1`.
The renumbered cards are both returned to the caller and written to the file, so the form and the folder never disagree.

## `public void ADraftStillHeldOpenIsNotReportedAsLeftover()`

An engine never offers back the drafts it started itself, and a fresh engine over the same folder offers every one that was left changed.
That is the shape of a crash: the process that held the files is gone, and only the next launch can see them as leftovers.
The untouched draft the third panel started is not counted, because a blank form costs nothing to lose.

## `public void ALeftoverDraftNamesTheEntryItWasStartedFrom()`

A draft naming an entry that already exists commits as an update, and the word is stored once.
That is the file a kill between the database write and the delete leaves behind.
Committing it again would store a second copy if the file still named nothing.

## `private static LCardDraft TDraftCardCreate(string title)`

A card carrying nothing but a title, for order that is read by title alone.

## `private static LDraft TDraftCreate(string origin, string headword)`

A draft with enough nesting to prove the whole shape survives the file.
