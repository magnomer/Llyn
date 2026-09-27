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

## `private bool _lDeskHalted;`

Whether the tenure was halted at the last bulletin.
A start clears it.

## `public event Action? LDeskStarted;`

A tenure was started, handed out so the veneer can attach its marshalling observers to it.
The observers the veneer registered are already on the new tenure when this fires.

## `internal LDesk(LDraftPort drafts, string scope, Func<bool> unreadableSeam)`

Only the controllers that own a desk build one, so the port never reaches a driver.

## `internal event Action<LDraft>? LDeskDraftPrepared;`

The held draft was read again, handed whole to the controllers that own the desk.
It fires before `LDeskDraftChanged`, so a controller's own announcement reaches its driver first.

## `public event Action<CDraft>? LDeskDraftChanged;`

The held draft was read again, so the edit area writes its controls from it.
A driver receives only the Conduct copy.

## `public event Action? LDeskStateChanged;`

The tenure came, went, or changed, so the store and chronicle buttons are read again.

## `public event Action<long>? LDeskFinished;`

The draft was stored under this id, raised after the tenure has been let go.
Only the finish without a stored action raises it.

## `public event Action<string>? LDeskRefused;`

The held tenure stopped taking requests.
It carries the scope's `HoldFailed` key, and it fires once per halt.

## `public QQuill LDeskQuill { get; }`

The gate for this desk's text rows, built over the desk itself.
A driver reaches it through the desk, so no driver builds a text request.

## `public QEasel LDeskEasel { get; }`

The gate for this desk's image and video rows, built over the desk itself.
A driver reaches it through the desk, so no driver builds a media request.

## `internal QErrand LDeskErrand { get; }`

The recording and reading searches over this desk's tenure, built over the desk itself.

## `public QVigil LDeskVigil { get; }`

The observers this desk puts on every tenure it starts, built over the desk itself.

## `public bool LDeskStored`

Whether the held draft stands on a stored record rather than a fresh one.
It reads `LDeskStoredRead`, so a failed read answers false like the tally.

## `public bool LDeskChanged`

Whether the held draft differs from what is stored, as the tenure last reported it, applying nothing first.

## `public bool LDeskStorable`

Whether the held draft has changed and nothing refuses its store.

## `public bool LDeskHalted`

Whether the tenure has stopped taking requests, so the edit area should go dead.

## `public bool LDeskRunning`

A tenure is held and still takes requests, so its edit area stays live.

## `private bool LDeskStalling`

The tenure halted since the last bulletin.

## `internal LTenure? LDeskTenure`

The held tenure, for the editor that reads the draft's language and reflex state from it.

## `internal LDraft? LDeskDraft`

The held draft as it stands, without applying what still waits, or null while nothing is held.
A reader inside a keystroke takes this, so the pending request keeps its delay and raises no bulletin mid-typing.

## `internal void LDeskVistaRestore(LVista vista)`

Takes the vista a start runs over, handed on by the controller that owns the desk.

## `public void LDeskStart(long? id)`

Drops any held tenure and starts one over the record, or over nothing when the id is null.
A refused start is announced under the scope's load key and leaves the desk empty.

## `public void LDeskStart(string origin, CSubject subject, long? id)`

The same start for a desk that holds no vista, naming the origin and the subject itself.
The repertoire and corpus desks start this way, since their vistas list the records rather than the tab.

## `internal LDraft? LDeskRead()`

The held draft after the deferred requests have been applied, or null while nothing is held.

## `public long? LDeskStoredRead()`

The stored id the held draft stands on, or null for a fresh draft, no tenure or a failed read.
A tally reads it, so a failed persist only blanks the count.

## `public CMentionDraft? LDeskMentionFind(long cardId, long sentenceId, string text, int start, int length)`

The Mention a selection in a sentence field lies inside, read from the held draft.
The selection arrives as the field gives it, in UTF-16 units, and the engine measures it.
The find answers from what the engine holds, never from a list the shell keeps.
It reads the draft without persisting, so a context menu asking many times sends nothing.
The tenure keeps the draft it read until the next change, so command checks read no file per keystroke.
A command about to act on the answer persists first, so pending typing cannot shift the offsets.

## `private static CMentionDraft? LDeskMentionRead(LMentionDraft? found)`

Shapes the found Mention through the card's map, or none when the selection lies in no Mention.

## `public void LDeskDraftUpdate()`

Reads the draft again and announces it, unless the announcement itself is what is running.

## `private LDraft? LDeskPrepare()`

The held draft after the deferred requests and the tenure's completion have run, or null while nothing is held.
The completion runs inside the tenure's prepare turn, so the rows it adds raise no draft bulletin.

## `private void LDeskDraftShow(LDraft? draft)`

Announces the draft with the filling flag raised, so the controls' echo is ignored.

## `public void LDeskStateUpdate()`

Announces the change of state, after raising the refusal if the tenure just halted.

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
The unreadable seam travels into the finish, so the tenure asks before it sweeps and the desk decides nothing.

## `public bool LDeskFinish(bool store, Action<long> stored)`

Ends the tenure like `LDeskFinish(bool store)`, and hands a stored id to `stored` instead of the event.
So a caller decides per finish what follows a store, without holding a flag.

## `public void LDeskCancel()`

Lets the held tenure go without storing, announcing the change of state.
