# LGalleyView.cs
Hash: `d9c8401caf636bf0`

## `public sealed record LGalleyView(IReadOnlyList<LGalleyDestination> LGalleyViewPrinter, IReadOnlyList<LGalleyStock> LGalleyViewPaper, int? LGalleyViewCopies, bool? LGalleyViewCollation, LPressScope LGalleyViewScope, string? LGalleyViewText, bool LGalleyViewValid, IReadOnlyList<bool> LGalleyViewIncluded, LPressOrientation LGalleyViewOrientation, string? LGalleyViewMargin, double? LGalleyViewTop, double? LGalleyViewBottom, double? LGalleyViewLeft, double? LGalleyViewRight, bool LGalleyViewMetric, int LGalleyViewScale, LPressColor? LGalleyViewColor, LPressSide? LGalleyViewSide, bool LGalleyViewBackground, bool LGalleyViewPdf, bool LGalleyViewKnown, int? LGalleyViewSheets, bool LGalleyViewStale)`

The galley's ready answer for the print window's settings column, as facts with every value decided.
It names no wording key, since choosing wording is the gate's concern.
A null field means the control is hidden or disabled, except the margin name.

**Parameters**

- `LGalleyViewPrinter`: the destinations, Save as PDF first with a null name.
- `LGalleyViewPaper`: the offered sheets in index order.
- `LGalleyViewCopies`: the copy count, or null for Save as PDF.
- `LGalleyViewCollation`: whether copies are collated, or null when collation is not offered.
- `LGalleyViewScope`: the pages mode the reader chose.
- `LGalleyViewText`: the typed range text, or null unless typed pages are chosen.
- `LGalleyViewValid`: false only when the typed text names no valid pages.
- `LGalleyViewIncluded`: one flag per proof page, true when the page prints.
- `LGalleyViewOrientation`: which way the sheet is turned.
- `LGalleyViewMargin`: the chosen preset's name, or null for a custom margin.
- `LGalleyViewTop`: the top margin in the region's unit, or null unless the margin is custom.
- `LGalleyViewBottom`: the bottom margin in the region's unit, or null unless the margin is custom.
- `LGalleyViewLeft`: the left margin in the region's unit, or null unless the margin is custom.
- `LGalleyViewRight`: the right margin in the region's unit, or null unless the margin is custom.
- `LGalleyViewMetric`: whether the margin unit is the millimetre rather than the inch.
- `LGalleyViewScale`: the scale in percent.
- `LGalleyViewColor`: the colour mode with the default folded to Full, or null for Save as PDF.
- `LGalleyViewSide`: the two-sided mode with the default folded to Single, or null for Save as PDF.
- `LGalleyViewBackground`: whether background colours and pictures print.
- `LGalleyViewPdf`: whether the output is a PDF file rather than a printer.
- `LGalleyViewKnown`: false only when the chosen printer did not report its options.
- `LGalleyViewSheets`: how many sheets of paper the print uses, or null for Save as PDF.
- `LGalleyViewStale`: whether the held proof no longer matches the layout.

## `public static LGalleyView LGalleyViewRead(LGalley galley)`

Everything the print window's settings column shows, decided here so the gate only maps it.
It reads the galley's live state and keeps none, so the galley file holds only the session.
The chosen printer, offered sheets, page flags and sheet count come ready from `LGalley.LGalleyDigest`.
So the view names no Core rule and answers facts only.
The range text and its validity show only for the Custom scope.
Save as PDF hides copies, collation, colour, sides and the sheet count, since a PDF file has none.
Collation shows only for several copies on a printer that can collate.
The printer's own default colour and sides show as Full and Single.
The window offers no default choice of its own.
Each sheet carries a Core standard name or null, unchecked here.
