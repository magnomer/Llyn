# LParadigmTable.cs
Hash: `f331155e3d0ce659`

## `public sealed record LParadigmTable(IReadOnlyList<string> LParadigmTableHeaders, IReadOnlyList<LParadigmLine> LParadigmTableLines)`

One view of an entry's inflection box, ready to show.
The headers are localization keys for the columns, and may be empty.
Core passes them through and never translates them.

**Parameters**

- `LParadigmTableHeaders`: the column headers as localization keys, empty when the view has none.
- `LParadigmTableLines`: the rows in layout order.
