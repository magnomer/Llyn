# CParadigmTable.cs
Hash: `89327dddd93bed5a`

## `public sealed record CParadigmTable(IReadOnlyList<string> CParadigmTableHeaders, IReadOnlyList<CParadigmLine> CParadigmTableLines)`

One view of the inflection box.
The headers are localization keys, and Deportment looks them up.

**Parameters**

- `CParadigmTableHeaders`: the column header keys, empty when the view has none.
- `CParadigmTableLines`: the rows of the view, in order.

## `internal static CParadigmTable CParadigmTableCreate(LParadigmTable table)`

Passes the headers through unchanged and maps each row.
It adds no rule.
