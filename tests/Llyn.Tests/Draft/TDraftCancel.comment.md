# TDraftCancel.cs

## `public sealed class TDraftCancel`

Covers ending a held draft without storing it, by cancel or by delete.
What matters is what the court and the other drafts are left holding.

## `public void DraftCancel_TargetAnotherDraftNames_KeepsTarget()`

Cancelling a draft another draft still names as a target leaves that target in place.
The link is the other draft's, so only that draft's end may settle it.

## `public void DraftCancel_TargetOfAnotherDraft_StrikesItsIdFromThatDraft()`

Cancelling a target strikes its throwaway id out of the draft that named it, and drops the court row.
An id left behind would commit as a link to nothing.

## `public void DraftCancel_TargetHeldByAnotherEngine_LeavesItAlone()`

Cancelling the owner of a link leaves the target another engine holds untouched.
That target is someone else's open work.

## `public void DraftDelete_DraftOwningLinks_DropsThem()`

Dropping a held draft takes the court rows it owns with it, and the tentative target nothing else wants.
A row left behind names an owner no call can reach, so nothing would ever collect it.

## `public void DraftDelete_OwnerDraftGone_CollectsRow()`

A row whose owner draft is already gone is collected by the next draft that ends.
That is what an earlier launch stranded, and neither resolving nor cancelling can find it by target alone.
