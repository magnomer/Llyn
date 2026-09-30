# QEnsignImage.cs

## `public static class QEnsignImage`

Resolves a language pack's cached SVG flag into a frozen drawing, and keeps the resolved drawings by key.
Which key was asked, the path it resolved to, and the age a moved workspace outran are `LEnsign`'s.
It subscribes to the engine for one announcement only, the workspace moving, and throws the kept drawings away on it.
The editor's language picker, the read-only entry display, and every catalog row use it.
A malformed or unknown flag becomes no image, which lets each surface show its neutral globe fallback.

## `public static void QEnsignIntroduce(CAtelier atelier)`

Subscribes the flag store to `CWorkspaceOpened` on the atelier's workspace, once, for the whole program.
The window subscribes it before any panel, so it answers each open first.

## `private static void QEnsignOpenRefine()`

Throws every kept drawing away whenever the workspace opens, and answers nothing else.
The engine forgot its paths when the workspace moved, so a surface reloading afterwards asks afresh.
A drawing for a language the new workspace lacks therefore never lingers.
A reload still in flight at the open refills the store once it lands.

## `private static DrawingImage? QEnsignDraw(string path, Action<string, Exception> delete)`

Reads the flag at `path` as a drawing that is frozen and so may be shared across rows.
A file that does not read is answered with nothing, and handed to `delete` with what the reader threw.
The engine keeps a locked or refused file and deletes one whose content was bad.
The file is a cache the workspace can fetch again, and a truncated one would otherwise fail every launch.

## `public static Action QEnsignDraw(IReadOnlyList<CEnsignRow> rows, Action<string, Exception> delete)`

Draws every row the engine kept and answers the commit that stores them.
Every Conduct flag load is handed it directly, and the engine runs one fill at a time.
A language already asked comes back in no row, so a warm load draws nothing.
A surface that shows a flag awaits its load first, and then reads without waiting.
The drawing is made here rather than when a row asks, so a row never waits on a file.
The engine calls it outside its lock, so parsing holds no lock at all.
The drawing stays on the shell's thread, where the fill was awaited.

## `private static void QEnsignStoreRefine(List<KeyValuePair<string, ImageSource?>> resolved)`

Stores each drawing under its row's key.
The engine runs it inside the cache's lock, only while the fill's workspace is current.
So a workspace move cannot slip between the engine's check and this store.

## `public static void QEnsignFlagRefine(Image flag, UIElement globe, string language)`

Shows the language's flag in the image, or the globe when the pack draws none.

## `public static ImageSource? QEnsignRead(string language)`

The drawing kept for `language`, or null when none was resolved for it.
A `language/variety` key reads a variety's flag the same way.
Every accent and index row reads it directly, so no row is handed a lookup.

## Inline notes

### `private static readonly Dictionary<string, ImageSource?> QEnsignStore = new(StringComparer.Ordinal);`

The drawings are held for the program rather than for one panel.
Every tab lists the same languages, and each would otherwise read the same files again.
The store is the lock as well, because it is what every reader and writer touches.
