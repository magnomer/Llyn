# LEnsign.cs

## `public static class LEnsign`

The cache half of the flag: which key has been asked, and the SVG path each one resolved to.
A key is a language name, or a language and variety joined by a slash.
The engine fills it and the veneer keeps only the drawings, so no file check or deletion happens above.

## `public static string LEnsignKeyFormat(string language, string variety)`

The key a variety's flag is kept under: its language and its name joined by a slash.
`PEnsign` spells the same key on its side, so a row hands no logic value down to ask for one.

## `private static int _lEnsignAge;`

Counts workspace changes, so a fetch that began before one is dropped when it lands.

## `public static void LEnsignClear()`

Forgets every path and advances the age, on a workspace change.

## `public static string[] LEnsignMissingRead(IEnumerable<string> keys, out int age)`

The keys not yet asked, each once, with the age the caller hands back when the paths arrive.

## `public static IReadOnlyList<LEnsignRow> LEnsignPathAdd(int age, IReadOnlyList<string> keys, IReadOnlyList<string?> paths)`

Records the path each key resolved to, null when nothing is on disk, and answers with the rows kept.
An age that no longer matches records nothing, since the workspace has moved on.

## `public static string? LEnsignPathRead(string key)`

The path a key resolved to, or null when it is unasked or resolved to nothing.

## `public static void LEnsignPathDelete(string path)`

Removes a cached SVG the renderer could not read, so the next fetch replaces it.
