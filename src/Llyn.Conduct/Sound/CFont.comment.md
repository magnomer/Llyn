# CFont.cs
Hash: `8adfffa58a28950f`

## `public sealed record CFont(string? CFontFamily, double? CFontSize, CFontSlant CFontStyle);`

The font a language's pack sets for one role.

**Parameters**

- `CFontFamily`: the family name, never blank, null to keep the theme's.
- `CFontSize`: the size, always finite and positive, null to keep the theme's.
- `CFontStyle`: the slant, `CFontSlantTheme` to keep the theme's.
