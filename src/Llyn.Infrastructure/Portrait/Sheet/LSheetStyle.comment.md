# LSheetStyle.cs
Hash: `5ca2e828a4d9768f`

## `public static class LSheetStyle`

The page's whole appearance, built from the display's theme.
One rule list serves the exported page and the Joplin style note, so the two never drift apart.

## `public static string LSheetStyleRead(LTheme theme)`

Colours become custom properties so one declaration feeds every rule.
Sizes, margins and radii are the display's own values in device-independent pixels.
Backgrounds are forced to print, because a card without its ground is not the card seen.
Cards are kept off page breaks, which the panel never has to think about.

## `public static string LSheetStyleFormat(LTheme theme, string scope)`

The same rules confined under the class `scope`, for a viewer that applies one style to its whole page.
Joplin applies a note's style to everything it shows, so an unscoped rule would restyle Joplin itself.
It throws `ArgumentException` unless `scope` is a single class selector.
That is a dot, a letter, then letters, digits, `-` or `_`.
The custom properties sit on `scope`, and the page and body rules land on it too.
The `@page` and `@media print` rules are left out, since Joplin prints through its own page.
Print declarations inside shared rules stay, and they are harmless on screen.
Resets for Joplin's own heading and list styles follow all shared rules.

## `private static string LSheetStyleBuild(LTheme theme, string? scope)`

The one writer behind both public forms.
A null `scope` writes the exported page exactly as it always was.

## `private static void LSheetVariableAppend(StringBuilder sheet, string name, string value, string? scope)`

Writes one custom property, cleaning theme text only for a scoped sheet.
The exported page keeps the theme text untouched.

## `private static string LSheetValueNormalize(string value)`

Strips braces, semicolons, `<`, backticks and the sequence `/*` from theme text.
A theme value then cannot close the scope's rule, open a global rule, or start a comment.
Joplin also escapes a `<` inside a style block, which would break the value.
The `/*` removal repeats until none remains, so a split sequence cannot rejoin.

## `private static bool LSheetScopeCheck(string scope)`

Tells whether `scope` is a single class selector.
Only ASCII letters, digits, `-` and `_` may follow the leading dot and letter.

## `private static string LSheetScopeFormat(string selector, string scope)`

Moves one selector under `scope`.
The universal rule covers the wrapper and its descendants, since the wrapper is part of the entry.
The page and body rules become the wrapper, which stands in for the page inside Joplin.
Every other selector in a list is prefixed on its own, so no part escapes the wrapper.

## Inline notes

### `private static readonly (string, string)[] _lSheetStyleRules =`

Each rule as a selector and its declarations, in the order the page has always written them.
Order matters in CSS, so a later rule still wins over an earlier one in both outputs.

### `private static readonly (string, string)[] _lSheetScopeRules =`

Scoped-only rules, written after all shared rules and never in the exported page.
Joplin's viewer underlines bare `h1` and `h2` and indents lists by margin.
Without these resets that styling would show inside Llyn's cards.

### `private const string LSheetStylePaper =`

The `@page` and `@media print` rules, kept apart because only the exported page is printed.
