# themes

The theme palettes, one JSON file each.
The palette is shared by the interface and every exported page.

## `default.json`

It is embedded into `Llyn.Infrastructure` and read once through `LThemeLoader`.

### `colors`

Each key is the theme's own name, mapped to a `Theme.*.Color` resource by `QTheme`.
A missing key reads as a built-in spare colour from `LTheme`, so no read fails.
A malformed colour stops the launch, because `QTheme` refuses it.
A new key needs a line in the `QTheme` map before any panel can bind it.

## Stamp

Commit: `c2f335c331fe`
- `default.json` `a58e316f51b3896a`
