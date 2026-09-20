# LDesk.cs

## `public sealed class LDesk`

The panel-side seat at one tenure, shared by every edit area through its own deportment.
It starts, reads, finishes and cancels the held draft, and defers the field requests the veneer forwards.
The tenure is started over the panel's vista, whose tab and subject name the draft's origin and kind.
The veneer attaches its observers when the tenure starts, so bulletins reach the desk on the veneer's thread.
The only question it asks is whether an unreadable draft may be swept, through a seam the veneer hands in.

## `private bool _lDeskFilling;`

Raised while the draft is being written into the controls, so the echo of that writing defers nothing.
It is read through its verdict and never branched on by name, so the walker sees a verdict.

## `public event Action? LDeskStarted;`

A tenure was started, handed out so the veneer can attach its marshalling observers to it.
The observers the veneer registered are already on the new tenure when this fires.

## `public void LDeskDraftAttach(LSubject subject, Action<LBulletin> observer)`

Registers an observer the desk puts on every tenure it starts, for the draft-level subjects.
An observer is a delegate over a bulletin, forwarded as it came, so the desk holds no contract.
The veneer registers once at attach time and never sees the tenure, which is what re-attaches per start.
A tenure already held gets the observer at once.
`LDeskObserverAttach` and `LDeskEntryAttach` do the same for the tenure-wide and entry-level subjects.

## `public event Action<LDraft>? LDeskDraftChanged;`

The held draft was read again, so the edit area writes its controls from it.

## `public event Action? LDeskStateChanged;`

The tenure came, went, or changed, so the store and chronicle buttons are read again.

## `public event Action<long>? LDeskFinished;`

The draft was stored under this id, raised after the tenure has been let go.

## `public bool LDeskStored`

Whether the held draft stands on a stored record rather than a fresh one.

## `public bool LDeskChanged`

Whether the held draft differs from what is stored, as the tenure last reported it, applying nothing first.

## `public bool LDeskStorable`

Whether the held draft has changed and nothing refuses its store.

## `public bool LDeskHalted`

Whether the tenure has stopped taking requests, so the edit area should go dead.

## `internal LTenure? LDeskTenure`

The held tenure, for the clip and notation deportments that start forays over it.

## `public void LDeskStart(long? id)`

Drops any held tenure and starts one over the record, or over nothing when the id is null.
A refused start is announced under the scope's load key and leaves the desk empty.

## `public void LDeskStart(string origin, LSubject subject, long? id)`

The same start for a desk that holds no vista, naming the origin and the subject itself.
The repertoire and corpus desks start this way, since their vistas list the records rather than the tab.

## `public LForay? LDeskRecordingStart(string word, long target, Action<LHarvestStep> sink)`

Starts a recording search over the held tenure, or nothing while none is held.
The sink is the veneer's delegate over each step, handed through untouched.
`LDeskTranscriptionStart` starts a reading search the same way, under one scheme.

## `public LDraft? LDeskRead()`

The held draft after the deferred requests have been applied, or null while nothing is held.

## `public void LDeskDraftUpdate()`

Reads the draft again and announces it, unless the announcement itself is what is running.

## `private LDraft? LDeskPrepare()`

The held draft after the deferred requests and the tenure's completion have run, or null while nothing is held.
The completion runs inside the tenure's prepare turn, so the rows it adds raise no draft bulletin.

## `private void LDeskDraftShow(LDraft? draft)`

Announces the draft with the filling flag raised, so the controls' echo is ignored.

## `public void LDeskPersist()`

Applies the deferred requests now, as a field is left, unless the controls are being filled.

## `public void LDeskDefer(LRequest request)`

Hands a field request to the tenure to apply in its own time, unless the controls are being filled.

## `public void LDeskSend(LRequest request)`

Hands a request to the tenure to apply at once, unless the controls are being filled.

## `public bool LDeskChangeCheck()`

Whether the held draft differs from what is stored, after the deferred requests have been applied.

## `public bool LDeskFinish(bool store)`

Ends the tenure, storing the draft or dropping it as asked, and reports whether it ended.
A refused store is announced under the scope's save key and keeps the tenure, so the user can read why.

## `private long? LDeskCommitRun(LTenure held, bool store)`

The finish itself, retried once after a sweep when the draft is unreadable and the seam allows it.

## `public void LDeskCancel()`

Lets the held tenure go without storing, announcing the change of state.
