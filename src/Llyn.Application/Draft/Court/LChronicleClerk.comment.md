# LChronicleClerk.cs

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

## `private const int LChronicleClerkCap = 100;`

The most snapshots one draft keeps behind it.
Beyond it the oldest snapshot is forgotten, so a long session cannot grow without bound.

## `private const int LChronicleClerkWindow = 1500;`

The milliseconds within which a request of the same type on the same draft continues the last step.

## `public LChronicleClerk(LRig rig)`

Reads the draft port and the clock out of `rig`.
The moment is read from the rig's clock, so a test holds or advances it through the rig.

## `public void LChronicleClerkRecord(LDraft held, LDraft saved, LRequest? request)`

Keeps the draft an edit is about to replace, and forgets the future.
The engine has already judged that the two drafts differ and that one of them is real work.
A new edit after an undo starts a new branch.
A request of the same type on the same draft within the window continues the last step.
Then nothing is pushed and only the moment is refreshed.
A null request is a card edit, which always pushes and leaves no step to continue.

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

## `private LDraft? LChronicleClerkRestore(long id, Dictionary<long, List<LDraft>> source, Dictionary<long, List<LDraft>> target)`

The one step undo and redo share, with the two lists swapped between them.
The newest snapshot of `source` is written as the held draft.
The draft it replaced is kept at the end of `target`.
Null when `source` holds nothing for the draft.
A draft whose file is gone is refused, since there is nothing to restore over.
