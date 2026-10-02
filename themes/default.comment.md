# default.json
Hash: `a58e316f51b3896a`

The one palette shared by the interface and every exported page.
It is embedded into `Llyn.Infrastructure` and read once through `LThemeLoader`.

## `colors`

Each key is the theme's own name, mapped to a `Theme.*.Color` resource by `QTheme`.
A missing key reads as a built-in spare colour from `LTheme`, so no read fails.
A malformed colour stops the launch, because `QTheme` refuses it.
A new key needs a line in the `QTheme` map before any panel can bind it.
