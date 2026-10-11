# LPressPrinter.cs
Hash: `17b4f1555fe5e279`

## `public sealed record LPressPrinter(string LPressPrinterName, bool LPressPrinterDefault, IReadOnlyList<LPressPaper> LPressPrinterPaper, IReadOnlyList<LPressSide> LPressPrinterSide, IReadOnlyList<LPressColor> LPressPrinterColor, bool LPressPrinterCollation, int? LPressPrinterCopies, bool LPressPrinterKnown)`

One installed printer and what it reported it can do.
A printer that is offline or has a broken driver may report nothing.
It is still listed, marked unknown, so the reader can choose it and print with its defaults.

**Parameters**

- `LPressPrinterName`: the queue name the browser engine prints to.
- `LPressPrinterDefault`: whether the system names this printer its default.
- `LPressPrinterPaper`: the sheets the printer offers, empty when unknown.
- `LPressPrinterSide`: the two-sided modes the printer offers, empty when unknown.
- `LPressPrinterColor`: the colour modes the printer offers, empty when unknown.
- `LPressPrinterCollation`: whether the printer can collate copies.
- `LPressPrinterCopies`: the most copies one job may ask for, or null when the printer states no limit.
- `LPressPrinterKnown`: whether the printer reported its capabilities at all.
