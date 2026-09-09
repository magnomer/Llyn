# LMarkupMark.cs

## `public static class LMarkupMark`

Writes one attribute of a markup element.

## `public static void LMarkupMarkAppend(StringBuilder text, string mark, LStateValue value)`

An unset value writes no attribute at all, which is how the reader tells it from an empty one.

## `public static void LMarkupMarkAppend(StringBuilder text, string mark, string? value)`

The same writer for a field the store keeps as plain text rather than as a state.
Nothing written means the attribute is left off, which is the only absence such a field has.

## `public static string LMarkupMarkNormalize(string value)`

The reader requires a quoted value and ends it at the first delimiter that is not doubled.
So the value is always wrapped in double quotes and a double quote inside it is written twice.
Section 7 of the format spec makes that pair read back as the one character.
Choosing the delimiter by what the value holds would fail a value holding both quote characters.
