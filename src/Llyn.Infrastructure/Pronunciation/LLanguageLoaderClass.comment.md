# LLanguageLoaderClass.cs

## `public static partial class LLanguageLoader`

The `tone` side of the pack loader: the rows saying which tone classes a reflex reading's contour may descend from.
The rows become a list of [LAnatomyTone](../../Llyn.Core/Pronunciation/LAnatomyTone.comment.md), empty without them.
They live in their own file beside `anatomy.json`, since only the Classical Chinese pack declares them.

## `private const string LLanguageLoaderTone = "tone";`

The key under which the pack names the file, and the key the file wraps its rows in.

## `private const string LLanguageLoaderClass = "class";`

The key of the table in one row, each contour to the classes it may descend from.

## `private static IReadOnlyList<LAnatomyTone> LLanguageClassRead(string language, JsonElement root)`

Reads the `tone` key: a file name opens that file in the pack folder, an array is read in place.
Any other value, or no key, yields no rows.

## `private static IReadOnlyList<LAnatomyTone> LLanguageClassLoad(string language, string file)`

Opens the named file in the language's pack folder and reads its rows.
The file holds either the array itself or an object wrapping it under `tone`.
The name must be a bare file name, so a pack cannot point outside its folder.
A missing or unreadable file yields no rows, and no placement is marked as estimated.

## `private static IReadOnlyList<LAnatomyTone> LLanguageClassScan(JsonElement rows)`

The rows of one array in written order, rows that read `null` left out.

## `private static LAnatomyTone? LLanguageEstimateRead(JsonElement row)`

One row: `language` the names it serves, `class` the table from contour to classes.
A row without a language or with an empty table reads `null`.

## `private static IReadOnlyDictionary<string, IReadOnlyList<string>> LLanguageContourScan(JsonElement table)`

The table as a dictionary: each contour trimmed, its classes one string or an array, blanks and repeats dropped.
A contour left with no class is dropped.
