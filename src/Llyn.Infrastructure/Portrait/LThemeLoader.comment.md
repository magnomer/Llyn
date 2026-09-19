# LThemeLoader.cs

## `public static class LThemeLoader`

Reads the embedded theme file into the core's `LTheme` record.
`themes/default.json` is embedded here alone, so every palette in the program comes through this one read.

## `public static LTheme LThemeLoaderLoad()`

A missing or broken theme is not a failure worth stopping an export for.
Every colour then falls back to the value the record ships with.
