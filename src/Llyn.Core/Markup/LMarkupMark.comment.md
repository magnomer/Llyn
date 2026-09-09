# LMarkupMark.cs

## `public static class LMarkupMark`

Writes one attribute of a markup element.

## `public static void LMarkupMarkAppend(StringBuilder text, string mark, LStateValue value)`

An unset value writes no attribute at all, which is how the reader tells it from an empty one.

## `public static string LMarkupMarkNormalize(string value)`

The reader requires a quoted value and ends it at the matching quote.
A value holding a double quote is wrapped in single quotes instead.
Any single quote inside such a value is dropped, because no escape exists to keep it.
