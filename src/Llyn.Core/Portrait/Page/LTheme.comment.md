# LTheme.cs

## `public sealed class LTheme`

The display's colour table, held where every ring can read it.
The infrastructure loads it from the embedded theme file.
The exporters and the shell both draw from the one record.
So an exported page cannot drift from the panel it copies.

## `public LTheme(IReadOnlyDictionary<string, string> colors)`

Takes the colours as read, empty when the loader found nothing.

## `public string LThemeFamily`

The page font stack, matching the shell's own family with web fallbacks appended.

## `public string LThemeSerif`

The example font stack, matching the serif the display uses for sentences.

## `public string LThemeColorRead(string name)`

The colour the theme file names, refused when the file lacks it.
The shell fills its resources through this, since a panel drawn from a spare colour would be wrong quietly.

## `public string LThemeRead(string name)`

The name is the theme file's own key, not a resource key.
An unknown name yields the spare value, and black when even that is absent.
