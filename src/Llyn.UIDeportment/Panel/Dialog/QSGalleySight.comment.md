# QSGalleySight.cs
Hash: `752c091379cb5060`

## `internal sealed record QSGalleySight(int QSGalleySightPage, int QSGalleySightFirst, int QSGalleySightLast, double QSGalleySightScale)`

What the canvas shows at one moment, measured from the laid-out sheets.
It is pure geometry, so it never decides which pages Conduct draws.
The scroll and DPI handlers hand the page, the visible range and the monitor scale to `CSGalleySightSet` unchanged.

**Parameters**

- `QSGalleySightPage`: the page a reader is on, or -1 when no sheet is in view.
  It is the first sheet reaching below a third of the view, or the last sheet at the scroll end.
  With neither, it is the first sheet in view.
- `QSGalleySightFirst`: the first sheet in view, or -1 when none is.
- `QSGalleySightLast`: the last sheet in view, or -1 when none is.
- `QSGalleySightScale`: the raw device pixels per device-independent pixel of the monitor showing the canvas.
  Conduct turns it into a resolution, so the driver never names dots per inch.
