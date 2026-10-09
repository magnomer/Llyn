# QParadigmTable.cs
Hash: `3dfbc81903cdf33d`

## `public sealed class QParadigmTable`

One table of the inflection box, with its header keys and its lines.

## `public IReadOnlyList<string> QParadigmTableHeaders`

The localization keys over the form columns, empty when the table has no header row.

## `public IReadOnlyList<QParadigmLine> QParadigmTableLines`

The lines of the table, in the order they are drawn.

## `internal static QParadigmTable QParadigmTableCreate(CParadigmTable table)`

Copies one ready Conduct table, mapping each line through `QParadigmLine.QParadigmLineCreate`.
