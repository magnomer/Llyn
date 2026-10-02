# LPackFile.cs

## `internal static class LPackFile`

Locates language packs and opens the side files a pack names beside its `source.json`.
Packs resolve against the application's base directory, so a pack is drop-in.

## `public const string LPackFileFolder = "languages";`

The folder under the base directory that holds one subfolder per language pack.

## `public static JsonElement? LPackFileLoad(string language, string file)`

Opens the named side file in the language's pack folder and returns its root.
The name must be a bare file name, so a pack cannot point outside its folder.
The root is a clone, since the parsed document is disposed before the caller reads it.
A blank name, a missing file, or an IO, JSON or access error reads as `null`.
The caller unwraps any key the file wraps its rows in.
