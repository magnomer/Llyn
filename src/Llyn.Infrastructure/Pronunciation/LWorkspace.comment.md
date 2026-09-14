# LWorkspace.cs

## `public static class LWorkspace`

The user's workspace on disk.
Downloads remote assets into the workspace and returns their local paths.
A chosen recording's bytes are saved under a per-language folder.
A language's flag image is fetched by country code and cached for reuse.
The engine calls these once a source or language is chosen.
Nothing about which source or language is baked in here.

## `public static async Task<string> LWorkspaceRecordingSave(`

Downloads a chosen recording into the workspace and returns the saved path.
The file sits under `audio/<language>/` and is named by the headword, the recording's variety and its address.
A recording carrying no variety leaves the variety part out, so a pack without varieties reads the same.

## `private static async Task LWorkspaceFileSave(string path, byte[] content, CancellationToken cancellation)`

Writes the bytes beside the target and moves them over it in one step.
A kill mid-write leaves a `.tmp` file, never a truncated recording or flag that would be served forever.

## `private static string LWorkspaceStemRead(string word, string variety, string address)`

The file stem a saved recording takes, before its extension.
A tagged recording appends the variety after a dot, so `tomato.British` and `tomato.American` sit side by side.
A short digest of the address ends the stem, so `Weg` and `weg` never land on one file.
Windows folds case in file names, and two words that differ only by case are still two words.
The same digest keeps two homographs apart when their recordings differ.
One recording fetched twice lands on one file.
Both parts are normalized, so a variety name never carries a path character into the file name.

## `private static string LWorkspaceDigestRead(string address)`

The hex digest of an address, the identity every cached or saved file is named by.

## `public static async Task<string> LWorkspaceRecordingPrepare(`

Downloads a recording to a temporary cache file for immediate playback and returns its path.
Playing a local file is reliable, whereas streaming a remote, token-bearing URL through the media stack is not.
The file is named by a digest of the full address, so two sources' same-named files never collide.
A file already in the cache is returned as it stands, without another fetch.
Rewriting it would fail anyway, since the player keeps the file it last played open.

## `private static string LWorkspaceExtensionRead(string address)`

The extension the saved file takes, read from the address and kept only when it names an audio format.
Anything else falls back to `.mp3`, so a source cannot name a file `.exe` or `.html` on disk.

## `private static string LWorkspaceNormalize(string value)`

A value made safe as one file name segment.
Path characters become underscores.
A name Windows reserves for a device, such as `con` or `nul`, is prefixed.
The file could not be created otherwise.
The reservation covers the part before the first dot, which is why the check reads only that far.

## `public static async Task<string?> LWorkspaceFlagRead(`

Returns the local path to the flag image for `code`, an ISO 3166-1 alpha-2 country code.
It downloads it from the flag-icons set into the workspace cache on first use.
It serves the cached copy thereafter.
Returns `null` when the download fails, so a missing flag never blocks the UI.

## Inline notes

### `private const string LWorkspaceFlagHost = "https://cdn.jsdelivr.net/gh/lipis/flag-icons/flags/4x3/";`

The flag-icons set (github.com/lipis/flag-icons), served over jsDelivr's CDN of the repo.
The 4x3 SVGs match the flag box's aspect.
The leaf is the lowercased ISO 3166-1 alpha-2 code.

### `string directory = Path.Combine(root, LWorkspaceBucket, LWorkspaceNormalize(language));`

Saved audio is workspace data, so it lives under the chosen workspace root, never elsewhere.

### `string directory = Path.Combine(root, LWorkspaceCache);`

Temporary files also stay inside the workspace, under its own temp folder.

### `return "audio" + LWorkspaceExtension;`

Fall through to the default name below.
