# LEngine.cs

## `public sealed class LEngine : IDisposable`

The shell engine: the single boundary the UI shell talks to.
The UI sends a request here.
It hands a delegate over `LLookupStep` for pronunciation or over `LHarvestStep` for audio.
It subscribes a delegate over `LBulletin` to learn that stored data changed.
The first two stream one answer to the caller that asked.
The third announces a change to everyone.

The deportment never holds the engine itself.
It holds the six `L*Port` slices under `Port/`, cut by concern so each later clerk implements one.
Each sealed `L*Outlet` implements one port and forwards its members to this engine.
The veneer window hands those outlets in.

The engine is serialised behind `LEngineGate`.
Every public entry point holds a single lock for the whole of its work.
The lock is reentrant, which is what lets an entry point call another one.
The entry points that return a `Task` hold the gate briefly and run the fetch outside it.
A lock held across an await would stall the shell for a whole network call.
The clerks that fetch in the background are handed the same gate object.
So a fetch that lands writes under the lock every other vault call holds.
A held draft is driven by an `LTenure`, made in `LEngineTenure.cs`, so no panel sequences the draft calls itself.

The engine holds no vault of its own.
Every port arrives in one `LRig` and is handed to the staff `LEngineStaffBuild` builds over it.
The composition root, `App.xaml.cs`, builds the rig through `LRigFactory`, and a test builds one from fakes.
The engine never names the infrastructure and needs no SQLite to start.
All use cases sit in `Llyn.Application` as sealed clerks over the rig, one per concern.
The core keeps the gate, the staff, shared state, observers and one property per facade.
Each sealed facade holds the engine and uses its gate to call a clerk.
`LEngineStaff` keeps the clerks in dependency order and swaps them together when the rig changes.

## `public LEngine(LRig rig)`

Binds the engine to the ports of `rig`, already built over the workspace they stand on.
It neither reads nor rewrites the recorded workspace pointer.
Changing the user's workspace is `LEngineRigApply` with a rig over the new folder.
The clerks are built first, then the settings, the rescue and the realm are read through them.
The facades come last, so none of them can ever see an engine without its staff.
A database this build can no longer read costs the user a launch rather than the program.

## `internal LVistaFacade LEngineVista`

Owns the vista registry and serves the entry and favorite rows shown by each catalog tab.

## `internal LEngineStaff LEngineStaffHeld`

Returns the current staff record for engine facades that need a clerk.
The record changes as one unit when `LEngineRigApply` moves to another workspace.
It is read under the gate, so a caller outside the gate still sees one whole record.

## `internal LSettings LEngineSettingsRead()`

The current settings snapshot, read under the gate.
The fetch clerks hold this reader, so it exists before any facade does.

## `internal LDraftFacade LEngineDraft`

The draft facade, built once with the others like every facade.

## `private void LEngineWorkspaceOpen()`

The three steps a bound workspace needs before the first read.
The controlled vocabularies come from the language packs on disk and are written into the workspace.
The 音韻地位 categories are derived again from the stored placements and the hypothesis on disk.
A workspace rebuilt from an older schema has its derived strings filled once through the workspace clerk.

## `internal long LEngineIdentityCreate()`

Issues the next temporary id for a draft row of the open workspace.
The engine owns the issuer, because the floor it counts from belongs to the workspace and changes with it.

## `internal object LEngineGate`

The shared lock every facade and fetching clerk uses for engine state.
Callers hold it while reading or changing the state below.

## `internal LTrove LEngineTrove`

The engine session cache, shared with the facades that read and clear it.

## `internal LSettings LEngineSettingsHeld`

The current settings snapshot.
Callers read and update it under `LEngineGate`.

## `internal HashSet<long> LEngineDraftStale`

Draft ids claimed in a former workspace, kept so a surviving tenure cannot write into the new one.

## `public void LEngineObserverAttach(Action<LBulletin> observer)`

Subscribes `observer` to future announcements.
An already attached delegate is not added a second time.

## `public void LEngineObserverDetach(Action<LBulletin> observer)`

Stops announcing to `observer`, so a closed surface is never called again.

## `internal void LEngineBulletinRaise(LSubject subject, long id)`

Announces one stored change to every subscriber.
The list is copied under the gate, then callbacks run outside it, so a subscriber can detach without disturbing iteration.
Announcements happen when a stored record is finished, so an import announces once after all its records are written.

## `internal LRealm LEngineRealmRead()`

The realm of the open workspace, read once when the workspace opened.

## `public LDoctorRescue LEngineRescueRead()`

Reports what the workspace doctor had to do to the database this engine opened.
The engine holds the answer rather than raising it, because the shell asks once the engine exists.
The answer is replaced when `LEngineRigApply` opens another workspace.

## `public string LEngineWorkspaceRead()`

Returns the current workspace folder, where the user's settings and database are stored.
The folder is kept apart from the workspace facade, which serves the workspace row.

## `public string LEngineWorkspaceFormat()`

The workspace folder's own name, for the settings ledger, and the full path when the root has none.

## `public string? LEngineAuditRecord(Exception exception)`

Writes one unexpected fault into the open workspace's audit log through the workspace clerk.
It answers with the file the fault went to, or `null` when nothing could be written.

## `public string? LEngineNoticeRead(Exception exception)`

The reason key of a refusal standing anywhere inside the failure, or null for a fault.
The workspace clerk walks the inner chain, since the shells name no exception type.

## `public void LEngineRigApply(LRig rig)`

Moves the engine onto the workspace `rig` was built over, without touching the workspace pointer.
The caller builds the rig and writes the pointer after.
The new rig's rescue, realm and settings are read through the clerk's static open steps before anything here changes.
A folder that cannot be opened therefore leaves the old clerks and caches untouched.
A workspace that already holds a settings file is opened on its own settings.
One without any receives the current settings and has them written.
The drafts this engine claimed are forgotten with the old folder and each id is marked stale.
A shell still holding one is refused instead of writing here.
The pending fetches of the old clerks are cancelled, and the flag cache and the trove are cleared.
The move is then announced, so every surface holding a stored record learns that all of it is stale.

## `private void LEngineFetchClear()`

Cancels every pending fetch of the five fetching clerks, on a rig apply and on dispose.

## `internal static bool LEngineOwnerCheck(LOwner owner)`

Which of the two card sides an owner id names, through the card clerk.
True is the Collocation side, and any other side throws as a caller mistake.

## `internal static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)`

The one failure for a side an entity has no association table for.
Returned rather than thrown so a switch arm can throw it.

## `public void Dispose()`

Cancels every pending fetch, so a closed engine leaves no task writing into the workspace.
