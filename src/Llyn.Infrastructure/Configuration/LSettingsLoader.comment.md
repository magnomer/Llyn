# LSettingsLoader.cs

## `public sealed class LSettingsLoader : LSettingsVault`

Loads and saves the user's `LSettings` as `settings.json` inside a workspace folder.
The workspace folder is supplied by the caller, resolved through `LWorkspaceRoot`.
This loader never decides where the workspace is.
It decides only how the settings file within it is read and written.
A missing or unknown file yields defaults so the program always starts.
A key it does not know is passed over.
So a file written before the window posture moved out still loads.
An unreadable file is copied aside as `settings.broken.json` first, so the next save does not destroy it.
It is written under a pending name and moved into place, so a crash mid-write leaves the old file whole.

## `public LSettingsLoader(string root)`

Binds the loader to the workspace `root` whose `settings.json` it reads and writes.

## `public bool LSettingsExist()`

Reports whether the workspace already holds a settings file.
The engine asks before moving onto a workspace, so one that has its own preferences keeps them.

## Inline notes

### `Dictionary<string, object> payload = new(StringComparer.Ordinal)`

Persisted keys are a data contract, so they stay lowercase and independent of member names.

### `flag.ValueKind == JsonValueKind.True;`

The respelling switch is read only as a JSON boolean, and anything else means off.
A missing key means off, so a workspace written before the switch existed shows source transcriptions as before.

### `fetch.ValueKind != JsonValueKind.False;`

The frequency switch defaults on, so only an explicit JSON `false` turns the fetch off.
A missing key means on, so an older workspace starts fetching without being edited.

### `byname.ValueKind != JsonValueKind.False;`

The epithet switch defaults on the same way, so an older workspace lists its Han characters with their readings.

### `set.ValueKind == JsonValueKind.True;`

The tally switch defaults off like the respelling switch, so an older workspace prints its tallies in IPA.
