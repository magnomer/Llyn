# CMarker.cs
Hash: `ba21ad91b9546fcd`

## `public sealed record CMarker(IReadOnlyList<string> CMarkerSpeeches, string CMarkerTyped, CCategory CMarkerCategory, IReadOnlyList<(string, bool)> CMarkerUnits);`

The part of speech field as the editor shows it, with the lexical unit dropper that heads its row.

**Parameters**

- `CMarkerSpeeches`: the committed parts, one chip each by its shown name.
- `CMarkerTyped`: the pending text the field keeps.
- `CMarkerCategory`: the category menu for the pending text and the chips.
- `CMarkerUnits`: the units the draft's language offers, each as its localization key and whether the draft holds it.
  They are plain pairs, so the unit menu adds no type to what Conduct offers its drivers.
