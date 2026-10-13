# Press

This folder holds the print window's gate and its wording map.
It also holds the settings column the window shows with every printer, paper and layout choice.

## `CSPress`

The print window's gate over one galley and its relay, which every print driver calls alike.
Each setting gate makes one galley call and answers the ready setting, so the driver repaints from one record.
Each also passes that setting through `LSPressSettingRaise`, which schedules one deferred redraw when stale.
So the driver keeps no clock.
The same helper raises `CSPressSettingChanged`, so every part of the window repaints from one event.
Clamps, range parsing, units and setting facts stay in the galley and the Core press types.
`CSPressWording` turns their answers into Conduct records and chooses every wording key.
The galley is bound to the thread that opened it, so every gate runs on that thread.

It also holds the window's own state, which lives only while a person looks at the window.
- `_csPressPage` is the page the canvas shows, counted from zero.
  A Current range prints this page, and the setting carries it so the canvas scrolls without counting.
- `_csPressRunning` says a print or PDF save is under way.
  Only `CSPressPrint` writes it, and `CSPressClose` and the print refusal read it.
- `_csPressAnswer` is the setting last answered, recorded by the read and restore gates.
  It is null only before the first answer.

Each index gate maps the picker's raw index through the matching list of that answer.
So the index always means a row the window was painted with.
An index of -1 or past the list changes nothing, and the gate only reads the setting.
The range text and margin edge gates take their other inputs from the same answer.
So each driver event hands one value.
The printer list opens with Save as PDF at index zero.
Save as PDF answers no two-sided list, so every index there changes nothing.

`CSPressRestore` asks the galley in one call to restore the kept setting.
The galley lists the printers and reads the store in that call, so a broken spooler surfaces here.
A failure shows `Print.Failed` and leaves the window open on the setting the galley holds.
That offers only Save as PDF when the printer listing failed.
A restored stale setting brings the first proof.

`CSPressRangeSet` hands the galley the mode, the typed text and the canvas's page in one call.
The text matters only for Custom and the page only for Current.
An invalid typed text keeps the last valid range and shows `Print.PagesInvalid`.
`LSPressPageSelect` is called only by the sight gate `CSGalley.CSGalleySightSet`, so one scroll reaches one gate.
A negative page or an unchanged page is ignored, so a scroll within one page raises no setting.
`CSPressPageSelect` hands the galley the raw typed page, and the answer's page tells the canvas where to scroll.
Custom margin keeps the current values as a hand margin, so the four fields open on what the reader sees.
Four typed margins are parsed by the galley, and values equal to a preset select that preset.
Each single-edge gate takes the other three from the last answer, formatted as the window paints them.
An edge the setting leaves null passes as empty text, which the galley refuses.
Colour and two-sided mode fall back to the printer default when the printer lacks them.

`CSPressSystemShow` opens the system print dialog for driver options the window does not offer.
A failure shows `Print.Failed` and answers that key, so the footer shows it and the window stays open.

`CSPressPrint` answers `Print.Printing` and calls nothing while a print runs.
So Admit and Ctrl+P need no gating of their own in the driver.
It makes one relay call, which prints or saves a PDF file as the setting chooses.
A save asks for the path through the envoy inside that call, offering only the PDF format.
The relay hands the offered file name to that request, so the gate reads no galley property.
Whether the envoy was asked tells a save failure from a print failure.
A cancelled path request is no failure and answers null.
A status answers `Print.PrinterUnavailable` or `Print.Failed`.
An exception shows `Print.Failed` or `Print.SaveFailed` through the ledger and answers the same key.
It raises the setting once the relay call has started and again when it ends.
The running flag holds while the relay's task is still pending after the call returns.
A cancelled save or a print that finished at once never shows as running.

`CSPressClose` is the one closing gate.
`cancel` is the closing event's own flag, so a closing already refused stays refused.
It answers false while a print runs, because a spooling job could be lost under a closed window.
A refused close calls no galley member, so nothing is saved.
A close it allows keeps the live setting for the next window with one galley call.
A store that cannot be written shows `Print.Failed` and still answers true, since only the remembered choices are lost.

## `CSPressWording`

The read-only mapping from the galley's view to the print window's Conduct records.
It holds no state, so `CSPress` keeps every field its gates change.
Every Core enum maps through an exhaustive switch, so a new member throws instead of slipping through.
The page and the running flag come from the caller, which owns them.
An invalid typed range shows `Print.PagesInvalid`, and the margin unit follows the metric flag.
The button reads `Print.Save` for Save as PDF, else `Print.Print`.
A printer that did not report its options shows `Print.CapabilityUnknown`.
The sheet count wording is singular for exactly one sheet.
The footer reads `Print.Printing` while a print runs, else `Print.Idle`.

Destinations read `Print.SaveAsPdf` or `Print.PrinterDefault`, and any other shows its queue name with braces doubled.
The window formats either with the queue name, so it shows one value without choosing.
A null paper name is a custom sheet.
Any other name comes from the Core standard list, so a name outside the mapped set throws.
The range rows follow the enum's declared order, the only order the picker and its gate share.
A null margin name means Custom, and an unknown one throws.
A null side means Save as PDF, so its list is null too.

## `CSPressSetting`

Everything the print window's settings column shows, read in one call after every gate.
A null field means the control is hidden or disabled, so the driver decides nothing itself.
Each picker is a list of ready `CSPressChoice` rows in display order.
The driver fills the picker from the list alone and hands back the raw row number.
The index gate maps that number through the same list, so no order is shared in secret.
Collation is null for Save as PDF, a single copy, or a printer that cannot collate.
The typed range text is non-null only for Custom, and the margin edges only for a Custom margin.
`CSPressSettingFailure` is `Print.PagesInvalid` or null.
`CSPressSettingStale` says the shown proof no longer matches the layout, so a draw is due.
`CSPressSettingIdle` enables Admit and Quit.
A typed page number arrives parsed and clamped, so the canvas scrolls to it as given.

## Print enums and records

- `CSPressColor` folds the printer's own default into Full, since a printer left to itself prints in colour.
- `CSPressSide` folds the printer's default into Single, since the window offers no default choice.
- `CSPressMargin` and `CSPressOrientation` map member for member to Core, so no driver sees a Core enum.
  Margin presets map by their Core name, so the window never compares numbers itself.
- `CSPressPaper` is one sheet, in the order `CSPressPaperSelect` indexes.
  Its key is `Print.PaperCustom` for an unnamed form.
- `CSPressPrinter` carries a queue name, null for Save as PDF, and a display text the window formats with that name.

## Stamp

Commit: `c2f335c331fe`
- `CSPress.cs` `fb31b5df2beed5a4`
- `CSPressChoice.cs` `babf390eb082c58b`
- `CSPressColor.cs` `73bb0ddebb17f6c9`
- `CSPressMargin.cs` `b371f748bf3f5b58`
- `CSPressOrientation.cs` `7e31a845641d76d9`
- `CSPressPaper.cs` `5792698814ac039b`
- `CSPressPrinter.cs` `44d28bded7f044a8`
- `CSPressSetting.cs` `f8d65f543ec5194a`
- `CSPressSide.cs` `61727c9666229313`
- `CSPressWording.cs` `f68774bb39cc60f9`
