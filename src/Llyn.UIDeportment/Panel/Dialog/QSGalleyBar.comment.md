# QSGalleyBar.cs
Hash: `2a6f78b0970c3984`

## `internal sealed class QSGalleyBar`

The zoom half of the print canvas bar, with its controls only.
`QSGalley` builds it over the same window and repaints the sheets when the factor changes.
The zoom and its rules live in Conduct `CSLens`, since typed or wheeled input is raw input.
Each Observe hands one raw value to one lens gate and gives the answer to `QSGalleyLensRefine`.

## `private readonly Window _qsGalleySurface;`

The print window, which holds the zoom controls, the canvas and the keys.

## `private readonly CSLens _csLens;`

The galley's zoom gates, which own the factor, the fit mode, the ladder and the parse.

## `internal QSGalleyBar(Window surface, CSLens lens)`

Sets the zoom icons, paints the lens's starting zoom and wires the zoom controls and keys.
It hears the canvas size and Ctrl+wheel itself, since both only change the zoom.

## `internal event Action<double>? QSGalleyFactorChanged;`

Raised after every zoom paint with the factor from before it.
The canvas sizes its sheets again and scales its scroll offset by the change.
It fires even for an equal factor, since a new draw needs its sheets sized.

## `internal CSLensState QSGalleyLens`

The zoom last painted, which the canvas scales its sheets and pictures by.
It only remembers what Conduct answered, and only `QSGalleyLensRefine` sets it.

## `private ScrollViewer QSGalleyCanvas`

The canvas, measured for the lens and heard for resize and Ctrl+wheel.

## `private ItemsControl QSGalleyList`

The sheet list, whose margin narrows the area the lens fits into.

## `private RadioButton QSGalleyWidth`

Fits the page width to the canvas.

## `private RadioButton QSGalleyPage`

Fits the whole page to the canvas.

## `private Button QSGalleyMinus`

Steps the zoom down the ladder.

## `private TextBox QSGalleyPercent`

Shows the zoom as a percent and takes a typed one on Enter.

## `private Button QSGalleyPlus`

Steps the zoom up the ladder.

## `internal void QSGalleyLensRefine(CSLensState lens)`

Paints a lens answer onto the bar and raises `QSGalleyFactorChanged`.
It checks the radio of the answered mode and shows the answered text.
`QSGalley` calls it with the lens a new proof carries.

## `private (double, double) QSGalleyAreaDraw()`

Measures the area a page is laid in, the canvas view less the sheet list margins.
The lens fits into it, so the measure stays raw and may be zero or negative before layout.

## `private void QSGalleyWidthObserve(object sender, RoutedEventArgs e)`

Hands the width fit choice to the lens.

## `private void QSGalleyPageObserve(object sender, RoutedEventArgs e)`

Hands the page fit choice to the lens.

## `private void QSGalleyMinusObserve(object sender, RoutedEventArgs e)`

Asks the lens for one rung down.

## `private void QSGalleyPlusObserve(object sender, RoutedEventArgs e)`

Asks the lens for one rung up.

## `private void QSGalleyPercentObserve(object sender, KeyEventArgs e)`

Hands the zoom box text to the lens on Enter, exactly as typed.
The lens reads it, so text that is no number refits in the current mode.

## `private void QSGalleyWheelObserve(object sender, MouseWheelEventArgs e)`

Hands the raw wheel delta to the lens while Ctrl is held.
Without Ctrl the wheel scrolls the canvas as usual.

## `private void QSGalleySizeObserve(object sender, SizeChangedEventArgs e)`

Hands the measured area to the lens on every canvas resize.
A fitting lens refits, and a free one answers its factor unchanged.

## `private void QSGalleyKeyObserve(object sender, KeyEventArgs e)`

Steps the zoom on Ctrl+plus and Ctrl+minus, and fits the page on Ctrl+0.
It ignores keys while a field or list has focus, as the page keys do.
