# LTheme.cs

## `public sealed class LTheme`

The display's own colors, read where an export can reach them.
`themes/default.json` is embedded in both the shell and here, from the one file in the repository.
So an exported page cannot drift from the panel it copies.

## `public string LThemeFamily`

The page font stack, matching the shell's own family with web fallbacks appended.

## `public string LThemeSerif`

The example font stack, matching the serif the display uses for sentences.

## `public static LTheme LThemeLoad()`

A missing or broken theme is not a failure worth stopping an export for.
Every colour then falls back to the value shipped with the program.

## `public string LThemeRead(string name)`

The name is the theme file's own key, not a resource key.
An unknown name yields the spare value, and black when even that is absent.
