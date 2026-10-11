# LPressSetting.cs
Hash: `9ac846a39a0554bb`

## `public sealed record LPressSetting(string? LPressSettingPrinter, int LPressSettingCopies, bool LPressSettingCollation, LPressRange LPressSettingRange, LPressOrientation LPressSettingOrientation, LPressPaper LPressSettingPaper, LPressMargin LPressSettingMargin, int LPressSettingScale, LPressColor LPressSettingColor, LPressSide LPressSettingSide, bool LPressSettingBackground)`

Every choice a print session holds, in the form the board consumes.
A null printer means the sheet is saved as a PDF file instead of printed.
The record is replaced on each change, so a draw in flight keeps the setting it started with.

**Parameters**

- `LPressSettingPrinter`: the printer queue name, or null to save as PDF.
- `LPressSettingCopies`: how many copies to print.
- `LPressSettingCollation`: whether copies print as whole sets.
- `LPressSettingRange`: the pages to print or save.
- `LPressSettingOrientation`: whether the sheet stands upright or lies on its side.
- `LPressSettingPaper`: the sheet size, given upright.
- `LPressSettingMargin`: the page margins in inches.
- `LPressSettingScale`: the scale of the content in percent.
- `LPressSettingColor`: the colour mode sent to the printer.
- `LPressSettingSide`: the two-sided mode sent to the printer.
- `LPressSettingBackground`: whether background colours and pictures print.

## `public static LPressSetting LPressSettingCreate(LPressPrinter? printer)`

The setting a session opens with, before the reader changes anything.
It holds the regional sheet, twelve millimetre margins, full scale and backgrounds on.
The result is clamped against `printer`, so the opening setting is already valid for it.

## `public LPressSetting LPressSettingClamp(LPressPrinter? printer)`

Answers this setting made valid for `printer`, or for saving as PDF when `printer` is null.
Copies stay between one and the printer's limit, capped at the engine's own 999.
Saving as PDF writes one file, so copies fall to one there.
Collation, colour and two-sided modes fall back when the printer does not offer them.
A printer that reported nothing gets printer defaults for colour and two-sided printing.
A sheet the printer lacks becomes the regional sheet, or the first offered.
The offered sheets come from `LPressPaper.LPressPaperRead`.
An orientation outside the enum becomes upright.
A margin that is negative, not finite or leaves no room on the sheet falls back to the default margin.
Scale stays between 10 and 200 percent, the range the engine accepts.

## `public bool LPressSettingMatch(LPressSetting other)`

Whether `other` lays the sheet out the same way as this setting.
Only orientation, sheet, margins, scale and backgrounds change layout.
A change elsewhere keeps the pages already drawn, so the preview does not redraw for nothing.
Sheets are compared by size alone, since two names for one size print the same.
