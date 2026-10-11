# LPressPaper.cs
Hash: `93fd868f6166255b`

## `public sealed record LPressPaper(double LPressPaperWidth, double LPressPaperHeight, string? LPressPaperName = null)`

One sheet size, measured in inches because that is the unit the browser engine prints in.
The browser assumes Letter when told nothing, which is the wrong sheet for most of the world.
A named sheet carries the name its localized label is found by.

**Parameters**

- `LPressPaperWidth`: the width of the sheet in inches.
- `LPressPaperHeight`: the height of the sheet in inches.
- `LPressPaperName`: the sheet name such as `A4`, or null for a form without a known name.

## `private const double LPressPaperTolerance`

How far two sizes may differ in inches and still be one sheet.
Printers report sizes in their own units, so exact equality would miss a matching sheet.

## `public static LPressPaper LPressPaperLetter`

The North American sheet.

## `public static LPressPaper LPressPaperMetric`

The sheet used nearly everywhere else.

## `public static LPressPaper LPressPaperLocal`

The sheet a fresh setting starts on, and the clamp's fallback when a printer lacks the chosen sheet.
A metric region gets A4 and any other gets Letter, which is what the machine's own printers default to.

## `public static IReadOnlyList<LPressPaper> LPressPaperStandard`

The sheets offered when a printer reports none of its own, and when saving as PDF.
They are the common office sizes, each carrying its name.
The Letter and A4 entries are the same instances as the two named sheets above.

## `public static IReadOnlyList<LPressPaper> LPressPaperRead(LPressPrinter? printer)`

The sheets offered for `printer`, which are its own when it reported any.
A printer that reported nothing, and saving as PDF, get the standard list.
The setting's clamp and the galley's sheet list both read it, so the two never disagree.

## `public bool LPressPaperMatch(LPressPaper other)`

Whether `other` is the same sheet size, whatever either is called.
