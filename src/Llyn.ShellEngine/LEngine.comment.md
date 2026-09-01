# LEngine.cs

## `public sealed partial class LEngine : IDisposable`

The shell engine: the single boundary the UI shell talks to. The UI sends a request here and subscribes through an `LReceiver` (pronunciation) or `LListener` (audio); all logic — loading language packs, source fan-out, fetching, parsing, saving — lives behind this engine, so none of it sits in the UI shell.

This class is also the composition root. It owns the shared `HttpClient`, loads each language pack on first use through `LLanguageLoader`, and builds that language's sources through `LSourceFactory`. It holds no source- or language-specific facts of its own: everything language-specific comes from `languages//source.json`.

## `public LEngine()`

Binds the engine to the workspace the pointer file records, creating it on first run.

## `public LEngine(string workspace)`

Binds the engine to `workspace` directly, without reading or rewriting the recorded workspace pointer — the folder is given, so there is nothing to resolve. Changing the user's workspace is `LEngineWorkspaceChange`; this only says which folder to open.

## `public IReadOnlyList<string> LEngineLanguageRead()`

Returns the names of the languages that have a pack on disk, for the UI to offer as choices.

## `public async Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)`

Returns the local path to the given language's flag image, for the UI to display beside the language, or `null` when the pack declares no flag or the download fails. The pack declares only an ISO country code; the engine downloads the matching flag from the flag-icons set and caches it in the workspace, so the UI never reaches into the `languages/` folder itself.

## `public string LEngineWorkspaceRead()`

Returns the current workspace folder — where the user's settings and database are stored.

## `public static string LEngineWorkspaceResolve()`

The workspace folder the pointer file records, creating it on first run — the folder the parameterless constructor opens. The shell asks for it before building an engine, so a workspace that fails to open can still be named in the message the user sees.

## `public void LEngineWorkspaceChange(string path)`

Moves the workspace to `path`: records the new folder and writes the current settings into it, so settings and database follow the workspace to its new location.

## `public LSettings LEngineSettingsRead()`

Returns the user's persisted settings.

## `public void LEngineSettingsSave(LSettings settings)`

Persists `settings` into the current workspace and keeps them current.

## `public LWorkspaceState LEngineStateRead()`

Opens the workspace row — which Entry each pane shows, the display mode, the split, and the current revision — creating it on first use so the UI always receives a state.

## `public void LEngineStateSave(LWorkspaceState state)`

Writes `state` back into the workspace row.

## `public Task LEnginePronunciationFind(string word, string language, LReceiver receiver, CancellationToken cancellation)`

Starts a pronunciation lookup for `word` in `language` and streams results to `receiver`. The task completes when every source finishes.

## `public Task LEngineRecordingFind(string word, string language, LListener listener, CancellationToken cancellation)`

Starts an audio-recording discovery for `word` in `language` and streams results to `listener`. The task completes when every source finishes.

## `public Task<string> LEngineRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation)`

Downloads the chosen `recording` into the workspace and returns the saved path.

## `public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)`

Downloads the `recording` to a temporary file for playback and returns its path.

## Inline notes

### `_lEngineDatabase = new LDatabase(_lEngineWorkspace);`

Initialize the database once the workspace is known, so the store is ready before any UI request and the UI never opens the database itself.

### `LEngineLanguageImport();`

The controlled vocabularies come from the language packs on disk, so binding to a workspace is also when they are written into it: an entry can carry a part of speech from the first save, without the shell ever seeding anything.

### `_lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(`

Cambridge (and some Wiktionary edge caches) reject requests without a browser-like agent.

### `_lEngineDatabase = new LDatabase(_lEngineWorkspace);`

The database follows the workspace: initialize one in the new folder.

### `private string LEngineRecordingFormat(string path)`

The workspace-relative form of a downloaded recording's path, which is how it is stored. A path outside the workspace has no relative form and is stored as it stands.

### `private string LEngineRecordingResolve(string file)`

The full path of a stored recording within the workspace in use now. A path that was stored absolute — one saved outside the workspace — is returned unchanged.

### `private static bool LEngineOwnerCheck(LOwner owner)`

Which of the two card sides an owner id names, for the entities that hang from a Meaning or a Collocation and from nothing else — Tags and Situations. True is the Collocation side. It is one helper rather than a check repeated per seam because it is one rule: the pair of sides those entities have, and the refusal that meets any other side. A side the entity has no association for is a caller mistake, not a request a user can correct, so it throws rather than refusing.

### `private static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)`

The one failure for a side an entity has no association table for. Returned rather than thrown so a switch arm can throw it, which keeps the arm an expression and the set of valid sides visible in one place per seam.
