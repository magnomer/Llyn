# LCase.cs

## `public static class LCase`

Changes the case of the first text element of a value under a culture.
A text element is taken whole, so a combined character never splits.

## `public static string LCaseUpperChange(string value, CultureInfo culture)`

The value with its first text element in upper case.

## `public static string LCaseLowerChange(string value, CultureInfo culture)`

The value with its first text element in lower case.

## `private static string LCaseChange(string value, Func<string, string> changeCase)`

Applies the case change to the first text element and keeps the rest as written.
