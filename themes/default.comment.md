# default.json

The one palette shared by the interface and every exported page.
It is embedded into `Llyn.Infrastructure` and read once through `LThemeLoader`.

## `colors`

Each key is the theme's own name, mapped to a `Theme.*.Color` resource by `PThemeLoader`.
Every key that map names is required, and a missing or malformed colour stops the launch.
An exporter reading a missing key falls back instead, so a page export never fails on the palette.
A new key needs a line in the `PThemeLoader` map before any panel can bind it.
