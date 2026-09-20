# PThemeLoader.cs

## `internal static class PThemeLoader`

Fills the application resources with the palette an `LTheme` record carries.
The bootstrap loads the record once, so the panels and an exported page share one palette.

## `private static readonly IReadOnlyDictionary<string, string> PThemeLoaderColors`

Which theme key fills which resource, every one of them required.

## `internal static void PThemeLoaderApply(Func<string, string> colorRead, ResourceDictionary resources)`

Converts each named colour text into a `Color` resource under its resource key.
The reader is the theme's own colour lookup, handed in by the bootstrap so the veneer names no theme type.
The theme refuses a missing key itself, so the loop holds no branch over what it read.
A palette missing a key, or holding a text that is no colour, stops the launch before a panel draws.
