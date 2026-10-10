# CParadigmLine.cs
Hash: `c11e8f1a3105a68a`

## `public sealed record CParadigmLine(string CParadigmLineGroup, string CParadigmLineLabel, IReadOnlyList<CParadigmForm> CParadigmLineForms)`

One row of the inflection box.
Group and label are localization keys, and Deportment looks them up.

**Parameters**

- `CParadigmLineGroup`: the key of the group heading when the row leads its group, else empty.
- `CParadigmLineLabel`: the key of the row label, or empty.
- `CParadigmLineForms`: the cells of the row, in column order.

## `internal static CParadigmLine CParadigmLineCreate(LParadigmLine line)`

Passes group and label through unchanged and maps each cell.
It adds no rule.
