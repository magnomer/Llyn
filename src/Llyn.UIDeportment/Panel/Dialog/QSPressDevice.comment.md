# QSPressDevice.cs
Hash: `237add3b065aab30`

## `internal sealed class QSPressDevice`

The print window's driver for the printer controls.
It drives the printer picker and its notice, copies, collation, colour and two-sided printing.
These controls follow what the chosen printer can do, so they share one driver.
Every observer hands one raw control value to one `CSPress` gate and paints nothing itself.
The gate raises `CSPressSettingChanged`, and `QSPress` paints the whole window from it.

## `private readonly Window _qsPressSurface;`

The print window that `QSPress` pulled and handed over.

## `private readonly CSPress _csPress;`

The print gate every observer calls.

## `internal QSPressDevice(Window surface, CSPress press)`

Subscribes each printer control to its observer.
`QSPress` paints the first setting after building this driver.

## `private ComboBox QSPressDestination`

Each named part is pulled from the window by its contract ID.

## `internal void QSPressDeviceRefine(CSPressSetting setting)`

Paints the printer controls from one ready setting, their enablement included.
A null field disables or hides its control, as the setting's record promises.
Printer names are painted exactly as given.
It unsubscribes the selection and text observers while it fills, as `QLocalization` does.
Painting so reaches no gate, and no flag is needed to tell the two apart.
A click is never raised by painting, so the click observers stay subscribed.

## `private void QSPressDestinationObserve(object sender, SelectionChangedEventArgs e)`

Hands the picker's raw selected index to `CSPressPrinterSelect`.
The gate maps it to a printer through the setting it last answered, and ignores -1.

## `private void QSPressCopiesObserve(object sender, TextChangedEventArgs e)`

Hands the copies field's raw text to `CSPressCopiesSet`.

## `private void QSPressCollationObserve(object sender, RoutedEventArgs e)`

Hands the collation switch's state to `CSPressCollationSet`.

## `private void QSPressFullObserve(object sender, RoutedEventArgs e)`

Hands the full colour value to `CSPressColorSet`.
Each radio button has its own observer, so no observer inspects its sender.

## `private void QSPressGrayObserve(object sender, RoutedEventArgs e)`

Hands the gray value to `CSPressColorSet`.

## `private void QSPressSideObserve(object sender, SelectionChangedEventArgs e)`

Hands the two-sided picker's raw selected index to `CSPressSideSelect`.
The gate maps the row to its mode and ignores -1.
