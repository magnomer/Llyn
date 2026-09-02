# LSettingsLoader.cs

## `public static class LSettingsLoader`

Loads and saves the user's `LSettings` as `settings.json` inside a workspace folder.
The workspace folder is supplied by the caller, resolved through `LWorkspaceRoot`.
This loader never decides where the workspace is.
It decides only how the settings file within it is read and written.
A missing or unreadable file yields defaults so the program always starts.

## Inline notes

### `Dictionary<string, string> payload = new(StringComparer.Ordinal)`

Persisted keys are a data contract, so they stay lowercase and independent of member names.
