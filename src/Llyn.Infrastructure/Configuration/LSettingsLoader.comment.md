# LSettingsLoader.cs
Hash: `891e0b43424675b8`

## `public sealed class LSettingsLoader : LSettingsVault`

Loads and saves the user's `LSettings` as `settings.json` inside a workspace folder.
The workspace folder is supplied by the caller, resolved through `LWorkspaceRoot`.
This loader never decides where the workspace is.
It decides only how the settings file within it is read and written.
A missing or unknown file yields defaults so the program always starts.
A key it does not know is passed over.
So a file written before the window posture moved out still loads.
An unparseable file is copied aside as `settings.broken.json` first, so the next save does not destroy it.
It is written under a pending name and moved into place, so a crash mid-write leaves the old file whole.

## `public LSettingsLoader(string root)`

Binds the loader to the workspace `root` whose `settings.json` it reads and writes.

## `public bool LSettingsExist()`

Reports whether the workspace already holds a settings file.
The engine asks before moving onto a workspace, so one that has its own preferences keeps them.

## `public LSettings LSettingsRead()`

Valid JSON whose root is not an object reads as defaults and is not copied aside.
So the next save writes over it.
A file that cannot be opened reads as defaults too, so a locked file never stops the start.
It also marks the loader, so a save refuses to write until a later read succeeds.
An unparseable file whose copy aside fails marks the loader the same way.

## `public void LSettingsSave(LSettings settings)`

Writes the settings as JSON through a pending file, so a crash mid-write leaves the old file whole.
A folder that cannot be created or a file that cannot be written raises `LVaultFault` around the system's exception.
After a read that failed to open the file, it raises `LVaultFault` without writing.

## Inline notes

### `Dictionary<string, object> payload = new(StringComparer.Ordinal)`

Persisted keys are a data contract, so they stay lowercase and independent of member names.

### `flag.ValueKind == JsonValueKind.True;`

The respelling switch is read only as a JSON boolean, and anything else means off.
A missing key means off, so a workspace written before the switch existed shows source transcriptions as before.

### `fetch.ValueKind != JsonValueKind.False;`

The frequency switch defaults on, so only an explicit JSON `false` turns the fetch off.
A missing key means on, so an older workspace starts fetching without being edited.

### `custom.ValueKind != JsonValueKind.False;`

The custom analysis switch is read under the `analysis` key and defaults on the same way.
A missing key means on, so an older workspace keeps its custom sheets and marked letters.

### `byname.ValueKind != JsonValueKind.False;`

The epithet switch defaults on the same way, so an older workspace lists its Han characters with their readings.

### `set.ValueKind == JsonValueKind.True;`

The tally switch defaults off like the respelling switch, so an older workspace prints its tallies in IPA.

### `rime.ValueKind == JsonValueKind.True;`

The rime-book box state is read under the `fanqie` key, only as a JSON boolean.
Anything else means closed, so an older workspace shows the box folded as before.

### `writing.ValueKind == JsonValueKind.True;`

The script box state is read under the `script` key the same way, closed unless the file says open.
Both states are set after construction, so the positional fields keep their order.

### `&& speech.GetString() is { Length: > 0 } gloss`

The gloss language is read only as a non-empty JSON string, and anything else keeps the record's default.
A missing key keeps the default, so an older workspace starts new translations in English as before.

### `port.TryGetInt32(out int number) &&`

The Joplin port is read under the `outpost` key, only as a whole number from 1 to 65535.
Anything else means the default Joplin port, so a hand-edited file never aims the push at no port.

### `token.ValueKind == JsonValueKind.String`

The Joplin token is read under the `warrant` key, only as a string.
Anything else means empty, so an older workspace starts with no token set.
