# QSPress.cs
Hash: `fe249b024fecd914`

## `internal sealed class QSPress`

The print window's driver for the window itself, the footer, and the busy and failure states.
It builds the setting column's two drivers and the canvas driver, and paints them all from one setting.
`QSPressDevice` drives the printer controls and `QSPressLayout` drives the page layout controls.
The driver computes no setting, parses no text and chooses no wording key of its own.
The window is the veneer's `PSPress` page, pulled fresh by contract ID and held in `_qsPressSurface`.
The page canvas and its bar belong to `QSGalley`, which this driver builds and feeds.
The zoom half of the bar belongs to `QSGalleyBar`, which `QSGalley` builds.

## `private readonly Window _qsPressSurface;`

The print window, owned by the active window so it stays above it and closes with it.

## `private readonly CSPress _csPress;`

The print gate Conduct handed to the envoy, which owns every setting and the proof.

## `private readonly QPosture _qsPressPosture;`

The main window's posture, where the print window's size lives between runs.
The setting itself is kept by the engine's store through `CSPressClose`.

## `private readonly QSGalley _qsPressGalley;`

The page canvas's driver, built over the same window and gate.
The canvas is its own area with its own state, so it lives in its own class.

## `private readonly QSPressDevice _qsPressDevice;`

The printer controls' driver, built over the same window and gate.

## `private readonly QSPressLayout _qsPressLayout;`

The page layout controls' driver, built over the same window and gate.

## `private QSPress(Window owner, CSPress press, QPosture posture)`

Pulls the window over `owner`, sizes it from the remembered `LCapsulePress`, and paints the gate's first setting.
A dialog is outside the main window's look reach, so it attaches the look to itself.
The child drivers subscribe their controls on construction.
Their own paints unsubscribe while they fill, so the first paint reaches no gate.
It subscribes `QSPressSettingRefine` to `CSPressSettingChanged`.
Every setting gate raises that event, so a pick in any column repaints the whole window.
It subscribes `QSPressStateRefine` to `CSGalleyStateChanged`, which the galley raises after a deferred redraw.
The galley holds a layout change back, so a fast typist starts one proof draw, not one per key.
The canvas is built before the first paint, since every setting paint also reaches the canvas.
A scrolled page selects through `CSGalleySightSet`, whose setting event repaints the Current range.
`QSPressKeyObserve` hears the window's keys after the canvas's own handler.

## `private Grid QSPressBusy`

Each named part is pulled from the window by its contract ID.

## `internal static bool QSPressConsult(Window owner, CSPress press, QPosture posture)`

Shows the print window modally and answers true when the reader printed or saved.
The modal loop pumps messages, so the gate's awaited draws and prints run inside it.

## `internal static void QSPressFieldRefine(TextBox field, string? text)`

Writes a field's text unless the field holds the keyboard focus.
The focused text box keeps what the reader is typing, so a clamp never jumps under the caret.
Both child drivers paint their fields through it.

## `internal static void QSPressItemRefine<QSPressKind>(ComboBox choice, IReadOnlyList<CSPressChoice<QSPressKind>>? rows)`

Fills a picker from the setting's ready rows, in their order, and selects the chosen one.
The markup declares no rows, so the list is the picker's only source.
The gate maps the raw selected index back through the same list.
A null list leaves the picker empty, which only a disabled two-sided picker shows.
Both child drivers paint their choice pickers through it.

## `private void QSPressSettingRefine(CSPressSetting setting)`

Paints the whole window from one ready setting.
The child drivers paint their columns first, then the footer and the canvas follow.
Admit and Quit enablement and the footer key are painted exactly as given.
A stale setting shows busy and hides the failure panel until the galley's deferred redraw paints the state.
The canvas gets the same setting for its gray view, its shading and its sheet count.

## `private void QSPressStateRefine(CSPressState state)`

Clears busy after a draw and shows the failure panel with its wording when the draw failed.
It answers `CSGalleyStateChanged` and the retry button's own draw alike.
The canvas rebuilds its sheets first, then the setting read after the draw is painted.
That setting carries the new page flags and sheet count, which only a finished draw knows.
A failed draw's setting is never stale, so busy stays hidden under the failure panel.

## `private void QSPressAlertRefine(string? key)`

Shows the footer status worded from `key`, or clears it when `key` is null.

## `private async void QSPressShownObserve(object? sender, EventArgs e)`

Asks `CSPressRestore` for the kept setting once the window first renders.
The gate raises `CSPressSettingChanged` with it, so the answer itself is not painted here.
The gate lists printers first, so the list fills in once the spooler answers.
The restored setting is stale, so the galley's first deferred draw uses the remembered layout.

## `private async void QSPressRetryObserve(object sender, RoutedEventArgs e)`

Draws the proof again through `CSGalleyDraw` after a failure and paints the new state.
A draw that a newer one replaced answers null and paints nothing.

## `private async void QSPressAdmitObserve(object sender, RoutedEventArgs e)`

Prints or saves through `CSPressPrint`, whose change events disable Admit and Quit meanwhile.
The answer goes to `QSPressAdmitRefine`.

## `private async void QSPressKeyObserve(object sender, KeyEventArgs e)`

Ctrl+P prints as Admit does, through `CSPressPrint`.
The gate refuses a second print while one runs, so the driver reads no enablement.
Every other key passes on, so a focused field keeps its own keys.

## `private void QSPressAdmitRefine(string? key)`

A null answer closes the window with true, and a failure key goes to the footer status.
Admit and Ctrl+P share it, so both end a print the same way.

## `private async void QSPressSystemObserve(object sender, RoutedEventArgs e)`

Opens the system print dialog through `CSPressSystemShow` and shows any failure in the footer.

## `private void QSPressQuitObserve(object sender, RoutedEventArgs e)`

Closes the window with false.

## `private void QSPressClosingObserve(object? sender, CancelEventArgs e)`

Hands the event's own cancel flag to `CSPressClose` and cancels closing when it answers false.
That one gate refuses while a print runs, and keeps the setting when it lets the window close.
It then stores the window size as `LCapsulePress`, even on a refused close, since a later close stores it again.
The width and height are the restored size, so a maximized close keeps the normal size.
