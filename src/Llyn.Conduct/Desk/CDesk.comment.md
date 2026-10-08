# CDesk.cs
Hash: `c78c6c751713c474`

## `public sealed class CDesk`

The Conduct seat at one tenure, shared by every edit area through its own deportment.
It starts, finishes and cancels the held draft, and applies the requests deferred on the tenure.
The tenure is started over the panel's vista, whose tab and subject name the draft's origin and kind.
The vigil attaches the driver's observers when the tenure starts.
The only question it asks is whether an unreadable draft may be swept, through `CEnvoy`.
It shows its own load and save failures through the same envoy, so no driver can lose one.
Its chronicle shows the hold failures through that envoy too.

## `public event Action? CDeskStarted;`

A tenure was started, announced so the owner can refresh what it shows.
The observers a driver registered are already on the new tenure when this fires.

## `internal CDesk(LDraftPort drafts, LSettingsPort settings, string scope, CEnvoy envoy)`

Only the controllers that own a desk build one, so the port never reaches a driver.

## `public event Action? CDeskStateChanged;`

The tenure came, went, or changed, so the store and chronicle buttons are read again.
Every chronicle announcement is raised again here, so listeners hear one event.

## `public event Action<long>? CDeskFinished;`

The draft was stored under this id, raised after the tenure has been let go.
Only the finish without a stored action raises it.

## `private readonly LSettingsPort _cDeskSettings;`

The settings port the ledger reads a failure's notice through.

## `public CDeskDraft CDeskDraft { get; }`

The held tenure with the quills built over it, and the draft announcements.
The desk sets and clears it on start, finish and cancel, so callers only read it.

## `public CDeskChronicle CDeskChronicle { get; }`

The undo and redo steps of the held draft, and the halt it reports.
It reads the tenure through `CDeskDraft`, so the draft keeps the only hold.

## `public CErrand CDeskErrand { get; }`

The recording and reading searches over this desk's tenure, built over the desk itself.

## `internal LVigil CDeskVigil { get; }`

The observers this desk puts on every tenure it starts, built over the desk itself.

## `public long CDeskId`

The held draft's id, not the stored record's, or zero while nothing is held.

## `public bool CDeskStored`

Whether the held draft stands on a stored record rather than a fresh one.
It reads `CDeskStoredRead`, so a refused read answers false.

## `public bool CDeskHeld`

Whether a tenure is held, whether or not it still takes requests.

## `private void CDeskStartRun(Func<LTenure> start)`

Runs the given start and hands the tenure it returns to `LDeskDraftSet`, which builds the quills over it.
It clears the chronicle's halt through `LDeskChronicleClear`, so a new tenure starts unhalted.
The vigil applies its observers to the new tenure before the start is announced.
A failing start leaves the desk empty and shows the scope's `LoadFailed` notice.

## `public void CDeskObserverAttach(Action<Action> marshal)`

Attaches the observers with the draft holder's `CDeskDraftResonate` as the draft action.

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

## `internal void LDeskRun(Func<LDraftPort, LVista, LTenure> start)`

Drops any held tenure and runs `start` over the desk's port and vista.
The panels hand in their own engine start, such as an Occurrence linked to its Situation.
The engine starts and links in one call, so the first draft shown carries the link.
A desk with no vista starts nothing, and announces only when it dropped a held tenure.

## `internal CDesk(LDraftPort drafts, LSettingsPort settings, string scope, CEnvoy envoy, string origin, CSubject subject)`

Builds a desk that starts under its own origin and subject, so no owner decides the start.

## `public long? CDeskStoredRead()`

The stored id the held draft stands on, or null for a fresh draft, no tenure or a refused read.
It makes the one tenure call `LTenureStoredRead`, which persists nothing, so pending edits stay pending.
The tenure answers a refused read with null, so the desk catches nothing itself.

## `internal bool LDeskChangeCheck()`

Whether the held draft differs from what is stored, after the deferred requests have been applied.
The tenure writes and answers in one call, and an empty desk answers false.

## `internal bool LDeskReadyCheck()`

Whether the held draft may be stored, after the deferred requests have been applied.
`LTenureReadyCheck` writes them itself, so the desk adds no write of its own.
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

## `private bool CDeskUnreadableConfirm()`

Asks through `CEnvoy` whether an unreadable draft may be dropped.
The key is chosen here, so no driver decides what is asked.

## `internal void LDeskFailureShow(string key, Exception exception)`

Shows a failure caught by a Conduct helper that holds this desk, such as `CErrand` or `CByline`.
`key` is the suffix after the scope, such as `.RecordingFailed`, so each area's key keeps its prefix.
It routes through the desk's own envoy and settings with `CLedger.LLedgerFailureShow`.
It answers a user act, so every failure shows.

## `internal void LDeskRepaintShow(CLedgerNoticed noticed, string key, Exception exception)`

Shows a failure caught by a helper's repaint read, such as `CByline`'s search, through `noticed`.
`key` is the suffix after the scope, as for `LDeskFailureShow`.
The memory belongs to the atelier, not the desk, so the helper hands it in.
A key already shown is merged until the user acts.

## `public void CDeskObserverAttach(Action<Action> marshal, Action drafted)`

Decides which bulletins refresh the desk, for every tenure it starts.
A tenure bulletin runs the chronicle's `CDeskChronicleResonate`, and a draft bulletin runs `drafted`.
Both run through `marshal`, which the driver hands in to reach its own thread.
The overload without `drafted` rereads and announces the draft through `CDeskDraft.CDeskDraftResonate`.
The errand takes the same marshal for the steps of its recording search.
