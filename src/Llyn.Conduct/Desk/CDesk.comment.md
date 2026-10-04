# CDesk.cs
Hash: `1c4b6974eb8a4259`

## `public sealed class CDesk`

The Conduct seat at one tenure, shared by every edit area through its own deportment.
It starts, reads, finishes and cancels the held draft, and applies the requests deferred on the tenure.
The tenure is started over the panel's vista, whose tab and subject name the draft's origin and kind.
The vigil attaches the driver's observers when the tenure starts.
The only question it asks is whether an unreadable draft may be swept, through `CEnvoy`.
It shows its own load, save and hold failures through the same envoy, so no driver can lose one.

## `private bool _cDeskFilling;`

Raised while the draft is being written into the controls, so the echo of that writing defers nothing.
It is read through its verdict and never branched on by name, so the walker sees a verdict.

## `private bool _cDeskHalted;`

Whether the tenure was halted at the last bulletin.
A start clears it.

## `public event Action? CDeskStarted;`

A tenure was started, announced so the owner can refresh what it shows.
The observers a driver registered are already on the new tenure when this fires.

## `internal CDesk(LDraftPort drafts, LSettingsPort settings, string scope, CEnvoy envoy)`

Only the controllers that own a desk build one, so the port never reaches a driver.

## `internal event Action<LDraft>? CDeskDraftPrepared;`

The held draft was read again, handed whole to the controllers that own the desk.
It fires before `CDeskDraftChanged`, so a controller's own announcement reaches its driver first.

## `public event Action<CDraft>? CDeskDraftChanged;`

The held draft was read again, so the edit area writes its controls from it.
A driver receives only the Conduct copy.

## `public event Action? CDeskStateChanged;`

The tenure came, went, or changed, so the store and chronicle buttons are read again.

## `public event Action<long>? CDeskFinished;`

The draft was stored under this id, raised after the tenure has been let go.
Only the finish without a stored action raises it.

## `private readonly LSettingsPort _cDeskSettings;`

The settings port the ledger reads a failure's notice through.

## `private LQuill? _cDeskQuill;`

The text edits of the held tenure, built when a tenure starts and dropped when it ends.

## `private LEasel? _cDeskEasel;`

The media edits of the held tenure, built when a tenure starts and dropped when it ends.

## `private LQuillChip? _cDeskChip;`

The chip row edits of the held tenure, built when a tenure starts and dropped when it ends.

## `internal LQuill? CDeskQuill`

The held tenure's text edits, so no driver builds a text request.
It is null while no tenure is held or while the desk fills its controls.
A fill echoes values the draft already holds, so nothing is written back.

## `internal LEasel? CDeskEasel`

The held tenure's media edits, so no driver builds a media request.
It is null while no tenure is held or while the desk fills its controls.

## `internal LQuillChip? CDeskChip`

The held tenure's chip row edits: tags, situations, registers and translations of a card.
It is null while no tenure is held or while the desk fills its controls.

## `public CErrand CDeskErrand { get; }`

The recording and reading searches over this desk's tenure, built over the desk itself.

## `internal LVigil CDeskVigil { get; }`

The observers this desk puts on every tenure it starts, built over the desk itself.

## `public bool CDeskFilling`

Whether the desk is writing the draft into the controls right now.
The areas that reach the tenure directly withhold it while this holds, so a fill's echo writes nothing back.

## `public long CDeskId`

The held draft's id, not the stored record's, or zero while nothing is held.

## `public bool CDeskStored`

Whether the held draft stands on a stored record rather than a fresh one.
It reads `CDeskStoredRead`, so a refused read answers false.

## `public bool CDeskChanged`

Whether the held draft differs from what is stored, as the tenure last reported it, applying nothing first.

## `public bool CDeskStorable`

Whether the held draft has changed and nothing refuses its store.
The tenure answers the verdict, so the desk combines no state fields itself.

## `public bool CDeskHalted`

Whether the tenure has stopped taking requests, so the edit area should go dead.

## `public bool CDeskRunning`

A tenure is held and still takes requests, so its edit area stays live.

## `private bool CDeskStalling`

The tenure halted since the last bulletin.

## `internal LTenure? CDeskTenure`

The held tenure, for the editor that reads the draft's language and reflex state from it.

## `internal void CDeskVistaRestore(LVista vista)`

Takes the vista a start runs over, handed on by the controller that owns the desk.

## `public void CDeskStart(long? id)`

Drops any held tenure and starts one over the record, or over nothing when the id is null.
A desk with a restored vista starts over it.
Without a vista, a desk built with an origin and a subject starts under them.
Its subject reaches the engine through the panel's map by name, never by cast.
The repertoire and corpus desks start this way, since their vistas list the records rather than the tab.
A refused start is announced under the scope's load key and leaves the desk empty.
Each start announces the change of state once, so listeners refresh the editor only once.
A start that starts nothing still announces when it dropped a held tenure.

## `internal void LDeskOccurrenceStart(long? situation)`

Drops any held tenure and starts a fresh entry over the vista, already linked to the Situation.
The engine starts and links in one call, so the first draft shown carries the link.
A desk with no vista starts nothing.

## `internal void LDeskQuotationStart(long? example)`

The same fresh start for the corpus, already citing the Example in the first sentence.

## `internal void LDeskFootnoteStart(long? reference)`

The same fresh start for the shelf, already citing the Source in the first sentence.

## `internal void LDeskMembershipStart(long? tag)`

The same fresh start for the taxonomy, already carrying the Tag on the first card.

## `internal void LDeskCohortStart(long? register)`

The same fresh start for the tenor panel, already carrying the Register on the first card.

## `internal CDesk(LDraftPort drafts, LSettingsPort settings, string scope, CEnvoy envoy, string origin, CSubject subject)`

Builds a desk that starts under its own origin and subject, so no owner decides the start.

## `internal LDraft? CDeskRead()`

The held draft after the deferred requests have been applied, or null while nothing is held.

## `public long? CDeskStoredRead()`

The stored id the held draft stands on, or null for a fresh draft, no tenure or a refused read.
It makes the one tenure call `LTenureStoredRead`, which persists nothing, so pending edits stay pending.
The tenure answers a refused read with null, so the desk catches nothing itself.

## `public void CDeskDraftResonate()`

Reads the draft again and announces it, unless the announcement itself is what is running.

## `private LDraft? CDeskPrepare()`

The held draft after the deferred requests and the tenure's completion have run, or null while nothing is held.
The completion runs inside the tenure's prepare turn, so the rows it adds raise no draft bulletin.

## `private void CDeskDraftShow(LDraft? draft)`

Announces the draft with the filling flag raised, so the controls' echo is ignored.

## `public void CDeskStateResonate()`

Announces the change of state, after showing the scope's `HoldFailed` notice if the tenure just halted.
The notice shows once per halt.

## `public void CDeskPersist()`

Applies the deferred requests now, as a field is left, unless the controls are being filled.

## `internal bool LDeskChangeCheck()`

Whether the held draft differs from what is stored, after the deferred requests have been applied.
The tenure writes and answers in one call, and an empty desk answers false.

## `internal bool LDeskReadyCheck()`

Whether the held draft may be stored, after the deferred requests have been applied.
Only a store asks it, since it writes.

## `public bool CDeskFinish(bool store)`

Ends the tenure, storing the draft or dropping it as asked, and reports whether it ended.
A failed store is shown under the scope's save key and keeps the tenure, so the user can read why.
The desk chooses the unreadable-drop key, and the tenure asks through `CEnvoy` before it sweeps.

## `public bool CDeskFinish(bool store, Action<long> stored)`

Ends the tenure like `CDeskFinish(bool store)`, and hands a stored id to `stored` instead of the event.
So a caller decides per finish what follows a store, without holding a flag.

## `public void CDeskCancel()`

Lets the held tenure go without storing, announcing the change of state.
It announces nothing when no tenure was held.

## `private bool LDeskTenureClear()`

Lets the held tenure go without announcing, and reports whether one was dropped.
Every start clears through it, so the start announces the change of state once.

## `public (bool CDeskBackward, bool CDeskForward) CDeskChronicleRead()`

Whether the held draft can step back and forward, or neither while nothing is held.

## `public void CDeskUndo()`

Steps the held draft back, then announces the change of state.
A failed step is shown under the scope's `HoldFailed` key and still announces.
`CDeskRedo` steps it forward the same way.

## `private bool CDeskUnreadableConfirm()`

Asks through `CEnvoy` whether an unreadable draft may be dropped.
The key is chosen here, so no driver decides what is asked.

## `public void CDeskObserverAttach(Action<Action> marshal, Action drafted)`

Decides which bulletins refresh the desk, for every tenure it starts.
A tenure bulletin updates the desk state, and a draft bulletin runs `drafted`.
Both run through `marshal`, which the driver hands in to reach its own thread.
The overload without `drafted` rereads and announces the draft through `CDeskDraftResonate`.
The errand takes the same marshal for the steps of its recording search.
