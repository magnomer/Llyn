# CWindowState.cs

## `public sealed record CWindowState(`

The main window's restored rectangle and whether it is maximized.
The window view hands it to the footprint, which clamps and stores it.

**Parameters**

- `CWindowStateLeft`: the left edge in device-independent pixels.
- `CWindowStateTop`: the top edge in device-independent pixels.
- `CWindowStateWidth`: the width in device-independent pixels.
- `CWindowStateHeight`: the height in device-independent pixels.
- `CWindowStateMaximized`: whether the window is maximized over that rectangle.
