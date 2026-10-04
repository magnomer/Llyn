# CFontSlant.cs
Hash: `1d5f688fcbe0df05`

## `public enum CFontSlant`

The slant a pack font sets for one role, as a closed set.
`LCatalogFontRead` maps the engine's style name into it by name.
Any name it does not know becomes `CFontSlantTheme`, so a driver never parses a style name.
