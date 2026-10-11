# QSPressLayout.cs
Hash: `e375ab16439e8bb6`

## `internal sealed class QSPressLayout`

The print window's driver for the page layout controls.
It drives the page range, orientation, paper, margins, scale and background.
These controls shape the sheets the proof draws, so they share one driver.
Every observer hands one raw control value to one `CSPress` gate and paints nothing itself.
The gate raises `CSPressSettingChanged`, and `QSPress` paints the whole window from it.

## `private readonly Window _qsPressSurface;`

The print window that `QSPress` pulled and handed over.

## `private readonly CSPress _csPress;`

The print gate every observer calls.

## `internal QSPressLayout(Window surface, CSPress press)`

Subscribes each layout control to its observer.
`QSPress` paints the first setting after building this driver.

## `private ComboBox QSPressRange`

Each named part is pulled from the window by its contract ID.

## `internal void QSPressLayoutRefine(CSPressSetting setting)`

Paints the layout controls from one ready setting, their enablement included.
A null field disables or hides its control, as the setting's record promises.
It unsubscribes the selection and text observers while it fills, as `QLocalization` does.
Painting so reaches no gate, and no flag is needed to tell the two apart.
A click is never raised by painting, so the click observers stay subscribed.

## `private void QSPressRangeObserve(object sender, SelectionChangedEventArgs e)`

Hands the pages mode picker's raw selected index to `CSPressRangeSelect`.
The gate maps the row to its mode and keeps the pages text it last answered.

## `private void QSPressPagesObserve(object sender, TextChangedEventArgs e)`

Hands the typed pages text to `CSPressRangeSet`, which keeps the mode it last answered.

## `private void QSPressUprightObserve(object sender, RoutedEventArgs e)`

Hands the portrait value to `CSPressOrientationSet`.
Each radio button has its own observer, so no observer inspects its sender.

## `private void QSPressLandscapeObserve(object sender, RoutedEventArgs e)`

Hands the landscape value to `CSPressOrientationSet`.

## `private void QSPressPaperObserve(object sender, SelectionChangedEventArgs e)`

Hands the chosen sheet's list position to `CSPressPaperSelect`, which indexes the same list.

## `private void QSPressMarginObserve(object sender, SelectionChangedEventArgs e)`

Hands the margin picker's raw selected index to `CSPressMarginSelect`.
The gate maps the row to its preset and ignores -1.

## `private void QSPressTopObserve(object sender, TextChangedEventArgs e)`

Hands the top margin field's raw text to `CSPressTopSet`.
The gate takes the other three edges from the setting it last answered.
Each field so hands one value, instead of the driver gathering all four.

## `private void QSPressBottomObserve(object sender, TextChangedEventArgs e)`

Hands the bottom margin field's raw text to `CSPressBottomSet`.

## `private void QSPressLeftObserve(object sender, TextChangedEventArgs e)`

Hands the left margin field's raw text to `CSPressLeftSet`.

## `private void QSPressRightObserve(object sender, TextChangedEventArgs e)`

Hands the right margin field's raw text to `CSPressRightSet`.

## `private void QSPressScaleObserve(object sender, TextChangedEventArgs e)`

Hands the scale field's raw text to `CSPressScaleSet`.

## `private void QSPressBackgroundObserve(object sender, RoutedEventArgs e)`

Hands the background switch's state to `CSPressBackgroundSet`.
