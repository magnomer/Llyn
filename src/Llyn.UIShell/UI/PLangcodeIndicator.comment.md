# PLangcodeIndicator.cs

## `internal static class PLangcodeIndicator`

Resolves a language pack's cached SVG flag into a frozen drawing, and keeps the resolved flags.
The editor's language picker, the read-only entry display, and every catalog row use it.
A malformed or unreadable flag becomes no image, which lets each surface show its neutral globe fallback.

## `internal static DrawingImage? PLangcodeIndicatorResolve(string path)`

Reads the flag at `path` as a drawing that is frozen and so may be shared across rows.

## `internal static async Task PLangcodeIndicatorLoad(LEngine engine)`

Resolves the flag of every language `engine` offers, and keeps each one under its language.
A language already resolved is left as it is.
A catalog row must be built the moment its list is filled, so it cannot wait for a file.
The panel loads the flags once when it is shown, and the rows read them without waiting.

## `internal static ImageSource? PLangcodeIndicatorFind(string language)`

The flag kept for `language`, or null when none was resolved for it.

## Inline notes

### `private static readonly Dictionary<string, ImageSource?> PLangcodeIndicatorStore = [];`

The flags are held for the program rather than for one panel.
A flag belongs to a language pack, not to a workspace, so reopening a workspace does not change it.
Three panels list the same languages, and each would otherwise read the same files again.
