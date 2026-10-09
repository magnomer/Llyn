# LInflectionLayout.cs
Hash: `01e6bf69064874cf`

## `public sealed record LInflectionLayout(long LInflectionLayoutPart, LInflectionSheet LInflectionLayoutCollapsed, LInflectionSheet LInflectionLayoutExpanded, LInflectionLayout? LInflectionLayoutCustom = null)`

How the forms of one part of speech are laid out in the inflection box.
Both views target the same part, but each sheet may select different cells.

**Parameters**

- `LInflectionLayoutPart`: the pack code of the part of speech whose paradigm list the box replaces.
- `LInflectionLayoutCollapsed`: the default short view, shown when custom analysis is off.
- `LInflectionLayoutExpanded`: the default full view, shown when custom analysis is off.
- `LInflectionLayoutCustom`: the pair of sheets shown when custom analysis is on.
  It targets the same part, and its own custom pair stays null.
  The loader fills it with the default sheets when the pack declares no custom pair.
  A null pair falls back to the default sheets as well.
