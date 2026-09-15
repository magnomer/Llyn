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

This class is also the composition root.
It owns the shared `HttpClient`.
It loads each language pack on first use through `LLanguageLoader`, which `LEngineLanguage.cs` owns.
It keeps each loaded pack by language name, so the file is parsed once and not per lookup.
It builds that language's transcription and recording sources separately through `LSourceFactory`.
The two sets are cached apart.
A lookup never reaches an audio source and a download never reaches a transcription one.
The frequency sources of each language are cached in a third set, which `LEngineFrequency.cs` owns.
Beside them sit the pending fills, one cancellation source per Entry.
A third set names the Entries whose sources answered with nothing this session.
A pending set of entry ids beside it stops the same Entry from filling twice at once.
The morphology sources of each language are cached in a fourth set, which `LEngineInflectionFetch.cs` owns.
Its pending fetches, its missed slots per Entry, and the Entries it lost sit beside them.
They are cleared with the frequency ones.
The script fetches, pending and missed per character, sit in `LEngineScript.cs` and are cleared with them too.
The fanqie fetches sit likewise in `LEngineFanqie.cs`, one at a time with an interval between posts.
The reflex fills sit in `LEngineReflexFetch.cs`, and the rows they store are edited through `LEngineReflex.cs`.
It holds no source- or language-specific facts of its own: everything language-specific comes from `languages//source.json`.

## `public LEngine()`

Binds the engine to the workspace the pointer file records, creating it on first run.

## `public LEngine(string workspace)`

Binds the engine to `workspace` directly.
It neither reads nor rewrites the recorded workspace pointer.
The folder is given, so there is nothing to resolve.
Changing the user's workspace is `LEngineWorkspaceChange`.
This only says which folder to open.

## `internal LEngine(string workspace, HttpClient? client)`

The constructor the public ones share, taking the client every source and download goes through.
A null client builds the real one with its timeout and browser-like agent.
The test suite hands in a client over a stub handler, so an engine-level search runs offline.

## `private long LEngineIdentityCreate()`

Issues the next temporary id for a draft row of the open workspace.

The engine owns the issuer, because the floor it counts from belongs to the workspace and changes with it.
It is private because only the engine mints.
A chip the UI builds carries id zero until the next draft save names it.

## `public LRealm LEngineRealmRead()`

The realm of the open workspace, read once when the workspace opened.

## `public static bool LEngineBusyCheck(Exception fault)`

Whether a launch failed because another program holds the database.
The shell asks so it can say so rather than report a generic failure.
The shell knows no SQLite, so the doctor's answer passes through here.

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

Moves the user onto the workspace at `path` and records it as the one to open next time.
The open comes first and the pointer second, so a folder that fails to open is never pointed at.
The pointer lives outside the workspace, which is why the tests exercise the open alone.

## `public void LEngineWorkspaceOpen(string path)`

Opens the workspace at `path` without recording it as the next one to open.
The path must be fully qualified, so a bare name never lands beside whatever folder the process runs from.
The new database, its rescue, realm and identity are opened before anything here changes.
A folder that cannot be opened therefore leaves the caches and the old database untouched.
A workspace that already holds a settings file is opened on its own settings.
One without any receives the current settings, so a fresh folder starts as the user left the last.
The drafts this engine claimed are forgotten with the old folder.
A claim only means something against the folder the file sits in.
Each forgotten id is marked stale rather than simply dropped.
A shell still holding one is refused instead of writing here.
The cached source lists go too, because a language keeps whichever lists the workspace it was read from declared.
The cached packs go with them, since the source lists were built from those packs.
The move is then announced, so every surface holding a stored record learns that all of it is stale.
One announcement replaces the list of panels the settings panel used to reset by name.
A panel added later is current without that list being edited.

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

### `LEngineDiweiApply();`

The 音韻地位 categories are derived from stored placements and the hypothesis on disk.
So binding to a workspace is also when they are written again.
An edited hypothesis file shows in the tone classes at the next start.

### `private const long LEngineClientCeiling = 8L * 1024 * 1024;`

The most bytes one response may hold before the client refuses it.
No source hands back a page or a recording bigger than that.
An unbounded one would fill memory.

### `_lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(`

Cambridge (and some Wiktionary edge caches) reject requests without a browser-like agent.

### `_lEngineDatabase = new LDatabase(_lEngineWorkspace);`

The database follows the workspace: initialize one in the new folder.

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
