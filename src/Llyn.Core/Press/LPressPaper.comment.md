# LPressPaper.cs

## `public sealed record LPressPaper`

One sheet size, measured in inches because that is the unit the browser engine prints in.
The browser assumes Letter when told nothing, which is the wrong sheet for most of the world.

**Parameters**

- `LPressPaperWidth` - the width of the sheet in inches.
- `LPressPaperHeight` - the height of the sheet in inches.

## `public static LPressPaper LPressPaperLetter`

The North American sheet.

## `public static LPressPaper LPressPaperMetric`

The sheet used nearly everywhere else.

## `public static LPressPaper LPressPaperLocal`

The sheet a file print falls back to when no dialog named one.
A metric region gets A4 and any other gets Letter, which is what the machine's own printers default to.
