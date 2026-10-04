# LTenure.cs
Hash: `583188d66de803cb`

## `public sealed partial class LTenure`

The engine's hold on one draft, from the start of editing to its commit or its cancel.
A panel holds one tenure and its controls, and nothing else about the draft.
Edits arrive as requests, either deferred behind a short quiet or applied at once.
`LTenureQueue` owns the halt, the end and the apply.
`LChronicleClerk` keeps the deferred requests and times their wait, as the queue hands them over.
The tenure decides when to defer, flush, commit or cancel, and the queue carries it out.
Every step ends by reading the state and raising a tenure bulletin when it moved.
The panel settles its buttons from that bulletin and keeps no dirty flag or halted flag of its own.
A request the engine refuses is dropped, and a draft bulletin sends the panel back to the draft.
A request that fails otherwise halts the tenure, since editing on would collect keystrokes nothing is holding.
Two locks.
The gate guards the queue and the flags, and the turn serialises the applies.
No engine call is made under the gate, because the engine raises bulletins that read the state back.
The clerk's deferral calls under the gate raise nothing and take only the clerk's own lock.

## `private static readonly LTenureState LTenureStateHalted = new(false, null, false, false, true);`

The state answered when the draft check itself fails, since the engine that just failed cannot be asked.

## `private static readonly LTenureState LTenureStateEnded = new(false, null, false, false, false);`

The one state an ended tenure answers, since its draft is gone.

## `private readonly object _lTenureGate = new();`

Guards every hand-over to the clerk, the queue's fault and its ended flag.
The queue takes the same gate, so a check and the change it allows share one hold.

## `private readonly object _lTenureTurn = new();`

Serialises the applies, so the timer's flush and a panel's apply never interleave.
The queue takes the same turn for the timer's flush and for undo and redo.

## `private readonly LEngine _lEngine;`

The engine whose draft this is.

## `private readonly LSubject _lTenureSubject;`

Which kind of record the draft holds, deciding which commit ends it.

## `private readonly LTenureQueue _lTenureQueue;`

The pipeline that queues, flushes and applies this draft's requests, and knows whether the tenure still lives.

## `internal LTenure(LEngine engine, LSubject subject, long id)`

Made by the engine alone, once the draft is started and on disk.
It subscribes its own bulletin handler to the engine here, so the engine names no tenure member to reach it.
The queue is built before the first state reading, since that reading asks the queue whether the tenure lives.

## `public long LTenureId { get; }`

The draft id, which is also the id every draft and tenure bulletin carries.

## `public LDraft? LTenureRead()`

The held draft as the engine has it, or null once the tenure ended.

## `public string LTenureLanguageRead()`

The held draft's language, or empty once the tenure ended.
The editor reads its language here rather than mirroring it in a field of its own.

## `public LTenureState LTenureStateRead()`

The state in one reading, made of changed, refusal, undo, redo and halted.
An ended tenure answers a fixed state without touching the engine.
A halted one still asks whether the draft changed, so a closing window can still warn.
It can neither undo nor redo, since nothing further applies.
A check that fails halts the tenure, because the draft can no longer be reached.
A live tenure reuses its kept state while the engine revision stands, sparing the disk and the database.
Every write to a draft, a stored record or the chronicle moves the revision.
The revision is read before computing, so a change landing mid-reading forces the next reading to compute.
A halted tenure neither keeps nor reuses a state, and a reading that fails keeps nothing.

## `public void LTenureRequestDefer(LRequest request)`

Hands the request to the queue, which files it at the clerk and restarts the quiet before a flush.
A halted or ended tenure drops it.
A delay of zero flushes at once, which is what the tests set.

## `public void LTenureRequestApply(LRequest request)`

Flushes what is waiting and then applies the request now.
For a change with no keystroke coming to end it, such as a chosen language or an added row.

## `public void LTenurePersist()`

Writes what is waiting now, in arrival order, and stops the wait.
It takes the turn even when nothing waits, so the next draft read sees a flush in flight finished.

## `public bool LTenureReadyCheck()`

Whether the draft carries no refusal the engine knows ahead, once the waiting requests are written.
It writes first, so a name typed a moment ago counts.
A save asks it before storing.

## `public bool LTenureChangeCheck()`

Whether the draft differs from what is stored, once the waiting requests are written.
It writes first, so a leave question sees what was typed a moment ago.

## `public bool LTenureStorable`

Whether the draft has changed and carries no refusal, read without writing the queue.
A store button asks it on every repaint, so it must not write.

## `private static bool LTenureReadyCheck(LTenureState state)`

The one owner of the no-refusal rule, shared by the ready check and the storable verdict.

## `public void LTenureHeadwordSet(string text)`

Defers the typed headword.

## `public void LTenureNoteSet(string text)`

Defers the note without the trailing line breaks a text box carries.

## `public static bool LTenureNoteCheck(string text, string note)`

Answers whether a typed text already holds the note, by the same trim the set applies.
A driver asks it before painting, so a line break just typed is not overwritten.

## `private static string LTenureNoteResolve(string text)`

The one owner of the note's trim, read by the set and by the check.

## `public void LTenureLanguageSet(string language)`

Sends the chosen language at once.
An empty choice changes nothing.

## `public LDraft? LTenureUndo()`

Steps the draft one snapshot back, after writing what was waiting.
So the snapshot stepped away from is the one on screen.
`LTenureQueueRestore` is the one path undo and redo share.

## `public LDraft? LTenureRedo()`

Steps the draft one snapshot forward again, the inverse of the undo.

## `public void LTenureSweep()`

Drops the unreadable values of the draft, so a refused commit can be tried again.

## `public void LTenureCancel()`

Discards the draft and ends the tenure, cancelling any search still in flight for it.
The tenure is marked ended before the engine is asked, so a failing discard cannot leave it writing.
A refused workspace is swallowed, since its draft can no longer be reached.
Any other failing discard reaches the caller, since it leaves the draft behind on disk.
The state is raised in a `finally`, so the panel settles its buttons even when the discard throws.
The engine passes over a draft marked stale, so cancelling one is no error.

## `public long? LTenureFinish(bool store, Func<bool> unreadableSeam)`

Ends the tenure, committing the draft or discarding it, and answers the stored record's id.
The facade's `LEngineTenureCommit` runs the commit the subject names.
An unreadable value refuses the commit, and the seam asks the user whether to drop it.
A yes sweeps the draft and finishes once more, and a no lets the refusal out to the desk.
The retry asks nothing, so a draft still unreadable after the sweep refuses for good.
Not storing cancels at once, so nothing waiting is written into a draft about to be dropped.
Otherwise what was waiting is written first, and nothing changed cancels and answers null.
A halted tenure rethrows the failure that halted it, so the panel shows why and stays open.
The failure is rethrown with its original stack, so the panel sees where it began.
The unreadable seam runs from an exception filter while the turn is held, never the gate.
A refused commit leaves the tenure alive over the draft still on disk.
A commit that went through cancels any search still in flight, since the draft it served is gone.
