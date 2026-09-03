# PEnsign.cs

## `internal static class PEnsign`

Resolves a language pack's cached SVG flag into a frozen drawing, and keeps the resolved flags.
The editor's language picker, the read-only entry display, and every catalog row use it.
A malformed or unreadable flag becomes no image, which lets each surface show its neutral globe fallback.

## `internal static DrawingImage? PEnsignResolve(string path)`

Reads the flag at `path` as a drawing that is frozen and so may be shared across rows.

## `internal static async Task PEnsignLoad(LEngine engine)`

Resolves the flag of every language `engine` offers, and keeps each one under its language.
A language already resolved is left as it is, so a warm call reads nothing.
A catalog row must be built the moment its list is filled, so it cannot wait for a file.
Every surface that shows a flag awaits this first, and then reads without waiting.
A pack installed while the program runs is picked up by the next call.

## `private static async Task<string?> PEnsignRead(LEngine engine, string language)`

The path of one language's flag, or null when the pack declares none.
A pack that is no longer on disk fails here rather than in the surface that asked.

## `internal static ImageSource? PEnsignFind(string language)`

The flag kept for `language`, or null when none was resolved for it.

## Inline notes

### `private static readonly Dictionary<string, ImageSource?> PEnsignStore = [];`

The flags are held for the program rather than for one panel.
A flag belongs to a language pack, not to a workspace, so reopening a workspace does not change it.
Every tab lists the same languages, and each would otherwise read the same files again.

### `string?[] paths = await Task.WhenAll(missing.Select(language => PEnsignRead(engine, language)));`

Every missing flag is asked for at once rather than one after the next.
Each is a first-time download with its own ten-second timeout.
Awaiting them in turn made the caller wait for the sum of them.
