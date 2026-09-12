# LEngine.cs

## `public sealed partial class LEngine : IDisposable`

The shell engine: the single boundary the UI shell talks to.
The UI sends a request here.
It subscribes through an `LReceiver` for pronunciation or an `LListener` for audio.
It subscribes through an `LObserver` to learn that stored data changed, which `LEngineObserver.cs` owns.
The first two stream one answer to the caller that asked.
The third announces a change to everyone.
All logic lives behind this engine.
That is loading language packs, source fan-out, fetching, parsing, and saving.
So none of it sits in the UI shell.

The engine is serialised behind one gate.
Every public entry point holds a single lock for the whole of its work.
One call at a time touches the workspace, the settings, the database and the rescue report.
The cached sources and the two draft sets are held the same way.
The lock is reentrant, which is what lets an entry point call another one.
A single-writer queue would have kept reads parallel.
Reads here run inside database sessions that are themselves single-threaded.
Parallel reads would have bought nothing and cost a second concurrency model.
The five entry points that return a `Task` hold the gate briefly.
They take only what they need from the engine.
They then run the fetch outside the gate.
A lock held across an await would stall the shell for a whole network call.

This class is also the composition root.
It owns the shared `HttpClient`.
It loads each language pack on first use through `LLanguageLoader`.
It builds that language's transcription and recording sources separately through `LSourceFactory`.
The two sets are cached apart.
A lookup never reaches an audio source and a download never reaches a transcription one.
It holds no source- or language-specific facts of its own: everything language-specific comes from `languages//source.json`.

## `public LEngine()`

Binds the engine to the workspace the pointer file records, creating it on first run.

## `public LEngine(string workspace)`

Binds the engine to `workspace` directly.
It neither reads nor rewrites the recorded workspace pointer.
The folder is given, so there is nothing to resolve.
Changing the user's workspace is `LEngineWorkspaceChange`.
This only says which folder to open.

## `private long LEngineIdentityCreate()`

Issues the next temporary id for a draft row of the open workspace.

The engine owns the issuer, because the floor it counts from belongs to the workspace and changes with it.
It is private because only the engine mints.
A chip the UI builds carries id zero until the next draft save names it.

## `public LRealm LEngineRealmRead()`

The realm of the open workspace, read once when the workspace opened.

## `public IReadOnlyList<string> LEngineLanguageRead()`

Returns the names of the languages that have a pack on disk, for the UI to offer as choices.

## `public async Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)`

Returns the local path to the given language's flag image, for the UI to display beside it.
It returns `null` when the pack declares no flag or the download fails.
The pack declares only an ISO country code.
The engine downloads the matching flag from the flag-icons set and caches it in the workspace.
So the UI never reaches into the `languages/` folder itself.

## `public IReadOnlyList<LVariety> LEngineVarietyRead(string language)`

Returns the regional varieties the language pack declares, in the pack's order.
Empty when the pack declares none, so a language without varieties shows plain rows.

## `public bool LEngineFlaggedCheck(string language)`

Reports whether the pack asks the UI to label a reading's variety by flag rather than by name.

## `public async Task<string?> LEngineVarietyResolve(string language, string variety, CancellationToken cancellation)`

Returns the local path to the flag image of one named variety of the language.
It returns `null` when the pack does not declare that variety, declares no flag for it, or the download fails.
The name is matched exactly, because it is the tag the pack's own readings carry.

## `private async Task<string?> LEngineFlagResolve(string? code, CancellationToken cancellation)`

The shared tail of both flag entry points.
A null or blank code answers null, any other is fetched through the workspace cache.

## `public LDoctorRescue LEngineRescueRead()`

Reports what the workspace doctor had to do to the database this engine opened.
A launch that found the database unusable started a clean one.
The user is owed that news before looking for work that is no longer there.
The engine holds the answer rather than raising it, because the shell asks once the engine exists.
The answer is replaced when `LEngineWorkspaceChange` opens another workspace.

## `public string LEngineWorkspaceRead()`

Returns the current workspace folder — where the user's settings and database are stored.

## `public string? LEngineAuditRecord(Exception exception)`

Writes one unexpected fault into the open workspace's audit log and answers with the file it went to.
The shell shows the user a plain sentence rather than a stack trace.
The trace has to be kept somewhere it can still be read.
It answers `null` when nothing could be written, and the shell then says only the plain sentence.

## `public static string LEngineWorkspaceResolve()`

The workspace folder the pointer file records, creating it on first run — the folder the parameterless constructor opens.
The shell asks for it before building an engine.
So a workspace that fails to open can still be named in the message the user sees.

## `public void LEngineWorkspaceChange(string path)`

Moves the workspace to `path`.
It records the new folder and writes the current settings into it.
So settings and database follow the workspace to its new location.
The drafts this engine claimed are forgotten with the old folder.
A claim only means something against the folder the file sits in.
Each forgotten id is marked stale rather than simply dropped.
A shell still holding one is refused instead of writing here.
The cached source lists go too, because a language keeps whichever lists the workspace it was read from declared.
The move is then announced, so every surface holding a stored record learns that all of it is stale.
One announcement replaces the list of panels the settings panel used to reset by name.
A panel added later is current without that list being edited.

## `public LSettings LEngineSettingsRead()`

Returns the user's persisted settings.

## `public void LEngineLocalizationSave(string language)`

Persists the chosen interface language and keeps it current.
Only that field is written, so a window geometry saved by another part of the shell survives the change.

## `public void LEngineWindowSave(LWindowState window)`

Persists the window geometry of the run that is ending and keeps it current.
Only that field is written, so an interface language chosen during the same session survives the change.

## `public void LEngineVolumeSave(double volume)`

Persists how loud a pronunciation is played and keeps it current.
The level is clamped here as well as on the way in from the file.
No caller can write a volume the player cannot take.
Every view plays through the same level.
A change made in one is the level the next one opens at.

## `private void LEngineSettingsChange(Func<LSettings, LSettings> change)`

Applies `change` to the settings held here and writes the result out, under the engine gate.
The read and the write are one step, so one writer never overwrites what another wrote between them.
The shell therefore never reads settings, changes a field and hands the whole record back.

## `public Task LEnginePronunciationFind(string session, string word, string language, LReceiver receiver, CancellationToken cancellation)`

Starts a pronunciation lookup for `word` in `language` and streams results to `receiver`.
The task completes when every source finishes.
`session` is the draft the asking editor holds, and it names the trove the answer is kept in.
A lookup already answered under that draft is replayed instead of searched again.
So reopening the menu on an unchanged headword costs no network at all.

## `public Task LEngineRecordingFind(string session, string word, string language, LListener listener, CancellationToken cancellation)`

Starts an audio-recording discovery for `word` in `language` and streams results to `listener`.
The task completes when every source finishes.
It reuses what the same draft already found, exactly as the lookup does.

## `private async Task LEngineCandidateScan(string session, string word, string language, IReadOnlyList<LSource> sources, LReceiver receiver, CancellationToken cancellation)`

Runs a real lookup and keeps what it returned in the trove under `session`.
The receiver still sees each candidate stream in, because the search is unchanged.
Nothing is kept when the search is cancelled, since the task then ends by throwing.

## `private async Task LEngineRecordingScan(string session, string word, string language, IReadOnlyList<LSource> sources, LListener listener, CancellationToken cancellation)`

The recording counterpart of `LEngineCandidateScan`.

## `private static Task LEngineCandidatePublish(IReadOnlyList<LCandidate> held, LReceiver receiver)`

Hands a kept answer to the receiver in one pass, then reports the search finished.
The receiver cannot tell a replay from a search, so the menu needs no second path.
The task is already complete, because nothing was awaited.

## `private static Task LEngineRecordingPublish(IReadOnlyList<LRecording> held, LListener listener)`

The recording counterpart of `LEngineCandidatePublish`.

## `public Task<string> LEngineRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation)`

Downloads the chosen `recording` into the workspace and returns the saved path.

## `public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)`

Downloads the `recording` to a temporary file for playback and returns its path.

## Inline notes

### `_lEngineDatabase = new LDatabase(_lEngineWorkspace);`

Initialize the database once the workspace is known.
So the store is ready before any UI request.
The UI never opens the database itself.
The doctor runs the initialization.
A database this build can no longer read costs the user a launch rather than the program.

### `LEngineLanguageImport();`

The controlled vocabularies come from the language packs on disk.
So binding to a workspace is also when they are written into it.
An entry can carry a part of speech from the first save.
The shell never seeds anything.

### `_lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(`

Cambridge (and some Wiktionary edge caches) reject requests without a browser-like agent.

### `_lEngineDatabase = new LDatabase(_lEngineWorkspace);`

The database follows the workspace: initialize one in the new folder.

### `private string LEngineRecordingFormat(string path)`

The workspace-relative form of a downloaded recording's path, which is how it is stored.
A path outside the workspace has no relative form and is stored as it stands.

### `private string LEngineRecordingResolve(string file)`

The full path of a stored recording within the workspace in use now.
A path that was stored absolute — one saved outside the workspace — is returned unchanged.

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
