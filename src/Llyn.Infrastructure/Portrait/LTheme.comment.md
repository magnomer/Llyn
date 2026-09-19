# LTheme.cs

## `public sealed class LTheme`

The display's own colors, read where an export can reach them.
`themes/default.json` is embedded here alone, and the shell reads its palette through this class.
So an exported page cannot drift from the panel it copies.

## `public string LThemeFamily`

The page font stack, matching the shell's own family with web fallbacks appended.

## `public string LThemeSerif`

The example font stack, matching the serif the display uses for sentences.

## `public IReadOnlyDictionary<string, string> LThemeColor`

Every colour the theme file names, keyed by the file's own key.
The shell walks it to fill its resources and refuses a palette that lacks a key it needs.

## `public static LTheme LThemeLoad()`

A missing or broken theme is not a failure worth stopping an export for.
Every colour then falls back to the value shipped with the program.

## `public string LThemeColorRead(string name)`

The colour the theme file names, refused when the file lacks it.
The shell fills its resources through this, since a panel drawn from a spare colour would be wrong quietly.

## `public string LThemeRead(string name)`

The name is the theme file's own key, not a resource key.
An unknown name yields the spare value, and black when even that is absent.
