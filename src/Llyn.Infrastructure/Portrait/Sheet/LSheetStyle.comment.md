# LSheetStyle.cs
Hash: `a011f8b916bf3a3b`

## `public static class LSheetStyle`

The appearance of a catalog sheet, built from the display's theme.
An entry sheet takes `LSheetCascade` instead.

## `public static string LSheetStyleRead(LTheme theme)`

Colours become custom properties so one declaration feeds every rule.
Sizes, margins and radii are the display's own values in device-independent pixels.
Backgrounds are forced to print, because a card without its ground is not the card seen.
Cards are kept off page breaks, which the panel never has to think about.

## `internal static string LSheetVariableFormat(LTheme theme)`

Answers the `:root` rule that names each theme font stack and each colour in `LTheme`'s spare set.
Each becomes a custom property.
The contour colours stay out, since each chart image carries its own.
The catalog style and `LSheetCascade` both open with it, so the two sheets share one palette.
Each property keeps the theme file's key in kebab case, so `surfaceRaised` becomes `--surface-raised`.

## `private static string LSheetStyleBuild(LTheme theme)`

Writes the custom properties from `LSheetVariableFormat`, then every shared rule, then `LSheetStylePaper`.

## `private static void LSheetVariableAppend(StringBuilder sheet, string name, string value)`

Writes one custom property with the theme text untouched.

## Inline notes

### `private static readonly (string, string)[] _lSheetStyleRules =`

Each rule as a selector and its declarations, in the order the page has always written them.
Order matters in CSS, so a later rule still wins over an earlier one.
The paradigm rule hides the table its checked radio does not name, so Short/Full switches without script.
A collocation row's summary sets the phrase and the meaning on one baseline, with no marker.
This sheet draws no chevron, since PDF and print open every row through `LPortraitPage.LPortraitPageOpen`.
A catalog HTML export keeps the page's folds, so a folded row there shows no fold mark.

### `private const string LSheetStylePaper =`

The `@page` and `@media print` rules, written after all shared rules.
