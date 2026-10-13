# Galley

This folder holds the print window's page canvas, zoom bar and sheets.
It also holds the sight measured from them.

## `QSGalley`

The print window's page canvas, with its sheets, page moves, sight and lazy page images.
`QSPress` builds it over the same window and gate, and feeds it every draw and setting.
It builds `QSGalleyBar` over the galley's lens and repaints the sheets when the painted zoom changes.
It keeps no clock that drives a gate.
Each scroll hands one raw sight to `CSGalleySightSet`, and the galley's wait decides when pages are drawn.
The current page lives in `CSPress`, so this driver keeps no copy and decides no page request.
DPI, the image cache and scrolling are canvas state, since printing is GUI-only.
Page count and page size come from the draw as given, and the canvas only scales them.
A typed page goes raw to the gate, which parses it.

### `_qsGalleyCache`

It holds the delivered PNG bytes, by page, and only remembers what was painted.
`QSGalleyPageRefine` is its one writer.
So it holds exactly the pages the galley counts delivered.
It keeps the bytes, not a decoded picture, so a zoom or DPI change decodes again at the shown size.

### `_qsGalleyItem`

One sheet per page of the current draw, replaced whole by each draw.

### `QSGalley`

Each scroll is heard twice, first by the Observe that calls the sight gate.
The Refine that paints runs second and measures the sight again itself, so it reads no gate state.

### `QSGalleyRefine`

The galley already dropped every delivered page during the draw, so the cache is empty here.
The draw's lens is painted on the bar, which Conduct refitted to the new paper.
It scrolls back to the page that was on top once the new sheets are laid out.

### `QSGalleySettingRefine`

A gray change decodes only cached pages again, since a later page arrives painted as the flag says.

### `QSGalleySightDraw`

It reads only geometry, so a console driver would measure the same thing its own way.
It reads no zoom, since Conduct turns the scale and its own zoom into a resolution.

### `QSGalleyWidthDraw`

The width is capped at twice the view width, so a deep zoom never decodes a huge image.
It never leaves the driver.

### `QSGalleyPictureDraw`

Decoding at the shown size keeps memory to what the screen shows, whatever resolution arrived.
A width of zero or less decodes at the picture's own size.

### `QSGalleyScrollObserve`

The gate selects the page and ignores an unchanged one, so this handler checks nothing.

### `QSGalleyScrollRefine`

It leaves the number alone while the reader types in it.
It drops no picture, since the galley's page event owns that.

### `QSGalleyPageRefine`

A page with a picture is cached and shown decoded at the shown size.
A page without one was dropped, so its bytes are forgotten and its sheet cleared.
A page beyond the current sheets is ignored, since a newer draw replaced them.

### `QSGalleyPictureRefine`

A zoom, a DPI change and a gray change call it, so no picture is decoded larger than shown.

### `QSGalleyFactorRefine`

When the factor moved, it keeps the reading place by scaling the scroll offset.

### `QSGalleyDpiObserve`

The gate may then need another resolution.

### `QSGalleyDpiRefine`

The cached pages are decoded again without asking the galley for them.

### `QSGalleyKeyRefine`

It ignores keys while a field or list has focus, so typing never turns pages.
The zoom keys belong to the bar's own handler.

## `QSGalleyBar`

The zoom half of the print canvas bar, with its controls only.
The zoom and its rules live in Conduct `CSLens`, since typed or wheeled input is raw input.
Each Observe hands one raw value to one lens gate and gives the answer to `QSGalleyLensRefine`.

### `QSGalleyFactorChanged`

It is raised after every zoom paint with the factor from before it.
The canvas sizes its sheets again and scales its scroll offset by the change.
It fires even for an equal factor, since a new draw needs its sheets sized.

### `QSGalleyLens`

It only remembers what Conduct answered.
The constructor seeds it from `CSLensRead`, and after that only `QSGalleyLensRefine` sets it.

### `QSGalleyAreaDraw`

The lens fits into the canvas view less the sheet list margins.
The measure stays raw and may be zero or negative before layout.

### `QSGalleyPercentObserve`

The text goes to the lens exactly as typed.
The lens reads it, so text that is no number refits in the current mode.

### `QSGalleyWheelObserve`

Without Ctrl the wheel scrolls the canvas as usual.

### `QSGalleySizeObserve`

A fitting lens refits, and a free one answers its factor unchanged.

### `QSGalleyKeyObserve`

It ignores keys while a field or list has focus, as the page keys do.

## `QSGalleyItem`

One paper sheet of the print canvas, drawn or still a placeholder.
Every sheet exists from the start, so the scroll length is right before any page is drawn.
A change is raised only when real, so an equal picture or shade refills nothing.

### `QSGalleyItemWidth`

The width is in device-independent pixels, as the draw answered it.
The canvas multiplies it by its zoom, so the item never changes with the zoom.

### `QSGalleyItemPicture`

Null for a placeholder.
The canvas sets it back to null when its cache drops the page.

## `QSGalleySight`

What the canvas shows at one moment, measured from the laid-out sheets.
It is pure geometry, so it never decides which pages Conduct draws.
The scroll and DPI handlers hand it to `CSGalleySightSet` unchanged.
The page is -1 when no sheet is in view.
Otherwise it is the first sheet reaching below a third of the view.
At the scroll end it is the last sheet.
With neither, it is the first sheet in view.
The scale is raw device pixels per device-independent pixel, so the driver never names dots per inch.

## Stamp

Commit: `c2f335c331fe`
- `QSGalley.cs` `a161b33095d3c149`
- `QSGalleyBar.cs` `2a6f78b0970c3984`
- `QSGalleyItem.cs` `e703bd752e50739a`
- `QSGalleySight.cs` `752c091379cb5060`
