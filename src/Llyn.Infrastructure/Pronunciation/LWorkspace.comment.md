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
The file sits under `audio/<language>/` and is named by the headword and the recording's variety.
A recording carrying no variety keeps the bare headword name, so a pack without varieties is unchanged.

## `private static string LWorkspaceStemRead(string word, string variety)`

The file stem a saved recording takes, before its extension.
A tagged recording appends the variety after a dot, so `tomato.British.mp3` and `tomato.American.mp3` sit side by side.
An untagged one is the headword alone, so `tomato.mp3` stays what it was.
Both parts are normalized, so a variety name never carries a path character into the file name.

## `public static async Task<string> LWorkspaceRecordingPrepare(`

Downloads a recording to a temporary cache file for immediate playback and returns its path.
Playing a local file is reliable, whereas streaming a remote, token-bearing URL through the media stack is not.
The file is named by a digest of the full address, so two sources' same-named files never collide.
A file already in the cache is returned as it stands, without another fetch.
Rewriting it would fail anyway, since the player keeps the file it last played open.

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
