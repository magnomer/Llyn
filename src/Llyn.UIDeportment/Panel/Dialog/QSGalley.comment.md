# QSGalley.cs
Hash: `a161b33095d3c149`

## `internal sealed class QSGalley`

The print window's page canvas, with its sheets, page moves, sight and lazy page images.
`QSPress` builds it over the same window and gate, and feeds it every draw and setting.
It builds `QSGalleyBar` over the galley's lens and repaints the sheets when the painted zoom changes.
It keeps no clock that drives a gate.
Each scroll hands one raw sight to `CSGalleySightSet`, and the galley's wait decides when pages are drawn.
The current page lives in `CSPress`, so this driver keeps no copy and decides no page request.
DPI, the image cache and scrolling are canvas state, since printing is GUI-only.
Page count and page size come from the draw as given, and the canvas only scales them.
A typed page goes raw to the gate, which parses it.

## `private readonly Window _qsGalleySurface;`

The print window, which holds the canvas and the keys the canvas hears.
The canvas parts sit inside `PSGalley`, and the lookup walks the logical tree into it.

## `private readonly CSPress _csPress;`

The print gate, which selects a typed page and holds the galley gate `CSPressGalley`.
The galley gate takes the sight, draws page images and owns the lens.

## `private readonly QSGalleyBar _qsGalleyBar;`

The zoom controls over the same window.
This driver reads the zoom it painted, hears its change and hands it the lens a draw carries.
It never sets the zoom itself.

## `private readonly Dictionary<int, byte[]> _qsGalleyCache = [];`

The delivered PNG bytes, by page, which only remembers what was painted.
`QSGalleyPageRefine` is its one writer, answering Conduct's one page event.
A page with a picture adds it and a page without one removes it.
So it holds exactly the pages the galley counts delivered.
It keeps the bytes, not a decoded picture, so a zoom or DPI change decodes again at the shown size.

## `private ObservableCollection<QSGalleyItem> _qsGalleyItem = [];`

One sheet per page of the current draw, replaced whole by each draw.

## `private bool _qsGalleyGray;`

Whether the sheets are painted gray, as the last setting asked.

## `internal QSGalley(Window surface, CSPress press)`

Builds the bar, wires the canvas, the page bar half and the window keys, and sets the page icons.
Each scroll is heard twice, first by the Observe that calls the sight gate.
The Refine that paints runs second and measures the sight again itself, so it reads no gate state.

## `private ScrollViewer QSGalleyCanvas`

The canvas, found by its contract ID `PSGalleyCanvas`.

## `private ItemsControl QSGalleyList`

The sheet list inside the canvas.

## `private Button QSGalleyEarlier`

Moves one page up.

## `private TextBox QSGalleyNumber`

Shows the current page and takes a typed page number on Enter.

## `private TextBlock QSGalleyTotal`

The page total beside the number.

## `private Button QSGalleyLater`

Moves one page down.

## `private TextBlock QSGalleyCount`

The sheet count, as the setting's tally key words it.

## `internal void QSGalleyRefine(CSPressState state)`

Replaces every sheet with placeholders for the new draw.
The galley already dropped every delivered page during the draw, so the cache is empty here.
It paints the draw's lens on the bar, which Conduct refitted to the new paper.
It scrolls back to the page that was on top once the new sheets are laid out.

## `internal void QSGalleySettingRefine(CSPressSetting setting)`

Paints the setting's gray flag, the shade of excluded pages and the sheet count.
A gray change decodes only cached pages again, since a later page arrives painted as the flag says.

## `private QSGalleySight QSGalleySightDraw()`

Measures the laid-out sheets against the canvas and answers the sight with the monitor scale.
It reads only geometry, so a console driver would measure the same thing its own way.
It reads no zoom, since Conduct turns the scale and its own zoom into a resolution.

## `private int QSGalleyWidthDraw()`

The device pixel width a page is shown and decoded at, from the painted zoom.
It is capped at twice the view width, so a deep zoom never decodes a huge image.
It never leaves the driver.

## `private ImageSource QSGalleyPictureDraw(byte[] png, int width)`

Decodes `png` at `width` device pixels, the size the sheet shows, and grays it when the setting asks.
Decoding at the shown size keeps memory to what the screen shows, whatever resolution arrived.
A width of zero or less decodes at the picture's own size.

## `private void QSGalleyScrollObserve(object sender, ScrollChangedEventArgs e)`

Hands the raw sight to the one gate `CSGalleySightSet`.
The gate selects the page and ignores an unchanged one, so this handler checks nothing.

## `private void QSGalleyScrollRefine(object sender, ScrollChangedEventArgs e)`

Paints the page number, the total and the move buttons from the sight.
It leaves the number alone while the reader types in it.
It drops no picture, since the galley's page event owns that.

## `private void QSGalleyPageRefine(CSPressPage page)`

Answers Conduct's one page event, the only writer of the cache.
A page with a picture is cached and shown decoded at the shown size.
A page without one was dropped, so its bytes are forgotten and its sheet cleared.
A page beyond the current sheets is ignored, since a newer draw replaced them.

## `private void QSGalleyPictureRefine()`

Decodes every cached page again at the size the sheets now show.
A zoom, a DPI change and a gray change call it, so no picture is decoded larger than shown.

## `private void QSGalleyItemRefine(FrameworkElement container, object item, string? _)`

Sizes one sheet by the painted factor and paints its picture, shade and badge.

## `private void QSGalleyFactorRefine(double before)`

Answers the bar's zoom paint by sizing every sheet again.
When the factor moved, it keeps the reading place by scaling the scroll offset.
Then it decodes the cached pages again at the new shown size.

## `private void QSGalleyDpiObserve(object sender, DpiChangedEventArgs e)`

Hands the sight with the new monitor scale to the gate, which may then need another resolution.

## `private void QSGalleyDpiRefine(object sender, DpiChangedEventArgs e)`

Decodes the cached pages again for the new DPI, without asking the galley for them.

## `private void QSGalleyEarlierRefine(object sender, RoutedEventArgs e)`

Scrolls to the page above the one on top.

## `private void QSGalleyLaterRefine(object sender, RoutedEventArgs e)`

Scrolls to the page below the one on top.

## `private void QSGalleyNumberObserve(object sender, KeyEventArgs e)`

Hands the typed page number to `CSPressPageSelect` on Enter.

## `private void QSGalleyNumberRefine(CSPressSetting setting)`

Paints the page the gate kept and scrolls to it.

## `private void QSGalleyTurnRefine(int page)`

Scrolls the sheet of `page` to the top, clamped to the sheets.

## `private void QSGalleyKeyRefine(object sender, KeyEventArgs e)`

Moves between pages on the arrow, page, Home and End keys without modifiers.
It ignores keys while a field or list has focus, so typing never turns pages.
The zoom keys belong to the bar's own handler.
