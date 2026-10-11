# TInterfaceConductDialog.cs
Hash: `3c2d58ee2694a179`

## `internal static class TInterfaceConductDialog`

The relays for Conduct's dialog rules.
They build the markup import's customs sheet and reach its rows.
They reach the coinage wording rule and every gate of the print window.
No relay builds a WPF window, so the rules are tested without a dialog or a printer.
Each relay is transparent and carries no test logic of its own.

## `internal static CSCustoms TCustomsCreate(IReadOnlyList<CMarkupEntry> entries, IReadOnlyList<CMarkupTarget> targets)`

Builds the customs sheet over the cargo's entries and the targets they may merge into.

## `internal static IReadOnlyList<CSCustomsRow> TCustomsRowsRead(this CSCustoms customs)`

Relays the customs sheet's read of every row.

## `internal static CSCustomsRow TCustomsRowRead(this CSCustoms customs, int row)`

Relays the customs sheet's read of one row.

## `internal static bool TCustomsModeSet(this CSCustoms customs, int row, CSCustomsMode mode)`

Relays the customs sheet's mode change of one row.
It answers whether the whole sheet is then ready.

## `internal static bool TCustomsTargetSet(this CSCustoms customs, int row, long target)`

Relays the customs sheet's target change of one row.
It answers whether the whole sheet is then ready.

## `internal static bool TCustomsReadyCheck(this CSCustoms customs)`

Relays the customs sheet's check that every row not imported fresh has a target.

## `internal static bool TCoinageWordingCheck(string? wording)`

Relays the coinage dialog's rule that a wording is not blank.

## `internal static CSPressSetting TPressSettingRead(this CSPress press)`

Relays the print gate's read of the current setting.
Most press relays are named `TPress` plus the gate's words after `CSPress` or `CSGalley`.
The lens relays are named `TLens` plus the words after `CSLens`.
The delay, dispose and attach relays are the exceptions.

## `internal static Task<CSPressSetting> TPressRestore(this CSPress press)`

Relays the restore of the kept setting, which lists the printers first.

## `internal static CSPressSetting TPressPrinterSelect(this CSPress press, string? name)`

Finds the place of the printer named `name` in the current setting and relays that index.
A null name finds Save as PDF.
An unlisted name finds -1, which the gate ignores.
Tests name printers, while the window hands the picker's raw index.

## `internal static CSPressSetting TPressPrinterSelect(this CSPress press, int index)`

Relays the printer choice by the picker's raw index, as the window hands it.

## `internal static CSPressSetting TPressCopiesSet(this CSPress press, string text)`

Relays the typed copies.

## `internal static CSPressSetting TPressCollationSet(this CSPress press, bool collation)`

Relays the collation choice.

## `internal static CSPressSetting TPressRangeSet(this CSPress press, CSPressRange range, string text)`

Relays the page range choice with its typed text.

## `internal static CSPressSetting TPressRangeSet(this CSPress press, string text)`

Relays the typed pages text alone, as the pages field hands it.

## `internal static CSPressSetting TPressRangeSelect(this CSPress press, int index)`

Relays the pages picker's raw selected index.

## `internal static CSPressSetting TPressPageSelect(this CSPress press, int page)`

Relays the selection of a proof page by index through the internal helper `LSPressPageSelect`.
The helper answers nothing, so the relay reads the setting after it.
The sight gate reaches the same helper, and the relay skips that gate's wait.

## `internal static CSPressSetting TPressPageSelect(this CSPress press, string text)`

Relays the selection of a proof page by typed number.

## `internal static CSPressSetting TPressOrientationSet(this CSPress press, CSPressOrientation orientation)`

Relays the orientation choice.

## `internal static CSPressSetting TPressPaperSelect(this CSPress press, int index)`

Relays the paper choice by its index in the offered list.

## `internal static CSPressSetting TPressMarginSet(this CSPress press, CSPressMargin margin)`

Relays the choice of a margin preset.

## `internal static CSPressSetting TPressMarginSet(this CSPress press, string top, string bottom, string left, string right)`

Relays the four typed margins.

## `internal static CSPressSetting TPressMarginSelect(this CSPress press, int index)`

Relays the margin picker's raw selected index.

## `internal static CSPressSetting TPressTopSet(this CSPress press, string text)`

Relays the typed top margin alone, as its field hands it.

## `internal static CSPressSetting TPressBottomSet(this CSPress press, string text)`

Relays the typed bottom margin alone, as its field hands it.

## `internal static CSPressSetting TPressLeftSet(this CSPress press, string text)`

Relays the typed left margin alone, as its field hands it.

## `internal static CSPressSetting TPressRightSet(this CSPress press, string text)`

Relays the typed right margin alone, as its field hands it.

## `internal static CSPressSetting TPressScaleSet(this CSPress press, string text)`

Relays the typed scale.

## `internal static CSPressSetting TPressColorSet(this CSPress press, CSPressColor color)`

Relays the colour choice.

## `internal static CSPressSetting TPressSideSet(this CSPress press, CSPressSide side)`

Relays the sides choice.

## `internal static CSPressSetting TPressSideSelect(this CSPress press, int index)`

Relays the sides picker's raw selected index.

## `internal static CSPressSetting TPressBackgroundSet(this CSPress press, bool background)`

Relays the background choice.

## `internal static Task<CSPressState?> TPressDraw(this CSPress press)`

Relays the proof draw of the press's galley, which answers null when a newer draw supersedes it.

## `internal static Task TPressSightSet(this CSPress press, int page, int first, int last, double scale)`

Relays the sight a driver hands over after a scroll, with the page, visible pages and monitor scale.
The task ends once the deferred page requests are answered, so a test awaits it.

## `internal static CSLensState TLensTextSet(this CSPress press, string text)`

Relays typed zoom box text to the galley's lens.

## `internal static CSLensState TLensLadderMove(this CSPress press, int delta)`

Relays a zoom step or a raw wheel delta to the galley's lens.

## `internal static CSLensState TLensModeSet(this CSPress press, CSLensMode mode)`

Relays a fit choice to the galley's lens.

## `internal static CSLensState TLensAreaSet(this CSPress press, double width, double height)`

Relays the measured view size to the galley's lens.

## `internal static void TPressDelaySet(this CSPress press, int delay)`

Sets the galley's wait after an edit and after a sight to the same `delay` in milliseconds.
It sets the time of the two delays apart, since each wait has its own.
Zero runs a deferred draw at once, and `Timeout.Infinite` keeps it pending until a newer request or close.

## `internal static void TPressGalleyDispose(this CSPress press)`

Closes the galley as the print core does once the window closes.
So a test checks that no event follows.

## `internal static Task<string?> TPressSystemShow(this CSPress press)`

Relays the request for the system print dialog, answering a failure key or null.

## `internal static Task<string?> TPressPrint(this CSPress press)`

Relays the print gate, which prints, or saves a PDF after asking the envoy for a file.
It answers a failure key, or null on success.

## `internal static bool TPressClose(this CSPress press)`

Relays the one closing gate with no earlier refusal, as a plain window close hands it.
It answers false while a print runs, and keeps the setting when it answers true.

## `internal static void TPressSettingAttach(this CSPress press, Action<CSPressSetting> watch)`

Subscribes `watch` to the gate's setting event.
So a test sees the setting a print reports at its start and its end.

## `internal static void TPressStateAttach(this CSPress press, Action<CSPressState> watch)`

Subscribes `watch` to the galley's state event, raised when a deferred redraw finishes.

## `internal static void TPressPageAttach(this CSPress press, Action<CSPressPage> watch)`

Subscribes `watch` to the galley's page event for the pages a sight delivers with a picture.
It passes on no drop, so a fact counting deliveries sees only deliveries.

## `internal static void TPressDropAttach(this CSPress press, Action<int> watch)`

Subscribes `watch` to the galley's page event for the pages it raises without a picture.
Those are the delivered pages the galley stops holding, handed on as their index.
