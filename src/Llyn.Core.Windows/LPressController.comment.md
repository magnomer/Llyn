# LPressController.cs
Hash: `cc054b88aa9d403b`

## `internal sealed class LPressController : LPressBoard`

One hidden browser kept open on one sheet for the life of a print window.
Each proof prints the loaded page again instead of opening a new browser.
Nothing is kept globally, so disposing the board releases everything it holds.
The mapping from setting to browser print settings lives here as private helpers, since only the board prints.

## `private readonly IntPtr _lPressControllerFrame`

The hidden parent window the browser lives in.
It is destroyed only after the browser closes.

## `private CoreWebView2Controller? _lPressControllerCore`

The browser that holds the loaded page.
It turns null once the board is disposed, which makes a second dispose do nothing.

## `private readonly string _lPressControllerPage`

The temporary page file the browser loaded.
It is removed when the board is disposed.

## `private readonly SemaphoreSlim _lPressControllerTurn`

The turn every browser print operation takes.
The browser runs one print operation at a time and throws on a second.
Disposing also waits for the turn, so a proof in flight finishes before the browser closes.

## `internal LPressController(IntPtr frame, CoreWebView2Controller core, string page)`

Takes ownership of a window, a browser and a page that `LPressBrowser.LPressBoardOpen` already loaded.

## `public async Task<LPressProof> LPressBoardDraw(LPressSetting setting)`

Prints the loaded page to a PDF stream and opens it as a proof.
The stream belongs to the proof from then on.
It continues on the calling thread, because every browser call must stay on the thread that made the browser.
A disposed board raises `ObjectDisposedException`.

## `public async Task<LPressStatus> LPressBoardPrint(LPressSetting setting)`

Prints the loaded page to the chosen printer as vector output.
It starts from the same layout settings as the proof and only adds the printer keys.
So the paper matches the preview page for page.
An unknown or offline printer answers `LPressStatusOffline`.
Any other browser failure answers `LPressStatusError`.
Settings the browser rejects raise `ArgumentException`, which is caught and answers `LPressStatusError`.
Save as PDF has no printer, so asking it to print raises `InvalidOperationException`.
It holds the turn, since the browser runs one print operation at a time.

## `public async Task LPressBoardExport(LPressSetting setting, string path)`

Saves the loaded page as a PDF at `path` from the proof's layout settings.
The page range is added, since only the proof must always hold every page.
The printer keys are left out because the PDF methods ignore them.
A false answer from the browser raises `IOException`, which `LPressBrowser.LPressSave` passes on.
It holds the turn like every other print operation.

## `public async Task LPressBoardShow()`

Opens the system print dialog on the loaded page as the fallback for driver-only features.
Stapling, trays and finishing exist only there.
It holds the turn until the dialog closes, so no proof starts while the dialog prints.

## `internal static async Task LPressDialogShow(CoreWebView2 core)`

Shows the system print dialog and waits until it closes.
The browser gives no completion for this dialog, so the page's `afterprint` event stands in for one.
The listener is in place before the dialog opens, so a quick cancel is not missed.
A cancelled dialog ends the wait the same way, since the page fires `afterprint` either way.

## `private static CoreWebView2PrintSettings LPressPrintingCreate(CoreWebView2Environment environment, LPressSetting setting)`

Turns a press setting into the browser's layout settings.
The proof, the export and the print all start from these layout settings.
So a preview, its file and its paper never lay out differently.

Sources are the `CoreWebView2PrintSettings` reference on learn.microsoft.com and the package's own XML for 1.0.4191.47.
The `MediaSize` remarks say a custom size should set `PageWidth` and `PageHeight`.
So a custom sheet takes the setting's width and height.
The documentation does not say whether `Landscape` swaps `PageWidth` and `PageHeight`.
It only says the orientation can be portrait or landscape.
Nothing there contradicts sending upright dimensions with the orientation alone turning the sheet.
The proof and the print share that rule, so both turn the sheet the same way.

Maps only the layout keys, which are paper, orientation, margins, scale and backgrounds.
The printer, copies, range, colour and sides do not move the layout, so a proof never carries them.
The sheet is set explicitly because the browser otherwise assumes Letter whatever the printer holds.
Paper always travels in portrait dimensions, and the orientation alone turns it.
So a sheet listed wide by a driver cannot be turned twice.
The margins are restated because the browser drops the page's own print rule when given any.
The scale is a percent in the setting and a factor in the browser.
The setting arrives already clamped, so no value here can be rejected by the browser.

## `private static void LPressPrintingApply(CoreWebView2PrintSettings printing, LPressSetting setting)`

Adds the printer-only keys to the layout settings, for printing alone.
These are the printer, copies, collation, colour, sides, page range and a custom sheet.
None of them moves the layout, so the print still matches the proof.
Each `Default` member maps to the browser's `Default`, which leaves the choice to the printer.
The sheet is sent as custom so the printer takes the setting's width and height.
With the browser's default sheet, the printer's own paper would win over the preview's.
Save as PDF carries no printer, so applying it raises `InvalidOperationException`.

## `public async ValueTask DisposeAsync()`

Waits for the turn, then closes the browser, destroys its window and deletes its page.
The browser closes before its window, since the window is its parent.
