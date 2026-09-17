# LTenure.cs

## `public sealed class LTenure`

The engine's hold on one draft, from the start of editing to its commit or its cancel.
A panel holds one tenure and its controls, and nothing else about the draft.
Edits arrive as requests, either deferred behind a short quiet or applied at once.
Deferred requests queue by `LRequestKey`, so a later request for the same field replaces the earlier one.
Every step ends by reading the state and raising a tenure bulletin when it moved.
The panel settles its buttons from that bulletin and keeps no dirty flag or halted flag of its own.
A request that fails to apply halts the tenure, since editing on would collect keystrokes nothing is holding.
Two locks: the gate guards the queue and the flags, and the turn serialises the applies.
No engine call is made under the gate, because the engine raises bulletins that read the state back.

## `private static readonly LTenureState LTenureStateHalted = new(false, null, false, false, true);`

The state answered when the draft check itself fails, since the engine that just failed cannot be asked.

## `private static readonly LTenureState LTenureStateEnded = new(false, null, false, false, false);`

The one state an ended tenure answers, since its draft is gone.

## `private readonly object _lTenureGate = new();`

Guards the queue, the timer, the fault and the ended flag.

## `private readonly object _lTenureTurn = new();`

Serialises the applies, so the timer's flush and a panel's apply never interleave.

## `private readonly LEngine _lEngine;`

The engine whose draft this is.

## `private readonly LSubject _lTenureSubject;`

Which kind of record the draft holds, deciding which commit ends it.

## `private readonly List<LRequest> _lTenureQueue = [];`

The deferred requests, in arrival order, one per key.

## `private CancellationTokenSource? _lTenurePending;`

The wait that ends in a flush, cancelled by every new deferral.

## `private Exception? _lTenureFault;`

The failure that halted the tenure, or null while it runs.

## `private bool _lTenureEnded;`

Whether the draft has been committed or cancelled, after which the tenure is inert.

## `private LTenureState _lTenureLast;`

The state last announced, so the bulletin is raised only when the state moved.

## `internal LTenure(LEngine engine, LSubject subject, long id)`

Made by the engine alone, once the draft is started and on disk.

## `public long LTenureId { get; }`

The draft id, which is also the id every draft and tenure bulletin carries.

## `public LDraft? LTenureRead()`

The held draft as the engine has it, or null once the tenure ended.

## `public LTenureState LTenureStateRead()`

The state in one reading: changed, refusal, undo, redo and halted.
An ended tenure answers a fixed state without touching the engine.
A halted one still asks whether the draft changed, so a closing window can still warn.
It can neither undo nor redo, since nothing further applies.
A check the engine refuses halts the tenure, because the draft can no longer be reached.

## `public void LTenureRequestDefer(LRequest request)`

Queues the request and restarts the quiet that ends in a flush.
A halted or ended tenure drops it.
A delay of zero flushes at once, which is what the tests set.

## `public void LTenureRequestApply(LRequest request)`

Flushes what is waiting and then applies the request now.
For a change with no keystroke coming to end it, such as a chosen language or an added row.

## `public void LTenurePersist()`

Writes what is waiting now, in arrival order, and stops the wait.
The queue is emptied under the gate and applied outside it.

## `public LDraft? LTenureUndo()`

Steps the draft one snapshot back, after writing what was waiting.
So the snapshot stepped away from is the one on screen.

## `public LDraft? LTenureRedo()`

Steps the draft one snapshot forward again, the inverse of the undo.

## `public void LTenureSweep()`

Drops the unreadable values of the draft, so a refused commit can be tried again.

## `public void LTenureCancel()`

Discards the draft and ends the tenure.
The tenure is marked ended before the engine is asked, so a failing discard cannot leave it writing.
A failure here is swallowed, because the caller is already leaving the work behind.

## `public long? LTenureFinish(bool store)`

Ends the tenure, committing the draft or discarding it, and answers the stored record's id.
What was waiting is written first, whatever the answer was.
Not storing, or nothing changed, cancels and answers null.
A halted tenure rethrows the failure that halted it, so the panel shows why and stays open.
A refused commit leaves the tenure alive over the draft still on disk.

## `private long LTenureCommit()`

The one commit the subject names, answering the id of the stored record.

## `private LDraft? LTenureRestore(Func<long, LDraft?> step)`

The one path undo and redo share.
A halted or ended tenure steps nowhere.

## `private void LTenureRequestInsert(LRequest request)`

Puts the request in the queue, replacing the one waiting under the same key.
The replaced request keeps its place, so the order of first arrival holds.

## `private async Task LTenureRun(CancellationTokenSource pending, int delay)`

Waits out the quiet and then flushes, unless another deferral cancels the wait first.
The wait is not awaited, because the keystroke that started it must return at once.
It resumes on a pool thread, and the engine's observers marshal to their own threads.
A wait that was replaced while resuming flushes nothing, leaving it to the newer wait.

## `private void LTenureStop()`

Cancels the wait in progress, if any.
Called under the gate.

## `private void LTenureApply(IReadOnlyList<LRequest> requests)`

Applies the requests in order and announces the state afterwards.
The first failure halts the tenure and the rest are dropped.
Called under the turn.

## `private void LTenureSuspend(Exception exception)`

Halts the tenure once, keeping the failure for the finish to rethrow.
The wait and the queue are dropped, since nothing further applies.

## `private void LTenureStateRaise()`

Reads the state and raises the tenure bulletin when it differs from the last one raised.
