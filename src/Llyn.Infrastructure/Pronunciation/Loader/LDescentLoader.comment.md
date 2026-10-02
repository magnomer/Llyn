# LDescentLoader.cs

## `internal static class LDescentLoader`

The `tone` side of the pack loader: the rows saying which tone classes a reflex reading's contour may descend from.
The rows become a list of [LDescent](../../../Llyn.Core/Pronunciation/Descent/LDescent.comment.md), empty without them.
They live in their own file beside `anatomy.json`, since only the Classical Chinese pack declares them.
`LLanguageLoader` calls it.

## `private const string LDescentKey = "tone";`

The key under which the pack names the file, and the key the file wraps its rows in.

## `private const string LDescentClass = "class";`

The key of the table in one row, each contour to the classes it may descend from.

## `public static IReadOnlyList<LDescent> LDescentPackRead(string language, JsonElement root)`

Reads the `tone` key: a file name opens that file in the pack folder, an array is read in place.
Any other value, or no key, yields no rows.
The named file is opened through `LPackFile`.
The file holds either the array itself or an object wrapping it under `tone`.
A missing or unreadable file yields no rows, and no placement is marked as estimated.

## `private static IReadOnlyList<LDescent> LDescentRowScan(JsonElement rows)`

The rows of one array in written order, rows that read `null` left out.

## `private static LDescent? LDescentRowRead(JsonElement row)`

One row: `language` the names it serves, `class` the table from contour to classes.
A row without a language or with an empty table reads `null`.

## `private static IReadOnlyDictionary<string, IReadOnlyList<string>> LDescentContourScan(JsonElement table)`

The table as a dictionary: each contour trimmed, its classes one string or an array, blanks and repeats dropped.
A contour left with no class is dropped.
