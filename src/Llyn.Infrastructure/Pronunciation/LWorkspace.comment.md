# LWorkspace.cs

## `public static class LWorkspace`

The user's workspace on disk. Downloads remote assets into the workspace and returns their local paths: a chosen recording's bytes saved under a per-language folder, and a language's flag image fetched by country code and cached for reuse. The engine calls these once a source or language is chosen; nothing about which source or language is baked in here.

## `public static async Task<string> LWorkspaceRecordingPrepare(`

Downloads a recording to a temporary cache file for immediate playback and returns its path. Playing a local file is reliable, whereas streaming a remote, token-bearing URL through the media stack is not. The file name derives from the audio's own name, so replaying reuses it.

## `public static async Task<string?> LWorkspaceFlagRead(`

Returns the local path to the flag image for `code` (an ISO 3166-1 alpha-2 country code), downloading it from the flag-icons set into the workspace cache on first use and serving the cached copy thereafter. Returns `null` when the download fails, so a missing flag never blocks the UI.

## Inline notes

### `private const string LWorkspaceFlagHost = "https://cdn.jsdelivr.net/gh/lipis/flag-icons/flags/4x3/";`

The flag-icons set (github.com/lipis/flag-icons), served over jsDelivr's CDN of the repo. The 4x3 SVGs match the flag box's aspect; the leaf is the lowercased ISO 3166-1 alpha-2 code.

### `string directory = Path.Combine(root, LWorkspaceBucket, LWorkspaceNormalize(language));`

Saved audio is workspace data, so it lives under the chosen workspace root, never elsewhere.

### `string directory = Path.Combine(root, LWorkspaceCache);`

Temporary files also stay inside the workspace, under its own temp folder.

### `return "audio" + LWorkspaceExtension;`

Fall through to the default name below.
