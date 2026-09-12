# PEnsign.cs

## `internal static class PEnsign`

Resolves a language pack's cached SVG flag into a frozen drawing, and keeps the resolved flags.
It subscribes to the engine for one announcement only, the workspace moving, and throws the kept flags away on it.
The editor's language picker, the read-only entry display, and every catalog row use it.
A malformed or unknown flag becomes no image, which lets each surface show its neutral globe fallback.

## `internal static void PEnsignAttach(LEngine engine)`

Subscribes the flag store to `engine`, once, for the whole program.
It owns no control, so its answer runs on the thread that announced rather than on the shell's.

## `private static void PEnsignBulletinHandle(LBulletin bulletin)`

Throws every kept flag away when the workspace moves, and answers nothing else.
A flag file is cached inside the workspace.
The path each drawing was read from is gone with the folder.
The surfaces reload on the same announcement, and this store is the first subscriber.
It is empty before they ask.

## `internal static DrawingImage? PEnsignResolve(string path)`

Reads the flag at `path` as a drawing that is frozen and so may be shared across rows.

## `internal static async Task PEnsignLoad(LEngine engine)`

Resolves the flag of every language `engine` offers, and keeps each one under its language.
A language already resolved is left as it is, so a warm call reads nothing.
A catalog row must be built the moment its list is filled, so it cannot wait for a file.
Every surface that shows a flag awaits this first, and then reads without waiting.
A pack installed while the program runs is picked up by the next call.

## `internal static async Task PEnsignVarietyLoad(LEngine engine, string language, IEnumerable<string> varieties)`

Resolves the flag of every named variety of `language`, and keeps each one under `language/variety`.
A variety already resolved is left as it is, so a warm call reads nothing.
The pronunciation menu awaits this before its search, then reads each reading's flag without waiting.
It shares the gate and the generation with the language flags, so a moved workspace discards it the same way.

## `internal static string PEnsignVarietyFormat(string language, string variety)`

The key a variety's flag is kept under, which is its language and its name joined by a slash.
A variety name alone would collide across languages that both declare one.

## `private static async Task<string?> PEnsignRead(LEngine engine, string language)`

The path of one language's flag, or null when the pack declares none.
A pack that is no longer on disk fails here rather than in the surface that asked.

## `private static async Task<string?> PEnsignVarietyRead(LEngine engine, string language, string variety)`

The path of one variety's flag, or null when the pack declares none for it.
A failure to fetch it answers null too, so one missing flag never stops the search.

## `internal static ImageSource? PEnsignFind(string language)`

The flag kept for `language`, or null when none was resolved for it.
A `language/variety` key reads a variety's flag the same way.

## Inline notes

### `private static readonly Dictionary<string, ImageSource?> PEnsignStore = [];`

The flags are held for the program rather than for one panel.
Every tab lists the same languages, and each would otherwise read the same files again.
The store is the lock as well, because it is what every reader and writer touches.

### `private static readonly SemaphoreSlim PEnsignGate = new(1, 1);`

One fill runs at a time.
Several panels ask on the same announcement, and each fill is a set of downloads.
Without this they would each start their own, and the same file would be fetched several times over.
The gate is never held by a plain wait on the shell's thread.
A fill awaiting the shell can always finish.

### `private static int _pEnsignAge;`

Which generation of the store a fill started under.
A workspace may move while a fill is in flight.
Its results then name files in a folder no longer open.
A fill that comes back under a newer generation throws its results away rather than putting them back.

### `string?[] paths = await Task.WhenAll(missing.Select(language => PEnsignRead(engine, language)));`

Every missing flag is asked for at once rather than one after the next.
Each is a first-time download with its own ten-second timeout.
Awaiting them in turn made the caller wait for the sum of them.
