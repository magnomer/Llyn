# LEngine.cs

## `public sealed partial class LEngine : IDisposable, LDraftPort, LEntryPort, LPhonologyPort, LSettingsPort, LMediaPort, LPortraitPort`

The shell engine: the single boundary the UI shell talks to.
The UI sends a request here.
It hands a delegate over `LLookupStep` for pronunciation or over `LHarvestStep` for audio.
It subscribes a delegate over `LBulletin` to learn that stored data changed, which `LEngineObserver.cs` owns.
The first two stream one answer to the caller that asked.
The third announces a change to everyone.

The deportment never holds the engine itself.
It holds the six `L*Port` slices under `Port/`, cut by concern so each later clerk implements one.
The engine implements all six, and the veneer window hands it in as each port.

The engine is serialised behind one gate.
Every public entry point holds a single lock for the whole of its work.
The lock is reentrant, which is what lets an entry point call another one.
The entry points that return a `Task` hold the gate briefly and run the fetch outside it.
A lock held across an await would stall the shell for a whole network call.
The clerks that fetch in the background are handed the same gate object.
So a fetch that lands writes under the lock every other vault call holds.
A held draft is driven by an `LTenure`, made in `LEngineTenure.cs`, so no panel sequences the draft calls itself.

The engine holds no vault of its own.
Every port arrives in one `LRig` and is handed to the clerks `LEngineRigSet` builds over it.
The composition root, `App.xaml.cs`, builds the rig through `LRigFactory`, and a test builds one from fakes.
The engine never names the infrastructure and needs no SQLite to start.
All use cases sit in `Llyn.Application` as sealed clerks over the rig, one per concern.
The engine keeps the gate, the observers, the stale marks, the session trove, the settings snapshot and the clerks.
Each part of the engine is a facade that takes the gate and calls a clerk.
The fields are many because each clerk is a plain relay to one concern.
`LEngineRigSet` news them in dependency order and nothing else.

## `public LEngine(LRig rig)`

Binds the engine to the ports of `rig`, already built over the workspace they stand on.
It neither reads nor rewrites the recorded workspace pointer.
Changing the user's workspace is `LEngineRigApply` with a rig over the new folder.
The clerks are built first, then the settings, the rescue and the realm are read through them.
A database this build can no longer read costs the user a launch rather than the program.

## `private void LEngineRigSet(LRig rig)`

Builds every clerk over `rig`, the one place the clerk fields are assigned.
The constructor and `LEngineRigApply` both call it, so a workspace change swaps every clerk at once.
A clerk built over the old rig would keep the old ports, so none survives a rig apply.
The fetching clerks take the gate, the bulletin raiser and a settings reader, which read the engine live.
The order follows the constructor arguments: a clerk is built after every clerk it composes.

## `private void LEngineWorkspaceOpen()`

The three steps a bound workspace needs before the first read.
The controlled vocabularies come from the language packs on disk and are written into the workspace.
The 音韻地位 categories are derived again from the stored placements and the hypothesis on disk.
A workspace rebuilt from an older schema has its derived strings filled once through the workspace clerk.

## `private long LEngineIdentityCreate()`

Issues the next temporary id for a draft row of the open workspace.
The engine owns the issuer, because the floor it counts from belongs to the workspace and changes with it.

## `internal LRealm LEngineRealmRead()`

The realm of the open workspace, read once when the workspace opened.

## `public LDoctorRescue LEngineRescueRead()`

Reports what the workspace doctor had to do to the database this engine opened.
The engine holds the answer rather than raising it, because the shell asks once the engine exists.
The answer is replaced when `LEngineRigApply` opens another workspace.

## `public string LEngineWorkspaceRead()`

Returns the current workspace folder, where the user's settings and database are stored.

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

## `private static bool LEngineOwnerCheck(LOwner owner)`

Which of the two card sides an owner id names, through the card clerk.
True is the Collocation side, and any other side throws as a caller mistake.

## `private static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)`

The one failure for a side an entity has no association table for.
Returned rather than thrown so a switch arm can throw it.

## `public void Dispose()`

Cancels every pending fetch, so a closed engine leaves no task writing into the workspace.
