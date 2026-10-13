# Press

This folder holds the print window's driver for the window itself.
It also holds the drivers for the printer controls and the page layout controls.

## `QSPress`

The print window's driver for the window itself, the footer, and the busy and failure states.
It builds the setting column's two drivers and the canvas driver, and paints them all from one setting.
`QSPressDevice` drives the printer controls, `QSPressLayout` the page layout, and `QSGalley` the canvas.
The driver computes no setting, parses no text and chooses no wording key of its own.

### `_qsPressSurface`

It is owned by the active window so it stays above it and closes with it.

### `_qsPressPosture`

The print window's size lives here between runs.
The setting itself is kept by the engine's store through `CSPressClose`.

### `QSPress`

The child drivers subscribe their controls on construction.
Their own paints unsubscribe while they fill, so the first paint reaches no gate.
Every setting gate raises `CSPressSettingChanged`, so a pick in any column repaints the whole window.
`CSGalleyStateChanged` is raised after a deferred redraw.
The galley holds a layout change back, so a fast typist starts one proof draw, not one per key.
The canvas is built before the first paint, since every setting paint also reaches the canvas.
`QSPressKeyObserve` hears the window's keys after the canvas's own handler.

### `QSPressConsult`

The modal loop pumps messages, so the gate's awaited draws and prints run inside it.

### `QSPressFieldRefine`

A field holding the keyboard focus is left alone.
The focused text box keeps what the reader is typing, so a clamp never jumps under the caret.

### `QSPressItemRefine`

The markup declares no rows, so the list is the picker's only source.
The gate maps the raw selected index back through the same list.
A null list leaves the picker empty, which only a disabled two-sided picker shows.

### `QSPressSettingRefine`

The child drivers paint their columns first, then the footer and the canvas follow.
A stale setting shows busy and hides the failure panel until the galley's deferred redraw paints the state.

### `QSPressStateRefine`

The canvas rebuilds its sheets first, then the setting read after the draw is painted.
That setting carries the new page flags and sheet count, which only a finished draw knows.
A failed draw's setting is never stale, so busy stays hidden under the failure panel.

### `QSPressShownObserve`

The gate raises `CSPressSettingChanged` with the restored setting, so the answer is not painted here.
The gate lists printers first, so the list fills in once the spooler answers.
The restored setting is stale, so the galley's first deferred draw uses the remembered layout.

### `QSPressRetryObserve`

A draw that a newer one replaced answers null and paints nothing.

### `QSPressKeyObserve`

The gate refuses a second print while one runs, so the driver reads no enablement.
Every other key passes on, so a focused field keeps its own keys.

### `QSPressAdmitRefine`

A null answer closes the window with true, and a failure key goes to the footer status.
Admit and Ctrl+P share it, so both end a print the same way.

### `QSPressClosingObserve`

`CSPressClose` refuses while a print runs, and keeps the setting when it lets the window close.
The window size is stored even on a refused close, since a later close stores it again.
The width and height are the restored size, so a maximized close keeps the normal size.

## `QSPressDevice`

The print window's driver for the printer controls.
They follow what the chosen printer can do, so they share one driver.
Every observer hands one raw control value to one `CSPress` gate and paints nothing itself.
`QSPress` paints the whole window from the setting event the gate raises.

### `QSPressDeviceRefine`

A null field disables or hides its control, as the setting's record promises.
Printer names are painted exactly as given.
The selection and text observers are unsubscribed while it fills, as `QLocalization` does.
So painting reaches no gate, and no flag is needed to tell the two apart.
A click is never raised by painting, so the click observers stay subscribed.

### `QSPressDestinationObserve`

The gate maps the index to a printer through the setting it last answered, and ignores -1.

### `QSPressFullObserve`

Each radio button has its own observer, so no observer inspects its sender.

### `QSPressSideObserve`

The gate maps the row to its mode and ignores -1.

## `QSPressLayout`

The print window's driver for the page layout controls.
They shape the sheets the proof draws, so they share one driver.
Every observer hands one raw control value to one `CSPress` gate and paints nothing itself.

### `QSPressLayoutRefine`

It follows the same paint rules as `QSPressDeviceRefine`.

### `QSPressRangeObserve`

The gate maps the row to its mode and keeps the pages text it last answered.

### `QSPressPagesObserve`

The gate keeps the mode it last answered.

### `QSPressUprightObserve`

Each radio button has its own observer, so no observer inspects its sender.

### `QSPressPaperObserve`

The chosen sheet's list position goes to the gate, which indexes the same list.

### `QSPressMarginObserve`

The gate maps the row to its preset and ignores -1.

### `QSPressTopObserve`

The gate takes the other three edges from the setting it last answered.
Each field so hands one value, instead of the driver gathering all four.

## Stamp

Commit: `c2f335c331fe`
- `QSPress.cs` `fe249b024fecd914`
- `QSPressDevice.cs` `237add3b065aab30`
- `QSPressLayout.cs` `e375ab16439e8bb6`
