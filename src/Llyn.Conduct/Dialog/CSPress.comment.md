# CSPress.cs
Hash: `fb31b5df2beed5a4`

## `public sealed class CSPress`

The print window's gate over one galley and its relay, which every print driver calls alike.
Each setting gate makes one galley call and answers the ready setting, so the driver repaints from one record.
Each setting gate also passes that setting through `LSPressSettingRaise` before answering it.
So a stale setting schedules one deferred redraw, and the driver keeps no clock.
The same helper raises `CSPressSettingChanged`, so every part of the window repaints from one event.
Clamps, range parsing, units, visibility and setting facts stay in the galley and the Core press types.
The internal mapper `CSPressWording` turns their answers into Conduct records and chooses every wording key.
This class keeps the gates and the state they change.
It also holds the window's own state, the page the canvas shows and whether a print runs.
That state lives only while a person looks at the window, so the galley keeps none of it.
Its public signatures carry only .NET and Conduct types, so no engine record crosses the cut.
The galley is bound to the thread that opened it, so every gate runs on that thread.
The proof draws live in `CSGalley`, which this gate builds and hands out.
This class keeps the setting gates and the output gates, print, system dialog and close.
The running flag sits here because the print gate that writes it sits here.

## `private readonly LGalley _csPressEngine;`

The setting session the window shapes, taken from the relay this gate is built over.

## `private readonly LGalleyRelay _csPressRelay;`

The proof and print relay, owned and disposed by `CPortrait.LPortraitPrint`.
The print and system dialog gates call it, so the setting gates never reach a draw.

## `private readonly CEnvoy _csPressEnvoy;`

The port the Save as PDF gate asks for a file path through.
Every failure notice of this gate is shown through it too.

## `private readonly LSettingsPort _csPressSettings`

The port a failure notice is read through, the same one the print gate opened with.

## `private int _csPressPage`

The page the canvas shows, counted from zero.
A Current mode prints this page, so the range gate hands it to the galley.
The setting carries it, so the canvas scrolls to a typed page without counting itself.

## `private bool _csPressRunning`

Whether a print or PDF save is under way.
Only `CSPressPrint` writes it, so it has one writer.
`CSPressClose` and the print refusal read it.

## `private CSPressSetting? _csPressAnswer`

The setting this gate last answered, recorded by the read and restore gates every answer passes through.
Each index gate maps the picker's index through the matching list of this answer.
So the index always means a row the window was painted with.
The range text and margin edge gates take their other inputs from it, so each driver event hands one value.
It is null only before the first answer.

## `internal CSPress(LGalleyRelay relay, CEnvoy envoy, LSettingsPort settings)`

Opens on the first page with no print running.
It takes the galley from `relay`, so both halves come from one opening.
Every setting value comes from the galley, so the window opens on whatever it holds.
It builds the galley gate over the same relay, envoy and settings port.

## `public CSGalley CSPressGalley { get; }`

The proof draw gates of this window, which the driver and the canvas call.
They read the setting through this gate, so the page and running flag stay here alone.

## `public event Action<CSPressSetting>? CSPressSettingChanged;`

Raised with a fresh setting when a print starts and when it ends.
The window paints the busy key and its enablement from it, so it decides nothing about printing.
Every gate that answers a setting raises it too, through `LSPressSettingRaise`.
So a driver part that calls a gate needs no path to the parts it does not paint.

## `public CSPressSetting CSPressSettingRead()`

Everything the settings column shows, mapped by `CSPressWording` from one galley view and the window's own state.
It records the answer in `_csPressAnswer` before returning it.

## `public async Task<CSPressSetting> CSPressRestore()`

Asks the galley with one call to restore the kept setting, and maps the view it answers.
The galley lists the printers and reads the store in that call, so a broken spooler surfaces here.
A failure shows `Print.Failed` through `CLedger.LLedgerFailureShow`.
The window then stays open on the setting the galley holds.
That offers only Save as PDF when the printer listing failed.
Every read, fallback and clamp rule stays below Conduct, and no setting text passes through here.
Both answers are recorded in `_csPressAnswer` and pass through `LSPressSettingRaise`.
So a restored stale setting brings the first proof.

## `private CSPressSetting LSPressSettingRaise(CSPressSetting setting)`

The one tail every setting answer passes through.
It hands the setting to `CSGalley.LSGalleyProofDefer`, raises `CSPressSettingChanged`, and answers the setting.

## `public CSPressSetting CSPressPrinterSelect(int index)`

Chooses a printer by the picker's raw selected index.
The index maps through the printer list of the setting last answered, so Save as PDF is index zero.
An index outside the list, such as -1 for no selection, changes nothing.
The gate then only reads the setting, as the other index gates do.

## `public CSPressSetting CSPressCopiesSet(string text)`

Sets the copy count from the field's raw text, which the galley parses and clamps to the printer's limit.

## `public CSPressSetting CSPressCollationSet(bool collation)`

Sets whether copies are collated.

## `public CSPressSetting CSPressRangeSet(CSPressRange range, string text)`

Hands the galley the pages mode, the typed text and the canvas's page in one call.
The galley decides the range text from the mode, so `text` matters only for Custom.
The page matters only for Current.
An invalid typed text keeps the last valid range and shows `Print.PagesInvalid`.

## `public CSPressSetting CSPressRangeSelect(int index)`

Chooses a pages mode by the picker's raw selected index.
The index maps through the range list of the setting last answered, the list the driver filled the picker from.
The typed text comes from the same answer, so Custom opens on the text the window shows.
An index of -1 or past the list changes nothing, and the gate only reads the setting.

## `public CSPressSetting CSPressRangeSet(string text)`

Hands the typed pages text with the mode the setting last answered as chosen.

## `internal void LSPressPageSelect(int page)`

Keeps the page the canvas shows, counted from zero, and hands it to the galley.
In production only the sight gate `CSGalley.CSGalleySightSet` calls it, so one scroll reaches one gate.
A negative page means no sheet is laid out, and an unchanged page changes nothing.
Both are ignored, so a scroll within one page raises no setting.
The galley changes the range only for a Current mode.
A changed page raises the setting, which the window paints from the event.

## `public CSPressSetting CSPressPageSelect(string text)`

Hands the galley the page number the reader typed, raw, with the current page.
The galley parses and clamps it and answers the page to keep.
The answer's page tells the canvas where to scroll.

## `public CSPressSetting CSPressOrientationSet(CSPressOrientation orientation)`

Turns the sheet, which makes the proof stale.

## `public CSPressSetting CSPressPaperSelect(int index)`

Chooses a sheet by its place in the setting's paper list.
The galley lists the same sheets in the same order, so the gate hands it the index.
An index of -1 or past the list of the setting last answered changes nothing.
The gate then only reads the setting.

## `public CSPressSetting CSPressMarginSet(CSPressMargin margin)`

Chooses a margin preset by its Core name.
Custom keeps the current values as a hand margin, so the four fields open on what the reader sees.

## `public CSPressSetting CSPressMarginSet(string top, string bottom, string left, string right)`

Sets four margins from the fields' raw text, typed in the unit the setting names.
The galley parses them, so the driver never reads a number.
Values that equal a preset select that preset.

## `public CSPressSetting CSPressMarginSelect(int index)`

Chooses a margin preset by the picker's raw selected index.
The index maps through the margin list of the setting last answered.
An index of -1 or past the list changes nothing, and the gate only reads the setting.

## `public CSPressSetting CSPressTopSet(string text)`

Sets the top margin from its field's raw text.
The other three edges come from the setting last answered, formatted as the window paints them.
So the four-value gate sees what the four fields would show.
An edge the setting leaves null passes as empty text, which the galley refuses.

## `public CSPressSetting CSPressBottomSet(string text)`

Sets the bottom margin from its field's raw text, as `CSPressTopSet` does for the top.

## `public CSPressSetting CSPressLeftSet(string text)`

Sets the left margin from its field's raw text, as `CSPressTopSet` does for the top.

## `public CSPressSetting CSPressRightSet(string text)`

Sets the right margin from its field's raw text, as `CSPressTopSet` does for the top.

## `private static string LSPressInsetRead(double? inset)`

Formats one answered margin edge in the current culture, or empty for none.

## `public CSPressSetting CSPressScaleSet(string text)`

Sets the scale percent from the field's raw text, which the galley parses and clamps.

## `public CSPressSetting CSPressColorSet(CSPressColor color)`

Sets the colour mode, which falls back to the printer default when the printer lacks it.

## `public CSPressSetting CSPressSideSet(CSPressSide side)`

Sets two-sided printing, which falls back to the printer default when the printer lacks it.

## `public CSPressSetting CSPressSideSelect(int index)`

Chooses a two-sided mode by the picker's raw selected index.
The index maps through the two-sided list of the setting last answered.
Save as PDF answers no list, so every index there changes nothing.
An index of -1 or past the list changes nothing, and the gate only reads the setting.

## `public CSPressSetting CSPressBackgroundSet(bool background)`

Sets whether background colours and pictures print.

## `public async Task<string?> CSPressSystemShow()`

Opens the system print dialog on the sheet, for driver options the window does not offer.
A failure shows `Print.Failed` through `CLedger.LLedgerFailureShow`.
It still answers `Print.Failed`, so the footer shows it and the window stays open.

## `public async Task<string?> CSPressPrint()`

A print that starts while one runs answers `Print.Printing` and calls nothing.
So Admit and Ctrl+P need no gating of their own in the driver.

Makes one relay call, which prints or saves a PDF file as the setting chooses.
A save asks for the path through the envoy inside that call, offering only the PDF format.
The relay hands the offered file name to that request, so the gate reads no galley property.
Whether the envoy was asked tells a save failure from a print failure.
A cancelled path request is no failure and answers null.
A status answers `Print.PrinterUnavailable` or `Print.Failed` for the window to show.
An exception shows `Print.Failed` or `Print.SaveFailed` through `CLedger.LLedgerFailureShow`.
It answers the same key, so the window stays open with the footer status.
It raises `CSPressSettingChanged` once the relay call has started and again when it ends.
The running flag holds while the relay's task is still pending after the call returns.
A cancelled save or a print that finished at once never shows as running.

## `public bool CSPressClose(bool cancel)`

The one closing gate, which answers whether the window may close and keeps the setting when it may.
`cancel` is the closing event's own flag, so a closing already refused stays refused.
It answers false while a print runs, because a spooling job could be lost under a closed window.
A refused close calls no galley member, so nothing is saved.
A close it allows keeps the live setting for the next window with one galley call.
A store that cannot be written shows `Print.Failed` through `CLedger.LLedgerFailureShow`.
It still answers true, since only the remembered choices are lost.
