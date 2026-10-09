# LInflectionSheet.cs
Hash: `d4db41b1b6784598`

## `public sealed record LInflectionSheet(IReadOnlyList<string> LInflectionSheetHeaders, IReadOnlyList<LInflectionLine> LInflectionSheetLines)`

The plan of one view of the inflection box.
Headers and labels are localization keys the pack names, carried opaque and never translated here.
The UI language picks their text, so the engine never names a language.

**Parameters**

- `LInflectionSheetHeaders`: column header keys, empty when the view shows none.
- `LInflectionSheetLines`: the rows in display order.
