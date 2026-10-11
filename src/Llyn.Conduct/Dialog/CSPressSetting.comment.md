# CSPressSetting.cs
Hash: `f8d65f543ec5194a`

## `public sealed record CSPressSetting(IReadOnlyList<CSPressPrinter> CSPressSettingPrinter, IReadOnlyList<CSPressPaper> CSPressSettingPaper, int? CSPressSettingCopies, bool? CSPressSettingCollation, IReadOnlyList<CSPressChoice<CSPressRange>> CSPressSettingRange, string? CSPressSettingText, string? CSPressSettingFailure, IReadOnlyList<bool> CSPressSettingIncluded, int CSPressSettingPage, CSPressOrientation CSPressSettingOrientation, IReadOnlyList<CSPressChoice<CSPressMargin>> CSPressSettingMargin, double? CSPressSettingTop, double? CSPressSettingBottom, double? CSPressSettingLeft, double? CSPressSettingRight, string CSPressSettingUnit, int CSPressSettingScale, CSPressColor? CSPressSettingColor, IReadOnlyList<CSPressChoice<CSPressSide>>? CSPressSettingSide, bool CSPressSettingBackground, string CSPressSettingAction, string? CSPressSettingHint, int? CSPressSettingSheets, string CSPressSettingTally, bool CSPressSettingStale, bool CSPressSettingIdle, string CSPressSettingBusy)`

Everything the print window's settings column shows, read in one call after every gate.
A null field means the control is hidden or disabled, so the driver decides nothing itself.
Each picker is a list of ready rows in display order.
The driver fills the picker from it and hands back the raw index.

**Parameters**

- `CSPressSettingPrinter`: the destinations, Save as PDF first, with the chosen one marked.
- `CSPressSettingPaper`: the sheets the chosen destination offers, with the chosen one marked.
- `CSPressSettingCopies`: the copy count, or null for Save as PDF.
- `CSPressSettingCollation`: whether copies are collated.
  It is null for Save as PDF, a single copy, or a printer that cannot collate.
- `CSPressSettingRange`: the pages modes in display order, with the chosen one marked.
- `CSPressSettingText`: the typed range text, or null unless the mode is Custom.
- `CSPressSettingFailure`: `Print.PagesInvalid` when the typed text names no valid pages, else null.
- `CSPressSettingIncluded`: one flag per proof page, true when the page prints.
  It is empty before the first draw.
- `CSPressSettingPage`: the page the canvas shows, counted from zero.
  A typed page number arrives here parsed and clamped, so the canvas scrolls to it as given.
- `CSPressSettingOrientation`: which way the sheet is turned.
- `CSPressSettingMargin`: the margin presets and Custom in display order, with the chosen one marked.
- `CSPressSettingTop`: the top margin in the region's unit, or null unless the margin is Custom.
- `CSPressSettingBottom`: the bottom margin in the region's unit, or null unless the margin is Custom.
- `CSPressSettingLeft`: the left margin in the region's unit, or null unless the margin is Custom.
- `CSPressSettingRight`: the right margin in the region's unit, or null unless the margin is Custom.
- `CSPressSettingUnit`: the wording key of the margin unit, millimetres or inches.
- `CSPressSettingScale`: the scale in percent.
- `CSPressSettingColor`: the colour mode, or null for Save as PDF.
- `CSPressSettingSide`: the two-sided modes in display order with the chosen one marked, or null for Save as PDF.
- `CSPressSettingBackground`: whether background colours and pictures print.
- `CSPressSettingAction`: the wording key of the primary button, print or save.
- `CSPressSettingHint`: `Print.CapabilityUnknown` when the printer did not report its options, else null.
- `CSPressSettingSheets`: how many sheets of paper the print uses, or null for Save as PDF.
- `CSPressSettingTally`: the wording key of the sheet count, singular for one sheet, else plural.
- `CSPressSettingStale`: whether the shown proof no longer matches the layout, so a draw is due.
- `CSPressSettingIdle`: whether no print runs, so Admit and Quit are enabled.
- `CSPressSettingBusy`: the footer's wording key, `Print.Printing` while a print runs, else `Print.Idle`.
