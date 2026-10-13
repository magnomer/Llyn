# Ticket

This folder holds the choices one print job carries, with the setting record and its parts.

## `LPressSetting`

Every choice a print session holds, in the form the board consumes.
A null printer means the sheet is saved as a PDF file instead of printed.
The record is replaced on each change, so a draw in flight keeps the setting it started with.
The paper is given upright.

## `LPressSettingCreate`

The setting a session opens with, before the reader changes anything.
It holds the regional sheet, twelve millimetre margins, full scale and backgrounds on.
The result is clamped against the printer, so the opening setting is already valid for it.

## `LPressSettingClamp`

Answers this setting made valid for a printer, or for saving as PDF when the printer is null.
Copies stay between one and the printer's limit, capped at the engine's own 999.
Saving as PDF writes one file, so copies fall to one there.
Collation, colour and two-sided modes fall back when the printer does not offer them.
A printer that reported nothing gets printer defaults for colour and two-sided printing.
A sheet the printer lacks becomes the regional sheet, or the first offered.
The offered sheets come from `LPressPaper.LPressPaperRead`.
An orientation outside the enum becomes upright.
A margin that is negative, not finite or leaves no room on the sheet falls back to the default margin.
Scale stays between 10 and 200 percent, the range the engine accepts.

## `LPressSettingMatch`

Whether another setting lays the sheet out the same way.
Only orientation, sheet, margins, scale and backgrounds change layout.
A change elsewhere keeps the pages already drawn, so the preview does not redraw for nothing.
Sheets are compared by size alone, since two names for one size print the same.

## `LPressPaper`

One sheet size in inches.
The browser assumes Letter when told nothing, which is the wrong sheet for most of the world.
A named sheet carries the name its localized label is found by.
The name is null for a form without a known name.

## `LPressPaperTolerance`

How far two sizes may differ in inches and still be one sheet.
Printers report sizes in their own units, so exact equality would miss a matching sheet.

## `LPressPaperLocal`

The sheet a fresh setting starts on, and the clamp's fallback when a printer lacks the chosen sheet.
A metric region gets A4 and any other gets Letter, which is what the machine's own printers default to.

## `LPressPaperStandard`

The sheets offered when a printer reports none of its own, and when saving as PDF.
They are the common office sizes, each carrying its name.
The Letter and A4 entries are the same instances as the two named sheets `LPressPaperLetter` and `LPressPaperMetric`.

## `LPressPaperRead`

The offered sheets are the printer's list only when its capabilities are known and the list is nonempty.
A printer that reported nothing, and saving as PDF, get the standard list.
The setting's clamp and the galley's sheet list both read it, so the two never disagree.

## `LPressPaperMatch`

Whether another paper is the same sheet size, whatever either is called.

## `LPressMargin`

The four page margins in inches.
A named preset carries its name, and unmatched typed margins carry none.
So a choice list can show which preset is chosen without comparing numbers itself.

## `LPressMarginTolerance`

How close typed values must come to a preset to count as that preset.
Converting millimetres to inches leaves tiny remainders that exact equality would reject.

## `LPressMarginDefault`

Twelve millimetres on every side, the margin the page's own print rule declares.
It is the margin a print used before the preview window existed, so output does not shift.

## `LPressMarginPreset`

The presets are none, a quarter inch narrow, one inch wide, and the default.
The list holds them in the order a choice list offers them.
`LPressMarginResolve` matches typed values against it, and a galley finds a preset in it by name.

## `LPressMarginResolve`

Turns four typed values into a margin, read as millimetres when metric is set and inches otherwise.
A negative or non-numeric value becomes zero, since the browser rejects a negative margin outright.
Values that equal a preset answer that preset, so typing its numbers selects it.

## `LPressRange`

The pages to print, kept as the text the browser engine reads, such as `1-3, 5, 8-`.
Empty text means every page.
The parser rewrites typed text into one spelling before a range holds it.

## `LPressRangeParse`

Reads typed page text against a document of a given page count, and answers null when the text is invalid.
Pages count from one, as the reader sees them.
A page past the count or a span running backwards makes the whole text invalid.
An open end such as `8-` is kept open, so it still means the last page after the layout changes.
Numbers and spacing are rewritten consistently.
Spans keep their input order and overlaps, so equivalent selections need not share one spelling.

## `LPressRangeCheck`

Whether the page at an index, counted from zero, falls inside the range.
The index counts from zero because a proof numbers its pages that way.
The view dims a page outside the range rather than hiding it, so page numbers stay put.
Malformed text, which only a hand-built range can hold, includes no page.

## `LPressRangeRead`

Splits range text into spans of page numbers, or null when any piece is malformed.
An open end reads as the largest number, so one comparison covers open and closed spans.
Blank text answers no spans, which both callers treat as every page.

## `LPressScope`

Which pages print, as the mode the reader picked.
Only the custom mode reads the typed range text.
The modes are all, current and custom.

## `LPressColor`

The colour mode sent to the printer.
The preview greys its pictures itself, so this never changes the drawn pages.
The modes are default, full and gray.

## `LPressSide`

Whether and how the printer prints on both sides of the sheet.
The modes are default, single, long edge and short edge.

## `LPressOrientation`

Which way the sheet is turned when the page is laid out.
It is portrait or landscape.

## Stamp

Commit: `c2f335c331fe`
- `LPressColor.cs` `58a6077eef703d03`
- `LPressMargin.cs` `26f8f571b5c2d9a4`
- `LPressOrientation.cs` `1618d88429ba7a36`
- `LPressPaper.cs` `93fd868f6166255b`
- `LPressRange.cs` `b7584b3fe0561822`
- `LPressScope.cs` `3080efedf126db15`
- `LPressSetting.cs` `9ac846a39a0554bb`
- `LPressSide.cs` `e75462b3396ac678`
