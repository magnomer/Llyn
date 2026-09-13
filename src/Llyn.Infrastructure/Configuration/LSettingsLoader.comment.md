# LSettingsLoader.cs

## `public static class LSettingsLoader`

Loads and saves the user's `LSettings` as `settings.json` inside a workspace folder.
The workspace folder is supplied by the caller, resolved through `LWorkspaceRoot`.
This loader never decides where the workspace is.
It decides only how the settings file within it is read and written.
A missing or unknown file yields defaults so the program always starts.

## Inline notes

### `Dictionary<string, object> payload = new(StringComparer.Ordinal)`

Persisted keys are a data contract, so they stay lowercase and independent of member names.

### `Math.Clamp(level.GetDouble(), 0, LSettingsLoaderLoudest)`

A volume read from the file is clamped, because a hand-edited number outside the range would otherwise reach the player.
A missing number means full, so a workspace written before this setting existed plays as loudly as it did.

### `flag.ValueKind == JsonValueKind.True;`

The respelling switch is read only as a JSON boolean, and anything else means off.
A missing key means off, so a workspace written before the switch existed shows source transcriptions as before.

### `fetch.ValueKind != JsonValueKind.False;`

The frequency switch defaults on, so only an explicit JSON `false` turns the fetch off.
A missing key means on, so an older workspace starts fetching without being edited.

### `if (settings.LSettingsWindow is LWindowState window)`

A window block is written only once a window has closed and reported its geometry.
Its absence is what a first run looks like, and the window then falls back to its designed size.

### `LWindowLoader.LWindowLoaderRead(block)`

The block's own shape is `LWindowLoader`'s business, so only its presence is decided here.

### `share.ValueKind != JsonValueKind.False;`

The linked switch defaults on, so only an explicit JSON `false` lets tabs keep separate widths.
A missing key means on, so an older workspace starts with its panels linked.

### `if (settings.LSettingsLayout is { Count: > 0 } layout)`

A layout block is written only once some panel has been dragged.
Its absence means every tab still stands at its designed width.

### `LLayoutLoader.LLayoutLoaderRead(panels)`

The block's own shape is `LLayoutLoader`'s business, so only its presence is decided here.
