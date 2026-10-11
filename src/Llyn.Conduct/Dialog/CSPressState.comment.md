# CSPressState.cs
Hash: `79da20a85e5cd51c`

## `public sealed record CSPressState(int CSPressStateCount, double CSPressStateWidth, double CSPressStateHeight, CSLensState CSPressStateLens, string? CSPressStateFailure, CSPressSetting CSPressStateSetting)`

The outcome of one proof draw, which the canvas lays its placeholder sheets out from.

**Parameters**

- `CSPressStateCount`: how many pages the proof holds, zero after a failure.
- `CSPressStateWidth`: the first page's width in device-independent pixels, zero when there is no page.
  The canvas sizes its sheets from it at 96 to the inch, so it computes no unit itself.
- `CSPressStateHeight`: the first page's height in device-independent pixels, zero when there is no page.
- `CSPressStateLens`: the zoom after the lens learned this proof's paper size.
  A fitting lens refits to the new paper, so the driver paints the zoom with the sheets.
- `CSPressStateFailure`: the wording key of a failed draw, or null when the draw succeeded.
- `CSPressStateSetting`: the ready setting read after the draw.
  A draw changes which pages print and how many sheets they take, so the window repaints from it.
