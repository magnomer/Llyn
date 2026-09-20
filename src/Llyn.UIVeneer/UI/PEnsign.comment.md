# PEnsign.cs

## `internal static class PEnsign`

Resolves a language pack's cached SVG flag into a frozen drawing, and keeps the resolved drawings by key.
Which key was asked, the path it resolved to, and the age a moved workspace outran are `LEnsign`'s.
It subscribes to the engine for one announcement only, the workspace moving, and throws the kept drawings away on it.
The editor's language picker, the read-only entry display, and every catalog row use it.
A malformed or unknown flag becomes no image, which lets each surface show its neutral globe fallback.

## `internal static void PEnsignAttach(LWindow window)`

Subscribes the flag store through `window`, once, for the whole program.
It owns no control, so its answer runs on the thread that announced rather than on the shell's.

## `private static void PEnsignBulletinHandle(LBulletin bulletin)`

Throws every kept drawing away when the workspace moves, and answers nothing else.
The engine has already forgotten the paths before it announces, so a surface reloading on the announcement asks afresh.
The surfaces reload on the same announcement, and this store is the first subscriber.
It is empty before they ask.

## `internal static DrawingImage? PEnsignResolve(LWindow window, string path)`

Reads the flag at `path` as a drawing that is frozen and so may be shared across rows.
A file that does not read, whatever the reader threw, is deleted through the engine and answered with nothing.
The file is a cache the workspace can fetch again, and a truncated one would otherwise fail every launch.

## `internal static async Task PEnsignLoad(LWindow window)`

Asks the engine to fill the flag of every language it offers, and draws each row that came back new.
A language already asked comes back in no row, so a warm call draws nothing.
A catalog row must be built the moment its list is filled, so it cannot wait for a file.
Every surface that shows a flag awaits this first, and then reads without waiting.
A pack installed while the program runs is picked up by the next call.

## `internal static async Task PEnsignVarietyLoad(LWindow window, string language, IEnumerable<string> varieties)`

The same fill for the named varieties of `language`, each drawn under `language/variety`.
The pronunciation menu awaits this before its search, then reads each reading's flag without waiting.
It shares the gate with the language flags, so one fill runs at a time whichever kind it is.

## `private static void PEnsignStoreAdd(LWindow window, IReadOnlyList<LEnsignRow> rows)`

Draws every row the engine kept and stores the drawing under the row's key.
The drawing is made here rather than when a row asks, so a row never waits on a file.

## `internal static string PEnsignVarietyFormat(string language, string variety)`

The key a variety's flag is kept under, which is its language and its name joined by a slash.
It is spelled here and in `LEnsign` alike, so a row asking for its flag hands no logic value down.

## `internal static void PEnsignFlagShow(Image flag, UIElement globe, string language)`

Shows the language's flag in the image, or the globe when the pack draws none.

## `internal static ImageSource? PEnsignFind(string language)`

The drawing kept for `language`, or null when none was resolved for it.
A `language/variety` key reads a variety's flag the same way.

## Inline notes

### `private static readonly Dictionary<string, ImageSource?> PEnsignStore = new(StringComparer.Ordinal);`

The drawings are held for the program rather than for one panel.
Every tab lists the same languages, and each would otherwise read the same files again.
The store is the lock as well, because it is what every reader and writer touches.

### `private static readonly SemaphoreSlim PEnsignGate = new(1, 1);`

One fill runs at a time.
Several panels ask on the same announcement, and each fill is a set of downloads.
Without this they would each start their own, and the same file would be fetched several times over.
The gate is never held by a plain wait on the shell's thread.
A fill awaiting the shell can always finish.
