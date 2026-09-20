# LEngine.cs

## `public sealed partial class LEngine : IDisposable, LDraftPort, LEntryPort, LPhonologyPort, LSettingsPort, LMediaPort, LPortraitPort`

The shell engine: the single boundary the UI shell talks to.
The UI sends a request here.
It subscribes through an `LReceiver` for pronunciation or an `LListener` for audio.
It subscribes through an `LObserver` to learn that stored data changed, which `LEngineObserver.cs` owns.
The first two stream one answer to the caller that asked.
The third announces a change to everyone.
All logic lives behind this engine.
That is loading language packs, source fan-out, fetching, parsing, and saving.
So none of it sits in the UI shell.

The deportment never holds the engine itself.
It holds the six `L*Port` slices under `Port/`, cut by concern so each later clerk implements one.
The engine implements all six, and the veneer window hands it in as each port.

The engine is serialised behind one gate.
Every public entry point holds a single lock for the whole of its work.
One call at a time touches the workspace, the settings, the database and the rescue report.
The settings are read and changed in `LEngineSettings.cs`, under the same gate.
The cached sources and the two draft sets are held the same way.
The lock is reentrant, which is what lets an entry point call another one.
A single-writer queue would have kept reads parallel.
Reads here run inside database sessions that are themselves single-threaded.
Parallel reads would have bought nothing and cost a second concurrency model.
The five entry points that return a `Task` hold the gate briefly.
They take only what they need from the engine.
They then run the fetch outside the gate.
A lock held across an await would stall the shell for a whole network call.
The lookup and recording entry points, with the source caches they read, live in `LEngineRecording.cs`.
A held draft is driven by an `LTenure`, made in `LEngineTenure.cs`, so no panel sequences the draft calls itself.

The engine builds no adapter: every port arrives in one `LRig` and is copied into a field by `LEngineRigSet`.
The composition root, `App.xaml.cs`, builds the rig through `LRigFactory`, and a test builds one from fakes.
The engine never names the infrastructure and needs no SQLite to start.
It loads each language pack on first use through an `LLanguageCache` over the language port.
The cache is shared with the draft clerk, so the file is parsed once and not per lookup.
The use cases that have left the engine sit in `Llyn.Application` as clerks over the same rig.
`LDraftClerk` applies a request to a draft and `LEntryClerk` reads, finds and deletes an entry.
The engine keeps the gate, the observers, the held set and the caches, and calls a clerk under the gate.
It builds that language's transcription and recording sources separately through the source factory.
The two sets are cached apart.
A lookup never reaches an audio source and a download never reaches a transcription one.
The frequency sources of each language are cached in a third set, which `LEngineFrequency.cs` owns.
Beside them sit the pending fills, one cancellation source per Entry.
A third set names the Entries whose sources answered with nothing this session.
A pending set of entry ids beside it stops the same Entry from filling twice at once.
The morphology sources of each language are cached in a fourth set, which `LEngineInflectionFetch.cs` owns.
Its pending fetches sit beside them and are cleared with the frequency ones.
The slots it missed and the Entries it lost are lacuna rows of the workspace, not fields of the engine.
The script fetches, pending and missed per character, sit in `LEngineScript.cs` and are cleared with them too.
The fanqie fetches sit likewise in `LEngineFanqie.cs`, one at a time with an interval between posts.
The reflex fills sit in `LEngineReflexFetch.cs`, and the rows they store are edited through `LEngineReflex.cs`.
It holds no source- or language-specific facts of its own: everything language-specific comes from `languages//source.json`.
The flag cache `_lEngineEnsign` is built in `LEngineRigSet` over the usher the rig hands in.
The usher also answers `LEngineLocation.cs` whether a resolved file is present.

## `public LEngine(LRig rig)`

Binds the engine to the ports of `rig`, already built over the workspace they stand on.
It neither reads nor rewrites the recorded workspace pointer.
The rig names its root, so there is nothing to resolve.
Changing the user's workspace is `LEngineRigApply` with a rig over the new folder.
The settings, the rescue and the realm are read through the ports right after the set.
The doctor runs the initialization.
A database this build can no longer read costs the user a launch rather than the program.

## `private void LEngineRigSet(LRig rig)`

Copies every port of `rig` into its field, the one place the fields are assigned.
The constructor and `LEngineRigApply` both call it, so a workspace change swaps every port at once.
The identity issuer, the language cache, the two clerks and the flag cache are rebuilt here over the same rig.
A clerk built over the old rig would keep the old ports, so none survives a rig apply.

## `private long LEngineIdentityCreate()`

Issues the next temporary id for a draft row of the open workspace.

The engine owns the issuer, because the floor it counts from belongs to the workspace and changes with it.
It is private because only the engine mints.
A chip the UI builds carries id zero until the next draft save names it.

## `internal LRealm LEngineRealmRead()`

The realm of the open workspace, read once when the workspace opened.

## `public LDoctorRescue LEngineRescueRead()`

Reports what the workspace doctor had to do to the database this engine opened.
A launch that found the database unusable started a clean one.
The user is owed that news before looking for work that is no longer there.
The engine holds the answer rather than raising it, because the shell asks once the engine exists.
The answer is replaced when `LEngineRigApply` opens another workspace.

## `public string LEngineWorkspaceRead()`

Returns the current workspace folder — where the user's settings and database are stored.

## `public string LEngineWorkspaceFormat()`

The workspace folder's own name, for the settings ledger, and the full path when the root has none.

## `public string? LEngineAuditRecord(Exception exception)`

Writes one unexpected fault into the open workspace's audit log through the audit port.
It answers with the file the fault went to.
The shell shows the user a plain sentence rather than a stack trace.
The trace has to be kept somewhere it can still be read.
It answers `null` when nothing could be written, and the shell then says only the plain sentence.

## `public void LEngineRigApply(LRig rig)`

Moves the engine onto the workspace `rig` was built over, without touching the workspace pointer.
The caller builds the rig and writes the pointer after.
A folder that fails to open is therefore never pointed at.
The new rig's rescue, realm and settings are read before anything here changes.
A folder that cannot be opened therefore leaves the caches and the old ports untouched.
A workspace that already holds a settings file is opened on its own settings.
One without any receives the current settings and has them written.
A fresh folder therefore starts as the user left the last.
A settings file that exists is not rewritten here.
The posture still reads its legacy window keys from it on the announcement.
The drafts this engine claimed are forgotten with the old folder.
A claim only means something against the folder the file sits in.
Each forgotten id is marked stale rather than simply dropped.
A shell still holding one is refused instead of writing here.
The cached source lists go too, because a language keeps whichever lists the workspace it was read from declared.
The cached packs go with them, since the source lists were built from those packs.
The flag cache is cleared here too, ahead of the announcement, so a surface reloading on it asks afresh.
The move is then announced, so every surface holding a stored record learns that all of it is stale.
One announcement replaces the list of panels the settings panel used to reset by name.
A panel added later is current without that list being edited.

## Inline notes

### `LEngineLanguageImport();`

The controlled vocabularies come from the language packs on disk.
So binding to a workspace is also when they are written into it.
An entry can carry a part of speech from the first save.
The shell never seeds anything.

### `LEngineDiweiApply();`

The 音韻地位 categories are derived from stored placements and the hypothesis on disk.
So binding to a workspace is also when they are written again.
An edited hypothesis file shows in the tone classes at the next start.
The readings stored on the placements are written again by the same pass.

### `if (_lEngineVault.LVaultMigrated)`

A workspace rebuilt from an older schema has its derived strings filled once, here and in the constructor alike.
So the reading view prints stored strings from the first open after a migration.
An ordinary open skips the pass, since every save keeps the strings current.

### `private static bool LEngineOwnerCheck(LOwner owner)`

Which of the two card sides an owner id names.
It is for the entities that hang from a Meaning or a Collocation and from nothing else.
Those are Tags and Situations.
True is the Collocation side.
It is one helper rather than a check repeated per seam, because it is one rule.
The rule is the pair of sides those entities have, and the refusal that meets any other.
A side the entity has no association for is a caller mistake.
It is not a request a user can correct, so it throws rather than refusing.

### `private static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)`

The one failure for a side an entity has no association table for.
Returned rather than thrown so a switch arm can throw it.
That keeps the arm an expression.
It keeps the set of valid sides visible in one place per seam.
