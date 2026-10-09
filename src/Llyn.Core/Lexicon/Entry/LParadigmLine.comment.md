# LParadigmLine.cs
Hash: `71d11de1b157d318`

## `public sealed record LParadigmLine(string LParadigmLineGroup, string LParadigmLineLabel, IReadOnlyList<LParadigmForm> LParadigmLineForms)`

One row of an inflection view, with its cells in column order.
The group and the label are localization keys taken from the pack's layout.
Core passes them through and never translates them.
The group key sits on the first line of its group only, so later lines carry an empty group.

**Parameters**

- `LParadigmLineGroup`: the group's localization key on its first line, otherwise empty.
- `LParadigmLineLabel`: the row's localization key, or empty when the row has no label.
- `LParadigmLineForms`: the cells of the row in column order.
