# TDraftLeftover.cs

## `public sealed class TDraftLeftover`

Covers what a launch finds in the drafts folder that no open window holds.
A leftover is offered back, swept when the entry already has it, or set aside when it cannot be read.

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

## `public void LeftoverSweep_BlankDraftNamingNoEntry_SweepsIt()`

A blank draft naming no entry is deleted by the sweep, claim and all.
Every panel starts one at launch, and a kill leaves ten such files behind each time.
Recovery never offers a blank one back, so nothing else would ever collect them.
A draft naming no entry but holding text survives and is still offered back.

## `public void LeftoverSweep_ClaimWithoutDraftFile_DropsIt()`

A claim file whose draft is gone is deleted by the sweep.
A claim is only examined when its draft is asked about, and nothing asks about a draft that is gone.

## `public void LeftoverSweep_DraftFileOfAnotherVersion_DropsItWithClaimAndLinks()`

A draft file stamped with another version is not read.
The sweep sets it aside under `drafts/broken` and drops its claim and the links naming it.
A file written before a field rename would otherwise read back naming no entry and commit as a duplicate.
The draft owning the swept link is left alone, because it is of the current shape.

## `public void LeftoverSweep_UnreadableDraftFile_SetsItAsideUnchanged()`

A file that does not read as a draft at all is moved aside byte for byte, never deleted.
It is the user's work in some shape, and the next build may read it again.
It is not offered as a leftover, because the listing does not look under `broken`.

## `public void LeftoverSweep_StalePendingFile_RemovesIt()`

A half-written `.json.tmp` older than an hour is deleted from the drafts folder and from the court.
One written a moment ago is left.
A save in the other copy of the program may still be in flight.
