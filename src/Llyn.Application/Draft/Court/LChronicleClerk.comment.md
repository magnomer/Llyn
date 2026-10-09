# LChronicleClerk.cs
Hash: `e36ae342c56f6f67`

## `public sealed class LChronicleClerk`

Editorial undo and redo over one held draft, kept in memory for the session only.
A chronicle is the ordered snapshots a held draft passed through, walked by undo and redo.
It is not `Revision`, which is the stored commit history, and not `Voyage`, which is the reader's trail.
`LDraft` is an immutable record and every edit already ends in one draft save.
So a step is one whole `LDraft`, and no request needs an inverse of its own.
Each draft id keeps a past list and a future list, the newest snapshot at the end of each.
Lists rather than stacks, because trimming the oldest snapshot is a removal at the front.
The engine records the draft an edit replaces, once it has judged the edit worth a step.
Ending a draft drops its chronicle, because the file it walked is gone.
A settled court link drops the owner's chronicle too.
A snapshot from before the id rewrite would resurrect a draft translation id.
Steps that continue the same edit within a short window merge into one.
It also keeps the keystrokes a tenure defers, and times the quiet before they flush.
That is a timing rule about draft edits, holding with no user present, so it sits beside the merge window.
The quiet waits on the rig's clock, so a replay on a virtual clock drives it.
The deferred requests have their own lock, since every tenure defers from its own thread.
That lock is never held while the clerk calls out, so it is always the innermost one.

## `private const int LChronicleClerkCap = 100;`

The most snapshots one draft keeps behind it.
Beyond it the oldest snapshot is forgotten, so a long session cannot grow without bound.

## `private const int LChronicleClerkWindow = 1500;`

The milliseconds within which a request of the same type on the same draft continues the last step.

## `private readonly Dictionary<long, List<LRequest>> _lChronicleClerkDeferred = [];`

The requests waiting to flush, per draft id, in arrival order and one per `LRequestKey`.
It is also the lock guarding itself and the pending waits.

## `private readonly Dictionary<long, CancellationTokenSource> _lChronicleClerkPending = [];`

The wait in progress per draft id, cancelled by every new deferral with a delay.

## `public LChronicleClerk(LRig rig)`

Reads the draft port and the clock out of `rig`.
The moment is read from the rig's clock, so a test holds or advances it through the rig.

## `public void LChronicleClerkRecord(LDraft held, LDraft saved, LRequest? request)`

Keeps the draft an edit is about to replace, and forgets the future.
The engine has already judged that the two drafts differ and that one of them is real work.
A new edit after an undo starts a new branch.
A request of the same type on the same draft within the window continues the last step.
Then nothing is pushed and only the moment is refreshed.
A null request always pushes and leaves no step to continue.

## `public void LChronicleClerkClear(long id)`

Forgets both lists of one draft, and the last step with them.

## `public LDraft? LChronicleClerkUndo(long id)`

Steps the held draft back one snapshot and returns the draft as saved.
Null when nothing is behind it.
The draft being replaced is kept for redo.

## `public LDraft? LChronicleClerkRedo(long id)`

Steps the held draft forward one snapshot and returns the draft as saved.
Null when nothing is ahead of it.
The draft being replaced is kept for undo, so the two calls mirror each other.

## `public bool LChronicleUndoCheck(long id)`

Whether an undo would step anywhere, so a button can dim before it is pressed.

## `public bool LChronicleRedoCheck(long id)`

Whether a redo would step anywhere.

## `public async Task<CancellationTokenSource?> LChronicleClerkDefer(long id, LRequest request, int delay)`

Files the request for draft `id` and waits out the quiet before its flush.
A later request with the same `LRequestKey` replaces the earlier one and keeps its place.
So the order of first arrival holds, and the last value per field wins.
The filing runs before the first await, so the request is queued when the call returns.
A positive delay cancels the wait in progress and starts a new one on the rig's clock.
A delay of zero files the request only and answers null, leaving the flush to the caller.
The replaced wait is cancelled outside the lock, so its ending runs with no clerk lock held.
The pause always yields before resuming, so an instant clock never flushes under the caller's locks.
It answers the wait that ran out, which the caller hands back to `LChronicleClerkDispatch`.
A cancelled wait answers null, since a newer deferral or a flush has replaced it.

## `public IReadOnlyList<LRequest> LChronicleClerkDispatch(long id, CancellationTokenSource? pending)`

Takes every request waiting for draft `id` and stops its wait.
Given the wait it resumes from, it takes nothing when a newer wait has replaced that one.
Given null, it takes regardless, which is how a flush, a close and a halt empty the set.
The check and the taking share one hold of the lock, so no deferral slips in between them.
The stopped wait is cancelled outside the lock.

## `private LDraft? LChronicleClerkRestore(long id, Dictionary<long, List<LDraft>> source, Dictionary<long, List<LDraft>> target)`

The one step undo and redo share, with the two lists swapped between them.
The newest snapshot of `source` is written as the held draft.
The draft it replaced is kept at the end of `target`.
Null when `source` holds nothing for the draft.
A draft whose file is gone is refused, since there is nothing to restore over.
