# LPack.cs
Hash: `e0f7e8d6f46f7003`

## `internal static class LPack`

The field readers every section of a language pack shares.
Each reader tolerates a missing key or a value of the wrong kind and returns a blank instead.
So one malformed field never breaks the rest of the pack.
`LSpeechLoader` and `LRegisterLoader` keep their own readers, whose semantics differ from these.

## `public static string? LPackTextRead(JsonElement element, string key)`

The string under `key`, untrimmed.
A missing key or any other kind reads as `null`.

## `public static int LPackNumberRead(JsonElement element, string key)`

The whole number under `key`.
A missing key, another kind or a value outside `int` reads as zero.

## `public static double LPackMeasureRead(JsonElement element, string key)`

The positive number under `key`, such as a font size.
A missing key, another kind, zero or a negative value reads as zero, so the theme's default stands.

## `public static bool LPackBooleanRead(JsonElement element, string key)`

True only for a literal `true` under `key`.
A missing key or any other value reads as false.

## `public static IReadOnlyList<string> LPackTextScan(JsonElement row, string key)`

The value under `key` as a list.
A string gives one name and an array gives every string in it.
Each name is trimmed, and blanks and non-strings are dropped.
The anatomy and tone rows pass `language` to read the names a row serves.
