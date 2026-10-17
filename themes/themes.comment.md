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

## `console-blue.json` and `console-dark.json`

The console's palettes, embedded into `Llyn.Infrastructure` beside `default.json`.
`LThemeLoaderLoad(name)` reads one by its file name, and the console start asks for `console-blue`.

### `colors`

Each key is a role a console cell can name, and each value reads `colour on colour`.
The colours are the sixteen console names such as `darkBlue` and `white`.
A `#RRGGBB` after a colour adds an exact shade, though neither file uses one yet.
The two files hold the same keys, and a test pins that every role `BTheme` knows is present.
A role missing from a file draws as `text`, but a missing `text` stops the theme from loading.
A malformed value stops the theme from loading, and the start then uses the built-in spare palette.
`console-blue` also equals that spare palette, as a test pins.
`console-dark` puts most roles on black in place of dark blue.

## Stamp

Commit: `92c4426f8506`
- `console-blue.json` `dcc881539d5d06c4`
- `console-dark.json` `7604d774752426aa`
- `default.json` `a58e316f51b3896a`
