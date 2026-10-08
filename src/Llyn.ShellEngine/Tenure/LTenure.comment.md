# LTenure.cs
Hash: `db77afb7863c374b`

## `public sealed class LTenure`

The engine's hold on one draft, from the start of editing to its commit or its cancel.
A panel holds one tenure and its controls, and nothing else about the draft.
Edits arrive as requests, either deferred behind a short quiet or applied at once.
`LTenureQueue` owns the halt, the end and the apply.
`LChronicleClerk` keeps the deferred requests and times their wait, as the queue hands them over.
The tenure decides when to defer, flush, commit or cancel, and the queue carries it out.
Every step ends with `LTenureGauge` reading the state and raising a tenure bulletin when it moved.
The panel settles its buttons from that bulletin and keeps no dirty flag or halted flag of its own.
A request the engine refuses is dropped, and a draft bulletin sends the panel back to the draft.
A request that fails otherwise halts the tenure, since editing on would collect keystrokes nothing is holding.
Two locks.
The gate guards the queue and the flags, and the turn serialises the applies.
No engine call is made under the gate, because the engine raises bulletins that read the state back.
The clerk's deferral calls under the gate raise nothing and take only the clerk's own lock.
A tenure subscribes to the engine for its lifetime and owns the shell's subject-specific observers.
An observer is a delegate over a bulletin, so the shell hands a method and implements no contract.
The tenure's own handler is a private method attached as a method group and detached by the same group.
General observers receive every notice of their subject, while draft observers require the held draft identity.
Entry observers require a stored entry identity, because frequency and related notices do not name the draft.
Preparation drops the held draft's own notices, because the caller reads the newest draft from the return value.
A replayed notice would show the same draft twice, and the editor's show is its costliest step.
The scope unwinds on failure and returns the newest draft after successful preparation.
Cancel and successful finish detach the engine subscription and release the observer list.

## `private readonly object _lTenureGate = new();`

Guards every hand-over to the clerk, the queue's fault and its ended flag.
The queue, the gauge and the errand take the same gate.
So a check and the change it allows share one hold.

## `private readonly object _lTenureTurn = new();`

Serialises the applies, so the timer's flush and a panel's apply never interleave.
The queue takes the same turn for the timer's flush and for undo and redo.

## `private readonly LEngine _lEngine;`

The engine whose draft this is.

## `private readonly LSubject _lTenureSubject;`

Which kind of record the draft holds, deciding which commit ends it.

## `private readonly LTenureQueue _lTenureQueue;`

The pipeline that queues, flushes and applies this draft's requests, and knows whether the tenure still lives.

## `private readonly LBulletinRoster _lTenureRoster = new(null);`

The observers the shell attached to this tenure, each with the subject and draft identity it asked for.
The roster is read and emptied under the gate, and its callbacks run outside it.

## `private int _lTenurePreparing;`

How many preparations are running now.
While it is above zero, the handler drops the held draft's own draft notices.

## `private LDraft? _lTenureKept;`

The draft the last Mention find read, reused until a draft bulletin or a preparation clears it.

## `private int _lTenureRound;`

Counts the clears of the kept draft, so a read that raced a clear is answered but not kept.

## `internal LTenure(LEngine engine, LSubject subject, long id)`

Made by the engine alone, once the draft is started and on disk.
It subscribes its own bulletin handler to the engine here, so the engine names no tenure member to reach it.
The queue is built before the gauge, since the gauge's first reading asks the queue whether the tenure lives.
The queue's observer skips the gauge while it is still being built, so a failing first reading only halts.

## `public long LTenureId { get; }`

The draft id, which is also the id every draft and tenure bulletin carries.

## `public LTenureGauge LTenureGauge { get; }`

The draft's state reading and its announcement, one per tenure for its whole life.

## `public LErrand LTenureErrand { get; }`

The recording and lookup searches for this draft, and the reading writes they end in.
One per tenure, so its cancel and finish stop the searches it started.

## `internal LEngine LTenureEngine`

The engine the tenure edits through, opened to the quills of this assembly.
A quill's reads ask the engine's lookups, while its edits still go through the tenure's requests.

## `public LDraft? LTenureRead()`

The held draft as the engine has it, or null once the tenure ended.

## `public string LTenureLanguageRead()`

The held draft's language, or empty once the tenure ended.
The editor reads its language here rather than mirroring it in a field of its own.

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
The rule itself is `LTenureGauge.LTenureGaugeCheck`, which the storable verdict shares.
A save asks it before storing.

## `public bool LTenureChangeCheck()`

Whether the draft differs from what is stored, once the waiting requests are written.
It writes first, so a leave question sees what was typed a moment ago.

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

## `public void LTenureObserverAttach(LSubject subject, Action<LBulletin> observer)`

An attach after the tenure ended is dropped, so a late panel holds nothing past cancel or finish.
The draft and entry attaches drop it the same way.

## `public void LTenureDraftAttach(LSubject subject, Action<LBulletin> observer)`

Attaches an observer that receives only the subject's notices that name this tenure's own draft.

## `public void LTenureEntryAttach(LSubject subject, Action<LBulletin> observer)`

The entry id is read at attach time, through the draft's own `LDraftStored` rule.
A draft never stored attaches nothing, and storing it later does not attach it.

## `private void LTenureObserverInsert(LSubject subject, Action<LBulletin> observer, long? id)`

The one place an observer joins the roster, under the gate and only while the tenure lives.
A null id receives every notice of the subject.

## `private void LTenureBulletinHandle(LBulletin bulletin)`

The tenure's own engine subscription, which forwards a notice to the roster.
A draft notice first clears the kept draft, since the draft it holds may be stale.
An ended tenure forwards nothing.
During a preparation, the held draft's own draft notices are dropped.
The roster is read under the gate and dispatched outside it.

## `private const int LTenurePrepareRounds = 3;`

The most rounds of completion a preparation runs.

## `public LDraft? LTenurePrepare()`

The prepare turn with the engine's own completion of an entry draft, so every edit area shows the same rows.
Other subjects complete nothing and only read the draft.

## `private void LTenureDraftPrepare()`

Applies the completion requests and asks again, since a card added in one round needs its sentence in the next.
Three rounds cover cards, then their sentences, then the check that nothing is left.

## `private bool LTenureDraftApply()`

One round: applies what the engine still finds missing and says whether anything was.

## `public LDraft? LTenurePrepare(Action prepare)`

Runs `prepare` holding the turn, so no other turn interleaves with it.
An ended tenure runs nothing and answers null.
The kept draft is dropped afterwards, so a Mention find never trusts a read taken mid-preparation.

## `public long? LTenureStoredRead()`

The stored id the held draft stands on, or null for a fresh draft or an ended tenure.
It persists nothing, so a paint asking for it never forces a save of pending edits.
A refused read, such as a stale draft after a workspace switch, answers null.
Every other failure propagates to the caller.

## `internal LDraft? LTenureKeptRead()`

The held draft as the engine stored it, for the Mention find of `LQuillMention`.
The draft is kept until the next draft bulletin or prepare, so command checks read the file once per change.
A read that raced a bulletin is answered but not kept.
A stale draft after a workspace switch is refused, and the read then answers none.

## `private void LTenureKeptClear()`

Drops the kept draft and advances the round, so a read already under way is not kept.
It runs under the gate, since bulletins can arrive off the veneer's thread.

## `private void LTenureObserverClear()`

Detaches the tenure's engine handler and empties the observer roster.
Cancel and a successful finish call it, so nothing is delivered after the tenure ends.
