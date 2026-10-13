# Pack

This folder holds what every section of a language pack shares.
One part reads fields tolerantly and the other locates the pack and opens its side files.
It stands apart from Loader because every section loader leans on it.

## `LPack`

These are the field readers every section of a language pack shares.
Each reader tolerates a missing key or a value of the wrong kind and returns a blank instead.
So one malformed field never breaks the rest of the pack.
`LSpeechLoader` and `LRegisterLoader` keep their own readers, whose semantics differ from these.
`LPackTextRead` returns the string untrimmed, or `null` for a missing key or any other kind.
`LPackNumberRead` returns a whole number, and reads zero for a missing key, another kind or a value outside `int`.
`LPackMeasureRead` returns a positive number such as a font size.
A missing key, another kind, zero or a negative value reads as zero, so the theme's default stands.
`LPackBooleanRead` is true only for a literal `true`.
`LPackTextScan` reads a string as one name and an array as every string in it.
Each name is trimmed, and blanks and non-strings are dropped.
The anatomy and tone rows pass `language` to read the names a row serves.

## `LPackFile`

It locates language packs and opens the side files a pack names beside its `source.json`.
Packs resolve against the application's base directory, so a pack is drop-in.
`LPackFileFolder` is the folder under the base directory that holds one subfolder per pack.
`LPackFileLoad` opens a named side file in the language's pack folder and returns its root.
The name must be a bare file name, so a pack cannot point outside its folder.
The root is a clone, since the parsed document is disposed before the caller reads it.
A blank name, a missing file, or an IO, JSON or access error reads as `null`.
The caller unwraps any key the file wraps its rows in.

## Stamp

Commit: `c2f335c331fe`
- `LPack.cs` `e0f7e8d6f46f7003`
- `LPackFile.cs` `6ec9b768f0a1d41e`
