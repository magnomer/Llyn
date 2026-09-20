# TClaimClerk.cs

## `public sealed class TClaimClerk`

The claim clerk over a rig of fakes, posing as one process and then another over the same folder.
A held draft is checked, seen from another process, finished, swept and cancelled with its court.

## `public void ClaimClerkStart_ThisProcess_HoldsTheDraft()`

A started draft is in the held set, is this clerk's to take away, and reads back.

## `public void ClaimForeignCheck_AnotherProcessHoldsTheDraft_SeesTheClaim()`

A clerk posing as another process sees the first clerk's claim as foreign and holds nothing itself.
The draft file still reads back from either side.

## `public void ClaimClerkFinish_HeldDraft_DropsFileClaimAndHold()`

Finishing a draft empties the held set, the file and the claim, so no other process sees it any more.

## `public void ClaimClerkSweep_StaleDraft_CancelsItWithItsClaim()`

A draft the archive drops as stale is cancelled with its claim, and the live draft beside it is kept.

## `public void ClaimClerkCancel_OwnerOfATentativeTarget_DropsTargetAndRow()`

Cancelling the only draft linking a tentative target takes the row and the target with it.

## `public void ClaimClerkCancel_TargetAnotherDraftStillLinks_KeepsTarget()`

Cancelling one of two drafts linking a target drops that draft's row and leaves the target held.
