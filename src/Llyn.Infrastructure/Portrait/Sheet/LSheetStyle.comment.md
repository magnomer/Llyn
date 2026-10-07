# LSheetStyle.cs
Hash: `391f57a964630e82`

## `public static class LSheetStyle`

The exported page's whole appearance, built from the display's theme.

## `public static string LSheetStyleRead(LTheme theme)`

Colours become custom properties so one declaration feeds every rule.
Sizes, margins and radii are the display's own values in device-independent pixels.
Backgrounds are forced to print, because a card without its ground is not the card seen.
Cards are kept off page breaks, which the panel never has to think about.

## `private static string LSheetStyleBuild(LTheme theme)`

Writes the custom properties on `:root`, then every shared rule, then `LSheetStylePaper`.

## `private static void LSheetVariableAppend(StringBuilder sheet, string name, string value)`

Writes one custom property with the theme text untouched.

## Inline notes

### `private static readonly (string, string)[] _lSheetStyleRules =`

Each rule as a selector and its declarations, in the order the page has always written them.
Order matters in CSS, so a later rule still wins over an earlier one.

### `private const string LSheetStylePaper =`

The `@page` and `@media print` rules, written after all shared rules.
