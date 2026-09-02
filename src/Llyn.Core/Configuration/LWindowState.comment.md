# LWindowState.cs

## `public sealed record LWindowState(`

The window geometry carried between two runs of the program.
It is part of the settings file, so it belongs to the workspace like every other preference.
The stored rectangle is always the restored one, never the maximized one.
A maximized window that is closed must come back maximized over the size it had before.

**Parameters**

- `LWindowStateLeft` — The left edge of the restored window, in device-independent pixels.
- `LWindowStateTop` — The top edge of the restored window, in device-independent pixels.
- `LWindowStateWidth` — The width of the restored window.
- `LWindowStateHeight` — The height of the restored window.
- `LWindowStateMaximized` — Whether the window was maximized when it closed.
