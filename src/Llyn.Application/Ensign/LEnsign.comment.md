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

## `public IReadOnlyList<LEnsignRow> LEnsignPathAdd(int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths)`

Records the path each key resolved to, null when the usher finds nothing there, and answers with the rows kept.
An age that no longer matches records nothing, since the workspace has moved on.

## `public void LEnsignPathDelete(string path)`

Asks the usher to remove a cached SVG the renderer could not read, so the next fetch replaces it.
