# LEnsign.cs

## `public sealed class LEnsign`

The cache half of the flag: which key has been asked, and the SVG path each one resolved to.
A key is a language name, or a language and variety joined by a slash.
The engine owns one and fills it, and the veneer keeps only the drawings.
Whether a path is present, and dropping a file, are asked of the `LUsher` handed in.
So no file check or deletion happens in this ring or above it.

## `public LEnsign(LUsher usher)`

Takes the port that answers path facts, an `LUsherFile` in the engine and a recording fake in a test.

## `public static string LEnsignKeyFormat(string language, string variety)`

The key a variety's flag is kept under: its language and its name joined by a slash.
`LEnsignImage` spells the same key on its side, so a row hands no logic value down to ask for one.

## `private int _lEnsignAge;`

Counts workspace changes, so a fetch that began before one is dropped when it lands.

## `public void LEnsignClear()`

Forgets every path and advances the age, on a workspace change.

## `public string[] LEnsignMissingRead(IEnumerable<string> keys, out int age)`

The keys not yet asked, each once, with the age the caller hands back when the paths arrive.

## `public void LEnsignPathAdd(int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Records the path each key resolved to, null when the usher finds nothing there.
Throws when `keys` and `paths` differ in length, since each key needs its own path.
Hands the rows kept to `store`, with the delete a surface calls for a file it cannot draw.
An age that no longer matches records nothing and calls nothing, since the workspace has moved on.
The seam parses outside the lock, so no reader or clear waits on a file.
It answers a commit, which runs inside the lock only while the age still matches.
So a workspace move between the parse and the commit stores nothing.

## `public void LEnsignPathDelete(string path, Exception exception)`

Forgets every key that resolved to `path`, so the next load asks for it again.
Asks the usher to remove a cached SVG the renderer could not read, so the next fetch replaces it.
When the usher judges `exception` a lock or a refusal, the file is kept.
Such a lock may be brief, so deleting would throw away a good flag.
The forgotten key lets the next load draw it once the lock is gone.
