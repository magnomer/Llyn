# LSettingsLoader.cs

## `public static class LSettingsLoader`

Loads and saves the user's `LSettings` as `settings.json` inside a workspace folder.
The workspace folder is supplied by the caller, resolved through `LWorkspaceRoot`.
This loader never decides where the workspace is.
It decides only how the settings file within it is read and written.
A missing or unreadable file yields defaults so the program always starts.

## Inline notes

### `Dictionary<string, object> payload = new(StringComparer.Ordinal)`

Persisted keys are a data contract, so they stay lowercase and independent of member names.

### `if (settings.LSettingsWindow is LWindowState window)`

A window block is written only once a window has closed and reported its geometry.
Its absence is what a first run looks like, and the window then falls back to its designed size.

### `LWindowLoader.LWindowLoaderRead(block)`

The block's own shape is `LWindowLoader`'s business, so only its presence is decided here.
