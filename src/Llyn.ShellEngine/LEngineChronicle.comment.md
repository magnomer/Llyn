# LEngineChronicle.cs

## `public sealed partial class LEngine`

Editorial undo and redo over one held draft, kept in memory for the session only.
A chronicle is the ordered snapshots a held draft passed through, walked by undo and redo.
It is not `Revision`, which is the stored commit history, and not `Voyage`, which is the reader's trail of places.
`LDraft` is an immutable record and every edit already ends in one `LDraftArchiveSave`.
So a step is one whole `LDraft`, and no request needs an inverse of its own.
Each draft id keeps a past list and a future list, the newest snapshot at the end of each.
Lists rather than stacks, because trimming the oldest snapshot is a removal at the front.
Every edit that writes a changed draft records the draft it replaced, in `LEngineRequestApply` and `LEngineCardApply`.
The saves that only keep house record nothing, since undoing bookkeeping is not an edit a user made.
Ending a draft, by commit, cancel or delete, drops its chronicle, because the file it walked is gone.
A settled court link drops the owner's chronicle too.
A snapshot from before the id rewrite would resurrect a draft translation id.
The editor already debounces each field, so one step is one pause in typing.
That is still too fine, since a sentence typed slowly becomes ten steps.
So steps that continue the same edit within a short window merge into one.

## `private const int LEngineChronicleCap = 100;`

The most snapshots one draft keeps behind it.
Beyond it the oldest snapshot is forgotten, so a long session cannot grow without bound.

## `private const int LEngineChronicleWindow = 1500;`

The milliseconds within which a request of the same type on the same draft continues the last step.

## `private readonly Dictionary<long, List<LDraft>> _lEngineChroniclePast = [];`

The snapshots an undo can step back to, per draft id, newest last.

## `private readonly Dictionary<long, List<LDraft>> _lEngineChronicleFuture = [];`

The snapshots an undo left, per draft id, that a redo can step forward to again.

## `private (long, Type, DateTimeOffset)? _lEngineChronicleLast;`

The last recorded step: its draft id, its request type and the moment it was last continued.
Null after any undo, redo or clear, so typing after one starts a new step.
Null too after a card edit, which never merges.
The moment is read from the rig's clock port, so a test holds or advances it through the rig.

## `internal LDraft? LEngineChronicleUndo(long id)`

Steps the held draft back one snapshot and returns the draft as saved.
Null when nothing is behind it.
The draft being replaced is kept for redo.
A draft bulletin follows, so every open editor of the draft reloads.

## `internal LDraft? LEngineChronicleRedo(long id)`

Steps the held draft forward one snapshot and returns the draft as saved.
Null when nothing is ahead of it.
The draft being replaced is kept for undo, so the two calls mirror each other.

## `internal bool LEngineUndoCheck(long id)`

Whether an undo would step anywhere, so a button can dim before it is pressed.

## `internal bool LEngineRedoCheck(long id)`

Whether a redo would step anywhere.

## `private void LEngineChronicleRecord(LDraft held, LDraft saved, LRequest? request)`

Keeps the draft an edit is about to replace, and forgets the future.
Nothing is kept when the two drafts match by the measure the dirty check uses.
Nothing is kept either when neither draft differs from what it was started from.
An editor opening a draft adds blank cards, blank sentences and its language before the user types.
Those requests move nothing the user could see undone, so they leave no step behind.
Without that rule the first undo would strip the scaffolding and the reload would put it straight back.
A new edit after an undo starts a new branch.
The snapshots ahead of it no longer follow from anything.
Called under the gate by the two edit sites, just before their save.
A request of the same type on the same draft within the window continues the last step.
Then nothing is pushed and only the moment is refreshed.
Same type means the same field family, so two card titles typed together merge, which is rare and accepted.
A null request is a card edit, which always pushes and leaves no step to continue.

## `private void LEngineChronicleClear(long id)`

Forgets both lists of one draft, and the last step with them.

## `private LDraft? LEngineChronicleRestore(long id, Dictionary<long, List<LDraft>> source, Dictionary<long, List<LDraft>> target)`

The one step undo and redo share, with the two lists swapped between them.
The newest snapshot of `source` is written as the held draft.
The draft it replaced is kept at the end of `target`.
Null when `source` holds nothing for the draft.
