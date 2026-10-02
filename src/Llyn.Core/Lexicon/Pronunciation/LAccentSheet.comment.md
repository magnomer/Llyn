# LAccentSheet.cs
Hash: `e0f80fe78410f836`

## `public sealed record LAccentSheet(`

The pronunciation block of the shown entry, answered ready by the engine.
It carries the pack's verdicts beside the rows, so no caller asks the pack again.

**Parameters**

- `LAccentSheetLanguage` — The entry's language, which keys each variety's flag.
- `LAccentSheetFlagged` — True when the pack draws its varieties as flags.
- `LAccentSheetContour` — The primary reading's tone contour syllables, empty when nothing is drawn.
- `LAccentSheetRespelled` — True when the readings show their respelling.
- `LAccentSheetOpener` — The bracket that opens a reading.
- `LAccentSheetCloser` — The bracket that closes a reading.
- `LAccentSheetPrimary` — The first pronunciation, blank when the entry has none.
- `LAccentSheetRows` — The further pronunciations that carry a reading, in the entry's order.

## `public bool LAccentSheetSpoken`

True when the primary pronunciation has a reading to show.

## `public IReadOnlyList<string> LAccentSheetVarieties`

The variety labels whose flags the block draws, the primary's included.
A pronunciation without a label draws no flag, so it is left out.
