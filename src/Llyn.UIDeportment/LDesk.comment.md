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

## `public event Action<LTenure>? LDeskStarted;`

A tenure was started, handed out so the veneer can attach its marshalling observers to it.

## `public event Action<LDraft>? LDeskDraftChanged;`

The held draft was read again, so the edit area writes its controls from it.

## `public event Action? LDeskStateChanged;`

The tenure came, went, or changed, so the store and chronicle buttons are read again.

## `public event Action<long>? LDeskFinished;`

The draft was stored under this id, raised after the tenure has been let go.

## `public bool LDeskStored`

Whether the held draft stands on a stored record rather than a fresh one.

## `public void LDeskStart(long? id)`

Drops any held tenure and starts one over the record, or over nothing when the id is null.
A refused start is announced under the scope's load key and leaves the desk empty.

## `public LDraft? LDeskRead()`

The held draft after the deferred requests have been applied, or null while nothing is held.

## `public void LDeskDraftUpdate()`

Reads the draft again and announces it, unless the announcement itself is what is running.

## `private void LDeskDraftShow(LDraft? draft)`

Announces the draft with the filling flag raised, so the controls' echo is ignored.

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
