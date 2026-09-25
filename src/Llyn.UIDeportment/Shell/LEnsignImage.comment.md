# LEnsignImage.cs

## `public static class LEnsignImage`

Resolves a language pack's cached SVG flag into a frozen drawing, and keeps the resolved drawings by key.
Which key was asked, the path it resolved to, and the age a moved workspace outran are `LEnsign`'s.
It subscribes to the engine for one announcement only, the workspace moving, and throws the kept drawings away on it.
The editor's language picker, the read-only entry display, and every catalog row use it.
A malformed or unknown flag becomes no image, which lets each surface show its neutral globe fallback.

## `public static void LEnsignAttach(LWindow window)`

Subscribes the flag store through `window`, once, for the whole program.
It owns no control, so its answer runs on the thread that announced rather than on the shell's.

## `private static void LEnsignBulletinHandle(LBulletin bulletin)`

Throws every kept drawing away when the workspace moves, and answers nothing else.
The engine has already forgotten the paths before it announces, so a surface reloading on the announcement asks afresh.
The surfaces reload on the same announcement, and this store is the first subscriber.
It is empty before they ask.

## `public static DrawingImage? LEnsignResolve(string path, Action<string, Exception> delete)`

Reads the flag at `path` as a drawing that is frozen and so may be shared across rows.
A file that does not read is answered with nothing, and handed to `delete` with what the reader threw.
The engine keeps a locked or refused file and deletes one whose content was bad.
The file is a cache the workspace can fetch again, and a truncated one would otherwise fail every launch.

## `public static async Task LEnsignLoad(LWindow window)`

Asks the engine to fill the flag of every language it offers, and draws each row that came back new.
A language already asked comes back in no row, so a warm call draws nothing.
A catalog row must be built the moment its list is filled, so it cannot wait for a file.
Every surface that shows a flag awaits this first, and then reads without waiting.
A pack installed while the program runs is picked up by the next call.

## `public static async Task LEnsignVarietyLoad(LWindow window, string language, IEnumerable<string> varieties)`

The same fill for the named varieties of `language`, each drawn under `language/variety`.
The pronunciation menu awaits this before its search, then reads each reading's flag without waiting.
It shares the gate with the language flags, so one fill runs at a time whichever kind it is.

## `private static Action LEnsignStoreAdd(IReadOnlyList<LEnsignRow> rows, Action<string, Exception> delete)`

Draws every row the engine kept and answers the commit that stores them.
The drawing is made here rather than when a row asks, so a row never waits on a file.
The engine calls it outside its lock, so parsing holds no lock at all.
The drawing stays on the shell's thread, where the fill was awaited.

## `private static void LEnsignStoreCommit(List<KeyValuePair<string, ImageSource?>> resolved)`

Stores each drawing under its row's key.
The engine runs it inside the cache's lock, only while the fill's workspace is current.
So a workspace move cannot slip between the engine's check and this store.

## `public static void LEnsignFlagShow(Image flag, UIElement globe, string language)`

Shows the language's flag in the image, or the globe when the pack draws none.

## `public static ImageSource? LEnsignFind(string language)`

The drawing kept for `language`, or null when none was resolved for it.
A `language/variety` key reads a variety's flag the same way.
Every accent and index row reads it directly, so no row is handed a lookup.

## Inline notes

### `private static readonly Dictionary<string, ImageSource?> LEnsignStore = new(StringComparer.Ordinal);`

The drawings are held for the program rather than for one panel.
Every tab lists the same languages, and each would otherwise read the same files again.
The store is the lock as well, because it is what every reader and writer touches.

### `private static readonly SemaphoreSlim LEnsignGate = new(1, 1);`

One fill runs at a time.
Several panels ask on the same announcement, and each fill is a set of downloads.
Without this they would each start their own, and the same file would be fetched several times over.
The gate is never held by a plain wait on the shell's thread.
A fill awaiting the shell can always finish.
